using System.Collections.Generic;
using System.Linq;
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
        if (playerInfo.SteamID == 0) // Steam ID as 0 means there is no player or steam is not used
        {
            LethalLogger.AddLog(new Log(
                $"[Ban] {playerInfo} is not a player or steam is disabled, when not using steam bans are not possible!", "Error"
            ));
            return;
        }
        
        KickPlayer(playerInfo); // Always kick the player we are trying to ban
        
        if (BannedUsers.Any(bannedPlayer => bannedPlayer.SteamID == playerInfo.SteamID)) // If the player is already banned don't ban them again
        {
            LethalLogger.AddLog(new Log(
                $"[Ban] Could not ban {playerInfo} as this user is already banned", "Warning"
            ));
            return;
        }
        
        BannedUsers.Add(playerInfo); // Add the player to the ban list and then kick them
        LethalLogger.AddLog(new Log(
            $"[Ban] {playerInfo} has been banned"
        ));
    }
    
    public static void UnbanPlayer(PlayerInfo player)
    {
        StartOfRound.Instance.KickedClientIds.Remove(player.SteamID); // Make sure it's not in the banned list anymore
        BannedUsers.Remove(player);
    }
    
    public static void KickPlayer(PlayerInfo playerInfo)
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;
        
        for (var id = 0; id < playerControllers.Length; id++)
        {
            var controller = playerControllers[id];
            if (controller.playerUsername != playerInfo.Username
                || controller.playerSteamId != playerInfo.SteamID) continue; // Both username and steamID need to match
            
            StartOfRound.Instance.KickPlayer(id);
            
            LethalLogger.AddLog(new Log(
                $"[Kick] {controller.playerUsername} ({controller.playerSteamId}@steam) has been kicked (id={id})"
            ));
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