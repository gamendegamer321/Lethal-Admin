using HarmonyLib;
using LethalAdmin.UI;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(QuickMenuManager))]
public class MenuPatch
{
    [HarmonyPatch(nameof(QuickMenuManager.OpenQuickMenu))]
    [HarmonyPostfix]
    public static void OnOpenMenu()
    {
        LethalAdminUI.SetMenuForAll(true);
    }
    
    [HarmonyPatch(nameof(QuickMenuManager.CloseQuickMenu))]
    [HarmonyPostfix]
    public static void OnCloseMenu()
    {
        LethalAdminUI.SetMenuForAll(false);
    }
}