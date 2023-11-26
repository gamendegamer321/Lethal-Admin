using HarmonyLib;

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
}