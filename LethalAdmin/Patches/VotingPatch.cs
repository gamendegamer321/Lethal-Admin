using HarmonyLib;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
public class VotingPatch
{
    [HarmonyPatch("SetShipLeaveEarlyServerRpc")]
    [HarmonyPrefix]
    public static bool OnShipEarlyVote(TimeOfDay __instance)
    {
        // If any type of early departure has been called, ignore any incoming votes
        return !__instance.shipLeavingAlertCalled;
    }
}