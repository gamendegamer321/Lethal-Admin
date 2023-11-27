using HarmonyLib;

namespace LethalAdmin.Patches;

[HarmonyPatch(typeof(QuickMenuManager))]
public class MenuPatch
{
    [HarmonyPatch("OpenQuickMenu")]
    [HarmonyPostfix]
    public static void OnOpenMenu()
    {
        UI.SetMenuForAll(true);
    }
    
    [HarmonyPatch("CloseQuickMenu")]
    [HarmonyPostfix]
    public static void OnCloseMenu()
    {
        UI.SetMenuForAll(false);
    }
}