using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalAdmin.Patches;

namespace LethalAdmin
{
    [BepInPlugin("gamendegamer.lethaladmin", "Lethal Admin", "1.0.4")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new("LethalAdmin");
        public static Plugin Instance;

        private ConfigEntry<string> Bans;

        private void Awake()
        {
            Logger.LogInfo("Starting Lethal Admin");
            _harmony.PatchAll(typeof(RoundPatch));
            _harmony.PatchAll(typeof(MenuPatch));
            _harmony.PatchAll(typeof(ControllerPatch));
            _harmony.PatchAll(typeof(VotingPatch));

            Instance = this;
            Bans = Config.Bind("Lethal Admin", "bans", "", 
                "The steam IDs of all banned players, comma seperated");

            LoadConfigBans();
            
            Logger.LogInfo("Finished starting Lethal Admin");
        }

        private void LoadConfigBans()
        {
            var bansList = Bans.Value.Split(",");
            var bannedPlayers = new List<KickBanTools.PlayerInfo>();
            
            foreach (var id in bansList)
            {
                if (!ulong.TryParse(id, out var idValue)) continue;
                bannedPlayers.Add(new KickBanTools.PlayerInfo { SteamID = idValue, Username = "UNKNOWN"});
            }
            
            KickBanTools.SetBannedPLayers(bannedPlayers);
        }

        internal void AddConfigBan(ulong value)
        {
            if (Bans.Value.Length != 0) Bans.Value += ",";
            Bans.Value += value;
            
            Config.Save();
        }

        internal void RemoveConfigBan(ulong value)
        {
            var bansList = Bans.Value.Split(",");
            var newBansList = new StringBuilder();

            foreach (var ban in bansList)
            {
                if (!ulong.TryParse(ban, out var id)) continue;
                if (id == value) continue;
                if (newBansList.Length != 0) newBansList.Append(",");
                newBansList.Append(id);
            }

            Bans.Value = newBansList.ToString();
            Config.Save();
        }
        
        internal void LogInfo(string message)
        {
            Logger.LogInfo(message);
        }
    }
}