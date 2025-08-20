using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LethalAdmin.Bans;
using LethalAdmin.Logging;
using Steamworks;
using Unity.Netcode;

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
        var result = playerControllers.Select((player, i) => new PlayerInfo
        {
            Username = player.playerUsername,
            SteamID = player.playerSteamId,
            ConnectionState = StartOfRound.Instance.fullyLoadedPlayers.Contains((ulong)i)
                ? ConnectionState.Connected
                : ConnectionState.Disconnected,
            WalkieMode = GetWalkieMode(player),
            IsPlayerDead = player.isPlayerDead,
            PlayerController = player
        }).ToList();

        // Any IDs not in the list, add them here
        foreach (var id in ConnectionTracker.SteamIds.Where(x => result.All(y => x.Value != y.SteamID)))
        {
            result.Add(new PlayerInfo
            {
                Username = "Player without script",
                SteamID = id,
                ConnectionState = ConnectionTracker.SteamIdsToNetworkIds.ContainsKey(id.Value)
                    ? ConnectionState.OnlySteamConnected
                    : ConnectionState.Disconnected,
                WalkieMode = WalkieMode.Disabled,
                IsPlayerDead = false
            });
        }

        return result;
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

        var kicked = false;
        for (var id = 0; id < playerControllers.Length; id++)
        {
            var controller = playerControllers[id];
            if (controller.playerSteamId != playerInfo.SteamID) continue;

            StartOfRound.Instance.KickPlayer(id);
            LethalLogger.AddLog(new Log($"[Kick] {playerInfo} has been kicked (id={id})"));
            kicked = true;
        }

        if (kicked || playerInfo.SteamID == 0 || !Plugin.Instance.SteamChecker) return;

        LethalLogger.AddLog(new Log($"[Kick] {playerInfo} could not be kicked, no matching player controller"));
        if (playerInfo.ConnectionState == ConnectionState.OnlySteamConnected)
        {
            if (ConnectionTracker.SteamIdsToNetworkIds.TryGetValue(playerInfo.SteamID, out var connectionId))
            {
                LethalLogger.AddLog(new Log(
                    $"[Kick] Only a steam connection found, attempting to force stop the connection! (con. id={connectionId})"
                ));
                NetworkManager.Singleton.DisconnectClient(connectionId);
            }
            else
            {
                LethalLogger.AddLog(new Log(
                    "[Kick] Could not find the connection associated with the steam ID, kicking is not possible for this user!"
                ));
            }
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
        public ConnectionState ConnectionState;
        public WalkieMode WalkieMode;
        public bool IsPlayerDead;
        public PlayerControllerB PlayerController;

        public override string ToString()
        {
            return $"{Username} ({SteamID}@steam)";
        }
    }
}