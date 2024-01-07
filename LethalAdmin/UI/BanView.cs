using LethalAdmin.Bans;
using UnityEngine;

namespace LethalAdmin.UI;

public static class BanView
{
    private static BanInfo _selectedBan;
    
    public static void DrawView()
    {
        if (_selectedBan == null)
        {
            DrawBanList();
            return;
        }
        
        DrawBanInfo();
    }

    private static void DrawBanList()
    {
        foreach (var banInfo in BanHandler.Bans.Values)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{banInfo.Username} ({banInfo.SteamID}@steam)", LethalAdminUI.WideLabelOptions);

            if (GUILayout.Button("View ban info"))
            {
                _selectedBan = banInfo;
            }
            
            GUILayout.EndHorizontal();
        }
    }

    private static void DrawBanInfo()
    {
        if (GUILayout.Button("<- Back to ban list"))
        {
            _selectedBan = null;
            return;
        }
        
        GUILayout.Space(20);
        GUILayout.Label($"User: {_selectedBan.Username} ({_selectedBan.SteamID}@steam)");
        GUILayout.Label($"Ban reason: {_selectedBan.BanReason}");
        GUILayout.Space(20);

        if (GUILayout.Button("Unban user"))
        {
            BanHandler.RemoveBan(_selectedBan.SteamID);
            _selectedBan = null;
            return;
        }

        if (GUILayout.Button("Open steam profile"))
        {
            KickBanTools.ShowProfile(_selectedBan.Username, _selectedBan.SteamID);
        }
    }
}