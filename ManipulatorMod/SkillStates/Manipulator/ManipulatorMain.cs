using UnityEngine;
using EntityStates;
using ManipulatorMod.Modules.Survivors;
using RoR2;
using ManipulatorMod.Modules.Components;
using System;
using ManipulatorMod.Modules.Misc;
using ManipulatorMod.SkillStates.Manipulator;

namespace ManipulatorMod.SkillStates
{
    public class ManipulatorMain : GenericCharacterMain
    {
        private Animator animator;

        //for primary attack resetting regardless of which primary or special is used.
        public static bool attackReset;

        public static float buffDuration = 6f;

        public bool jetBuff = false;

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
        private EntityStateMachine jetpackStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();

            this.manipulatorController = characterBody.GetComponent<ManipulatorController>();
            this.jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jet");

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

        public override void ProcessJump()
        {
            base.ProcessJump();
            //Debug.LogWarning($"speed: {this.characterBody.moveSpeed}");

            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                bool obj = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded && !this.manipulatorController.endJet;
                //Debug.LogWarning($"obj: {obj}");
                bool flag = this.jetpackStateMachine.state.GetType() == typeof(ManipulatorJetpack);
                //Debug.LogWarning($"flag: {flag}");
                bool obj2 = obj;
                if (obj2 == true && !flag)
                {
                    //Debug.LogWarning("1");
                    //this.jetpackStateMachine.SetNextState(new ManipulatorJetpack());
                    this.jetpackStateMachine.SetState(new ManipulatorJetpack());
                    this.characterBody.AddBuff(Modules.Buffs.hiddenJetBuff);
                    this.manipulatorController.hasJetBuff = true;
                }
                if (obj2 == false && flag)
                {
                    //Debug.LogWarning("2");
                    this.jetpackStateMachine.SetNextState(new Idle());
                    this.characterBody.RemoveBuff(Modules.Buffs.hiddenJetBuff);
                    this.manipulatorController.hasJetBuff = false;
                }
            }
            if (base.characterMotor.isGrounded) this.manipulatorController.endJet = false;
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

                this.animator.SetBool("useAdditive", (!this.animator.GetBool("isSprinting")));

                this.animator.SetBool("isHovering", i == 1 && this.characterBody.HasBuff(Modules.Buffs.hiddenJetBuff));
            }
        }
    }
}