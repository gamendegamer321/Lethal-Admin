using System;
using LethalAdmin.Logging;
using UnityEngine;

namespace LethalAdmin.UI;

public static class SettingsView
{
    private static string _minVotes = Plugin.Instance.MinVotes.ToString();
    private static bool _leverLocked = Plugin.Instance.LockLever;
    private static bool _requireSteam = Plugin.Instance.RequireSteam;
    
    private static string _settingsErrorMessage = "";
    
    public static void DrawView()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Minimum departure votes: ");
        _minVotes = GUILayout.TextField(_minVotes);
        GUILayout.EndHorizontal();

        _leverLocked = GUILayout.Toggle(_leverLocked, "Only owner can start ship");
        _requireSteam = GUILayout.Toggle(_requireSteam, "Require valid steam ID");

        if (GUILayout.Button("Apply settings"))
        {
            try
            {
                var newMinVotes = int.Parse(_minVotes);

                if (newMinVotes < 1)
                {
                    _minVotes = Plugin.Instance.MinVotes.ToString();
                    _settingsErrorMessage = "Minimum departure votes can not be negative.";
                }
                else
                {
                    Plugin.Instance.MinVotes = newMinVotes;
                    Plugin.Instance.LockLever = _leverLocked;
                    Plugin.Instance.RequireSteam = _requireSteam;
                    _settingsErrorMessage = "Successfully saved the settings!";
                }
            }
            catch (FormatException)
            {
                _minVotes = Plugin.Instance.MinVotes.ToString();
                _settingsErrorMessage = "New minimum departure votes is not a valid integer.";
            }
        }

        GUILayout.Label(_settingsErrorMessage, LethalAdminUI.YellowText);

        var logs = LethalLogger.GetLogs();

        GUILayout.Label("Logs:");

        foreach (var log in logs)
        {
            GUILayout.Label(log.GetTimeFormattedString());
        }
    }
}