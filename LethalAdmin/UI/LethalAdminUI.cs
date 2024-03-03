using System;
using System.Collections.Generic;
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
    
    private readonly GUILayoutOption[] _minimizedOptions =
    {
        GUILayout.Width(300),
        GUILayout.Height(20)
    };

    public static readonly GUILayoutOption[] LabelOptions =
    {
        GUILayout.Width(300)
    };

    public static readonly GUILayoutOption[] WideLabelOptions =
    {
        GUILayout.Width(500)
    };

    public static GUIStyle RedText { get; private set; }
    public static GUIStyle WhiteText { get; private set; }
    public static GUIStyle YellowText { get; private set; }
    private static bool _guiPrepared;
    private static bool _guiMinimized;

    private int _toolbarInt;
    private readonly string[] _toolbarStrings = { "Users", "Settings & Logs", "Bans" };

    private Vector2 _scrollPosition;
    private bool _menuAlwaysOpen;

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
            _guiMinimized ? _minimizedOptions : _options);
    }

    private void DrawUI(int windowID)
    {
        if (_guiMinimized)
        {
            if (GUILayout.Button("Expand UI")) _guiMinimized = false;
        }
        else
        {
            ExpandedUI();
        }

        GUI.DragWindow(new Rect(0, 0, 10000, 500));
    }

    private void ExpandedUI()
    {
        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUI.skin.box);

        switch (_toolbarInt)
        {
            case 0:
                UsersView.DrawView();
                break;
            case 1:
                SettingsView.DrawView();
                break;
            case 2:
                BanView.DrawView();
                break;
            default:
                UsersView.DrawView();
                break;
        }

        GUILayout.FlexibleSpace(); // Fill box to the bottom
        GUILayout.EndScrollView();

        DefaultUI();

        GUILayout.EndVertical();
    }
    
    private void DefaultUI()
    {
        _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarStrings);
        GUILayout.Space(10);

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

        GUILayout.Space(20);
        _menuAlwaysOpen = GUILayout.Toggle(_menuAlwaysOpen, "Always show menu");
        
        if (GUILayout.Button("Minimize UI"))
        {
            _guiMinimized = true;
            _menuAlwaysOpen = false;
        }
    }

    private static void PrepareGui()
    {
        YellowText = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.yellow },
        };

        RedText = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.red },
        };

        WhiteText = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.white },
        };

        GUI.skin.toggle = new GUIStyle(GUI.skin.toggle)
        {
            stretchWidth = false,
        };
    }
}