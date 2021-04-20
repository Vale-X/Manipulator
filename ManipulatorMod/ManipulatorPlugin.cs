using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ManipulatorMod
{
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(StatValues.MODUID, StatValues.MODNAME, StatValues.MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "OrbAPI",
        "EffectAPI",
        "BuffAPI",
        "UnlockableAPI"
    })]

    public class ManipulatorPlugin : BaseUnityPlugin
    {
        // a prefix for name tokens to prevent conflicts
        public const string developerPrefix = "VALE";

        // soft dependency stuff
        public static bool starstormInstalled = false;
        public static bool scepterInstalled = false;
        public static bool scrollableLobbyInstalled = false;

        public static ManipulatorPlugin instance;

        private void Awake()
        {
            instance = this;

            // check for soft dependencies
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2")) starstormInstalled = true;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter")) scepterInstalled = true;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ScrollableLobbyUI")) scrollableLobbyInstalled = true;

            // load assets and read config
            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            Modules.Orbs.RegisterOrbs(); // register orbs
            Modules.Effects.RegisterEffects(); // register effects
            Modules.Unlockables.RegisterUnlockables(); // add unlockables

            Modules.Survivors.Manipulator.CreateCharacter();



            //new Modules.ContentPacks().CreateContentPack();
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private void Start()
        {
            Modules.Survivors.Manipulator.SetItemDisplays();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.PreGameController.Start += PreGameController_Start;
            On.RoR2.Run.CCRunEnd += Run_CCRunEnd;
        }

        private void Run_CCRunEnd(On.RoR2.Run.orig_CCRunEnd orig, ConCommandArgs args)
        {
            Modules.Survivors.Manipulator.ResetIcons();
            orig(args);
        }

        private void PreGameController_Start(On.RoR2.PreGameController.orig_Start orig, PreGameController self)
        {
            Modules.Survivors.Manipulator.ResetIcons();
            orig(self);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
        }
    }
}