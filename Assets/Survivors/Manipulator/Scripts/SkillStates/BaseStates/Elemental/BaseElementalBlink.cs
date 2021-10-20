using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityStates;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules;

namespace ManipulatorMod.SkillStates.BaseStates
{
    class BaseElementalBlink : BaseSkillElement
    {
        [SerializeField]
        public GameObject blinkEffect;

        [SerializeField]
        public GameObject explosionPrefab;

        [SerializeField]
        public float explosionDelay;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtBoxGroup;
        private bool hasFired;

        private Vector3 entryLocation = Vector3.zero;
        private Vector3 entryVelocity = Vector3.zero;
        private Vector3 blinkVector = Vector3.zero;

        public override void OnEnter()
        {
            base.OnEnter();

            this.modelTransform = base.GetModelTransform();
            this.entryLocation = base.transform.position;

            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtBoxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
                this.entryVelocity = base.characterMotor.velocity;

                if (this.characterModel) this.characterModel.invisibilityCount++;
                if (this.hurtBoxGroup) hurtBoxGroup.hurtBoxesDeactivatorCounter += 1;
            }
            this.blinkVector = this.GetBlinkVector();
            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
            effectData.origin = origin;
            if (this.blinkEffect) EffectManager.SpawnEffect(this.blinkEffect, effectData, false);
        }

        protected virtual Vector3 GetBlinkVector()
        {
            return entryVelocity.normalized;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.fixedAge >= this.explosionDelay)
            {
                this.CreateExplosion();
            }
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * StaticValues.blinkSpeedCoefficient * Time.fixedDeltaTime);
            }
            if (this.fixedAge >= StaticValues.blinkDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        protected virtual void CreateExplosion()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();

                    FireProjectileInfo blinkExplosionInfo = new FireProjectileInfo
                    {
                        projectilePrefab = this.explosionPrefab,
                        position = this.entryLocation,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        owner = base.gameObject,
                        damage = StaticValues.explosionDamageCoefficient * this.damageStat * this.fireDamageBonus,
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

        public override void OnExit()
        {
            if (!this.outer.destroying)
            {
                this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                if (this.modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.6f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                    TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 0.7f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                }
            }

            if (this.characterModel) this.characterModel.invisibilityCount--;
            if (this.hurtBoxGroup) hurtBoxGroup.hurtBoxesDeactivatorCounter -= 1;
            if (this.characterMotor) this.characterMotor.velocity = entryVelocity;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
