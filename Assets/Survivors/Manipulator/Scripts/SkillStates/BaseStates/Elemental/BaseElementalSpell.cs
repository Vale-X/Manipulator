using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityStates;
using UnityEngine;
using ManipulatorMod.Modules;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseElementalSpell : BaseSkillElement
    {
        [SerializeField] 
        public GameObject projectilePrefab;

        [SerializeField] 
        public float smallHopVelocity;

        [SerializeField] 
        public float baseDuration;

        [SerializeField] 
        public float baseAttackStartTime;

        [SerializeField] 
        public float antiGravCoefficient;

        protected float duration;
        protected float attackStartTime;
        protected bool hasFired;
        protected bool hasFiredAttempted;
        protected bool hasJumped;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = this.baseDuration / this.attackSpeedStat;
            this.attackStartTime = this.baseAttackStartTime / this.attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.hasFired && !this.hasJumped && !this.characterMotor.isGrounded)
            {
                base.SmallHop(base.characterMotor, this.smallHopVelocity);
                this.hasJumped = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
