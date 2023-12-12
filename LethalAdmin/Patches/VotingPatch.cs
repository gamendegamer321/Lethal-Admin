using HarmonyLib;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
public class VotingPatch
{
    [HarmonyPatch("SetShipLeaveEarlyServerRpc")]
    [HarmonyPrefix]
    public static bool OnServerVote(TimeOfDay __instance)
    {
        // Ignore this patch if min votes is set to 1 or less (disabled), or if this is not on the server
        if (Plugin.Instance.MinVotes <= 1 || !__instance.IsServer)
        {
            return true;
        }
        
        // Only make the method run if there are at least the minimum amount of votes (if we include this new vote)
        if (__instance.votesForShipToLeaveEarly + 1 >= Plugin.Instance.MinVotes) return true;
        
        __instance.votesForShipToLeaveEarly++; // Still increase vote count if we skip
        return false;
    }
}