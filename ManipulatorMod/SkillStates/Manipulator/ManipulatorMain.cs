﻿using UnityEngine;
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                float i = 1;
                if (this.animator.GetBool("isGrounded")) i = 0;
                this.animator.SetFloat("inAir", i);

                this.animator.SetBool("inCombat", (!base.characterBody.outOfCombat || !base.characterBody.outOfDanger));

                this.animator.SetBool("useAdditive", (i == 1 && !this.animator.GetBool("isSprinting")));
            }
        }
    }
}