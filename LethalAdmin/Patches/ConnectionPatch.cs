using System;
using System.Linq;
using System.Text;
using HarmonyLib;
using LethalAdmin.Bans;
using LethalAdmin.Logging;
using Unity.Netcode;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class ConnectionPatch
{
    [HarmonyPatch(nameof(GameNetworkManager.ConnectionApproval))]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static bool BanApproval(
        ref RequestInformation __state,
        ref NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        __state = new RequestInformation();

        // Don't run for the host
        if ((long)request.ClientNetworkId == (long)NetworkManager.Singleton.LocalClientId) return false;

        var str = Encoding.ASCII.GetString(request.Payload);
        var strArray = str.Split(",");

        // Check if there is even a steam id given
        if (strArray.Length < 2)
        {
            if (!Plugin.Instance.RequireSteam || GameNetworkManager.Instance.disableSteam) return true;

            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }

        ulong steamId;

        try
        {
            // Try to parse the steam id
            steamId = ulong.Parse(strArray[1]);
        }
        catch (Exception)
        {
            if (!Plugin.Instance.RequireSteam || GameNetworkManager.Instance.disableSteam) return true;

            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }


        if (Plugin.Instance.RequireSteam && steamId == 0)
        {
            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }

        if (Plugin.Instance.RequireSteam && Plugin.Instance.SteamChecker &&
            ConnectionTracker.SteamIds.All(x => x.Value != steamId))
        {
            LethalLogger.AddLog(new Log(
                $"[Connect] Connection request for steamId {steamId} denied"
            ));

            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }
        
        if (BanHandler.TryGetBan(steamId, out var banInfo))
        {
            // Kick the user if they are banned, also giving the ban reason.
            DeclineConnection(ref __state, response, "<b>You are banned from this lobby:</b>\n" + banInfo.BanReason);
            return false;
        }
        
        ConnectionTracker.SteamIdsToNetworkIds[steamId] = request.ClientNetworkId;
        return true;
    }

    [HarmonyPatch(nameof(GameNetworkManager.ConnectionApproval))]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    public static void AfterApproval(
        ref RequestInformation __state,
        NetworkManager.ConnectionApprovalResponse response)
    {
        // Make sure that if we denied it in the prefix, it is still denied now.
        if (__state != null && __state.IsDenied && response.Approved)
        {
            DeclineConnection(ref __state, response, __state.DenyReason);
        }
    }

    [HarmonyPatch(nameof(GameNetworkManager.StartHost))]
    [HarmonyPrefix]
    public static void StartHost()
    {
        ConnectionTracker.SteamIds.Clear();
        ConnectionTracker.SteamIdsToNetworkIds.Clear();
    }

    private static void DeclineConnection(ref RequestInformation state,
        NetworkManager.ConnectionApprovalResponse response, string reason)
    {
        state.IsDenied = true;
        state.DenyReason = reason;

        response.Reason = reason;
        response.CreatePlayerObject = false;
        response.Approved = false;
        response.Pending = false;
    }

    public class RequestInformation
    {
        public bool IsDenied;
        public string DenyReason;
    }
}