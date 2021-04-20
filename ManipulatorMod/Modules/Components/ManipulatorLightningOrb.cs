using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ManipulatorMod.Modules;
using RoR2.Orbs;
using RoR2;

namespace ManipulatorMod.Modules.Components
{
	// Token: 0x020005E2 RID: 1506
	public class ManipulatorLightningOrb : Orb
	{
		GameObject loadedOrb = ManipulatorMod.Modules.Orbs.chainLightningOrb;

		// Token: 0x06002507 RID: 9479 RVA: 0x00098C90 File Offset: 0x00096E90
		public override void Begin()
		{
			base.duration = 0.1f;
			/*string path = null;
			switch (this.lightningType)
			{
				case ManipulatorLightningOrb.LightningType.Ukulele:
					path = "Prefabs/Effects/OrbEffects/LightningOrbEffect";
					break;
				case ManipulatorLightningOrb.LightningType.Tesla:
					path = "Prefabs/Effects/OrbEffects/TeslaOrbEffect";
					break;
				case ManipulatorLightningOrb.LightningType.BFG:
					path = "Prefabs/Effects/OrbEffects/BeamSphereOrbEffect";
					base.duration = 0.4f;
					break;
				case ManipulatorLightningOrb.LightningType.TreePoisonDart:
					path = "Prefabs/Effects/OrbEffects/TreePoisonDartOrbEffect";
					this.speed = 40f;
					base.duration = base.distanceToTarget / this.speed;
					break;
				case ManipulatorLightningOrb.LightningType.HuntressGlaive:
					path = "Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect";
					base.duration = base.distanceToTarget / this.speed;
					this.canBounceOnSameTarget = true;
					break;
				case ManipulatorLightningOrb.LightningType.Loader:
					path = "Prefabs/Effects/OrbEffects/LoaderLightningOrbEffect";
					break;
				case ManipulatorLightningOrb.LightningType.RazorWire:
					path = "Prefabs/Effects/OrbEffects/RazorwireOrbEffect";
					base.duration = 0.2f;
					break;
				case ManipulatorLightningOrb.LightningType.CrocoDisease:
					path = "Prefabs/Effects/OrbEffects/CrocoDiseaseOrbEffect";
					base.duration = 0.6f;
					this.targetsToFindPerBounce = 2;
					break;
			}*/
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(loadedOrb, effectData, true);
			//Debug.LogWarning(loadedOrb);
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x00098DB8 File Offset: 0x00096FB8
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = this.inflictor;
					damageInfo.force = Vector3.zero;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					damageInfo.damageType = this.damageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
				this.failedToKill |= (!healthComponent || healthComponent.alive);
				if (this.bouncesRemaining > 0)
				{
					for (int i = 0; i < this.targetsToFindPerBounce; i++)
					{
						if (this.bouncedObjects != null)
						{
							if (this.canBounceOnSameTarget)
							{
								this.bouncedObjects.Clear();
							}
							this.bouncedObjects.Add(this.target.healthComponent);
						}
						HurtBox hurtBox = this.PickNextTarget(this.target.transform.position);
						if (hurtBox)
						{
							ManipulatorLightningOrb lightningOrb = new ManipulatorLightningOrb();
							lightningOrb.search = this.search;
							lightningOrb.origin = this.target.transform.position;
							lightningOrb.target = hurtBox;
							lightningOrb.attacker = this.attacker;
							lightningOrb.inflictor = this.inflictor;
							lightningOrb.teamIndex = this.teamIndex;
							lightningOrb.damageValue = this.damageValue * this.damageCoefficientPerBounce;
							lightningOrb.bouncesRemaining = this.bouncesRemaining - 1;
							lightningOrb.isCrit = this.isCrit;
							lightningOrb.bouncedObjects = this.bouncedObjects;
							lightningOrb.lightningType = this.lightningType;
							lightningOrb.procChainMask = this.procChainMask;
							lightningOrb.procCoefficient = this.procCoefficient;
							lightningOrb.damageColorIndex = this.damageColorIndex;
							lightningOrb.damageCoefficientPerBounce = this.damageCoefficientPerBounce;
							lightningOrb.speed = this.speed;
							lightningOrb.range = this.range;
							lightningOrb.damageType = this.damageType;
							lightningOrb.failedToKill = this.failedToKill;
							OrbManager.instance.AddOrb(lightningOrb);
						}
					}
					return;
				}
				if (!this.failedToKill)
				{
					Action<ManipulatorLightningOrb> action = ManipulatorLightningOrb.onLightningOrbKilledOnAllBounces;
					if (action == null)
					{
						return;
					}
					action(this);
				}
			}
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x00099064 File Offset: 0x00097264
		public HurtBox PickNextTarget(Vector3 position)
		{
			if (this.search == null)
			{
				this.search = new BullseyeSearch();
			}
			this.search.searchOrigin = position;
			this.search.searchDirection = Vector3.zero;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.teamIndex);
			this.search.filterByLoS = false;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.range;
			this.search.RefreshCandidates();
			HurtBox hurtBox = (from v in this.search.GetResults()
							   where !this.bouncedObjects.Contains(v.healthComponent)
							   select v).FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}

		// Token: 0x1400009E RID: 158
		// (add) Token: 0x0600250A RID: 9482 RVA: 0x00099138 File Offset: 0x00097338
		// (remove) Token: 0x0600250B RID: 9483 RVA: 0x0009916C File Offset: 0x0009736C
		public static event Action<ManipulatorLightningOrb> onLightningOrbKilledOnAllBounces;

		// Token: 0x04001FB6 RID: 8118
		public float speed = 100f;

		// Token: 0x04001FB7 RID: 8119
		public float damageValue;

		// Token: 0x04001FB8 RID: 8120
		public GameObject attacker;

		// Token: 0x04001FB9 RID: 8121
		public GameObject inflictor;

		// Token: 0x04001FBA RID: 8122
		public int bouncesRemaining;

		// Token: 0x04001FBB RID: 8123
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001FBC RID: 8124
		public TeamIndex teamIndex;

		// Token: 0x04001FBD RID: 8125
		public bool isCrit;

		// Token: 0x04001FBE RID: 8126
		public ProcChainMask procChainMask;

		// Token: 0x04001FBF RID: 8127
		public float procCoefficient = 1f;

		// Token: 0x04001FC0 RID: 8128
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001FC1 RID: 8129
		public float range = 20f;

		// Token: 0x04001FC2 RID: 8130
		public float damageCoefficientPerBounce = 1f;

		// Token: 0x04001FC3 RID: 8131
		public int targetsToFindPerBounce = 1;

		// Token: 0x04001FC4 RID: 8132
		public DamageType damageType;

		// Token: 0x04001FC5 RID: 8133
		private bool canBounceOnSameTarget;

		// Token: 0x04001FC6 RID: 8134
		private bool failedToKill;

		// Token: 0x04001FC7 RID: 8135
		public ManipulatorLightningOrb.LightningType lightningType;

		// Token: 0x04001FC8 RID: 8136
		private BullseyeSearch search;

		// Token: 0x020005E3 RID: 1507
		public enum LightningType
		{
			// Token: 0x04001FCB RID: 8139
			Ukulele,
			// Token: 0x04001FCC RID: 8140
			Tesla,
			// Token: 0x04001FCD RID: 8141
			BFG,
			// Token: 0x04001FCE RID: 8142
			TreePoisonDart,
			// Token: 0x04001FCF RID: 8143
			HuntressGlaive,
			// Token: 0x04001FD0 RID: 8144
			Loader,
			// Token: 0x04001FD1 RID: 8145
			RazorWire,
			// Token: 0x04001FD2 RID: 8146
			CrocoDisease,
			// Token: 0x04001FD3 RID: 8147
			Count
		}
	}
}
