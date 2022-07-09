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
    class ProjectileEventOverlapAttack : MonoBehaviour
    {
        public float damageCoefficient = 1f;
        public float procCoefficient = 1f;
        public Vector3 bonusForce;
        public string attackSoundString = "";
        public GameObject impactEffect;
        public bool useDamageTypeOverride;
        public DamageType damageTypeOverride = DamageType.Generic;
        public AttackerFiltering attackerFiltering = AttackerFiltering.NeverHit;

        public UnityEvent onServerHit;

        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private OverlapAttack attack;

        private void Start()
        {
            this.projectileController = base.GetComponent<ProjectileController>();
            this.projectileDamage = base.GetComponent<ProjectileDamage>();

            this.ResetOverlap();
        }

        public void UseDamageOverride(bool useOverride)
        {
            this.useDamageTypeOverride = useOverride;
        }

        public void ResetThenFireAttack()
        {
            this.ResetOverlap();
            this.FireAttack();
        }

        public void SetDamageCoefficient(float newDamage)
        {
            this.damageCoefficient = newDamage;
        }

        public void AddDamageCoefficient(float addDamage)
        {
            this.damageCoefficient += addDamage;
        }

        public void SetProcCoefficient(float newProc)
        {
            this.procCoefficient = newProc;
        }

        public void AddProcCoefficient(float addProc)
        {
            this.procCoefficient += addProc;
        }

        public void SetBonusForce(Vector3 newForce)
        {
            this.bonusForce = newForce;
        }
        public void AddBonusForce(Vector3 addForce)
        {
            this.bonusForce += addForce;
        }

        public void SetAttackString(string newString)
        {
            this.attackSoundString = newString;
        }

        public void SetImpactEffect(GameObject inEffect)
        {
            this.impactEffect = inEffect;
        }

        public void FireAttack()
        {
            if (!String.IsNullOrEmpty(this.attackSoundString))
            {
                Util.PlaySound(this.attackSoundString, base.gameObject);
            }
            if (NetworkServer.active)
            {
                if (this.attack.Fire())
                {
                    UnityEvent unityEvent = this.onServerHit;
                    if (unityEvent == null) { return; }

                    unityEvent.Invoke();
                }
            }
        }

        public void ResetOverlap()
        {
            this.attack = new OverlapAttack();
            this.attack.procChainMask = this.projectileController.procChainMask;
            this.attack.procCoefficient = this.projectileController.procCoefficient * this.procCoefficient;
            this.attack.attacker = this.projectileController.owner;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = this.projectileController.teamFilter.teamIndex;
            this.attack.attackerFiltering = this.attackerFiltering;
            this.attack.damage = this.damageCoefficient * this.projectileDamage.damage;
            this.attack.forceVector = this.bonusForce + this.projectileDamage.force * base.transform.forward;
            this.attack.hitEffectPrefab = this.impactEffect;
            this.attack.isCrit = this.projectileDamage.crit;
            this.attack.damageColorIndex = this.projectileDamage.damageColorIndex;
            this.attack.damageType = this.useDamageTypeOverride ? this.damageTypeOverride : this.projectileDamage.damageType;
            this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
        }
    }
}
