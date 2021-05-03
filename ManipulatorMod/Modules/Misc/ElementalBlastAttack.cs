using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace ManipulatorMod.Modules.Misc
{
    class ElementalBlastAttack : BlastAttack
    {
        public void ApplyIceDebuff(bool iceDebuff)
        {
            if (iceDebuff)
            {
                BlastAttack.HitPoint[] array = this.CollectHits();
                foreach (BlastAttack.HitPoint hitPoint in array)
                {
                    HealthComponent healthComponent = hitPoint.hurtBox ? hitPoint.hurtBox.healthComponent : null;
                    if (healthComponent)
                    {
                        healthComponent.body.AddTimedBuff(Modules.Buffs.iceChillDebuff, StatValues.chillDebuffDuration, StatValues.chillDebuffMaxStacks);
                    }
                }
            }
        }
    }
}
