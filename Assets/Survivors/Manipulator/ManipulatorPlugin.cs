using BepInEx;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using StubbedConverter;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using RoR2.Networking;
using RoR2.UI;
using System;
using HG;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Debug = UnityEngine.Debug;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//Do a 'Find and Replace' on the ThunderHenry namespace. Make your own namespace, please.
namespace ManipulatorMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.valex.ShaderConverter", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI",
        "DotAPI"
    })]
    public class ManipulatorPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.valex.ManipulatorMod";
        public const string MODNAME = "ManipulatorMod";
        public const string MODVERSION = "0.4.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string developerPrefix = "VALE";

        // use this to toggle debug on stuff, make sure it's false before releasing
        public static bool debug = true;

        public static bool cancel;

        public static ManipulatorPlugin instance;

        private void Awake()
        {
            instance = this;

            // Load/Configure everything
            Modules.Config.ReadConfig();
            Modules.Assets.Init();
            if (cancel) return;
            Modules.Shaders.init();
            Modules.Tokens.Init();
            Modules.Prefabs.Init();
            Modules.Buffs.Init();
            Modules.ItemDisplays.Init();
            Modules.UnlockablesTemp.Init();
            
            
            // Any debug stuff you need to do can go here before initialisation
            if (debug) { Modules.Helpers.AwakeDebug(); }

            //Initialize Content Pack
            Modules.ContentPackProvider.Initialize();

            Hook();
        }

        private void Start()
        {
            if (debug) { Modules.Helpers.StartDebug(); }
        }

        private void Hook()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            GameNetworkManager.onServerSceneChangedGlobal += Modules.Prefabs.AddBlackBox;
            Stage.onStageStartGlobal += Modules.Prefabs.StatueHook;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                Modules.Buffs.HandleBuffs(self);
                Modules.Buffs.HandleDebuffs(self);
            }
        }
    }
}
