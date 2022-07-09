using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    class Buffs
    {
        internal static List<BuffDef> buffs = new List<BuffDef>();

        // Buffs
        internal static BuffDef fireBuff;
        internal static BuffDef lightningBuff;
        internal static BuffDef iceBuff;
        internal static BuffDef jetBuff;
        internal static BuffDef overloadBuff;

        // "Debuffs"
        internal static BuffDef chillDebuff;
        internal static BuffDef chillCooldown;

        internal static void Init()
        {
            CollectBuffs();
        }


        // Grabs all the buffDefs in your content pack for reference in code
        // Order should be the same as the SerializedContentPack BuffDefs list.
        private static void CollectBuffs()
        {
            fireBuff = GetBuff("ManipulatorSwitchFireBuff", "FIRE");
            lightningBuff = GetBuff("ManipulatorSwitchLightningBuff", "LIGHTNING");
            iceBuff = GetBuff("ManipulatorSwitchIceBuff", "ICE");
            jetBuff = GetBuff("ManipulatorJetpackBuff", "JETPACK");
            overloadBuff = GetBuff("ManipulatorOverloadSlashBuff", "OVERLOAD");
            chillDebuff = GetBuff("ManipulatorChillDebuff", "CHILL");
            chillCooldown = GetBuff("ManipulatorChillCooldown", "CHILLCD");
        }

        internal static BuffDef GetBuff(string buffName, string tokenName)
        {
            BuffDef def = Assets.mainAssetBundle.LoadAsset<BuffDef>(buffName);
            if (def)
            {
                buffs.Add(def);

                if (ModCompatibility.BetterUICompat.betterUIInstalled)
                {
                    ModCompatibility.BetterUICompat.RegisterBuffInfo(def, $"{Tokens.prefix}BUFF_{tokenName}_NAME", $"{Tokens.prefix}BUFF_{tokenName}_DESC");
                }

                return def;
            }
            else return null;
        }

        internal static void BetterUIBuffDescs()
        {
            
        }

        internal static void HandleBuffs(CharacterBody body)
        {
            if (body)
            {
                if (body.HasBuff(jetBuff))
                {
                    body.moveSpeed = body.moveSpeed * StaticValues.jetpackSpeedMulti;
                    body.acceleration = body.acceleration * (StaticValues.jetpackSpeedMulti * 2);
                }
                if (body.HasBuff(overloadBuff))
                {
                    body.moveSpeed = body.moveSpeed * (1 + StaticValues.OverloadSpeedBuffMulti);
                    body.attackSpeed = body.attackSpeed * (1 + StaticValues.OverloadAttackBuffMulti);
                }
            }
        }

        internal static void HandleDebuffs(CharacterBody body)
        {
            if (body)
            {
                if (body.HasBuff(chillDebuff))
                {
                    var chillCount = body.GetBuffCount(chillDebuff);
                    body.moveSpeed *= 1f - ((StaticValues.chillDebuffSlowMax / StaticValues.chillDebuffMaxStacks) * chillCount);
                    if (StaticValues.useAttackSlow) body.attackSpeed *= 1f - ((StaticValues.chillDebuffAttackMax / StaticValues.chillDebuffMaxStacks) * chillCount);
                    if (chillCount >= StaticValues.chillDebuffMaxStacks)
                    {
                        body.healthComponent.TakeDamage(new DamageInfo { damage = StaticValues.freezeDamage, damageType = DamageType.Freeze2s });
                        body.ClearTimedBuffs(chillDebuff.buffIndex);
                        body.AddTimedBuff(chillCooldown, StaticValues.chillCooldownDuration);
                    }
                }
            }
        }
    }
}
