using System;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;
using ManipulatorMod;
using ManipulatorMod.SkillStates;
using ManipulatorMod.Modules.Components;
using RoR2.Projectile;
using RoR2;

namespace ManipulatorMod.Modules.Components
{
	// Token: 0x02000628 RID: 1576
	[RequireComponent(typeof(EntityStateMachine))]
	[RequireComponent(typeof(ProjectileSimple))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileIcePillarExplode : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06002661 RID: 9825 RVA: 0x000A02AD File Offset: 0x0009E4AD
		protected void Awake()
		{

			this.projectileController = base.GetComponent<ProjectileController>();

			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lifetime += UnityEngine.Random.Range(0f, this.lifetimeRandomOffset);
			//Debug.LogWarning($"Controller get: {base.GetComponent<ProjectileController>()}");
			//Debug.LogWarning($"activation state: {this.owner.skillRef.activationState}");
			//Debug.LogWarning($"GetType: {this.owner.stateMachine.GetType()}");
			//Debug.LogWarning($"GetComponent: {this.owner.stateMachine.GetComponent<ElementalSpell>()}");
			//Debug.LogWarning($"MainStateType: {this.owner.stateMachine.mainStateType}");
			//Debug.LogWarning($"StateMachine: {this.owner.stateMachine}");

			
		}

		protected void Start()
        {
			this.owner = new ProjectileIcePillarExplode.OwnerInfo(this.projectileController.owner);
			
			/*Debug.LogWarning($"base.GetComponent<ProjectileController>(): {base.GetComponent<ProjectileController>()}");
			Debug.LogWarning($"this.owner: {this.owner}");
			Debug.LogWarning($"this.projectileController: {this.projectileController}");
			Debug.LogWarning($"this.projectileController.Networkowner: {this.projectileController.Networkowner}");
			Debug.LogWarning($"this.projectileController.owner: {this.projectileController.owner}");*/

			this.AssignReferenceToBody();
		}

		// Token: 0x06002662 RID: 9826 RVA: 0x000A02E4 File Offset: 0x0009E4E4
		protected void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			//Debug.LogWarning($"State: {this.owner.stateMachine.state}");
			if (NetworkServer.active || this.projectileController.isPrediction)
			{
				if (this.timerAfterImpact && this.hasImpact)
				{
					this.stopwatchAfterImpact += Time.fixedDeltaTime;
				}
				bool flag = this.stopwatch >= this.lifetime;
				bool flag2 = this.timerAfterImpact && this.stopwatchAfterImpact > this.lifetimeAfterImpact;
				if (flag || flag2)
				{
					this.alive = false;
				}
				if (this.alive && !this.hasPlayedLifetimeExpiredSound)
				{
					bool flag4 = this.stopwatch > this.lifetime - this.offsetForLifetimeExpiredSound;
					if (this.timerAfterImpact)
					{
						flag4 |= (this.stopwatchAfterImpact > this.lifetimeAfterImpact - this.offsetForLifetimeExpiredSound);
					}
					if (flag4)
					{
						this.hasPlayedLifetimeExpiredSound = true;
						if (NetworkServer.active && this.lifetimeExpiredSound)
						{
							PointSoundManager.EmitSoundServer(this.lifetimeExpiredSound.index, base.transform.position);
						}
					}
				}

				if (!this.hasImpact)
                {
					this.DetonateServer();
					this.hasImpact = true;
				}

				if (this.stopwatch >= this.detonateTimer && !this.hasDetonated)
                {
					this.FreezeDetonate();
					this.hasDetonated = true;
                }

				if (!this.alive)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void FreezeDetonate()
        {
			if (this.projectileDamage)
			{
				new BlastAttack
				{
					position = base.transform.position,
					baseDamage = this.projectileDamage.damage,
					baseForce = this.projectileDamage.force,
					radius = this.blastRadius * 2,
					attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null),
					inflictor = base.gameObject,
					teamIndex = this.projectileController.teamFilter.teamIndex,
					crit = this.projectileDamage.crit,
					procChainMask = this.projectileController.procChainMask,
					procCoefficient = this.projectileController.procCoefficient * this.blastProcCoefficient,
					bonusForce = new Vector3(0f, 0f, 0f),
					falloffModel = this.falloffModel,
					damageColorIndex = this.projectileDamage.damageColorIndex,
					damageType = this.projectileDamage.damageType,
					attackerFiltering = this.blastAttackerFiltering
				}.Fire();
				this.alive = false;
			}
		}

		private void AssignReferenceToBody()
        {
			ElementalSpellState elementalSpell;
			//Debug.LogWarning($"owner: {this.owner}");
			//Debug.LogWarning($"projectileowner: {this.projectileController.owner}");
			//Debug.LogWarning($"projectileowner controller: {this.projectileController.owner.GetComponent<ManipulatorController>()}");
			//Debug.LogWarning($"owner statemachine: {this.owner.stateMachine}");
			//Debug.LogWarning($"statemachine customname: {this.owner.stateMachine.customName}");
			//Debug.LogWarning($"owner statemachine name: {this.owner.stateMachine.name}");
			//Debug.LogWarning($"owner state: {this.owner.stateMachine.state}");
			//Debug.LogWarning($"owner state as ele: {this.owner.stateMachine.state as ElementalSpell}");
			elementalSpell = this.owner.stateMachine.state as ElementalSpellState;
			//Debug.LogWarning($"this.owner: {this.owner}");
			//Debug.LogWarning($"this.owner.stateMachine: {this.owner.stateMachine}");
			//Debug.LogWarning($"this.owner.stateMachine.state: {this.owner.stateMachine.state}");
			//Debug.LogWarning($"this.owner.stateMachine.state as ElementalSpell: {elementalSpell}");
			if (this.owner.stateMachine && elementalSpell != null)
			{
				//elementalSpell.SetPillarReference(base.gameObject);
				this.elementalRef = elementalSpell;
				//Debug.LogWarning(elementalSpell);
				//Debug.LogWarning($"this.elementalRef: {this.elementalRef}");
			}
			/*Transform modelTransform = this.owner.gameObject.GetComponent<ModelLocator>().modelTransform;
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(this.muzzleStringOnBody);
					if (transform)
					{
						this.ropeEndTransform.SetParent(transform, false);
					}
				}
			}*/
		}

		// Token: 0x06002663 RID: 9827 RVA: 0x000A0434 File Offset: 0x0009E634
		protected void DetonateServer()
		{
			if (this.impactEffect)
			{
				EffectManager.SpawnEffect(this.impactEffect, new EffectData
				{
					origin = base.transform.position,
					scale = this.blastRadius
				}, true);
			}
			if (this.projectileDamage)
			{
				this.pillarBlast = new BlastAttack
				{
					position = base.transform.position,
					baseDamage = this.projectileDamage.damage,
					baseForce = this.projectileDamage.force,
					radius = this.blastRadius,
					attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null),
					inflictor = base.gameObject,
					teamIndex = this.projectileController.teamFilter.teamIndex,
					crit = this.projectileDamage.crit,
					procChainMask = this.projectileController.procChainMask,
					procCoefficient = this.projectileController.procCoefficient * this.blastProcCoefficient,
					bonusForce = this.bonusBlastForce,
					falloffModel = this.falloffModel,
					damageColorIndex = this.projectileDamage.damageColorIndex,
					damageType = this.projectileDamage.damageType,
					attackerFiltering = this.blastAttackerFiltering
				}.Fire();
				this.elementalRef.ElementalBonus(this.pillarBlast.hitCount);
			}
			/*if (this.explosionSoundString.Length > 0)
			{
				Util.PlaySound(this.explosionSoundString, base.gameObject);
			}*/
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x000A0788 File Offset: 0x0009E988
		public void SetExplosionRadius(float newRadius)
		{
			this.blastRadius = newRadius;
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000A0794 File Offset: 0x0009E994
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			/*if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			this.impactNormal = impactInfo.estimatedImpactNormal;
			if (collider)
			{
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = impactInfo.estimatedPointOfImpact;
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
						HealthComponent healthComponent = component.healthComponent;
				}
				this.hasImpact = true;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}*/
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x000A0924 File Offset: 0x0009EB24
		public void SetAlive(bool newAlive)
		{
			this.alive = newAlive;
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x000A0930 File Offset: 0x0009EB30
		protected void OnValidate()
		{
			if (Application.IsPlaying(this))
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.explosionSoundString))
			{
				Debug.LogWarningFormat(base.gameObject, "{0} ProjectileImpactExplosion component supplies a value in the explosionSoundString field. This will not play correctly over the network. Please move the sound to the explosion effect.", new object[]
				{
					Util.GetGameObjectHierarchyName(base.gameObject)
				});
			}
			if (!string.IsNullOrEmpty(this.lifetimeExpiredSoundString))
			{
				Debug.LogWarningFormat(base.gameObject, "{0} ProjectileImpactExplosion component supplies a value in the lifetimeExpiredSoundString field. This will not play correctly over the network. Please use lifetimeExpiredSound instead.", new object[]
				{
					Util.GetGameObjectHierarchyName(base.gameObject)
				});
			}
		}

		public int returnBlastResult
		{
			get { return pillarBlast.hitCount; }
			set { }
		}

		private ProjectileController projectileController;
		private ProjectileDamage projectileDamage;
		private bool alive = true;
		private Vector3 impactNormal = Vector3.up;
		public GameObject impactEffect;

		[Obsolete]
		public string explosionSoundString;

		[Obsolete("Use lifetimeExpiredSound instead.")]
		public string lifetimeExpiredSoundString;

		public NetworkSoundEventDef lifetimeExpiredSound;

		public float offsetForLifetimeExpiredSound;
		public bool destroyOnEnemy = true;
		public bool destroyOnWorld;
		public bool timerAfterImpact;

		public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;
		public float lifetime;
		public float lifetimeAfterImpact;
		public float lifetimeRandomOffset;
		public float blastRadius;

		[Tooltip("The percentage of the damage, proc coefficient, and force of the initial projectile. Ranges from 0-1")]
		public float blastDamageCoefficient;
		public float blastProcCoefficient = 1f;
		public AttackerFiltering blastAttackerFiltering;
		public Vector3 bonusBlastForce;

		[Tooltip("Does this projectile release children on death?")]
		public bool fireChildren;
		public GameObject childrenProjectilePrefab;
		public int childrenCount;

		[Tooltip("What percentage of our damage does the children get?")]
		public float childrenDamageCoefficient;

		public ProjectileIcePillarExplode.OwnerInfo owner;

		public struct OwnerInfo
		{
			public OwnerInfo(GameObject ownerGameObject)
			{
				this = default(ProjectileIcePillarExplode.OwnerInfo);
				this.gameObject = ownerGameObject;
				if (this.gameObject)
				{
					this.characterBody = this.gameObject.GetComponent<CharacterBody>();
					this.characterMotor = this.gameObject.GetComponent<CharacterMotor>();
					this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
					//this.characterController = this.gameObject.GetComponent<ManipulatorController>();
					//Debug.LogWarning($"characterController: {this.characterController}");
					this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.gameObject);

					EntityStateMachine[] components = this.gameObject.GetComponents<EntityStateMachine>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i].customName == "Weapon")
						{
							this.stateMachine = components[i];
							//Debug.LogWarning($"Pillar statemachine: {this.stateMachine}");
							//return;
						}
					}
					
					this.skillLocator = this.gameObject.GetComponent<SkillLocator>();
					//Debug.LogWarning($"skillRef: {this.skillLocator}");
					//Debug.LogWarning($"Secondary: {this.skillLocator.secondary}");
					//Debug.Log($"StateMachine: {this.skillLocator.secondary.stateMachine}");
					//Debug.Log($"State: {this.skillLocator.secondary.stateMachine.state}");
					//this.elementalRef = this.skillRef.stateMachine.state as ElementalSpell;
				}
			}

			public readonly GameObject gameObject;
			public readonly CharacterBody characterBody;
			public readonly CharacterMotor characterMotor;
			public readonly Rigidbody rigidbody;
			public readonly EntityStateMachine stateMachine;
			public readonly bool hasEffectiveAuthority;
			public readonly ElementalSpellState elementalRef;
			public readonly SkillLocator skillLocator;
			public readonly ManipulatorController characterController;
		}

		private BlastAttack.Result freezeBlast;
		public BlastAttack.Result pillarBlast;
		private ElementalSpellState elementalRef;
		private bool hasDetonated;

		public Vector3 minAngleOffset;
		public Vector3 maxAngleOffset;
		public ProjectileImpactExplosion.TransformSpace transformSpace;
		public HealthComponent projectileHealthComponent;

		private float stopwatch;
		private float stopwatchAfterImpact;
		private float detonateTimer = StatValues.icePillarLifetime;
		private bool hasImpact;
		private bool hasPlayedLifetimeExpiredSound;


		public enum TransformSpace
		{
			World,
			Local,
			Normal
		}
	}
}
