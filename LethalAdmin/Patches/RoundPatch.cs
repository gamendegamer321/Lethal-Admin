using HarmonyLib;
using LethalAdmin.Logging;
using UnityEngine;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public static class RoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.KickPlayer))]
    [HarmonyPatch(nameof(StartOfRound.Awake))]
    [HarmonyPostfix]
    public static void OnKick() // Make sure only banned players are unable to rejoin
    {
        KickBanTools.UpdateKickedIDs();
    }

    [HarmonyPatch(nameof(StartOfRound.EndGameClientRpc))]
    [HarmonyPostfix]
    public static void OnShipLeave(StartOfRound __instance, int playerClientId)
    {
        if (playerClientId >= __instance.allPlayerScripts.Length) return;

        var script = __instance.allPlayerScripts[playerClientId];
        LethalLogger.AddLog(new Log(
            $"[Departure] {script.playerUsername} ({script.playerSteamId}@steam) has started the ship"
        ));
    }
    
    [HarmonyPatch(nameof(StartOfRound.StartGameServerRpc))]
    [HarmonyPrefix]
    public static bool NonServerStartGame(StartOfRound __instance)
    {
        // We want the default method to run if we are not the server, or the feature is disabled
        if (!Plugin.Instance.LockLever || !__instance.IsServer) return true;
        
        LethalLogger.AddLog(new Log(
            $"[Start Game] Blocked bypass attempt on {(__instance.IsServer ? "Server" : "Client")}")
        );
        
        var lever = Object.FindObjectOfType<StartMatchLever>();
        lever.CancelStartGameClientRpc();
        lever.triggerScript.interactable = true; // Make it still usable on the server (but not the clients)
        
        return false;
    }
}