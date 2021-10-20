using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
    public class ProjectileEventFireChildren : MonoBehaviour
    {
        public GameObject childProjectilePrefab;
        public float childDamageCoefficient = 1f;
        public float childProcCoefficient = 1f;
        public int spawnCount = 1;
        public float spawnCooldown = 0.1f;

        private ProjectileDamage projectileDamage;
        private ProjectileController projectileController;
        private float stopwatch;
        private int fireQueue;
        private bool spawnAvailable = true;

        private void Start()
        {
            this.projectileDamage = base.GetComponent<ProjectileDamage>();
            this.projectileController = base.GetComponent<ProjectileController>();
        }

        private void FixedUpdate()
        {
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.spawnCooldown)
            {
                this.spawnAvailable = true;
                this.stopwatch = 0f;
                if (fireQueue > 0)
                {
                    FireChildInternal(base.transform.position, false);
                    this.fireQueue--;
                }
            }
        }

        public void FireOnCooldown(int amount)
        {
            this.fireQueue += amount;
        }

        public void FireChildOnEnemy(ProjectileImpactInfo impactInfo)
        {
            HurtBox hurtBox = impactInfo.collider.GetComponent<HurtBox>();
            if (hurtBox)
            {
                this.FireChildInternal(hurtBox.gameObject.transform.position, false);
            }
        }

        public void FireChild()
        {
            FireChildInternal(base.transform.position, false);
        }

        public void FireChild(Vector3 location)
        {
            FireChildInternal(location, false);
        }
        
        public void FireChild(Vector3 location, bool over)
        {
            FireChildInternal(location, over);
        }
        public void FireChild(bool over)
        {
            FireChildInternal(base.transform.position, over);
        }

        private void FireChildInternal(Vector3 location, bool over)
        {
            if (!this.spawnAvailable && !over) { return; }
            for (int i = 0; i < this.spawnCount; i++)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childProjectilePrefab, location, Util.QuaternionSafeLookRotation(base.transform.forward));
                ProjectileController controller = gameObject.GetComponent<ProjectileController>();
                if (controller)
                {
                    controller.procChainMask = this.projectileController.procChainMask;
                    controller.procCoefficient = this.projectileController.procCoefficient * this.childProcCoefficient;
                    controller.Networkowner = this.projectileController.owner;
                }
                gameObject.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
                ProjectileDamage damage = gameObject.GetComponent<ProjectileDamage>();
                if (damage)
                {
                    damage.damage = this.projectileDamage.damage * this.childDamageCoefficient;
                    damage.crit = this.projectileDamage.crit;
                    damage.force = this.projectileDamage.force;
                    damage.damageColorIndex = this.projectileDamage.damageColorIndex;
                }
                NetworkServer.Spawn(gameObject);
            }
            this.spawnAvailable = false;
        }

        public void AddSpawnCount(int addCount)
        {
            this.spawnCount += addCount;
        }

        public void SetSpawnCount(int newCount)
        {
            this.spawnCount = newCount;
        }

        public void AddDamageCoefficient(float addAmount)
        {
            this.childDamageCoefficient += addAmount;
        }

        public void SetDamageCoefficient(float newAmount)
        {
            this.childDamageCoefficient = newAmount;
        }

        public void AddProcCoefficient(float addAmount)
        {
            this.childProcCoefficient += addAmount;
        }

        public void SetProcCoefficient(float newAmount)
        {
            this.childProcCoefficient = newAmount;
        }
    }
}
