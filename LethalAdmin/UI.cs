using System.Collections.Generic;
using UnityEngine;

namespace LethalAdmin;

public class UI : MonoBehaviour
{
    private static readonly List<UI> Instances = new();
    private bool _menuOpen = false;

    private Vector2 _menuSize = new(800, 400);
    private Vector2 _menuPos;
    
    public void Awake()
    {
        Instances.Add(this);

        _menuPos = new Vector2(Screen.width, Screen.height) - _menuSize / 2f;
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
    
    private void OnGui()
    {
        if (StartOfRound.Instance.IsServer && _menuOpen) return;
        GUI.Box(new Rect(3f, 3f, 800f, 400f), "Admin Menu", GUI.skin.box);
    }
}