using HarmonyLib;
using LethalAdmin.UI;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(QuickMenuManager))]
public class MenuPatch
{
    [HarmonyPatch("OpenQuickMenu")]
    [HarmonyPostfix]
    public static void OnOpenMenu()
    {
        LethalAdminUI.SetMenuForAll(true);
    }
    
    [HarmonyPatch("CloseQuickMenu")]
    [HarmonyPostfix]
    public static void OnCloseMenu()
    {
        LethalAdminUI.SetMenuForAll(false);
    }
}