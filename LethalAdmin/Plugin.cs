using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalAdmin.Patches;

namespace LethalAdmin
{
    [BepInPlugin("gamendegamer.lethaladmin", "Lethal Admin", "1.0.5")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new("LethalAdmin");
        public static Plugin Instance;

        private const string ConfigSection = "Lethal Admin";

        private ConfigEntry<string> _bans;
        private ConfigEntry<int> _minVotesConfig;

        public int MinVotes
        {
            get => _minVotesConfig.Value;
            set
            {
                _minVotesConfig.Value = value;
                Config.Save();
            }
        }

        private void Awake()
        {
            Logger.LogInfo("Starting Lethal Admin");
            _harmony.PatchAll(typeof(RoundPatch));
            _harmony.PatchAll(typeof(MenuPatch));
            _harmony.PatchAll(typeof(ControllerPatch));
            _harmony.PatchAll(typeof(VotingPatch));

            Instance = this;
            _bans = Config.Bind(ConfigSection, "bans", "",
                "The steam IDs of all banned players, comma seperated.");
            _minVotesConfig = Config.Bind(ConfigSection, "minVotes", 1,
                "The minimum amount of votes before the autopilot starts. Use a value of 1 to disable.");
            
            LoadConfigBans();

            Logger.LogInfo("Finished starting Lethal Admin");
        }

        private void LoadConfigBans()
        {
            var bansList = _bans.Value.Split(",");
            var bannedPlayers = new List<KickBanTools.PlayerInfo>();

            foreach (var id in bansList)
            {
                if (!ulong.TryParse(id, out var idValue)) continue;
                bannedPlayers.Add(new KickBanTools.PlayerInfo { SteamID = idValue, Username = "UNKNOWN" });
            }

            KickBanTools.SetBannedPLayers(bannedPlayers);
        }

        internal void AddConfigBan(ulong value)
        {
            if (_bans.Value.Length != 0) _bans.Value += ",";
            _bans.Value += value;

            Config.Save();
        }

        internal void RemoveConfigBan(ulong value)
        {
            var bansList = _bans.Value.Split(",");
            var newBansList = new StringBuilder();

            foreach (var ban in bansList)
            {
                if (!ulong.TryParse(ban, out var id)) continue;
                if (id == value) continue;
                if (newBansList.Length != 0) newBansList.Append(",");
                newBansList.Append(id);
            }

            _bans.Value = newBansList.ToString();
            Config.Save();
        }

        internal void LogInfo(string message)
        {
            Logger.LogInfo(message);
        }
    }
}