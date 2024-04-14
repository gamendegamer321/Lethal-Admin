using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LethalAdmin.Bans;
using LethalAdmin.Logging;
using Steamworks;

namespace LethalAdmin;

public static class KickBanTools
{
    public static void UpdateKickedIDs()
    {
        // Still keep this so a mod that breaks the ConnectionPatch still uses the bans
        if (StartOfRound.Instance == null) return;

        StartOfRound.Instance.KickedClientIds.Clear();
        foreach (var ban in BanHandler.GetBans())
        {
            StartOfRound.Instance.KickedClientIds.Add(ban.SteamID);
        }
    }

    public static List<PlayerInfo> GetPlayers()
    {
        var playerControllers = StartOfRound.Instance.allPlayerScripts;

        return playerControllers.Select((player, i) => new PlayerInfo
        {
            Username = player.playerUsername,
            SteamID = player.playerSteamId,
            Connected = StartOfRound.Instance.fullyLoadedPlayers.Contains((ulong)i),
            WalkieMode = GetWalkieMode(player),
            IsPlayerDead = player.isPlayerDead
        }).ToList();
    }

    public static void BanPlayer(PlayerInfo playerInfo, string reason = null)
    {
        if (playerInfo.SteamID == 0) // Steam ID as 0 means there is no player or steam is not used
        {
            LethalLogger.AddLog(new Log(
                $"[Ban] {playerInfo} is not a player or steam is disabled, when not using steam bans are not possible!",
                "Error"
            ));
            return;
        }

        if (BanHandler.AddBan(playerInfo.SteamID, playerInfo.Username, reason))
        {
            LethalLogger.AddLog(new Log(
                $"[Ban] {playerInfo} has been banned"
            ));
        }
        else
        {
            LethalLogger.AddLog(new Log(
                $"[Ban] Could not ban {playerInfo} as this user is already banned", "Warning"
            ));
        }

        KickPlayer(playerInfo);
    }

    public static void UnbanPlayer(ulong steamID)
    {
        if (BanHandler.RemoveBan(steamID))
        {
            LethalLogger.AddLog(new Log($"[Ban] {steamID}@steam has been unbanned"));
        }
        else
        {
            LethalLogger.AddLog(new Log(
                $"[Ban] Could not unban {steamID}@steam as this user is not banned", "Warning"
            ));
        }

        StartOfRound.Instance.KickedClientIds.Remove(steamID); // Make sure it's not in the banned list anymore
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
            LethalLogger.AddLog(new Log($"[Kick] {playerInfo} has been kicked (id={id})"));
        }
    }

    public static void ShowProfile(string username, ulong steamId)
    {
        SteamFriends.OpenUserOverlay(steamId, "steamid");
        LethalLogger.AddLog(new Log($"[ProfileCheck] {username} ({steamId}@steam))"));
    }

    private static WalkieMode GetWalkieMode(PlayerControllerB player)
    {
        var mode = WalkieMode.Disabled;

        if (player.holdingWalkieTalkie) mode = WalkieMode.Listening;
        if (player.speakingToWalkieTalkie) mode = WalkieMode.Talking;

        return mode;
    }

    public class PlayerInfo
    {
        public string Username;
        public ulong SteamID;
        public bool Connected;
        public WalkieMode WalkieMode;
        public bool IsPlayerDead;

        public override string ToString()
        {
            return $"{Username} ({SteamID}@steam)";
        }
    }
}