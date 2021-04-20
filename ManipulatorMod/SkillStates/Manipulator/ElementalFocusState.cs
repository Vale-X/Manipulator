using ManipulatorMod.SkillStates.BaseStates;
using UnityEngine;
using RoR2;
using RoR2.Audio;
using EntityStates;
using RoR2.Skills;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using ManipulatorMod.Modules.Survivors;

namespace ManipulatorMod.SkillStates.Manipulator
{
    class ElementalFocusState : BaseSkillState
    {
        private ManipulatorController manipulatorController;

        private float baseDuration = StatValues.castDuration;
        private float buffDuration = StatValues.buffFocusDuration;

        public override void OnEnter()
        {
            base.OnEnter();

            //Get current element

            this.manipulatorController = characterBody.GetComponent<ManipulatorController>();
            ManipulatorMain.attackReset = true;
        }

        public void ApplyFocusBuff()
        {
            switch (this.manipulatorController.currentElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    break;
                default:
                    return;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.baseDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
