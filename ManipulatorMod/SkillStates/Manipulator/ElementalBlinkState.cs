using System;
using RoR2;
using UnityEngine;
using ManipulatorMod.Modules.Survivors;
using ManipulatorMod.Modules.Components;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates;
using UnityEngine.Networking;

namespace ManipulatorMod.SkillStates
{
    public class ElementalBlinkState : BaseSkillState
    {
		//necessary stuff
		private CharacterModel characterModel;
		private HurtBoxGroup hurtboxGroup;
		private Transform modelTransform;
		private Vector3 blinkVector = Vector3.zero;
		private Vector3 entryVelocity = Vector3.zero;
		private Vector3 entryLocation = Vector3.zero;
		public static GameObject blinkPrefab;
		private float stopwatch;
		private bool hasFired;
		private string muzzleString;
		private GameObject explosionPrefab = Modules.Projectiles.explosionFirePrefab;

		//stats from statvalues
		public float duration = StatValues.blinkDuration;
		public float speedCoefficient = StatValues.blinkSpeed;
		static float explosionDamage = StatValues.explosionDamage;
		

		public override void OnEnter()
		{
			base.OnEnter();

			this.checkFireBuff();

			ManipulatorController manipulatorController = characterBody.GetComponent<ManipulatorController>();
			
			//element check, set different wave and damage type based on current element. Can this be made better?
			switch (manipulatorController.currentElement)
			{
				case ManipulatorController.ManipulatorElement.Fire:
					explosionPrefab = Modules.Projectiles.explosionFirePrefab;
					break;
				case ManipulatorController.ManipulatorElement.Lightning:
					explosionPrefab = Modules.Projectiles.explosionLightningPrefab;
					break;
				case ManipulatorController.ManipulatorElement.Ice:
					explosionPrefab = Modules.Projectiles.explosionIcePrefab;
					break;
			}
			
			this.muzzleString = "Muzzle";
			this.modelTransform = base.GetModelTransform();
			this.entryLocation = base.transform.position;
			if (this.modelTransform)
			{
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
				this.entryVelocity = base.characterMotor.velocity;
			}
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount++;
			}
			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			this.blinkVector = this.GetBlinkVector();
			this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
		}

		private void CreateBlinkEffect(Vector3 origin)
        {
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
			effectData.origin = origin;
			EffectManager.SpawnEffect(Modules.Effects.ventBlinkEffect, effectData, false);
		}

		protected virtual Vector3 GetBlinkVector()
        {
			return entryVelocity.normalized;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.explosionDelay)
            {
				this.CreateExplosion();
            }
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity = Vector3.zero;
				base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime);
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}
		void CreateExplosion()
        {
			if (!this.hasFired)
			{
				this.hasFired = true;
				//EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);
				//Util.PlaySound("ManipulatorBombThrow", base.gameObject);

				if (base.isAuthority)
				{
					Ray aimRay = base.GetAimRay();

					FireProjectileInfo blinkExplosionInfo = new FireProjectileInfo
					{
						projectilePrefab = explosionPrefab,
						position = this.entryLocation,
						rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
						owner = base.gameObject,
						damage = ElementalBlinkState.explosionDamage * this.damageStat * this.fireDamageBonus,
						force = 400f,
						crit = base.RollCrit(),
						damageColorIndex = DamageColorIndex.Default,
						target = null,
						speedOverride = -1f
					};

					ProjectileManager.instance.FireProjectile(blinkExplosionInfo);

					//this.ElementalBonus(1);
				}
			}
		}

        public override void OnExit()
        {
			if (!this.outer.destroying)
			{
				this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
				this.modelTransform = base.GetModelTransform();
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
			if (this.characterModel)
			{
				this.characterModel.invisibilityCount--;
			}
			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			base.OnExit();
        }

		private void checkFireBuff()
        {
			if (base.HasBuff(Modules.Buffs.fireBonusBuff))
			{
				this.fireDamageBonus = 1 + StatValues.fireBuffAmount;
				//Debug.LogWarning($"Has Fire buff! {this.fireDamageBonus}");
			}
		}

		public void ElementalBonus(int enemiesHit)
        {
			//Debug.LogWarning($"BonusTriggered! {enemiesHit}");
			if (base.HasBuff(Modules.Buffs.fireBonusBuff))
			{
				this.fireDamageBonus = 1f;
				base.characterBody.RemoveBuff(Modules.Buffs.fireBonusBuff);
			}

			if (base.HasBuff(Modules.Buffs.lightningBonusBuff))
			{
				for (int i = 0; i < enemiesHit; i++)
				{
					base.skillLocator.utility.rechargeStopwatch = base.skillLocator.utility.rechargeStopwatch + (reductionPercent * (base.skillLocator.utility.finalRechargeInterval - base.skillLocator.utility.rechargeStopwatch));
				}
				base.characterBody.RemoveBuff(Modules.Buffs.lightningBonusBuff);
			}

			if (base.HasBuff(Modules.Buffs.iceBonusBuff))
			{
				for (int i = 0; i < enemiesHit; i++)
				{

					base.healthComponent.AddBarrier(base.healthComponent.fullHealth * StatValues.iceBarrierPercent);
				}
				base.characterBody.RemoveBuff(Modules.Buffs.iceBonusBuff);
			}
			this.hasRemovedBuff = true;
		}

		private bool hasRemovedBuff;
		private float fireDamageBonus = 1f;
		private float reductionPercent = StatValues.lightningCooldownReduction;
		private float explosionDelay = StatValues.blinkExplosionDelay;

	}
}
