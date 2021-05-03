using BepInEx.Configuration;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    public static class Config
    {
        //General
        public static ConfigEntry<bool> characterEnabled;

        //Skills
        public static ConfigEntry<float> jetDuration;
        public static ConfigEntry<float> jetBoostAmount;
        public static ConfigEntry<bool> useAttackSlow;
        public static ConfigEntry<float> attackSlowMulti;

        //Keybinds
        public static ConfigEntry<KeyCode> restKeybind;
        public static ConfigEntry<KeyCode> danceKeybind;

        public static void ReadConfig()
        {
            //Template
            //ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("section", "name"), false, new ConfigDescription("description"));

            //General
            characterEnabled = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("General", "Manipulator Enabled"), true, new ConfigDescription("Set to false to disable Manipulator."));

            //Skills
            jetDuration = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Hover Duration"), 3f, new ConfigDescription("How long should the ACE Pack's hover last?"));
            jetBoostAmount = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Hover Speed Boost"), 1.45f, new ConfigDescription("How much does the ACE Pack's hover speed boost multiply your speed?"));
            useAttackSlow = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Skills", "Chill Attack Slow"), false, new ConfigDescription("Should Ice Element's Chill debuff also reduce attack speed?"));
            attackSlowMulti = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Chill Attack Slow Amount"), 0.75f, new ConfigDescription("How much does the maximum Chill debuff reduce attack speed?"));

            //Emote
            //restKeybind = ManipulatorPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Rest"), KeyCode.Alpha1, new ConfigDescription("Keybind used to perform the Rest emote"));
            //danceKeybind = ManipulatorPlugin.instance.Config.Bind<KeyCode>(new ConfigDefinition("Keybinds", "Dance"), KeyCode.Alpha3, new ConfigDescription("Keybind used to perform the Dance emote"));
        }
    }
}