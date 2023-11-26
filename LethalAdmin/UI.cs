using System.Collections.Generic;
using UnityEngine;

namespace LethalAdmin;

public class UI : MonoBehaviour
{
    private static readonly List<UI> Instances = new();
    private bool _menuOpen = false;

    private Rect _windowRect;

    private GUILayoutOption[] _options = new GUILayoutOption[]
    {
        GUILayout.Width(800),
        GUILayout.Height(400)
    };

    private Vector2 scrollPosition;
    private bool toggleToggled;

    public void Awake()
    {
        Instances.Add(this);
    }

    public void OnDestroy()
    {
        Instances.Remove(this);
    }

    public static void SetMenuForAll(bool value)
    {
        if (Instances.Count == 0 && value)
        {
            // Create new UI if we want to make it visible, but there exists none yet
            var obj = new GameObject("Lethal Admin UI");
            obj.AddComponent<UI>();

            Plugin.Instance.LogInfo("Creating Admin UI instance");
        }

        foreach (var instance in Instances)
        {
            instance.SetMenu(value);
        }
    }

    public void SetMenu(bool value)
    {
        _menuOpen = value;
    }

    private void OnGUI()
    {
        if (!StartOfRound.Instance.IsServer || !_menuOpen) return;

        var controlID = GUIUtility.GetControlID(FocusType.Passive);
        _windowRect = GUILayout.Window(controlID, _windowRect, DrawUI, "Lethal Admin Menu", _options);
    }

    private void DrawUI(int windowID)
    {
        var players = KickBanTools.GetPlayers();
        
        GUILayout.BeginVertical();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

        foreach (var player in players)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label((player.UsingWalkie ? "[X] " : "[ ] ") + player.Username);

            if (GUILayout.Button("Kick"))
            {
                KickBanTools.KickPlayer(player.Username);
            }
            
            if (GUILayout.Button("Ban"))
            {
                KickBanTools.BanPlayer(player.Username);
            }
            
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear"))
        {
        }

        if (GUILayout.Button("Clear Control Locks"))
        {
        }

        GUILayout.EndHorizontal();
        toggleToggled = (GUILayout.Toggle(toggleToggled, "toggle"));
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, 10000, 500));
    }
}