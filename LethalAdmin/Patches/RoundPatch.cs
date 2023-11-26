using System.Collections.Generic;
using HarmonyLib;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class RoundPatch
{
    [HarmonyPatch("KickPlayer")]
    [HarmonyPostfix]
    public static void OnKick(StartOfRound __instance) // When someone is kicked, don't add them to the ban list
    {
        var traverse = Traverse.Create(__instance);
        var bannedClientIds = traverse.Field("KickedClientIds").GetValue() as List<ulong>;

        bannedClientIds?.Clear();
    }

    [HarmonyPatch("OnClientConnect")]
    [HarmonyPostfix]
    public static void OnClientConnect() // Kick all banned players
    {
        KickBanTools.KickBannedPlayers();
    }
}