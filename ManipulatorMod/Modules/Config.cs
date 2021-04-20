﻿using BepInEx.Configuration;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<KeyCode> restKeybind;
        public static ConfigEntry<KeyCode> danceKeybind;

        public static void ReadConfig()
        {
            restKeybind = ManipulatorPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Rest"), KeyCode.Alpha1, new ConfigDescription("Keybind used to perform the Rest emote"));
            danceKeybind = ManipulatorPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Dance"), KeyCode.Alpha3, new ConfigDescription("Keybind used to perform the Dance emote"));
        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }

        internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
        {
            return ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this enemy"));
        }
    }
}