using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityStates;
using ManipulatorMod.SkillStates.BaseStates;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using ManipulatorMod.Modules.Scriptables;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSwitch : BaseSkillState
    {
        [SerializeField]
        public float smallHopStrength;

        protected ManipulatorController manipulatorController;
        public override void OnEnter()
        {
            base.OnEnter();

            this.manipulatorController = this.characterBody.GetComponent<ManipulatorController>();

            switch (this.manipulatorController.currentElement)
            {
                case ManipulatorController.Element.None:
                    this.SwitchElement(ManipulatorController.Element.Fire);
                    Debug.LogWarning(ManipulatorPlugin.MODNAME + ": Warning: Switched to Element None!");
                    break;
                case ManipulatorController.Element.Fire:
                    this.SwitchElement(ManipulatorController.Element.Lightning);
                    break;
                case ManipulatorController.Element.Lightning:
                    this.SwitchElement(ManipulatorController.Element.Ice);
                    break;
                case ManipulatorController.Element.Ice:
                    this.SwitchElement(ManipulatorController.Element.Fire);
                    break;
            }

            if (this.manipulatorController.hasJetBuff)
            {
                this.SmallHop(this.characterMotor, this.smallHopStrength);
            }
        }

        internal void SwitchElement(ManipulatorController.Element element)
        {
            this.manipulatorController.hasSwapped = true;
            this.manipulatorController.currentElement = element;
            this.manipulatorController.SetMaterialEmissive(element);
            this.ElementBonus(element);
            foreach (var item in this.manipulatorController.elementalDict)
            {
                item.Key.SwitchElement(item.Value, element);
            }
        }

        internal void ElementBonus(ManipulatorController.Element element)
        {
            switch (element)
            {
                case ManipulatorController.Element.Fire:
                    if (!base.characterBody.HasBuff(Modules.Buffs.fireBuff)) base.characterBody.AddBuff(Modules.Buffs.fireBuff);
                    break;
                case ManipulatorController.Element.Lightning:
                    if (!base.characterBody.HasBuff(Modules.Buffs.lightningBuff)) base.characterBody.AddBuff(Modules.Buffs.lightningBuff);
                    break;
                case ManipulatorController.Element.Ice:
                    if (!base.characterBody.HasBuff(Modules.Buffs.iceBuff)) base.characterBody.AddBuff(Modules.Buffs.iceBuff);
                    break;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= StaticValues.switchDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
