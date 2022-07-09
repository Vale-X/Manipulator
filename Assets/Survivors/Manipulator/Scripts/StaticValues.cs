using System;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    // StaticValues is a good place to put in any variables you might want to change at a moment's notice
    // good for easily making balance changes. Usually you'd have the body values (health, movement speed, etc) be here too,
    // but that's in the CharacterBody component instead.
    internal static class StaticValues
    {
        #region Manipulator Skills
        // Passive
        internal static readonly float jetpackSpeedMulti = Modules.Config.jetBoostAmount.Value;
        internal static readonly float jetpackDuration = Modules.Config.jetDuration.Value;
        internal const float jetAcceleration = 60f;
        internal const float jetVelocity = -4f;

        // Debuffs
        internal static readonly bool useAttackSlow = Modules.Config.useAttackSlow.Value;
        internal static readonly float chillDebuffAttackMax = Modules.Config.attackSlowMulti.Value;
        internal static readonly float chillDebuffSlowMax = Modules.Config.chillSlowMax.Value;
        internal static readonly float chillDebuffDuration = Modules.Config.chillDuration.Value;
        internal static readonly int chillDebuffMaxStacks = Modules.Config.chillMaxStacks.Value;
        internal static readonly float chillCooldownDuration = Modules.Config.chillCooldown.Value;
        internal static readonly float freezeDamage = Modules.Config.chillFreezeDamage.Value;

        // Primary
        internal const float crossDuration = 1f;
        internal const float crossDamageCoefficient = 1.25f;
        internal const float crossProcCoefficient = 1f;
        internal const float crossPushForce = 300f;
        internal const float crossStartTime = 0.2f;
        internal const float crossEndTime = 0.35f;
        internal const float crossEarlyExitTime = 0.3f;
        internal const float crossHitStopDuration = 0.012f;
        internal const float crossAttackRecoil = 0.75f;
        internal const float crossHitHopVelocity = 4f;


        internal const float waveDamageCoefficient = 1f;
        internal const float waveLaunchTime = 0.15f;
        internal const float waveForce = 10f;

        // Secondary
        internal const float fireSpellDamageCoefficient = 4f;
        internal const float fireTickDamageCoefficient = 1.5f;
        internal const float fireAttachTickMax = 5;
        internal const float fireSpellForce = 10f;
        internal const float fireAttachTickInterval = 1f;

        internal const float lightningDamageCoefficient = 3f;
        internal const float lightningBounceDamageMulti = 0.9f;
        internal const float lightningProcCoefficient = 0.5f;
        internal const float lightningBounceRange = 30f;
        internal const int lightningBounceCount = 6;

        internal const float icePillarDamageCoefficient = 2f;
        internal const float iceExplosionDamageCoefficient = 4f;
        internal const float icePillarMaxRange = 50f;
        internal const float icePillarMaxSlope = 45f;
        internal static readonly Vector3 icePillarIndicatorSize = new Vector3(3.5f, 10f, 3.5f);

        // Utility
        internal const float explosionDamageCoefficient = 2.5f;
        internal const float blinkSpeedCoefficient = 8f;
        internal const float blinkDuration = 0.2f;

        // Utility Alt1
        internal const float jumpDuration = 1f;
        internal const float jumpDamageCoefficient = 1.5f;
        internal const float jumpProcCoefficient = 1f;
        internal const float jumpAttackForce = 400f;
        internal const float jumpSpeedCoefficient = 0.4f;

        // Special
        internal const float switchDuration = 0.1f;

        // Special Alt1
        internal const float overloadBuffDuration = 4f;
        internal const float OverloadSpeedBuffMulti = 0.3f;
        internal const float OverloadAttackBuffMulti = 0.3f;
        internal const float overloadSlashMulti = 2.5f;
        internal const float burstDamageCoefficient = 4f;

        // Special Bonus
        internal const float fireBuffDamageMulti = 0.2f;
        internal const float lightningCooldownReduction = 0.2f;
        internal const float iceBarrierPercent = 0.05f;
        #endregion
    }
}