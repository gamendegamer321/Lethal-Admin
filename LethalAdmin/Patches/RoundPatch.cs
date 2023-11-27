using System;
using HarmonyLib;
using LethalAdmin.Logging;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class RoundPatch
{
    [HarmonyPatch("KickPlayer")]
    [HarmonyPostfix]
    public static void OnKick(StartOfRound __instance) // Make sure only banned players are unable to rejoin
    {
        __instance.KickedClientIds.Clear();

        foreach (var player in KickBanTools.GetBannedUsers())
        {
            __instance.KickedClientIds.Add(player.SteamID);
        }
    }
    
    [HarmonyPatch("OnPlayerDC")]
    [HarmonyPrefix]
    public static void OnPlayerDisconnect(StartOfRound __instance, object[] __args)
    {
        var playerID = (int) __args[1];

        if (playerID >= __instance.allPlayerScripts.Length) return; // If this cannot be a script, return
        
        var script = __instance.allPlayerScripts[playerID];
        LethalLogger.AddLog(new Log(
            $"[Disconnect] {script.playerUsername} ({script.playerSteamId}@steam) has disconnected"
        ));
    }
    
    [HarmonyPatch("EndGameClientRpc")]
    [HarmonyPostfix]
    public static void OnShipLeave(StartOfRound __instance, object[] __args)
    {
        var playerID = (int) __args[0];

        if (playerID >= __instance.allPlayerScripts.Length) return; 
        
        var script = __instance.allPlayerScripts[playerID];
        LethalLogger.AddLog(new Log(
            $"[Departure] {script.playerUsername} ({script.playerSteamId}@steam) has started the ship"
        ));
    }
}