using UnityEngine;
using EntityStates;
using ManipulatorMod.Modules.Survivors;
using RoR2;
using ManipulatorMod.Modules.Components;
using System;
using ManipulatorMod.Modules.Misc;

namespace ManipulatorMod.SkillStates
{
    public class ManipulatorMain : GenericCharacterMain
    {
        private Animator animator;

        //for primary attack resetting regardless of which primary or special is used.
        public static bool attackReset;

        public static float buffDuration = 6f;

        /*public static bool fireBonus;
        public static float fireTimer;
        public static bool lightningBonus;
        public static float lightningTimer;
        public static bool iceBonus;
        public static float iceTimer;*/

        private EntityStateMachine stateMachine;

        //enum of elements, used in other skills.
        public enum ManipulatorElement2
        { 
            None,
            Fire,
            Lightning,
            Ice
        }

        //set default element to fire.
        public static ManipulatorElement2 currentElement = ManipulatorElement2.Fire;
        private ManipulatorController manipulatorController;

        private static SkillLocator locatorRef;

        public static bool gotStartingElement;

        public override void OnEnter()
        {
            base.OnEnter();

            this.manipulatorController = characterBody.GetComponent<ManipulatorController>();

            /*if (gotStartingElement == false)
            {
                currentElement = GetStartingElement();
                gotStartingElement = true;
            }

            locatorRef = skillLocator;

            this.SetElementSkillIcons(currentElement);*/

            this.animator = base.GetModelAnimator();

            EntityStateMachine[] components = this.gameObject.GetComponents<EntityStateMachine>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].customName == "Weapon")
                {
                    this.stateMachine = components[i];
                    //Debug.LogWarning($"Main statemachine: {this.stateMachine}");
                    //return;
                }
            }
        }

        /*public void SetElementSkillIcons(ManipulatorElement2 newElement)
        {
            foreach (GenericSkill i in skillLocator.allSkills)
            {
                if (i.skillDef is SkillDefElement tempElement)
                {
                    tempElement.SwitchElementIcon(newElement);
                    Debug.LogWarning($"skillDef: {i.skillDef}");
                    Debug.LogWarning($"skillName: {i.skillDef.skillName}");
                    Debug.LogWarning($"skillNameToken: {i.skillDef.skillNameToken}");
                    Debug.LogWarning($"activationState: {i.skillDef.activationState}");
                    Debug.LogWarning($"skillIndex: {i.skillDef.skillIndex}");
                }
            }
        }

        public void ResetIcons()
        {
            if (locatorRef)
            {
                gotStartingElement = false;
                Debug.LogWarning("Icons Reset");
                //locatorRef.primary.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIconDef");
                //locatorRef.secondary.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconDef");
                //locatorRef.utility.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIconDef");
                //locatorRef.special.skillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconDef");
                SetElementSkillIcons(ManipulatorElement2.None);
            }
        }

        private ManipulatorElement2 GetStartingElement()
        {
            ManipulatorElementController elementController = base.characterBody.GetComponent<ManipulatorElementController>();
            switch (elementController.elementSkill.skillDef.skillName)
            {
                case "VALE_MANIPULATOR_BODY_ELEMENT_FIRE_NAME":
                    return ManipulatorElement2.Fire;
                case "VALE_MANIPULATOR_BODY_ELEMENT_LIGHTNING_NAME":
                    return ManipulatorElement2.Lightning;
                case "VALE_MANIPULATOR_BODY_ELEMENT_ICE_NAME":
                    return ManipulatorElement2.Ice;
                default:
                    return ManipulatorElement2.Fire;
            }

        }*/

        /*public static void ElementalBonus()
        {
            switch (ManipulatorMain.CurrentElement)
            {
                case ManipulatorMain.ManipulatorElement.Fire:
                    //Fire Bonus
                    base.characterBody.AddTimedBuff(Modules.Buffs.fireBonusBuff, buffDuration);
                    //fireBonus = true;
                    //fireTimer = 0f;
                    break;
                case ManipulatorMain.ManipulatorElement.Lightning:
                    base.characterBody.AddTimedBuff(Modules.Buffs.lightningBonusBuff, buffDuration);
                    //Lightning Bonus
                    //lightningBonus = true;
                    //lightningTimer = 0f;
                    break;
                case ManipulatorMain.ManipulatorElement.Ice:
                    base.characterBody.AddTimedBuff(Modules.Buffs.iceBonusBuff, buffDuration);
                    //Ice bonus
                    //iceBonus = true;
                    //iceTimer = 0f;
                    break;
            }
        }*/

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                float i = 1;
                if (this.animator.GetBool("isGrounded")) i = 0;
                this.animator.SetFloat("inAir", i);
            }

            //Debug.LogWarning($"StateMachine: {this.stateMachine}");
            //Debug.LogWarning($"State: {this.stateMachine.state}");

            /*if (fireBonus)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= bonusDuration)
                {
                    fireBonus = false;
                    fireTimer = 0f;
                }
            }
            if (lightningBonus)
            {
                lightningTimer += Time.deltaTime;
                if (lightningTimer >= bonusDuration)
                {
                    lightningBonus = false;
                    lightningTimer = 0f;
                }
            }
            if (iceBonus)
            {
                iceTimer += Time.deltaTime;
                if (iceTimer >= bonusDuration)
                {
                    iceBonus = false;
                    iceTimer = 0f;
                }
            }*/
        }
    }
}