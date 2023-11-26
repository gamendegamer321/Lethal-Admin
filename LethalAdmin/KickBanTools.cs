using System.Collections.Generic;
using LethalAdmin.Logging;

namespace LethalAdmin;

public static class KickBanTools
{
    private static readonly List<ulong> BannedSteamIDs = new();

    public static void Unban(ulong steamID)
    {
        StartOfRound.Instance.KickedClientIds.Remove(steamID); // Make sure it's not in the banned list anymore
        BannedSteamIDs.Remove(steamID);
    }

    public static ulong[] GetBannedSteamIDs()
    {
        return BannedSteamIDs.ToArray();
    }
    
    public static List<PlayerInfo> GetPlayers()
    {
        List<PlayerInfo> players = new();
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        
        foreach (var player in playerControllers)
        {
            players.Add(new PlayerInfo()
            {
                Username = player.playerUsername,
                UsingWalkie = player.speakingToWalkieTalkie
            });
        }
        
        return players;
    }

    public static void BanPlayer(string playerName)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        Plugin.Instance.LogInfo("Searching for player " + playerName);
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            var controller = playerControllers[id];
            
            if (controller.playerUsername != playerName) continue;
            
            if (!BannedSteamIDs.Contains(controller.playerSteamId)) BannedSteamIDs.Add(controller.playerSteamId);
            
            Plugin.Instance.LogInfo("Attempting to ban player: " + playerName + "@" + controller.playerSteamId);
            LethalLogger.AddLog(new BanLog(controller.playerUsername, controller.playerSteamId));
            
            StartOfRound.Instance.KickPlayer(id);
            return;
        }
    }
    
    public static void KickPlayer(string playerName)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        Plugin.Instance.LogInfo("Searching for player " + playerName);
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            if (playerControllers[id].playerUsername != playerName) continue;
            
            Plugin.Instance.LogInfo("Attempting to kick player: " + playerName);
            LethalLogger.AddLog(new KickLog(playerName));
            
            StartOfRound.Instance.KickPlayer(id);
            return;
        }
    }
    
    public class PlayerInfo
    {
        public string Username;
        public bool UsingWalkie;
    }
}