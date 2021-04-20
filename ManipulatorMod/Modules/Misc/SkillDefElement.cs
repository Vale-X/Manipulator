using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using ManipulatorMod.SkillStates;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.Modules.Misc
{
    class SkillDefElement : SkillDef
    {
        public Sprite defIcon;
        public Sprite fireIcon;
        public Sprite lightningIcon;
        public Sprite iceIcon;

        public void SwitchElementIcon(ManipulatorController.ManipulatorElement inElement)
        {
            switch (inElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    this.icon = fireIcon;
                    return;
                case ManipulatorController.ManipulatorElement.Lightning:
                    this.icon = lightningIcon;
                    return;
                case ManipulatorController.ManipulatorElement.Ice:
                    this.icon = iceIcon;
                    return;
                default:
                    this.icon = defIcon;
                    return;
            }

        }
    }
}
