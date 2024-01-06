using System;
using System.Collections.Generic;
using LethalAdmin.Bans;
using LethalAdmin.Logging;
using UnityEngine;

namespace LethalAdmin.UI;

public class LethalAdminUI : MonoBehaviour
{
    private static readonly List<LethalAdminUI> Instances = new();
    private bool _menuOpen;

    private Rect _windowRect;

    private readonly GUILayoutOption[] _options =
    {
        GUILayout.Width(900),
        GUILayout.Height(400)
    };

    private readonly GUILayoutOption[] _labelOptions =
    {
        GUILayout.MinWidth(300)
    };

    /***
     * Set up base style initializers to be used in the code later
     * moved from readonly to rw to utilize the GUI class
     */
    private static GUIStyle _redText = new();
    private static GUIStyle _whiteText = new();
    private static GUIStyle _yellowText = new();
    private static bool _guiPrepared;

    // Instantiate toolbar settings, this can be done later though.
    private int _toolbarInt;
    private readonly string[] _toolbarStrings = { "Users", "Settings & Logs", "Bans" };

    private Vector2 _scrollPosition;
    private bool _menuAlwaysOpen;

    private string _minVotes = Plugin.Instance.MinVotes.ToString();
    private bool _leverLocked = Plugin.Instance.LockLever;
    private string _settingsErrorMessage = "";

    private void Awake()
    {
        Instances.Add(this);
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }

    public static void SetMenuForAll(bool value)
    {
        if (Instances.Count == 0 && value)
        {
            // Create new UI if we want to make it visible, but there exists none yet
            var obj = new GameObject("Lethal Admin UI");
            obj.AddComponent<LethalAdminUI>();
        }

        foreach (var instance in Instances)
        {
            instance.SetMenu(value);
        }
    }

    private void SetMenu(bool value)
    {
        _menuOpen = value;
    }

    /**
     * Move style assignment to PrepareGui when we have access to the GUI class
     *
     * Also makes it easier to style things globally.
     * It also has sane bases with proper padding
     */
    private static void PrepareGui()
    {
        _yellowText = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.yellow },
        };

        _redText = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.red },
        };

        _whiteText = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.white },
        };

        GUI.skin.toggle = new GUIStyle(GUI.skin.toggle)
        {
            stretchWidth = false,
        };
    }

    private void OnGUI()
    {
        // run once
        if (!_guiPrepared)
        {
            _guiPrepared = true;
            PrepareGui();
        }

        if (!StartOfRound.Instance.IsServer || (!_menuOpen && !_menuAlwaysOpen)) return;

        var controlID = GUIUtility.GetControlID(FocusType.Passive);
        _windowRect = GUILayout.Window(controlID, _windowRect, DrawUI, "Lethal Admin Menu V" + Plugin.PluginVersion,
            _options);
    }

    private void DrawUI(int windowID)
    {
        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);

        switch (_toolbarInt)
        {
            case 0:
                DrawUsers();
                break;
            case 1:
                DrawSettings();
                break;
            case 2:
                DrawBans();
                break;
            default:
                DrawUsers();
                break;
        }

        GUILayout.EndScrollView();

        // GUILayout.FlexibleSpace(); // Possible flexbox space
        _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarStrings);
        // GUILayout.FlexibleSpace(); // Same as above

        // Should add a Lock Ship light switch somewhere
        if (GUILayout.Button("Toggle ship lights"))
        {
            StartOfRound.Instance.shipRoomLights.ToggleShipLights();
        }

        if (StartOfRound.Instance.connectedPlayersAmount + 1 - StartOfRound.Instance.livingPlayers >= 1 &&
            !TimeOfDay.Instance
                .shipLeavingAlertCalled) // Requires at least 1 dead player and that there has not been any early leave call
        {
            if (GUILayout.Button("Override vote (will trigger auto pilot)"))
            {
                var time = TimeOfDay.Instance;
                time.votesForShipToLeaveEarly =
                    Math.Max(StartOfRound.Instance.connectedPlayersAmount, Plugin.Instance.MinVotes);
                time.votedShipToLeaveEarlyThisRound = false; // Make sure the game is convinced we didn't vote yet
                time.VoteShipToLeaveEarly(); // Trigger the vote
            }
        }

        _menuAlwaysOpen = GUILayout.Toggle(_menuAlwaysOpen, "Always show menu");
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, 10000, 500));
    }

    private void DrawUsers()
    {
        GUILayout.Label("NotConnected", _yellowText, _labelOptions);
        GUILayout.Label("IsDead", _redText, _labelOptions);

        var players = KickBanTools.GetPlayers();
        var id = 0;

        foreach (var player in players)
        {
            GUILayout.BeginHorizontal();

            if (player.Connected || player.SteamID != 0)
            {
                GUILayout.Label($"({id}) {player}", player.IsPlayerDead ? _redText : _whiteText, _labelOptions);
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
                GUILayout.Label($"({id}) {player}", _yellowText, _labelOptions);
            }

            GUILayout.EndHorizontal();

            id++;
        }
    }

    private void DrawSettings()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Minimum departure votes: ");
        _minVotes = GUILayout.TextField(_minVotes);
        GUILayout.EndHorizontal();

        _leverLocked = GUILayout.Toggle(_leverLocked, "Only owner can start ship");

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
                    _settingsErrorMessage = "";
                }
            }
            catch (FormatException)
            {
                _minVotes = Plugin.Instance.MinVotes.ToString();
                _settingsErrorMessage = "New minimum departure votes is not a valid integer.";
            }
        }

        GUILayout.Label(_settingsErrorMessage, _yellowText);

        var logs = LethalLogger.GetLogs();

        GUILayout.Label("Logs:");

        foreach (var log in logs)
        {
            GUILayout.Label(log.GetTimeFormattedString());
        }
    }

    private void DrawBans()
    {
        foreach (var banInfo in BanHandler.Bans.Values)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{banInfo.Username} ({banInfo.SteamID}@steam), reason: {banInfo.BanReason}", _labelOptions);

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