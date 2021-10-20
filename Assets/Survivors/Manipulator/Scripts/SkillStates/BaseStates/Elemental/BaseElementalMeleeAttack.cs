/*using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using ManipulatorMod.SkillStates.BaseStates;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseElementalMeleeAttack : BaseElementalMelee
    {
        // Serialized Field variables
        [SerializeField] 
        public GameObject wavePrefab;

        [SerializeField] 
        public GameObject wavePrefabAlt;

        private float baseLaunchTime = StaticValues.waveLaunchTime;
        private float launchTime;

        //StaticValues used in this script
        // crossDamageCoefficient
        // crossPushForce
        // crossDuration
        // crossStartTime
        // crossEndTime
        // waveLaunchTime
        // waveForce
        // attackDuration

        public override void OnEnter()
        {
            this.launchTime = this.baseLaunchTime / base.attackSpeedStat;
            this.hitboxName = "Sword";
            this.damageType = this.GetDamageType();
            this.damageCoefficient = StaticValues.crossDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = StaticValues.crossPushForce;
            this.bonusForce = Vector3.zero;
            this.baseDuration = StaticValues.crossDuration;
            this.attackStartTime = StaticValues.crossStartTime;
            this.attackEndTime = StaticValues.crossEndTime;
            this.baseEarlyExitTime = StaticValues.crossEndTime;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;
            this.swingSoundString = "ManipulatorSwordSwing";
            this.hitSoundString = "";
            this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            base.OnEnter();
        }

        protected virtual DamageType GetDamageType()
        {
            return DamageType.Generic;
        }

        protected override void FireProjectile()
        {
            base.FireProjectile();
            if (!this.hasFiredAttack && (this.stopwatch >= this.launchTime))
            {
                if (base.isAuthority)
                {
                    GameObject tempPrefab = this.wavePrefab;
                    SetChildRotation rot = tempPrefab.GetComponent<SetChildRotation>();
                    rot.SetRotation("RotatedCollider", Quaternion.Euler(this.swingIndex == 0 ? new Vector3(0f, 0f, -30f) : new Vector3(0f, 0f, 30f)));

                    Ray aimRay = base.GetAimRay();
                    ProjectileManager.instance.FireProjectile(tempPrefab, //(this.swingIndex == 1 ? this.wavePrefab : this.wavePrefabAlt)
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        StaticValues.waveDamageCoefficient * this.damageStat,
                        StaticValues.waveForce,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null);
                }
            }
        }

        protected override void SetNextState()
        {
            //combo for second slash type
            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            var obj = (BaseElementalMeleeAttack)Activator.CreateInstance(this.activatorSkillSlot.activationState.stateType);
            obj.swingIndex = index;
            obj.activatorSkillSlot = this.activatorSkillSlot;
            this.outer.SetNextState(obj);
        }
    }
}
*/