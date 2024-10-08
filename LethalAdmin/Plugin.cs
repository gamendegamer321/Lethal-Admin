﻿using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalAdmin.Bans;
using LethalAdmin.Patches;

namespace LethalAdmin
{
    [BepInPlugin("gamendegamer.lethaladmin", "Lethal Admin", PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginVersion = "1.9.2";

        public static Plugin Instance;
        public static string ConfigFolder;

#if DEBUG
        public static bool DebugMode = true;
#else
        public static bool DebugMode = false;
#endif

        private readonly Harmony _harmony = new("LethalAdmin");
        private const string ConfigSection = "Lethal Admin";

        private ConfigEntry<string> _bans;
        private ConfigEntry<int> _minVotesConfig;
        private ConfigEntry<bool> _lockLeverConfig;
        private ConfigEntry<bool> _requireSteam;
        private ConfigEntry<bool> _furnitureLocked;
        private ConfigEntry<int> _buttonHeight;
        private ConfigEntry<bool> _openUIOnStart;

        public int MinVotes
        {
            get => _minVotesConfig.Value;
            set
            {
                _minVotesConfig.Value = value;
                Config.Save();
            }
        }

        public bool LockLever
        {
            get => _lockLeverConfig.Value;
            set
            {
                _lockLeverConfig.Value = value;
                Config.Save();
            }
        }

        public bool RequireSteam
        {
            get => _requireSteam.Value;
            set
            {
                _requireSteam.Value = value;
                Config.Save();
            }
        }

        public bool FurnitureLocked
        {
            get => _furnitureLocked.Value;
            set
            {
                _furnitureLocked.Value = value;
                Config.Save();
            }
        }

        public int ButtonHeight
        {
            get => _buttonHeight.Value;
            set
            {
                _buttonHeight.Value = value;
                Config.Save();
            }
        }

        public bool OpenUIOnStart
        {
            get => _openUIOnStart.Value;
            set
            {
                _openUIOnStart.Value = value;
                Config.Save();
            }
        }

        private void Awake()
        {
            Logger.LogInfo("Starting Lethal Admin");
            _harmony.PatchAll(typeof(BuildingPatch));
            _harmony.PatchAll(typeof(ConnectionPatch));
            _harmony.PatchAll(typeof(RoundPatch));
            _harmony.PatchAll(typeof(MenuPatch));
            _harmony.PatchAll(typeof(ControllerPatch));
            _harmony.PatchAll(typeof(VotingPatch));

            Instance = this;
            _bans = Config.Bind(ConfigSection, "bans", "",
                "The steam IDs of all banned players, comma seperated. [deprecated]");
            _minVotesConfig = Config.Bind(ConfigSection, "minVotes", 1,
                "The minimum amount of votes before the autopilot starts. Use a value of 1 to disable.");
            _lockLeverConfig = Config.Bind(ConfigSection, "leverLock", false,
                "When enabled (true) the ship departure lever can only be used by the host.");
            _requireSteam = Config.Bind(ConfigSection, "requireSteam", true,
                "When enabled, clients attempting to join without a valid steamID will be denied.");
            _furnitureLocked = Config.Bind(ConfigSection, "furnitureLocked", false,
                "When enabled, this will only allow the host to move the furniture. " +
                "This does NOT prevent furniture from being taken out of storage");
            _buttonHeight = Config.Bind(ConfigSection, "buttonHeight", 200,
                "The height the open UI button appears at in the pause menu. Recommended to change from within the game.");
            _openUIOnStart = Config.Bind(ConfigSection, "openUIOnStart", true,
                "Whether to automatically show the UI when the user goes to the pause menu the first time.");

            ConfigFolder = Path.GetDirectoryName(Config.ConfigFilePath);

            BanHandler.LoadBans();
            if (_bans.Value != "Deprecated!")
            {
                var bansList = _bans.Value.Split(",");
                BanHandler.LoadDeprecated(bansList);
                _bans.Value = "Deprecated!";
                Config.Save();
            }

            Logger.LogInfo("Finished starting Lethal Admin");
        }
    }
}