using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.Modules;
using ManipulatorMod.SkillStates.BaseStates;
using RoR2;

namespace ManipulatorMod.SkillStates
{
    public class ElementalOverloadIce : BaseElementalOverload
    {
        public override void ElementBurst()
        {
            base.ElementBurst();

            BlastAttack blastAttack = MakeBlastAttack();
            blastAttack.Fire();

            BlastAttack.HitPoint[] hits = blastAttack.CollectHits();
            foreach (BlastAttack.HitPoint hit in hits)
            {
                CharacterBody victimBody = hit.hurtBox.healthComponent.body;
                if (victimBody)
                {
                    if (!victimBody.HasBuff(Buffs.chillCooldown))
                    {
                        victimBody.AddTimedBuff(Buffs.chillDebuff, StaticValues.chillDebuffDuration, StaticValues.chillDebuffMaxStacks);
                    }
                }
            }
        }
    }
}
