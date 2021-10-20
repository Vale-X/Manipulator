using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace ManipulatorMod.Modules.Components
{
    class ProjectileWaveImpact : MonoBehaviour, IProjectileImpactBehavior
    {
        [Header("Single Target Impact")]
        public GameObject impactEffect;
        public string enemyHitSoundString = "";
        public string hitSoundString = "";
        public bool destroyOnWorld;
        public bool destroyWhenNotAlive;

        [Space(1)]
        [Header("Overlap Attack")]
        public float overlapCoefficient = 1f;
        public float overlapProcCoefficient = 1f;
        public int overlapMaxTargets = 100;
        public float fireFrequency = 60f;
        public UnityEvent onServerHit;

        private bool alive = true;
        private ProjectileDamage projectileDamage;
        private ProjectileController projectileController;

        private float fireTimer;
        private OverlapAttack attack;

        private void Awake()
        {
            this.projectileDamage = base.GetComponent<ProjectileDamage>();
            this.projectileController = base.GetComponent<ProjectileController>();
        }

        private void Start()
        {
            this.attack = new OverlapAttack();
            this.attack.procChainMask = this.projectileController.procChainMask;
            this.attack.procCoefficient = this.projectileController.procCoefficient * this.overlapProcCoefficient;
            this.attack.attacker = this.projectileController.owner;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = this.projectileController.teamFilter.teamIndex;
            this.attack.damage = this.overlapCoefficient * this.projectileDamage.damage;
            this.attack.forceVector = this.projectileDamage.force * base.transform.forward;
            this.attack.hitEffectPrefab = this.impactEffect;
            this.attack.isCrit = this.projectileDamage.crit;
            this.attack.damageColorIndex = this.projectileDamage.damageColorIndex;
            this.attack.damageType = this.projectileDamage.damageType;
            this.attack.procChainMask = this.projectileController.procChainMask;
            this.attack.maximumOverlapTargets = this.overlapMaxTargets;
            this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!this.alive)
            {
                return;
            }

            Collider collider = impactInfo.collider;
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
                    Debug.Log(ManipulatorPlugin.MODNAME + ": No projectile damage component on: " + this + "!");
                }
                HurtBox hurtBox = collider.GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent)
                    {
                        if (!this.attack.ignoredHealthComponentList.Contains(healthComponent))
                        {
                            this.attack.ignoredHealthComponentList.Add(healthComponent);
                            if (healthComponent.gameObject == this.projectileController.owner)
                            {
                                return;
                            }
                            if (FriendlyFireManager.ShouldDirectHitProceed(healthComponent, this.projectileController.teamFilter.teamIndex))
                            {
                                Util.PlaySound(this.enemyHitSoundString, base.gameObject);
                                if (NetworkServer.active)
                                {
                                    damageInfo.ModifyDamageInfo(hurtBox.damageModifier);
                                    healthComponent.TakeDamage(damageInfo);
                                    GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtBox.healthComponent.gameObject);
                                }
                            }
                            this.HitEnemyEvent();
                        }
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
                    EffectManager.SimpleImpactEffect(this.impactEffect, impactInfo.estimatedPointOfImpact, -base.transform.forward, !this.projectileController.isPrediction);
                }
                Util.PlaySound(this.hitSoundString, base.gameObject);
            }
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                this.fireTimer -= Time.fixedDeltaTime;
                if (this.fireTimer <= 0f)
                {
                    this.fireTimer = 1f / this.fireFrequency;
                    this.attack.damage = this.overlapCoefficient * this.projectileDamage.damage;
                    if (this.attack.Fire())
                    {
                        this.HitEnemyEvent();
                        
                    }
                }
            }
            if (!this.alive)
            {
                if (this.destroyWhenNotAlive)
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
            }
        }

        public void HitEnemyEvent()
        {
            if (this.alive)
            {
                UnityEvent unityEvent = this.onServerHit;
                if (unityEvent == null)
                {
                    return;
                }
                unityEvent.Invoke();
                this.alive = false;
            }
        }
    }
}
