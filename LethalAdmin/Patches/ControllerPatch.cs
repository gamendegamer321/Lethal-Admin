using GameNetcodeStuff;
using HarmonyLib;
using LethalAdmin.Logging;

namespace LethalAdmin.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class ControllerPatch
    {
        [HarmonyPatch("ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void OnPlayerJoin(PlayerControllerB __instance)
        {
            LethalLogger.AddLog(new Log(
                $"[Connect] {__instance.playerUsername} ({__instance.playerSteamId}@steam) has joined"
            ));

            if (__instance.playerSteamId == 0)
            {
                var playerInfo = new KickBanTools.PlayerInfo
                {
                    Username = __instance.playerUsername,
                    SteamID = __instance.playerSteamId,
                    Connected = true, // Assuming the player is connected if this method is called
                    IsPlayerDead = __instance.isPlayerDead
                    
                };

                KickBanTools.KickPlayer(playerInfo);
            }
        }
    }
}