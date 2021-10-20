using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using EntityStates;
using ManipulatorMod.Modules.Components;
using ManipulatorMod.Modules;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseElementalOverload : BaseSkillState
    {
        [SerializeField]
        public int nextElement;
        [SerializeField]
        public int damageType;
        [SerializeField]
        public float blastRadius;
        [SerializeField]
        public float blastAttackProcCoefficient;
        [SerializeField]
        public float blastAttackDamageCoefficient;
        [SerializeField]
        public float blastAttackForce;
        [SerializeField]
        public float shortHopVelocity;
        [SerializeField]
        public float antigravityStrength;
        [SerializeField]
        public float baseFireTime;
        [SerializeField]
        public float baseDuration;

        protected ManipulatorController manipulatorController;
        private bool hasFired;
        private float duration;
        private float fireTime;

        public override void OnEnter()
        {
            base.OnEnter();
            this.manipulatorController = this.characterBody.GetComponent<ManipulatorController>();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireTime = this.baseFireTime / this.attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.fixedAge >= this.fireTime && !this.hasFired && base.isAuthority)
            {
                this.hasFired = true;
                this.ElementBurst();
                this.SwitchElement((ManipulatorController.Element)this.nextElement);
            }

            if (this.fixedAge <= this.fireTime && !this.hasFired && base.isAuthority)
            {
                CharacterMotor characterMotor = base.characterMotor;
                characterMotor.velocity.y = characterMotor.velocity.y + this.antigravityStrength * Time.fixedDeltaTime * (1f - this.fixedAge / this.duration);
            }

            if (this.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public virtual BlastAttack MakeBlastAttack()
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = this.blastRadius;
            blastAttack.procCoefficient = this.blastAttackProcCoefficient;
            blastAttack.position = base.transform.position;
            blastAttack.attacker = base.gameObject;
            blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            blastAttack.baseDamage = base.characterBody.damage * this.blastAttackDamageCoefficient;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = (DamageType)this.damageType;
            blastAttack.baseForce = this.blastAttackForce;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
            return blastAttack;
        }

        public virtual void ElementBurst()
        {
            if (base.characterMotor)
            {
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, this.shortHopVelocity, base.characterMotor.velocity.z);
            }
        }

        public void SwitchElement(ManipulatorController.Element element)
        {
            this.manipulatorController.hasSwapped = true;
            this.manipulatorController.currentElement = element;
            this.manipulatorController.SetMaterialEmissive(element);
            foreach (var item in this.manipulatorController.elementalDict)
            {
                item.Key.SwitchElement(item.Value, element);
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
