using UnityEngine;
using System;

namespace ManipulatorMod
{
    class StatValues
    {
        //A list of stat values that are referenced throughout the character for quick/easy editing of stats for Manipulator.
        //credit to the Paladin Survivor mod for the idea for this.

        //developer info
        public const string MODUID = "com.valex.ManipulatorMod";
        public const string MODNAME = "ManipulatorMod";
        public const string MODVERSION = "0.3.4";

        //character info
        public const string characterName = "Manipulator";
        public const string characterSubtitle = "Weaver of Elements";
        public const string characterOutro = "...and so he left, I'm surprised you did this.";
        public const string characterOutroFaliure = "..and so he vanished, oof.";
        public const string characterLore = "\n_temp";

        //color for use in UI
        internal static readonly Color manipulatorColor = new Color(0.635f, 0.745f, 0.839f, 1f);
        internal static readonly Color fireColor = new Color(0.925f, 0.505f, 0.133f);
        internal static readonly Color lightningColor = new Color(0.596f, 0.313f, 0.623f);
        internal static readonly Color iceColor = new Color(0.207f, 0.733f, 0.886f);

        //Base stats
        public const float baseHealth = 90f;
        public const float baseRegen = 2.5f;
        public const float baseArmor = 12f;
        public const float baseShield = 0f;
        public const float baseMoveSpeed = 7f;
        public const float baseAcceleration = 40f;
        public const float baseJumpPower = 15f;
        public const float baseDamage = 12f; //12
        public const float baseAttackSpeed = 1f;
        public const float baseCrit = 1f;
        public const float baseSprint = 1.45f;
        public const int jumpCount = 1;
        public static readonly Vector3 localCameraPos = new Vector3(0f, 0f, -8f);
        public static readonly Vector3 localCameraPivot = new Vector3(0f, 1.3f, 0f);

        //per level stats
        public const float levelHealth = 27f;
        public const float levelRegen = 0.5f;
        public const float levelArmor = 12f;
        public const float levelShield = 0f;
        public const float levelMoveSpeed = 0f;
        public const float levelJumpPower = 0f;
        public const float levelDamage = 2.4f; //2.4
        public const float levelAttackSpeed = 0f;
        public const float levelCrit = 0f;

        //passive
        public static readonly float jetMoveMulti = Modules.Config.jetBoostAmount.Value;
        public static readonly float jetDuration = Modules.Config.jetDuration.Value;
        public const float jetAcceleration = 60f;
        public const float jetVelocity = -4f;

        //Debuffs
        public static readonly bool useAttackSlow = Modules.Config.useAttackSlow.Value;
        public const float chillDebuffSlowMax = 0.75f;
        public static readonly float chillDebuffAttackMax = Modules.Config.attackSlowMulti.Value;
        public const float chillDebuffDuration = 8f;
        public const int chillDebuffMaxStacks = 8;
        public const float freezeDamage = 0f;

        //Primary
        public const float attackDuration = 1f;
        public const float attackDamageCoefficient = 2.5f;
        public const float attackPushForce = 300f;
        public const float waveDamageCoefficient = 1.5f;
        public const float attackStartTime = 0.2f;
        public const float attackEndTime = 0.4f;
        public const float waveForce = 10f;
        public const float waveLifetime = 1f;
        public const float waveSpeed = 100f;
        public const float waveFireTime = 0.025f;
        public const float waveProcCoefficient = 1f;
        public static readonly Vector3 waveOuterSize = new Vector3(3.5f, 0.5f, 1.5f);
        public static readonly Vector3 waveCoreSize = new Vector3(0.1f, 0.1f, 1.6f);
        public static readonly Vector3 waveRotation1 = new Vector3(0f, 0f, 30f);
        public static readonly Vector3 waveRotation2 = new Vector3(0f, 0f, -30f);
        public const float lightningBlastLifetime = 0.4f;
        public const float lightningBlastAfterLifetime = 0.1f;
        public const float lightningBlastRadius = 2.5f;
        public const float lightningBlastCoefficient = 1f;
        public const float falloffRate = 0.75f;
        public const float falloffMin = 0.5f;
        public const float falloffMax = 1f;

        //Secondary
        public const float spellCooldown = 6f;

        public const float fireSpellCoefficient = 6f;
        public const float fireSpellProc = 1f;
        public const float fireSpellForce = 10f;
        public const float fireSpellSpeed = 80f;
        public const float fireAttachCoefficient = 2f;
        public const float fireAttachProc = 0.3f;
        public const float fireAttachDuration = 2f;
        public const int fireAttachTickMax = 5;
        public const float lightningCoefficient = 3f;
        public const float lightningCoefficientPerBounce = 1f;
        public const float LightningProcCoefficient = 0.5f;
        public const float lightningBounceRange = 30f;
        public const int lightningBounceCount = 6;
        public const float icePillarExplosionCoefficient = 4f;
        public const float icePillarProcCoefficient = 1f;
        public const float iceMaxDistance = 50f;
        public const float maxSlopeAngle = 45f;
        public const float iceDuration = .15f;
        public const float icePillarLifetime = 2f;
        public static Vector3 icePillarScale = new Vector3(2f, 2f, 2f);
        public static Vector3 icePillarBlastForce = new Vector3(0f, 0f, 0f);

        //Utility
        public const float blinkCooldown = 6f;

        public const float explosionDamage = 2f;
        public const float explosionRadius = 8f;
        public const float explosionDelay = 0.5f;
        public const float blinkSpeed = 8f;
        public const float blinkDuration = 0.3f;
        public const float blinkExplosionDelay = 0.1f;
        public const float blinkLightningRadius = 8f;
        public const float blinkLightningDelay = 0.2f;

        //Special
        public const float switchCooldown = 6f;
        public const float castDuration = 0.1f;
        public const float buffDuration = 6f;
        public const float fireBuffAmount = 0.2f;
        public const float lightningCooldownReduction = 0.2f;
        public const float iceBarrierPercent = 0.05f;

        //Special Focus
        public const float buffFocusDuration = 6f;
    }
}
