using HarmonyLib;
using LethalAdmin.Logging;
using UnityEngine;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class RoundPatch
{
    [HarmonyPatch("KickPlayer")]
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void OnKick(StartOfRound __instance) // Make sure only banned players are unable to rejoin
    {
        __instance.KickedClientIds.Clear();

        foreach (var player in KickBanTools.GetBannedUsers())
        {
            __instance.KickedClientIds.Add(player.SteamID);
        }
    }

    [HarmonyPatch("EndGameClientRpc")]
    [HarmonyPostfix]
    public static void OnShipLeave(StartOfRound __instance, object[] __args)
    {
        var playerID = (int)__args[0];

        if (playerID >= __instance.allPlayerScripts.Length) return;

        var script = __instance.allPlayerScripts[playerID];
        LethalLogger.AddLog(new Log(
            $"[Departure] {script.playerUsername} ({script.playerSteamId}@steam) has started the ship"
        ));
    }
    
    [HarmonyPatch("StartGameServerRpc")]
    [HarmonyPrefix]
    public static bool NonServerStartGame(StartOfRound __instance)
    {
        // We want the default method to run if we are not the server, or the feature is disabled
        if (!Plugin.Instance.LockLever || !__instance.IsServer) return true;
        
        Plugin.Instance.LogInfo($"Blocked bypass attempt on {(__instance.IsServer ? "Server" : "Client")}");
        
        var lever = Object.FindObjectOfType<StartMatchLever>();
        lever.CancelStartGameClientRpc();
        lever.triggerScript.interactable = true; // Make it still usable on the server (but not the clients)
        
        return false;
    }
}