using UnityEngine;

namespace LethalAdmin.UI;

public static class UsersView
{
    private static KickBanTools.PlayerInfo _selectedPlayer;
    private static string _banReason;
    
    public static void DrawView()
    {
        if (_selectedPlayer == null)
        {
            DrawUserList();
            return;
        }
        
        DrawUserInfo();
    }

    private static void DrawUserList()
    {
        GUILayout.Label("Yellow - Not connected", LethalAdminUI.YellowText, LethalAdminUI.LabelOptions);
        GUILayout.Label("Red - Dead", LethalAdminUI.RedText, LethalAdminUI.LabelOptions);
        GUILayout.Space(10);

        var players = KickBanTools.GetPlayers();
        var id = 0;

        foreach (var player in players)
        {
            GUILayout.BeginHorizontal();

            if (player.Connected || player.SteamID != 0)
            {
                GUILayout.Label($"({id}) {player}",
                    player.IsPlayerDead ? LethalAdminUI.RedText : LethalAdminUI.WhiteText, LethalAdminUI.LabelOptions);
                
                GUILayout.Label($"Walkie: {player.WalkieMode}", LethalAdminUI.LabelOptions);
                
                if (id != 0) // No need to view info on the owner
                {
                    if (GUILayout.Button("View info"))
                    {
                        _banReason = "No reason given";
                        _selectedPlayer = player;
                    }
                }
            }
            else
            {
                GUILayout.Label($"({id}) {player}", LethalAdminUI.YellowText, LethalAdminUI.LabelOptions);
            }

            GUILayout.EndHorizontal();

            id++;
        }
    }

    private static void DrawUserInfo()
    {
        if (GUILayout.Button("<- Back to users list"))
        {
            _selectedPlayer = null;
            return;
        }
        
        GUILayout.Space(20);
        GUILayout.Label($"User: {_selectedPlayer.Username} ({_selectedPlayer.SteamID}@steam)");
        GUILayout.Label($"Walkie talkie: {_selectedPlayer.WalkieMode}");
        GUILayout.Space(20);
        
        if (GUILayout.Button("Profile"))
        {
            KickBanTools.ShowProfile(_selectedPlayer.Username, _selectedPlayer.SteamID);
        }
        
        if (GUILayout.Button("Kick"))
        {
            KickBanTools.KickPlayer(_selectedPlayer);
        }

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        _banReason = GUILayout.TextField(_banReason, LethalAdminUI.WideLabelOptions);
        
        if (GUILayout.Button("Ban"))
        {
            KickBanTools.BanPlayer(_selectedPlayer, _banReason);
        }
        GUILayout.EndHorizontal();
    }
}