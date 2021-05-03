using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
	//this is an amalgamation of ProjectileSingleTargetImapct and ProjectileImpactExplosion, creating a child projectile after hitting a target.
	[RequireComponent(typeof(ProjectileController))]
	public class ManipulatorProjectileWaveImpact : MonoBehaviour, IProjectileImpactBehavior
	{
		[Tooltip("Does this projectile release children on death?")]
		public bool fireChildren;

		public GameObject childrenProjectilePrefab;
		public int childrenCount;

		[Tooltip("What percentage of our damage does the children get?")]
		public float childrenDamageCoefficient;

		private ProjectileController projectileController;
		private ProjectileDamage projectileDamage;
		private bool alive = true;
		public bool destroyWhenNotAlive = true;
		public bool destroyOnWorld;
		public GameObject impactEffect;
		public string hitSoundString;
		public string enemyHitSoundString;
		private Vector3 impactNormal = Vector3.up;
		public Vector3 minAngleOffset;
		public Vector3 maxAngleOffset;
		public ProjectileImpactExplosion.TransformSpace transformSpace;
		public enum TransformSpace
		{
			World,
			Local,
			Normal
		}

		private Collider hitEnemyCollider;
		private ProjectileImpactInfo hitEnemyInfo;

		public bool useIceDebuff;

		private float falloffRate = StatValues.falloffRate;
		private float maxCoefficient = StatValues.falloffMax;
		private float stopwatch;
		private float minCoefficient = StatValues.falloffMin;

		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.stopwatch = this.maxCoefficient;
			Debug.LogWarning($"start stopwatch: {this.stopwatch}");
		}
		public void OnChildImpact(Collider other)
		{
			HurtBox component = other.GetComponent<HurtBox>();
			if (component)
			{
				ProjectileImpactInfo impactInfo = new ProjectileImpactInfo();
				impactInfo.collider = other.GetComponent<Collider>();
				impactInfo.estimatedPointOfImpact = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
				impactInfo.estimatedImpactNormal = new Vector3(0f, 0f, 0f);

				OnProjectileImpact(impactInfo);
			}
        }

		public void FixedUpdate()
        {
			this.stopwatch -= (Time.fixedDeltaTime * this.falloffRate);
			if (this.stopwatch <= this.minCoefficient) this.stopwatch = this.minCoefficient;
		}

		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			Debug.LogWarning($"stopwatch: {this.stopwatch}");
			Debug.LogWarning($"stopwatch divided: {this.stopwatch / this.maxCoefficient}");
			Debug.LogWarning($"damage: {this.projectileDamage.damage * (this.stopwatch / this.maxCoefficient)}");
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
					damageInfo.damage = this.projectileDamage.damage * (this.stopwatch/this.maxCoefficient);
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
								this.ApplyIceDebuff(healthComponent);
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

		private void ApplyIceDebuff(HealthComponent healthComponent)
        {
			if (useIceDebuff)
            {
				healthComponent.body.AddTimedBuff(Modules.Buffs.iceChillDebuff, StatValues.chillDebuffDuration, StatValues.chillDebuffMaxStacks);
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
	}
}
