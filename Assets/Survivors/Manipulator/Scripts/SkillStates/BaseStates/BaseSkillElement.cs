using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EntityStates;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseSkillElement : BaseSkillState
    {
        protected ManipulatorController manipulatorController;
        protected bool hasRemovedBuff;
        protected float stopwatch;
        
        protected float fireDamageBonus
        { 
            get
            {
                return (this.characterBody.HasBuff(Modules.Buffs.fireBuff) ? 1f + Modules.StaticValues.fireBuffDamageMulti : 1f);
            }
        }

        public override void OnEnter()
        {
            this.manipulatorController = base.GetComponent<ManipulatorController>();
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
        }

        // Triggered by a component.
        public virtual void ReturnedHits(int hitCount)
        {
            this.ElementalBonus(hitCount);
        }

        // Removes buff and applies bonus effects.
        protected virtual void ElementalBonus(int hitCount)
        {
            // Fire Bonus
            if (base.HasBuff(Modules.Buffs.fireBuff))
            {
                base.characterBody.RemoveBuff(Modules.Buffs.fireBuff);
            }

            // Lightning Bonus
            if (base.HasBuff(Modules.Buffs.lightningBuff))
            {
                for (int i = 0; i < hitCount; i++)
                {
                    base.activatorSkillSlot.rechargeStopwatch = base.activatorSkillSlot.rechargeStopwatch + (StaticValues.lightningCooldownReduction * (base.activatorSkillSlot.finalRechargeInterval - base.activatorSkillSlot.rechargeStopwatch));
                }
                base.characterBody.RemoveBuff(Modules.Buffs.lightningBuff);
            }

            // Ice Bonus
            if (base.HasBuff(Modules.Buffs.iceBuff))
            {
                for (int i = 0; i < hitCount; i++)
                {
                    base.healthComponent.AddBarrier(base.healthComponent.fullHealth * StaticValues.iceBarrierPercent);
                }
                base.characterBody.RemoveBuff(Modules.Buffs.iceBuff);
            }
            this.hasRemovedBuff = true;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
