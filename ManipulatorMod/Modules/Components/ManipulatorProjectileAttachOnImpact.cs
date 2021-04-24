using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
	[RequireComponent(typeof(ProjectileController))]
	public class ManipulatorProjectileAttachOnImpact : MonoBehaviour, IProjectileImpactBehavior
	{
		public GameObject attachPrefab;
		public GameObject hitEnemeyObject;

		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x000A2980 File Offset: 0x000A0B80
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			this.hitEnemeyObject = impactInfo.collider.gameObject;
			if (collider)
			{
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = this.projectileController.owner;
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = impactInfo.estimatedPointOfImpact;
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
					damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
					damageInfo.damageType = this.projectileDamage.damageType;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						if (healthComponent.gameObject == this.projectileController.owner)
						{
							return;
						}
						if (FriendlyFireManager.ShouldDirectHitProceed(healthComponent, this.projectileController.teamFilter.teamIndex))
						{
							Util.PlaySound(this.enemyHitSoundString, this.gameObject);
							if (NetworkServer.active)
							{
								damageInfo.ModifyDamageInfo(component.damageModifier);
								healthComponent.TakeDamage(damageInfo);
								GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
								if (healthComponent.alive)
								{
									this.AttachToTarget(damageInfo, component);
								}
								else
                                {
									hasKilled = true;
                                }								
							}
						}
						this.alive = false;
					}
				}
				else if (this.destroyOnWorld)
				{
					this.alive = false;
				}
				damageInfo.position = base.transform.position;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}
			if (!this.alive)
			{
				if (NetworkServer.active && this.impactEffect)
				{
					EffectManager.SimpleImpactEffect(this.impactEffect, impactInfo.estimatedPointOfImpact, -this.transform.forward, !this.projectileController.isPrediction);
				}
				Util.PlaySound(this.hitSoundString, this.gameObject);
				if (this.destroyWhenNotAlive)
				{
					UnityEngine.Object.Destroy(this.gameObject);
				}
				else
                {
					if (impactInfo.collider.GetComponent<HurtBox>() && !hasKilled)
                    {
						//Debug.LogWarning("set FireSpell to inactive");
						this.gameObject.SetActive(false);
						this.ownerController.ghostPrefab.SetActive(false);
					}
					else 
                    {
							//Debug.LogWarning("Destroyed FireSpell");
							UnityEngine.Object.Destroy(this.gameObject);
					}
                }
			}
		}

		private void AttachToTarget(DamageInfo damageInfo, HurtBox hurtBox)
        {
			if (!this.hasAttached)
            {
				this.hasAttached = true;
				//Debug.LogWarning("AttachedToTarget");

				//Debug.LogWarning(this.attachPrefab);
				//Debug.LogWarning(this.attachPrefab.transform);
				//Debug.LogWarning(this.attachPrefab.transform.parent);
				//Debug.LogWarning(this.hitEnemeyObject.transform.root);
				this.attachPrefab.transform.SetParent(this.hitEnemeyObject.transform.root);

				ManipulatorAttachDamage attachPrefabDamage = this.attachPrefab.GetComponent<ManipulatorAttachDamage>();
				attachPrefabDamage.attachDamageInfo = damageInfo;
				attachPrefabDamage.attachDamageInfo.procCoefficient = StatValues.fireAttachProc;
				attachPrefabDamage.attachDamageInfo.damage = (this.projectileDamage.damage / StatValues.fireSpellCoefficient) * StatValues.fireAttachCoefficient;
				attachPrefabDamage.attachHurtBox = hurtBox;
				attachPrefabDamage.projectileOwner = this;
				attachPrefabDamage.tickMax = this.attachTickMax;
				attachPrefabDamage.attachDuration = this.attachDuration;
				attachPrefabDamage.EnableAttach();
			}
        }

		public void DestroyFromAttach()
        {
			//Debug.LogWarning("DestroyFromAttach");
			this.gameObject.SetActive(true);
			this.ownerController.ghostPrefab.SetActive(true);
			UnityEngine.Object.Destroy(this.gameObject);

		}

		private ProjectileController projectileController;
		private ProjectileDamage projectileDamage;
		private bool alive = true;
		private bool hasAttached = false;
		private bool hasKilled = false;
		public bool destroyWhenNotAlive = true;
		public bool destroyOnWorld;
		public GameObject impactEffect;
		public string hitSoundString;
		public string enemyHitSoundString;
		public int attachTickMax;
		public float attachDuration;
		public ProjectileController ownerController;
		public float attachDamageCoefficient;
	}
}
