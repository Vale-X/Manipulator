using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace ManipulatorMod.Modules.Components
{
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileEventExplosion : MonoBehaviour
    {
        public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;
        public float blastRadius;
        public float blastDamageCoefficient;
        public float blastProcCoefficient = 1f;
        public AttackerFiltering blastAttackerFiltering;
        public Vector3 bonusBlastForce;
        public GameObject explosionEffect;
        public bool destoryOnDetonate;

        protected ProjectileController projectileController;
        protected ProjectileDamage projectileDamage;

        public virtual void Awake()
        {
            this.projectileController = base.GetComponent<ProjectileController>();
            this.projectileDamage = base.GetComponent<ProjectileDamage>();
        }

        public void Detonate()
        {
            if (NetworkServer.active)
            {
                this.DetonateServer();
            }
            if (this.destoryOnDetonate)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }

        public void DetonateServer()
        {
            if (this.explosionEffect)
            {
                EffectManager.SpawnEffect(this.explosionEffect, new EffectData
                {
                    origin = base.transform.position,
                    scale = this.blastRadius
                }, true);
            }
            if (this.projectileDamage)
            {
                new BlastAttack
                {
                    position = base.transform.position,
                    baseDamage = this.projectileDamage.damage * this.blastDamageCoefficient,
                    baseForce = this.projectileDamage.force * this.blastDamageCoefficient,
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
            }
        }

        public void SetExplosionRadius(float newRadius) { this.blastRadius = newRadius; }
        public void AddExplosionRadius (float addRadius) { this.blastRadius += addRadius; }

        public void SetDamageCoefficient(float newDamage) { this.blastDamageCoefficient = newDamage; }
        public void AddDamageCoefficient(float addDamage) { this.blastDamageCoefficient += addDamage; }

        public void SetProcCoefficient(float newProc) { this.blastProcCoefficient = newProc; }
        public void AddProcCoefficient(float addProc) { this.blastProcCoefficient += addProc; }

        public void SetBlastForce(Vector3 newForce) { this.bonusBlastForce = newForce; }
        public void AddBlastForce(Vector3 addForce) { this.bonusBlastForce += addForce; }

        public void SetDestroyOnDetonate(bool newDetonateDestroy) { this.destoryOnDetonate = newDetonateDestroy; }

        protected virtual void OnValidate()
        {
            if (Application.IsPlaying(this))
            {
                return;
            }
        }
    }
}
