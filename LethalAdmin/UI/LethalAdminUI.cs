using System.Linq;
using BepInEx.Logging;
using LethalAdmin.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = BepInEx.Logging.Logger;

namespace LethalAdmin.UI;

public class LethalAdminUI : MonoBehaviour
{
    private static readonly ManualLogSource ManualLogger = Logger.CreateLogSource("Admin UI");

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

    private static bool _guiMinimized;
    private static bool _guiEnabled = true;

    private static LethalAdminUI _instance;

    private static bool _menuButtonFailed;
    private static RectTransform _menuButtonTransform;

    private int _toolbarInt;
    private bool _menuOpen;
    private Rect _windowRect;

    private readonly IView[] _toolbarViews =
    [
        new UsersView(),
        new SettingsView(),
        new BanView(),
        new WhitelistView()
    ];

    private readonly string[] _toolbarStrings;
    private Vector2 _scrollPosition;
    private bool _menuAlwaysOpen;

    public LethalAdminUI()
    {
        // Get all view names as an array
        _toolbarStrings = _toolbarViews.Select(view => view.GetViewName()).ToArray();
    }

    private void Awake()
    {
        if (_instance != this)
        {
            Destroy(_instance);
        }

        _instance = this;
        _guiEnabled = Plugin.Instance.OpenUIOnStart;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public static void SetMenuForAll(bool value)
    {
        if (!_instance)
        {
            // Create new UI if we want to make it visible, but there exists none yet
            var obj = new GameObject("Lethal Admin UI");
            obj.AddComponent<LethalAdminUI>();

            ManualLogger.LogInfo("Generating UI");
        }

        _instance._menuOpen = value;
    }

    public static void UpdateButtonHeight(int newHeight)
    {
        var localPosition = _menuButtonTransform.localPosition;
        _menuButtonTransform.localPosition = new Vector3(localPosition.x, newHeight, localPosition.z);
    }

    private void OnGUI()
    {
        // Only show when you are the server (or are in debug mode)
        if (!StartOfRound.Instance.IsServer && !Plugin.DebugMode) return;

        // run if there is no button yet
        if (!_menuButtonTransform && !_menuButtonFailed)
        {
            PrepareGui();
        }

        if ((!_menuOpen && !_menuAlwaysOpen) || !_guiEnabled) return;

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

        // Draw the selected toolbar view
        _toolbarViews[_toolbarInt].DrawView();

        GUILayout.FlexibleSpace(); // Fill box to the bottom
        GUILayout.EndScrollView();

        DefaultUI();

        GUILayout.EndVertical();
    }

    private void DefaultUI()
    {
        _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarStrings);
        GUILayout.Space(10);

        if (GUILayout.Button("Log debug information"))
        {
            LethalLogger.AddLog(new Log(
                $"disableSteam: {GameNetworkManager.Instance.disableSteam}, transport: {GameNetworkManager.Instance.transport != null}"
            ));
        }

        if (GUILayout.Button("Toggle ship lights"))
        {
            StartOfRound.Instance.shipRoomLights.ToggleShipLights();
        }

        if (RoundUtils.IsVoteOverrideAvailable() && GUILayout.Button("Override vote (will trigger auto pilot)"))
        {
            RoundUtils.OverrideVotes();
        }

        GUILayout.Space(20);
        _menuAlwaysOpen = GUILayout.Toggle(_menuAlwaysOpen, "Always show menu");

        if (GUILayout.Button("Minimize UI"))
        {
            _guiMinimized = true;
            _menuAlwaysOpen = false;
        }

        if (GUILayout.Button("Close UI")) _guiEnabled = false;
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

        // Clone one of the menu buttons
        var parent = StartOfRound.Instance.localPlayerController.quickMenuManager.mainButtonsPanel.transform;
        var newButton =
            (from Transform child in parent where child.name == "Resume" select Instantiate(child.gameObject, parent))
            .FirstOrDefault();

        // Make sure everything can be found
        if (newButton == null || !newButton.TryGetComponent<Button>(out var buttonComponent)
                              || !newButton.TryGetComponent<RectTransform>(out _menuButtonTransform))
        {
            FailedUI(newButton);
            return;
        }

        var text = newButton.GetComponentInChildren<TextMeshProUGUI>();

        // Make sure we also got a text component in the children
        if (text == null)
        {
            FailedUI(newButton);
            return;
        }

        // Set the info for the new button
        text.text = "> Open Admin UI";
        buttonComponent.onClick = new Button.ButtonClickedEvent();
        buttonComponent.onClick.AddListener(() => { _guiEnabled = true; });

        UpdateButtonHeight(Plugin.Instance.ButtonHeight);
    }

    private static void FailedUI(GameObject obj)
    {
        _menuButtonFailed = true;
        LethalLogger.AddLog(new Log("Failed to create the UI button, restart your game!"));
        ManualLogger.LogWarning("Could not find all components to create new button!");
        Destroy(obj);
    }
}