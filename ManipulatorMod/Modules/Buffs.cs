using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    public static class Buffs
    {
        //elemental buffs
        internal static BuffDef fireBonusBuff;
        internal static BuffDef lightningBonusBuff;
        internal static BuffDef iceBonusBuff;

        //jetpack move buff
        internal static BuffDef hiddenJetBuff;

        //custom ice debuff
        internal static BuffDef iceChillDebuff;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            // fix the buff catalog to actually register our buffs
            //IL.RoR2.BuffCatalog.Init += FixBuffCatalog;

            fireBonusBuff = AddNewBuff("0FireBonusBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffFireBonus"), Color.white, false, false);
            lightningBonusBuff = AddNewBuff("0LightningBonusBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffLightningBonus"), Color.white, false, false);
            iceBonusBuff = AddNewBuff("0IceBonusBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffIceBonus"), Color.white, false, false);

            hiddenJetBuff = AddNewBuff("0AJetBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffJetBoost"), Color.white, false, false);

            iceChillDebuff = AddNewBuff("iceChillDebuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChillDebuff"), Color.white, true, true);
        }

        public static void HandleDebuffs(CharacterBody self)
        {
            if (self)
            {
                if (self.HasBuff(iceChillDebuff))
                {
                    self.moveSpeed *= 1f - ((StatValues.chillDebuffSlowMax / StatValues.chillDebuffMaxStacks) * self.GetBuffCount(iceChillDebuff));
                    if (StatValues.useAttackSlow) self.attackSpeed *= 1f - ((StatValues.chillDebuffAttackMax / StatValues.chillDebuffMaxStacks) * self.GetBuffCount(iceChillDebuff));
                    if (self.GetBuffCount(iceChillDebuff) >= StatValues.chillDebuffMaxStacks)
                    {
                        self.healthComponent.TakeDamage(new DamageInfo { damage = StatValues.freezeDamage, damageType = DamageType.Freeze2s });
                        self.ClearTimedBuffs(iceChillDebuff.buffIndex);
                    }
                }
            }

        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}