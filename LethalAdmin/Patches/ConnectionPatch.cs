using System;
using System.Text;
using HarmonyLib;
using LethalAdmin.Bans;
using Unity.Netcode;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class ConnectionPatch
{
    [HarmonyPatch("ConnectionApproval")]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static bool BanApproval(
        ref RequestInformation __state,
        ref NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        __state = new RequestInformation();

        if ((long)request.ClientNetworkId == (long)NetworkManager.Singleton.LocalClientId) return false;

        var str = Encoding.ASCII.GetString(request.Payload);
        var strArray = str.Split(",");

        if (strArray.Length < 2)
        {
            if (!Plugin.Instance.RequireSteam) return true;
            
            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }

        ulong steamId;

        try
        {
            steamId = ulong.Parse(strArray[1]);
        }
        catch (Exception)
        {
            if (!Plugin.Instance.RequireSteam) return true;
            
            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }


        if (Plugin.Instance.RequireSteam && steamId == 0)
        {
            DeclineConnection(ref __state, response, "This lobby requires steam authentication.");
            return false;
        }

        if (BanHandler.TryGetBan(steamId, out var banInfo))
        {
            DeclineConnection(ref __state, response, "You are banned from this lobby:\n" + banInfo.BanReason);
            return false;
        }
        
        return true;
    }

    [HarmonyPatch("ConnectionApproval")]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    public static void AfterApproval(
        ref RequestInformation __state,
        ref NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (__state.IsDenied && response.Approved)
        {
            DeclineConnection(ref __state, response, __state.DenyReason);
        }
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