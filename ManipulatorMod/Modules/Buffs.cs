using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ManipulatorMod.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;

        //elemental buffs
        internal static BuffDef fireBonusBuff;
        internal static BuffDef lightningBonusBuff;
        internal static BuffDef iceBonusBuff;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            // fix the buff catalog to actually register our buffs
            //IL.RoR2.BuffCatalog.Init += FixBuffCatalog;

            fireBonusBuff = AddNewBuff("FireBonusBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffFireBonus"), Color.white, false, false);
            lightningBonusBuff = AddNewBuff("LightningBonusBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffLightningBonus"), Color.white, false, false);
            iceBonusBuff = AddNewBuff("IceBonusBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffIceBonus"), Color.white, false, false);
        }

        /*internal static void FixBuffCatalog(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.Next.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.buffDefs)))
            {
                return;
            }

            c.Remove();
            c.Emit(OpCodes.Ldsfld, typeof(ContentManager).GetField(nameof(ContentManager.buffDefs)));
        }*/

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