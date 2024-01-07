using LethalAdmin.Bans;
using UnityEngine;

namespace LethalAdmin.UI;

public static class BanView
{
    public static void DrawView()
    {
        foreach (var banInfo in BanHandler.Bans.Values)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{banInfo.Username} ({banInfo.SteamID}@steam), reason: {banInfo.BanReason}",
                LethalAdminUI.LabelOptions);

            if (GUILayout.Button("Profile"))
            {
                KickBanTools.ShowProfile(banInfo.Username, banInfo.SteamID);
            }

            if (GUILayout.Button("Unban"))
            {
                KickBanTools.UnbanPlayer(banInfo.SteamID);
            }

            GUILayout.EndHorizontal();
        }
    }
}