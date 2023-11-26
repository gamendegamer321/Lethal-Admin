using BepInEx;
using HarmonyLib;
using LethalAdmin.Patches;

namespace LethalAdmin
{
    [BepInPlugin("gamendegamer.lethaladmin", "Lethal Admin", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony("LethalAdmin");
        public static Plugin Instance;
        
        private void Awake()
        {
            Logger.LogInfo("Starting Lethal Admin");
            _harmony.PatchAll(typeof(RoundPatch));
            _harmony.PatchAll(typeof(MenuPatch));

            Instance = this;
            
            Logger.LogInfo("Finished starting Lethal Admin");
        }

        public void LogInfo(string message)
        {
            Logger.LogInfo(message);
        }
    }
}