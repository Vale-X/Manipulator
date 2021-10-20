using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseElementalJump : ManipulatorMain
    {
        [SerializeField]
        public float duration;
        [SerializeField]
        public float blastAttackRadius;
        [SerializeField]
        public float blastAttackProcCoefficient;
        [SerializeField]
        public float blastAttackDamageCoefficient;
        [SerializeField]
        public float blastAttackForce;
        [SerializeField]
        public int blastDamageType;
        [SerializeField]
        public string beginSoundString;
        [SerializeField]
        public string endSoundString;
        [SerializeField]
        public GameObject explosionPrefab;
        [SerializeField]
        public GameObject blinkPrefab;
        [SerializeField]
        public GameObject muzzleflashEffect;
        [SerializeField]
        public AnimationCurve speedCoefficientCurve;
        [SerializeField]
        public float speedCoefficient;

        protected float fireDamageBonus
        {
            get
            {
                return (this.characterBody.HasBuff(Modules.Buffs.fireBuff) ? 1f + Modules.StaticValues.fireBuffDamageMulti : 1f);
            }
        }

        private Vector3 flyVector = Vector3.zero;
        private Vector3 blastPosition;
        private Transform modelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!string.IsNullOrEmpty(this.beginSoundString)) Util.PlaySound(this.beginSoundString, base.gameObject);
            this.modelTransform = base.GetModelTransform();
            this.flyVector = Vector3.up;
            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            base.PlayCrossfade("Body", "FlyUp", "FlyUp.playbackRate", this.duration, 0.1f);
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            if (this.muzzleflashEffect)
            {
                EffectManager.SimpleMuzzleFlash(this.muzzleflashEffect, base.gameObject, "MuzzleLeft", false);
                EffectManager.SimpleMuzzleFlash(this.muzzleflashEffect, base.gameObject, "MuzzleRight", false);
            }
            if (base.isAuthority)
            {
                this.blastPosition = base.characterBody.corePosition;
            }
            if (NetworkServer.active)
            {
                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();

                    FireProjectileInfo blinkExplosionInfo = new FireProjectileInfo
                    {
                        projectilePrefab = this.explosionPrefab,
                        position = this.blastPosition,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        owner = base.gameObject,
                        damage = StaticValues.jumpDamageCoefficient * this.damageStat * this.fireDamageBonus,
                        force = 400f,
                        crit = base.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        target = null,
                        speedOverride = -1f
                    };
                    ProjectileManager.instance.FireProjectile(blinkExplosionInfo);
                }
            }
        }

        public override void HandleMovements()
        {
            base.HandleMovements();
            base.characterMotor.rootMotion += this.flyVector * (this.moveSpeedStat * this.speedCoefficientCurve.Evaluate(base.fixedAge / this.duration) * Time.fixedDeltaTime * this.speedCoefficient);
            base.characterMotor.velocity.y = 0f;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.blastPosition);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.blastPosition = reader.ReadVector3();
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            if (this.blinkPrefab)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = Util.QuaternionSafeLookRotation(this.flyVector);
                effectData.origin = origin;
                EffectManager.SpawnEffect(this.blinkPrefab, effectData, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= (this.duration - 0.2f) && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (!this.outer.destroying && !string.IsNullOrEmpty(endSoundString))
            {
                Util.PlaySound(this.endSoundString, base.gameObject);
            }
            base.OnExit();
        }
    }
}
