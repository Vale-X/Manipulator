using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.SkillStates.BaseStates;
using RoR2;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSlashLightning : BaseElementalMelee
    {
        // Most of the differences are handled by the EntityStateConfiguration.
        protected override DamageType GetDamageType()
        {
            return DamageType.Generic;
        }
    }
}
