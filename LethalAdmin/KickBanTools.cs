using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalAdmin;

public static class KickBanTools
{
    private static readonly List<string> BannedPlayers = new();
    private static readonly List<ulong> BannedSteamIDs = new();
    
    public static Dictionary<int, PlayerControllerB> GetPlayers()
    {
        Dictionary<int, PlayerControllerB> players = new();
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        
        for (var i = 0; i < playerControllers.Length; i++)
        {
            players.Add(i, playerControllers[i]);
        }
        
        return players;
    }

    public static void BanPlayer(string playerName)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            var controller = playerControllers[id];
            
            if (controller.playerUsername != playerName) continue;
            
            if (!BannedPlayers.Contains(controller.playerUsername)) BannedPlayers.Add(controller.playerUsername);
            if (!BannedSteamIDs.Contains(controller.playerSteamId)) BannedSteamIDs.Add(controller.playerSteamId);
            
            StartOfRound.Instance.KickPlayer(id);
            return;
        }
    }
    
    public static void KickPlayer(string playerName)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            if (playerControllers[id].playerUsername != playerName) continue;
            
            StartOfRound.Instance.KickPlayer(id);
            return;
        }
    }

    public static void KickBannedPlayers() // Simply try to kick all banned players (in case one managed to slip by)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;

        for (var id = 0; id < playerControllers.Length; id++)
        {
            var controller = playerControllers[id];

            if (BannedPlayers.Contains(controller.playerUsername) ||
                BannedSteamIDs.Contains(controller.playerSteamId))
            {
                StartOfRound.Instance.KickPlayer(id);
            }
        }
    }
}