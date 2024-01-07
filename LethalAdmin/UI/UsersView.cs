using UnityEngine;

namespace LethalAdmin.UI;

public static class UsersView
{
    public static void DrawView()
    {
        GUILayout.Label("NotConnected", LethalAdminUI.YellowText, LethalAdminUI.LabelOptions);
        GUILayout.Label("IsDead", LethalAdminUI.RedText, LethalAdminUI.LabelOptions);

        var players = KickBanTools.GetPlayers();
        var id = 0;

        foreach (var player in players)
        {
            GUILayout.BeginHorizontal();

            if (player.Connected || player.SteamID != 0)
            {
                GUILayout.Label($"({id}) {player}",
                    player.IsPlayerDead ? LethalAdminUI.RedText : LethalAdminUI.WhiteText, LethalAdminUI.LabelOptions);
                
                if (id != 0) // Owner should not kick/ban themselves
                {
                    if (GUILayout.Button("Kick"))
                    {
                        KickBanTools.KickPlayer(player);
                    }

                    if (GUILayout.Button("Ban"))
                    {
                        KickBanTools.BanPlayer(player);
                    }

                    if (GUILayout.Button("Profile"))
                    {
                        KickBanTools.ShowProfile(player.Username, player.SteamID);
                    }
                }

                GUILayout.Toggle(player.IsWalkieOn, "WalkieOn");
                GUILayout.Toggle(player.UsingWalkie, "Speaking");
                GUILayout.Toggle(player.IsSpeedCheating, "Cheating");
            }
            else
            {
                GUILayout.Label($"({id}) {player}", LethalAdminUI.YellowText, LethalAdminUI.LabelOptions);
            }

            GUILayout.EndHorizontal();

            id++;
        }
    }
}