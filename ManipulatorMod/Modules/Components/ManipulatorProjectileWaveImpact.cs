using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
	//this is an amalgamation of ProjectileSingleTargetImapct and ProjectileImpactExplosion, creating a child projectile after hitting a target.
	// Token: 0x02000639 RID: 1593
	[RequireComponent(typeof(ProjectileController))]
	public class ManipulatorProjectileWaveImpact : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x060026C9 RID: 9929 RVA: 0x000A2966 File Offset: 0x000A0B66

		private Collider hitEnemyCollider;
		private ProjectileImpactInfo hitEnemyInfo;

		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x000A2980 File Offset: 0x000A0B80
		public void OnChildImpact(Collider other)
		{
			HurtBox component = other.GetComponent<HurtBox>();
			if (component)
			{
				/*Debug.LogWarning("other: " + other);
				Debug.LogWarning("other.gameObject: " + other.gameObject);
				Debug.LogWarning("other.Collider: " + other.GetComponent<Collider>());
				Debug.LogWarning("other.Hurbox: " + other.GetComponent<HurtBox>());
				Debug.LogWarning("other.gameObject.HurtBox: " + other.gameObject.GetComponent<HurtBox>());
				Debug.LogWarning("other.Collider.HurtBox: " + other.GetComponent<Collider>().GetComponent<HurtBox>());
				Debug.LogWarning("other.CharacterBody: " + other.GetComponent<CharacterBody>());
				Debug.LogWarning("other.gameObject.CharacterBody: " + other.gameObject.GetComponent<CharacterBody>());
				Debug.LogWarning("other.HealthComponent: " + other.GetComponent<HealthComponent>());
				Debug.LogWarning("other.gameObject.HealthComponent: " + other.GetComponent<HealthComponent>());*/

				ProjectileImpactInfo impactInfo = new ProjectileImpactInfo();
				impactInfo.collider = other.GetComponent<Collider>();
				impactInfo.estimatedPointOfImpact = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
				impactInfo.estimatedImpactNormal = new Vector3(0f, 0f, 0f);

				OnProjectileImpact(impactInfo);
			}
        }

		/*public void OuterSetup()
        {
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.overlapAttack = new OverlapAttack();
			this.overlapAttack.procChainMask = this.projectileController.procChainMask;
			this.overlapAttack.procCoefficient = this.projectileController.procCoefficient * this.overlapProcCoefficient;
			this.overlapAttack.attacker = this.projectileController.owner;
			this.overlapAttack.inflictor = base.gameObject;
			this.overlapAttack.teamIndex = this.projectileController.teamFilter.teamIndex;
			this.overlapAttack.damage = this.overlapDamageCoefficient * this.projectileDamage.damage;
			this.overlapAttack.forceVector = this.overlapForceVector + this.projectileDamage.force * base.transform.forward;
			this.overlapAttack.hitEffectPrefab = this.impactEffect;
			this.overlapAttack.isCrit = this.projectileDamage.crit;
			this.overlapAttack.damageColorIndex = this.projectileDamage.damageColorIndex;
			this.overlapAttack.damageType = this.projectileDamage.damageType;
			this.overlapAttack.procChainMask = this.projectileController.procChainMask;
			this.overlapAttack.maximumOverlapTargets = this.overlapMaximumOverlapTargets;
			this.overlapAttack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
			Debug.LogWarning("setup attack info");
		}

		public void FixedUpdate()
		{
			Debug.LogWarning("Update");
			if (NetworkServer.active)
			{
				Debug.LogWarning("ServerIsActive");
				if (this.overlapResetInterval >= 0f)
				{
					Debug.LogWarning("ResetInterval >= 0f");
					this.overlapResetTimer -= Time.fixedDeltaTime;
					if (this.overlapResetTimer <= 0f)
					{
						Debug.LogWarning("overlapReset <= 0f");
						this.overlapResetTimer = this.overlapResetInterval;
						this.ResetOverlapAttack();
					}
				}
				this.overlapFireTimer -= Time.fixedDeltaTime;
				Debug.LogWarning("");
				if (this.overlapFireTimer <= 0f)
				{
					this.overlapFireTimer = 1f / this.overlapFireFrequency;
					this.overlapAttack.Fire(null);
					Debug.LogWarning("AttackFired");
				}
			}
		}

		public void ResetOverlapAttack()
		{
			this.overlapAttack.damageType = this.projectileDamage.damageType;
			this.overlapAttack.ResetIgnoredHealthComponents();
		}*/

		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{

			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			hitEnemyInfo = impactInfo;
			hitEnemyCollider = collider;
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
							Util.PlaySound(this.enemyHitSoundString, base.gameObject);
							if (NetworkServer.active)
							{
								damageInfo.ModifyDamageInfo(component.damageModifier);
								healthComponent.TakeDamage(damageInfo);
								GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
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
			if (this.fireChildren)
			{
				for (int i = 0; i < this.childrenCount; i++)
				{
					Vector3 vector = new Vector3(UnityEngine.Random.Range(this.minAngleOffset.x, this.maxAngleOffset.x), UnityEngine.Random.Range(this.minAngleOffset.z, this.maxAngleOffset.z), UnityEngine.Random.Range(this.minAngleOffset.z, this.maxAngleOffset.z));
					switch (this.transformSpace)
					{
						case ProjectileImpactExplosion.TransformSpace.World:
							this.FireChild(vector);
							break;
						case ProjectileImpactExplosion.TransformSpace.Local:
							this.FireChild(base.transform.forward + base.transform.TransformDirection(vector));
							break;
						case ProjectileImpactExplosion.TransformSpace.Normal:
							this.FireChild(this.impactNormal + vector);
							break;
					}
				}
			}
			if (!this.alive)
			{
				if (NetworkServer.active && this.impactEffect)
				{
					EffectManager.SimpleImpactEffect(this.impactEffect, impactInfo.estimatedPointOfImpact, -base.transform.forward, !this.projectileController.isPrediction);
				}
				Util.PlaySound(this.hitSoundString, base.gameObject);
				if (this.destroyWhenNotAlive)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		protected void FireChild(Vector3 direction)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childrenProjectilePrefab, base.transform.position, Util.QuaternionSafeLookRotation(direction));
			ProjectileController component = gameObject.GetComponent<ProjectileController>();
			if (component)
			{
				component.procChainMask = this.projectileController.procChainMask;
				component.procCoefficient = this.projectileController.procCoefficient;
				component.Networkowner = this.projectileController.owner;
			}
			gameObject.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
			ProjectileDamage component2 = gameObject.GetComponent<ProjectileDamage>();
			if (component2)
			{
				component2.damage = this.projectileDamage.damage * this.childrenDamageCoefficient;
				component2.crit = this.projectileDamage.crit;
				component2.force = this.projectileDamage.force;
				component2.damageColorIndex = this.projectileDamage.damageColorIndex;
			}
			if (gameObject.GetComponent<ProjectileStickOnImpact>() != null)
            {
				ProjectileStickOnImpact stickComponent = gameObject.GetComponent<ProjectileStickOnImpact>();
				stickComponent.TrySticking(hitEnemyCollider, hitEnemyInfo.estimatedImpactNormal);
            }
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x04002175 RID: 8565
		[Tooltip("Does this projectile release children on death?")]
		public bool fireChildren;

		// Token: 0x04002176 RID: 8566
		public GameObject childrenProjectilePrefab;

		// Token: 0x04002177 RID: 8567
		public int childrenCount;

		// Token: 0x04002178 RID: 8568
		[Tooltip("What percentage of our damage does the children get?")]
		public float childrenDamageCoefficient;

		// Token: 0x04002208 RID: 8712
		private ProjectileController projectileController;

		// Token: 0x04002209 RID: 8713
		private ProjectileDamage projectileDamage;

		// Token: 0x0400220A RID: 8714
		private bool alive = true;

		// Token: 0x0400220B RID: 8715
		public bool destroyWhenNotAlive = true;

		// Token: 0x0400220C RID: 8716
		public bool destroyOnWorld;

		// Token: 0x0400220D RID: 8717
		public GameObject impactEffect;

		// Token: 0x0400220E RID: 8718
		public string hitSoundString;

		// Token: 0x0400220F RID: 8719
		public string enemyHitSoundString;

		// Token: 0x04002163 RID: 8547
		private Vector3 impactNormal = Vector3.up;

		// Token: 0x04002179 RID: 8569
		public Vector3 minAngleOffset;

		// Token: 0x0400217A RID: 8570
		public Vector3 maxAngleOffset;

		// Token: 0x0400217B RID: 8571
		public ProjectileImpactExplosion.TransformSpace transformSpace;

		public enum TransformSpace
		{
			// Token: 0x04002182 RID: 8578
			World,
			// Token: 0x04002183 RID: 8579
			Local,
			// Token: 0x04002184 RID: 8580
			Normal
		}
	}
}
