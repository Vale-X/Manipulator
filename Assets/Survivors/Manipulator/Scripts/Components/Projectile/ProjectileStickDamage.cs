using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using RoR2;
using RoR2.Projectile;

namespace Assets.Survivors.Manipulator.Scripts.Components.Projectile
{
    [RequireComponent(typeof(ProjectileStickOnImpact))]
    class ProjectileStickDamage : MonoBehaviour
    {
		[Tooltip("Damage Coefficient of each hit, based on the ProjectileDamage's damage value.")]
        public float damageCoefficient = 1f;
		[Tooltip("Proc Coefficient of each hit, based on ProjectileController's proc coefficient.")]
        public float procCoefficient = 1f;
        [Tooltip("How much time between each hit?")]
        public float damageInterval = 1f;
		[Tooltip("Sound to play upon hitting an enemy.")]
		public string enemyHitSoundString;
		[Tooltip("Should the first hit occur upon sticking?")]
        public bool damageUponSticking;
		[Tooltip("Should the Projectile's Ghost be destroyed upon sticking?")]
		public bool destroyGhostOnStick;
		[Tooltip("Destroy the projectile if it sticks to the world.")]
		public bool destroyProjectileOnStickWorld;

        public UnityEvent onServerHit;

        private ProjectileStickOnImpact stickOnImpact;
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
		private ProjectileSingleTargetImpact singleTargetImpact;
		private bool active;
        private float stopwatch;

        public void Awake()
        {
            this.stickOnImpact = base.GetComponent<ProjectileStickOnImpact>();
            this.projectileController = base.GetComponent<ProjectileController>();
            this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.singleTargetImpact = base.GetComponent<ProjectileSingleTargetImpact>();
			this.stickOnImpact.stickEvent.AddListener(this.OnStickEvent);
        }

		public void OnStickEvent()
        {
			if (this.damageUponSticking) this.stopwatch = this.damageInterval;
			if (this.destroyGhostOnStick && this.projectileController.ghost)
            {
				UnityEngine.Object.Destroy(this.projectileController.ghost.gameObject);
				this.projectileController.ghost = null;
			}
			if(this.destroyProjectileOnStickWorld && !this.stickOnImpact.victim.GetComponent<HealthComponent>())
            {
				if (this.singleTargetImpact)
                {
					if (NetworkServer.active && this.singleTargetImpact.impactEffect)
					{
						EffectManager.SimpleImpactEffect(this.singleTargetImpact.impactEffect, base.transform.position, -base.transform.forward, !this.projectileController.isPrediction);
					}
					Util.PlaySound(this.singleTargetImpact.hitSoundString, base.gameObject);
				}
				UnityEngine.GameObject.Destroy(this.gameObject);
            }				
			this.active = true;
		}

        public void FixedUpdate()
        {
            if (!this.active) return;
            if (this.stickOnImpact.victim == null) { this.active = false; return; }

            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.damageInterval)
            {
				DamageVictim();
                this.stopwatch = 0f;
            }
        }

        public void DamageVictim()
        {
            if (!this.active) return;
			if (this.stickOnImpact.victim == null) return;

			if (this.stickOnImpact.victim)
            {
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage * this.damageCoefficient;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = this.projectileController.owner;
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = this.stickOnImpact.victim.transform.position;
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient * this.procCoefficient;
					damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
					damageInfo.damageType = this.projectileDamage.damageType;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}

				HealthComponent victimHealth = this.stickOnImpact.victim.GetComponent<HealthComponent>();
				if (!victimHealth)
                {
					Debug.Log("No Health Component on Victim!");
					return;
                }
				if (victimHealth)
                {
					if (victimHealth.gameObject == this.projectileController.owner)
					{
						return;
					}
					if (FriendlyFireManager.ShouldDirectHitProceed(victimHealth, this.projectileController.teamFilter.teamIndex))
					{
						Util.PlaySound(this.enemyHitSoundString, base.gameObject);
						if (NetworkServer.active)
						{
							CharacterBody victimBody = this.stickOnImpact.victim.GetComponent<CharacterBody>();
							damageInfo.ModifyDamageInfo(victimBody.mainHurtBox.damageModifier);
							victimHealth.TakeDamage(damageInfo);
							GlobalEventManager.instance.OnHitEnemy(damageInfo, victimHealth.gameObject);

						}
					}
				}
				damageInfo.position = base.transform.position;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, victimHealth.gameObject);

					UnityEvent unityEvent = this.onServerHit;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
			}
		}
    }
}
