using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.SkillStates.BaseStates;
using RoR2;

namespace ManipulatorMod.SkillStates
{
    public class ElementalOverloadFire : BaseElementalOverload
    {
        public override void ElementBurst()
        {
            base.ElementBurst();

            BlastAttack blastAttack = MakeBlastAttack();
            blastAttack.Fire();
        }
    }
}
