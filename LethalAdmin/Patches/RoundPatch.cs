using System;
using HarmonyLib;
using LethalAdmin.Logging;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class RoundPatch
{
    [HarmonyPatch("KickPlayer")]
    [HarmonyPostfix]
    public static void OnKick(StartOfRound __instance) // When someone is kicked, don't add them to the ban list
    {
        __instance.KickedClientIds.Clear();
        __instance.KickedClientIds.AddRange(KickBanTools.GetBannedSteamIDs());
    }
    
    [HarmonyPatch("OnPlayerDC")]
    [HarmonyPrefix]
    public static void OnPlayerDisconnect(StartOfRound __instance, Object[] __args)
    {
        var playerID = (int) __args[1];

        if (playerID < __instance.allPlayerScripts.Length)
        {
            LethalLogger.AddLog(new DisconnectLog(__instance.allPlayerScripts[playerID].playerUsername));
        }
    }
}