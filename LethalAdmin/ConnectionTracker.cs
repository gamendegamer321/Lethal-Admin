using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;

namespace LethalAdmin;

public static class ConnectionTracker
{
    internal static readonly List<SteamId> SteamIds = [];
    internal static readonly Dictionary<ulong, ulong> SteamIdsToNetworkIds = new();

    public static void RegisterEvents()
    {
        SteamMatchmaking.OnLobbyMemberJoined += OnJoinedLobby;
        SteamMatchmaking.OnLobbyMemberLeave += OnLeftLobby;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnect;
    }

    private static void OnJoinedLobby(Lobby lobby, Friend friend)
    {
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
        try
        {
            if (SteamIdsToNetworkIds.TryGetValue(friend.Id, out var connectionId))
            {
                NetworkManager.Singleton.DisconnectClient(connectionId);
            }
        }
        catch (Exception)
        {
            // Just make sure there isn't an exception here, as we need to be sure the code below runs
        }

        SteamIds.Remove(friend.Id);
        SteamIdsToNetworkIds.Remove(friend.Id);
    }

    private static void ClientDisconnect(ulong connectionId)
    {
        foreach (var pair in SteamIdsToNetworkIds)
        {
            if (pair.Value == connectionId)
            {
                SteamIds.Remove(pair.Key);
                SteamIdsToNetworkIds.Remove(pair.Key);
            }
        }
    }
}