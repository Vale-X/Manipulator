using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    public class SurgeOrb : Orb
    {
        public float damageValue;
        public float procCoefficient = 1f;
        public int bouncesRemaining;
        public bool isCrit;
        public float scale;
        public float range = 20f;
        [Tooltip("Unlike LightningOrb, this reduces the damageValue by a flat amount based on the damageValue of the initial bounce.")]
        public float damageCoefficientPerBounce = 1f;
        public float damageReduction = -1f;
        public TeamIndex teamIndex;
        public DamageColorIndex damageColorIndex;
        public ProcChainMask procChainMask;
        public GameObject attacker;
        public GameObject inflictor;
        public List<HealthComponent> bouncedObjects;
        public bool useLightningBonus;
        public bool useIceBonus;
        public bool checkedBonuses;
        public GenericSkill targetSkill;

        private BullseyeSearch search;

        public override void Begin()
        {
            if (GetOrbEffect())
            {
                EffectData effectData = new EffectData
                {
                    scale = scale,
                    origin = origin,
                    genericFloat = duration
                };
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(GetOrbEffect(), effectData, true);
            }

            if (!checkedBonuses)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    if (attackerBody.HasBuff(Buffs.lightningBuff)) useLightningBonus = true;
                    if (attackerBody.HasBuff(Buffs.iceBuff)) useIceBonus = true;
                }
                checkedBonuses = true;
            }
        }

        public GameObject GetOrbEffect()
        {
            //var effect = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/LightningOrbEffect");
            var effect = Assets.mainAssetBundle.LoadAsset<GameObject>("SurgeOrbEffect");
            effect.GetComponent<OrbEffect>().endEffect = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLightnings");
            //StubbedConverter.MaterialController.AddMaterialController(Object);
            return effect;
            //return 
        }

        public override void OnArrival()
        {
            if (target)
            {
                var hc = target.healthComponent;
                if (hc)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = damageValue;
                    damageInfo.attacker = attacker;
                    damageInfo.force = Vector3.zero;
                    damageInfo.crit = isCrit;
                    damageInfo.procChainMask = procChainMask;
                    damageInfo.procCoefficient = procCoefficient;
                    damageInfo.position = target.transform.position;
                    damageInfo.damageColorIndex = DamageColorIndex.Default;
                    damageInfo.damageType = DamageType.Generic;
                    ApplyElementBonuses(damageInfo);
                    hc.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, hc.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, hc.gameObject);
                }
                if (bouncesRemaining > 0)
                {
                    this.bouncedObjects.Add(target.healthComponent);
                    var hb = PickNextTarget(target.transform.position);
                    if (hb)
                    {
                        if (damageReduction <= -0.1f) damageReduction = damageValue - Mathf.Clamp(damageValue * damageCoefficientPerBounce, 0f, float.MaxValue);
                        Debug.LogWarning(damageReduction);
                        SurgeOrb surgeOrb = new SurgeOrb();
                        surgeOrb.search = search;
                        surgeOrb.origin = target.transform.position;
                        surgeOrb.target = hb;
                        surgeOrb.attacker = attacker;
                        surgeOrb.inflictor = inflictor;
                        surgeOrb.teamIndex = teamIndex;
                        surgeOrb.damageValue = damageValue - damageReduction;
                        surgeOrb.bouncesRemaining = bouncesRemaining - 1;
                        surgeOrb.isCrit = isCrit;
                        surgeOrb.bouncedObjects = bouncedObjects;
                        surgeOrb.procChainMask = procChainMask;
                        surgeOrb.procCoefficient = procCoefficient;
                        surgeOrb.damageColorIndex = damageColorIndex;
                        surgeOrb.damageCoefficientPerBounce = damageCoefficientPerBounce;
                        surgeOrb.damageReduction = damageReduction;
                        surgeOrb.range = range;
                        surgeOrb.duration = duration;
                        surgeOrb.targetSkill = targetSkill;
                        surgeOrb.checkedBonuses = checkedBonuses;
                        surgeOrb.useLightningBonus = useLightningBonus;
                        surgeOrb.useIceBonus = useIceBonus;
                        OrbManager.instance.AddOrb(surgeOrb);
                    }
                }
            }
        }

        private void ApplyElementBonuses(DamageInfo damageInfo)
        {
            if (useLightningBonus)
            {
                targetSkill.rechargeStopwatch = targetSkill.rechargeStopwatch + (StaticValues.lightningCooldownReduction * (targetSkill.finalRechargeInterval - targetSkill.rechargeStopwatch));
            }
            if (useIceBonus)
            {
                var ahc = attacker.GetComponent<HealthComponent>();
                if (ahc) ahc.AddBarrier(ahc.fullHealth * StaticValues.iceBarrierPercent);
            }
        }

        public HurtBox PickNextTarget(Vector3 position)
        {
            if (search == null) search = new BullseyeSearch();

            search.searchOrigin = position;
            search.searchDirection = Vector3.zero;
            search.teamMaskFilter = TeamMask.allButNeutral;
            search.teamMaskFilter.RemoveTeam(teamIndex);
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = range;
            search.RefreshCandidates();

            HurtBox hb = (from v in search.GetResults() where !bouncedObjects.Contains(v.healthComponent) select v).FirstOrDefault<HurtBox>();
            if (hb) bouncedObjects.Add(hb.healthComponent);
            return hb;
        }
    }
}
