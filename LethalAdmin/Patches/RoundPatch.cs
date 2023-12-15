using HarmonyLib;
using LethalAdmin.Logging;

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
        var playerID = (int) __args[0];

        if (playerID >= __instance.allPlayerScripts.Length) return; 
        
        var script = __instance.allPlayerScripts[playerID];
        LethalLogger.AddLog(new Log(
            $"[Departure] {script.playerUsername} ({script.playerSteamId}@steam) has started the ship"
        ));
    }
    
    [HarmonyPatch("StartGameServerRpc")]
    [HarmonyPrefix]
    public static bool NonServerStartGame(StartOfRound __instance) // TODO: Check reliability and it actually prevents the clients?
    {
        return !__instance.IsServer || !Plugin.Instance.LockLever;
    }
}