using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.SkillStates.BaseStates;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using EntityStates;
using RoR2.Orbs;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSpellLightning : BaseElementalSpell
    {
        [SerializeField] public GameObject chargePrefab;

        protected ManipulatorTracker tracker;
        protected HurtBox initialOrbTarget;
        protected Transform modelTransform;
        protected ChildLocator childLocator;
        protected bool successfulFire;

        private GameObject chargeEffect;
        private SurgeOrb firedOrb;

        public override void OnEnter()
        {
            base.OnEnter();

            this.modelTransform = base.GetModelTransform();
            this.tracker = base.GetComponent<ManipulatorTracker>();
            if (this.tracker && base.isAuthority)
            {
                this.initialOrbTarget = this.tracker.GetTrackingTarget();
            }
            if (this.modelTransform)
            {
                this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    Transform transform = this.childLocator.FindChild("hand.r");
                    if (transform && this.chargePrefab)
                    {
                        this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(this.chargePrefab, transform.position, transform.rotation);
                        this.chargeEffect.transform.parent = transform;
                    }
                }
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!this.hasFired && this.fixedAge >= this.attackStartTime)
            {
                if (this.chargeEffect)
                {
                    EntityState.Destroy(this.chargeEffect);
                }
                this.hasFiredAttempted = true;
                this.FireLightningSpell();
            }
            if (this.fixedAge >= this.duration && base.isAuthority)
            {
                if (!this.hasRemovedBuff && this.successfulFire) ElementalBonus(0);

                this.outer.SetNextStateToMain();
            }
        }
        
        private void FireLightningSpell()
        {
            if (initialOrbTarget)
            {
                this.hasFired = true;
                SurgeOrb surgeOrb = new SurgeOrb()
                {
                    damageValue = (damageStat * StaticValues.lightningDamageCoefficient) * fireDamageBonus,
                    damageCoefficientPerBounce = StaticValues.lightningBounceDamageMulti,
                    procCoefficient = StaticValues.lightningProcCoefficient,
                    bouncesRemaining = StaticValues.lightningBounceCount,
                    range = StaticValues.lightningBounceRange,
                    teamIndex = GetTeam(),
                    isCrit = RollCrit(),
                    attacker = gameObject,
                    bouncedObjects = new List<HealthComponent>(),
                    duration = 0.02f,
                    targetSkill = activatorSkillSlot
                };
                Transform transform = this.childLocator.FindChild("hand.l");
                surgeOrb.origin = transform.position;
                surgeOrb.target = initialOrbTarget;
                OrbManager.instance.AddOrb(surgeOrb);
                this.firedOrb = surgeOrb;
                this.successfulFire = true;
                
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.chargeEffect)
            {
                EntityState.Destroy(this.chargeEffect);
            }
            if (!this.successfulFire)
            {
                base.activatorSkillSlot.AddOneStock();
            }
        }
    }
}
