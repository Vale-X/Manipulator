using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using RoR2.Orbs;

namespace ManipulatorMod.Modules.Components
{
    public class ChainLightningOrb : LightningOrb
    {
        public string orbEffectName;
		public GenericSkill skillSlot;
		public bool useBonus;

		public static event Action<ChainLightningOrb> onChainLightningOrbKilledOnAllBounces;
		private GameObject orbEffect;
		private CharacterBody ownerBody;
		private bool hasLightningBuff;
		private bool hasIceBuff;

		public override void Begin()
        {
			this.ownerBody = this.attacker.GetComponent<CharacterBody>();
			if (this.ownerBody.HasBuff(Modules.Buffs.lightningBuff)) this.hasLightningBuff = true;
			if (this.ownerBody.HasBuff(Modules.Buffs.iceBuff)) this.hasIceBuff = true;
			orbEffect = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/LightningOrbEffect");
			this.damageCoefficientPerBounce = 0.9f;

			FlattenOrbEffect(orbEffect);

            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };
            effectData.SetHurtBoxReference(this.target);
            EffectManager.SpawnEffect(orbEffect, effectData, true);
        }

		public void FlattenOrbEffect(GameObject inOrb)
        {
			OrbEffect orbEffect = inOrb.GetComponent<OrbEffect>();
			orbEffect.startVelocity1 = new Vector3(0f, 0f, 0f);
			orbEffect.startVelocity2 = new Vector3(0f, 0f, 0f);
			orbEffect.endVelocity1 = new Vector3(0f, 0f, 0f);
			orbEffect.endVelocity2 = new Vector3(0f, 0f, 0f);
		}

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
					if (this.useBonus)
					{
						if (this.hasLightningBuff == true)
						{
							this.skillSlot.rechargeStopwatch = this.skillSlot.rechargeStopwatch + (StaticValues.lightningCooldownReduction * (this.skillSlot.finalRechargeInterval - this.skillSlot.rechargeStopwatch));
						}
						if (this.hasIceBuff == true)
						{
							HealthComponent health = this.attacker.GetComponent<CharacterBody>().healthComponent;
							if (health)
							{
								health.AddBarrier(health.fullHealth * StaticValues.iceBarrierPercent);
							}
						}
					}
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
							ChainLightningOrb lightningOrb = new ChainLightningOrb();
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
							lightningOrb.duration = this.duration;
							lightningOrb.orbEffectName = this.orbEffectName;
							lightningOrb.skillSlot = this.skillSlot;
							lightningOrb.useBonus = this.useBonus;
							OrbManager.instance.AddOrb(lightningOrb);
						}
					}
					return;
				}
				if (!this.failedToKill)
				{
					Action<ChainLightningOrb> action = ChainLightningOrb.onChainLightningOrbKilledOnAllBounces;
					if (action == null)
					{
						return;
					}
					action(this);
				}
			}
		}

	}
}
