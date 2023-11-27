using System.Collections.Generic;
using LethalAdmin.Logging;

namespace LethalAdmin;

public static class KickBanTools
{
    private static readonly List<PlayerInfo> BannedUsers = new();

    public static PlayerInfo[] GetBannedUsers()
    {
        return BannedUsers.ToArray();
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
                SteamID = player.playerSteamId,
                UsingWalkie = player.speakingToWalkieTalkie
            });
        }
        
        return players;
    }

    public static void BanPlayer(PlayerInfo playerInfo)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            if (playerControllers[id].playerSteamId != playerInfo.SteamID) continue; // Check if this is the steamID we want to ban
            
            if (BannedUsers.Contains(playerInfo)) // If the player is already banned don't ban the again
            {
                LethalLogger.AddLog(new Log(
                        $"[Ban] Could not ban {playerInfo} as this user is already banned", "Error"
                        ));
            }
            
            BannedUsers.Add(playerInfo); // Add the player to the ban list and then kick them
            StartOfRound.Instance.KickPlayer(id);
            
            LethalLogger.AddLog(new Log(
                $"[Ban] {playerInfo} has been banned"
            ));
            
            return;
        }
    }
    
    public static void UnbanPlayer(PlayerInfo player)
    {
        StartOfRound.Instance.KickedClientIds.Remove(player.SteamID); // Make sure it's not in the banned list anymore
        BannedUsers.Remove(player);
    }
    
    public static void KickPlayer(PlayerInfo playerInfo)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        Plugin.Instance.LogInfo("Searching for player " + playerInfo);
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            if (playerControllers[id].playerUsername != playerInfo.Username) continue;
            
            StartOfRound.Instance.KickPlayer(id);
            
            LethalLogger.AddLog(new Log(
                $"[Kick] {playerInfo} has been kicked"
            ));
            return;
        }
    }
    
    public class PlayerInfo
    {
        public string Username;
        public ulong SteamID;
        public bool UsingWalkie;

        public override string ToString()
        {
            return $"{Username} ({SteamID}@steam)";
        }
    }
}