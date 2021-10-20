using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.SkillStates.BaseStates;
using ManipulatorMod.Modules;
using RoR2;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSlashIce : BaseElementalMelee
    {
        // Most of the differences are handled by the EntityStateConfiguration.

        protected override DamageType GetDamageType()
        {
            return DamageType.Generic;
        }

        protected override void OnHitEnemyAuthority(List<HealthComponent> healthList)
        {
            base.OnHitEnemyAuthority(healthList);

            foreach (HealthComponent health in healthList)
            {
                if (!health.body.HasBuff(Buffs.chillCooldown))
                {
                    health.body.AddTimedBuff(Buffs.chillDebuff, StaticValues.chillDebuffDuration, StaticValues.chillDebuffMaxStacks);
                }
            }
        }
    }
}
