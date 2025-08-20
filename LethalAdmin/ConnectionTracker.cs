using System.Collections.Generic;
using System.Linq;
using LethalAdmin.Logging;
using Steamworks;
using Steamworks.Data;

namespace LethalAdmin;

public static class ConnectionTracker
{
    internal static readonly List<SteamId> SteamIds = [];
    internal static readonly Dictionary<ulong, ulong> SteamIdsToNetworkIds = new();

    public static void RegisterEvents()
    {
        SteamMatchmaking.OnLobbyMemberJoined += OnJoinedLobby;
        SteamMatchmaking.OnLobbyMemberLeave += OnLeftLobby;
    }

    private static void OnJoinedLobby(Lobby lobby, Friend friend)
    {
        LethalLogger.AddLog(new Log($"Friend joined with steamID: {friend.Id}"));
        SteamIds.Clear();
        SteamIds.AddRange(lobby.Members.Select(x => x.Id));

        List<ulong> toDelete = new();
        foreach (var id in SteamIdsToNetworkIds.Keys)
        {
            if (SteamIds.All(x => x.Value != id))
            {
                toDelete.Add(id);
            }
        }

        foreach (var id in toDelete)
        {
            SteamIdsToNetworkIds.Remove(id);
        }
    }

    private static void OnLeftLobby(Lobby lobby, Friend friend)
    {
        LethalLogger.AddLog(new Log($"Friend left with steamID: {friend.Id}"));
        SteamIds.Remove(friend.Id);
        SteamIdsToNetworkIds.Remove(friend.Id);
    }
}