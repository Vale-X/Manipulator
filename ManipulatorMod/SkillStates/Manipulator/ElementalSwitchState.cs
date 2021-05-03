using ManipulatorMod.SkillStates.BaseStates;
using UnityEngine;
using RoR2;
using RoR2.Audio;
using EntityStates;
using RoR2.Skills;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Survivors;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSwitchState : BaseSkillState
    {
        public float baseDuration = StatValues.castDuration;
        public float buffDuration = StatValues.buffDuration;

        public ManipulatorController manipulatorController;

        public override void OnEnter()
        {
            base.OnEnter();

            this.manipulatorController = characterBody.GetComponent<ManipulatorController>();

            //switch current element
            switch (this.manipulatorController.currentElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    //Lightning Switch
                    //Chat.AddMessage("Switched from Fire to Lightning");
                    SwitchElement(ManipulatorController.ManipulatorElement.Lightning);
                    this.manipulatorController.tracker.enabled = true;
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    //Ice Switch
                    //Chat.AddMessage("Switched from Lightning to Ice");
                    SwitchElement(ManipulatorController.ManipulatorElement.Ice);
                    this.manipulatorController.tracker.enabled = false;
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    //Fire Switch
                    //Chat.AddMessage("Switched from Ice to Fire");
                    SwitchElement(ManipulatorController.ManipulatorElement.Fire);
                    this.manipulatorController.tracker.enabled = false;
                    break;
            }

            
            //this.PlaySwitchAnimation();

            //trigger attack reset on primary attack,
            ManipulatorMain.attackReset = true;
            this.manipulatorController.attackReset = true;
        }

        public void SwitchElement(ManipulatorController.ManipulatorElement elementType)
        {
            skillLocator.primary.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon" + elementType.ToString());
            skillLocator.secondary.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon" + elementType.ToString());
            skillLocator.utility.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon" + elementType.ToString());
            skillLocator.special.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon" + elementType.ToString());

            this.manipulatorController.currentElement = elementType;
            this.manipulatorController.SetMaterialEmissive(elementType);
            ElementBonus();
        }

        private void PlaySwitchAnimation()
        {
            //base.PlayCrossfade("Gesture, Additive", "Switch", "Switch.playbackRate", 0.15f, 0.05f);
            base.PlayAnimation("Gesture, Additive", "Switch", "Switch.playbackRate", 0.5f);
        }

        public void ElementBonus()
        {
            switch (this.manipulatorController.currentElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    //Fire Bonus
                    //base.characterBody.AddTimedBuff(Modules.Buffs.fireBonusBuff, buffDuration);
                    base.characterBody.AddBuff(Modules.Buffs.fireBonusBuff);
                    //fireBonus = true;
                    //fireTimer = 0f;
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    //base.characterBody.AddTimedBuff(Modules.Buffs.lightningBonusBuff, buffDuration);
                    base.characterBody.AddBuff(Modules.Buffs.lightningBonusBuff);
                    //Lightning Bonus
                    //lightningBonus = true;
                    //lightningTimer = 0f;
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    //base.characterBody.AddTimedBuff(Modules.Buffs.iceBonusBuff, buffDuration);
                    base.characterBody.AddBuff(Modules.Buffs.iceBonusBuff);
                    //Ice bonus
                    //iceBonus = true;
                    //iceTimer = 0f;
                    break;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
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
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
