using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.Modules.Components
{
    public class GhostRingScaleController : MonoBehaviour
    {
        public ParticleSystem ringParticle;
        public Light ringLight;

        private ProjectileDotAttach dotAttach;
        internal GameObject refParent;

        public void Start()
        {
            this.dotAttach = this.refParent.GetComponent<ProjectileDotAttach>();
            UpdateScale(this.dotAttach.ringScale, this.dotAttach.ringRadius);
        }

        public void FixedUpdate()
        {
            if (this.dotAttach)
            {
                UpdateScale(this.dotAttach.ringScale, this.dotAttach.ringRadius);
            }
        }

        public void UpdateScale(Vector3 inScale, float inRadius)
        {
            var sh = this.ringParticle.shape;
            var em = this.ringParticle.emission;

            float originalRadius = sh.radius;
            var rateOverTime = em.rateOverTime;
            var originalRate = em.rateOverTime.constant;

            sh.radius = originalRadius * inScale.x;
            rateOverTime.constant = originalRate * inScale.x;        
        }
    }
}
