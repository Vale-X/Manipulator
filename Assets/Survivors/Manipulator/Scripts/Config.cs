using BepInEx.Configuration;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    public static class Config
    {
        // General
        public static ConfigEntry<bool> characterEnabled;
        public static ConfigEntry<bool> forceUnlock;
        public static ConfigEntry<bool> forceUnlockSkills;
        public static ConfigEntry<bool> forceUnlockSkins;

        // Skills
        // Passive
        public static ConfigEntry<float> jetDuration;
        public static ConfigEntry<float> jetBoostAmount;

        // Debuffs
        public static ConfigEntry<float> chillDuration;
        public static ConfigEntry<int> chillMaxStacks;
        public static ConfigEntry<bool> useAttackSlow;
        public static ConfigEntry<float> attackSlowMulti;
        public static ConfigEntry<float> chillSlowMax;
        public static ConfigEntry<float> chillFreezeDamage;
        public static ConfigEntry<float> chillCooldown;

        public static void ReadConfig()
        {
            // Template
            //ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("section", "name"), false, new ConfigDescription("description"));

            // General
            characterEnabled = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("General", "Manipulator Enabled"), true, new ConfigDescription("Set to false to disable Manipulator."));
            forceUnlock = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("General", "Force Unlock"), false, new ConfigDescription("Should Manipulator be unlocked by default?"));
            forceUnlockSkills = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("General", "Force Unlock Skills"), false, new ConfigDescription("Should Manipulator's alternate skills be unlocked by default?"));
            forceUnlockSkins = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("General", "Force Unlock Skins"), false, new ConfigDescription("Should Manipulator's skins be unlocked by default?"));

            // Skills
            // Passive
            jetDuration = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Hover Duration"), 3f, new ConfigDescription("How long should the ACE Pack's hover last?"));
            jetBoostAmount = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Hover Speed Boost"), 1.45f, new ConfigDescription("How much does the ACE Pack's hover speed boost multiply your speed?"));
            
            // Debuffs
            chillFreezeDamage = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Chill Freeze Damage"), 0f, new ConfigDescription("How much damage should be dealt when the Chill Debuff reaches maximum stacks?"));
            chillDuration = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Chill Duration"), 10f, new ConfigDescription("How long should the Chill debuff last?"));
            chillCooldown = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Chill Cooldown"), 4f, new ConfigDescription("How long should enemies be immune to Chill after freezing?"));
            chillMaxStacks = ManipulatorPlugin.instance.Config.Bind<int>(new ConfigDefinition("Skills", "Chill Max Stacks"), 10, new ConfigDescription("What is the maximum stack count for the Chill Debuff?"));
            useAttackSlow = ManipulatorPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Skills", "Chill Use Attack Slow"), false, new ConfigDescription("Should Ice Element's Chill debuff also reduce attack speed?"));
            attackSlowMulti = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Chill Attack Slow Amount"), 0.75f, new ConfigDescription("How much does the maximum Chill debuff reduce attack speed?"));
            chillSlowMax = ManipulatorPlugin.instance.Config.Bind<float>(new ConfigDefinition("Skills", "Chill Movement Slow Amount"), 0.75f, new ConfigDescription("What is the Chill Debuff's movement speed slow amount at maximum stacks?"));

        }
    }
}