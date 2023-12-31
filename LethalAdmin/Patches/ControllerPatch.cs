﻿using GameNetcodeStuff;
using HarmonyLib;
using LethalAdmin.Logging;

namespace LethalAdmin.Patches;

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
    }
}