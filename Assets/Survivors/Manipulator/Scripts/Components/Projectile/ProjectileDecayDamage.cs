using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
    [RequireComponent(typeof(ProjectileDamage))]
    public class ProjectileDecayDamage : MonoBehaviour
    {
        [Tooltip("How long it takes to reach minimum coefficient")]
        public float timeToDecay = 1f;
        [Tooltip("Multiplier for the speed of the decay timer")]
        public float decayMultiplier = 1f;
        [Tooltip("Usually should be 1. Starting coefficient multiplier.")]
        public float maxCoefficient = 1f;
        [Tooltip("Ending coefficient multiplier.")]
        public float minCoefficient = 0.5f;

        
        private float timer;
        private ProjectileDamage projectileDamage;
        private float originalDamage;

        private void Start()
        {
            this.timer = this.timeToDecay;

            this.projectileDamage = base.GetComponent<ProjectileDamage>();
            this.originalDamage = projectileDamage.damage;
        }

        private void FixedUpdate()
        {
            this.timer -= (Time.fixedDeltaTime * this.decayMultiplier);
            if (this.timer <= 0) this.timer = 0;
            float lerp = this.timer / this.timeToDecay;

            this.projectileDamage.damage = this.originalDamage * Mathf.Lerp(this.minCoefficient, this.maxCoefficient, lerp);
        }
    }
}
