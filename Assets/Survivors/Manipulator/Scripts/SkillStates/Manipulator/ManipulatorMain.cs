using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EntityStates;
using RoR2;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.SkillStates
{
    public class ManipulatorMain : GenericCharacterMain
    {
        private Animator animator;
        private ManipulatorController manipulatorController;
        private EntityStateMachine jetpackMachine;
        private ChildLocator childLocator;
        private DynamicBone tailBone;

        public override void OnEnter()
        {
            base.OnEnter();

            this.manipulatorController = this.characterBody.GetComponent<ManipulatorController>();
            this.jetpackMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jet");
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();
            if (this.childLocator)
            {
                this.tailBone = this.childLocator.FindChild("Tail").GetComponent<DynamicBone>();
            }
        }

        public override void ProcessJump()
        {
            base.ProcessJump();

            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                if (base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded && !this.manipulatorController.endJet)
                {
                    if (!(this.jetpackMachine.state.GetType() == typeof(ManipulatorJetpack)))
                    {
                        this.jetpackMachine.SetState(new ManipulatorJetpack());
                        this.characterBody.AddBuff(Modules.Buffs.jetBuff);
                        this.manipulatorController.hasJetBuff = true;
                    }
                }
                else
                {
                    if (this.jetpackMachine.state.GetType() == typeof(ManipulatorJetpack))
                    {
                        this.jetpackMachine.SetNextState(new Idle());
                        this.characterBody.RemoveBuff(Modules.Buffs.jetBuff);
                        this.manipulatorController.hasJetBuff = false;
                    }
                }
            }
            if (base.characterMotor.isGrounded) this.manipulatorController.endJet = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                float inAir = 0;
                if (!this.animator.GetBool("isGrounded")) inAir = 1;

                this.animator.SetFloat("inAir", inAir);

                this.animator.SetBool("inCombat", (!base.characterBody.outOfCombat || !base.characterBody.outOfDanger));

                this.animator.SetBool("useAdditive", !this.animator.GetBool("isSprinting"));

                this.animator.SetBool("isHovering", inAir == 1 && this.characterBody.HasBuff(Modules.Buffs.jetBuff));

                if (this.tailBone)
                {
                    if (this.animator.GetBool("isGrounded") && !this.animator.GetBool("isMoving"))
                    {
                        this.tailBone.enabled = false;
                    }
                    else
                    {
                        this.tailBone.enabled = true;
                    }
                }
            }
        }
    }
}
