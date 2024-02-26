using System;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using LethalAdmin.Bans;
using Unity.Netcode;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class ConnectionPatch
{
    [HarmonyPatch("ConnectionApproval")]
    [HarmonyBefore("BiggerLobby", "BiggerLobbyA")]
    [HarmonyPrefix]
    public static bool BanApproval(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if ((long)request.ClientNetworkId == (long)NetworkManager.Singleton.LocalClientId) return false;
        
        var str = Encoding.ASCII.GetString(request.Payload);
        var strArray = str.Split(",");

        if (strArray.Length < 2)
        {
            if (!Plugin.Instance.RequireSteam) return true;
            
            DeclineConnection(response, "This lobby requires steam authentication.");
            return false;
        };
        
        ulong steamId;

        try
        {
            steamId = ulong.Parse(strArray[2]);
        }
        catch (Exception)
        {
            return true;
        }


        if (Plugin.Instance.RequireSteam)
        {
            if (steamId == 0)
            {
                DeclineConnection(response, "This lobby requires steam authentication.");
                return false;
            }
        }

        if (BanHandler.Bans.TryGetValue(steamId, out var value))
        {
            DeclineConnection(response, "You are banned from this lobby:\n" + value.BanReason);
            return false;
        }
        
        return true;
    }

    private static void DeclineConnection(NetworkManager.ConnectionApprovalResponse response, string reason)
    {
        response.Reason = reason;
        response.CreatePlayerObject = false;
        response.Approved = false;
        response.Pending = false;
    }
}