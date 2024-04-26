using LethalAdmin.Bans;
using UnityEngine;

namespace LethalAdmin.UI;

public class WhitelistView
{
    public static void DrawView()
    {
        foreach (var whitelist in BanHandler.GetWhitelist())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{whitelist.Username} ({whitelist.SteamID}@steam)", LethalAdminUI.WideLabelOptions);

            if (GUILayout.Button("Steam profile"))
            {
                KickBanTools.ShowProfile(whitelist.Username, whitelist.SteamID);
            }
            
            if (GUILayout.Button("Remove whitelist"))
            {
                BanHandler.RemoveWhitelist(whitelist.SteamID);
            }
            
            GUILayout.EndHorizontal();
        }
    }
}