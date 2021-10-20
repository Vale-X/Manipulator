using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
    class ProjectileManipulatorReportHitcount : MonoBehaviour, IOnDamageInflictedServerReceiver
    {
        public float reportDelay;
        public TargetSkill targetSkill;

        public UnityEvent onReport;

        public enum TargetSkill
        {
            Primary,
            Secondary,
            Utility,
            Special
        }
        
        private int hitCount;
        private float stopwatch;
        private ProjectileController projectileController;
        private ManipulatorController manipulatorController;

        public void Awake()
        {
            this.projectileController = base.GetComponent<ProjectileController>();
            Debug.LogWarning(this.projectileController);
        }

        public void Start()
        {
            if (this.projectileController)
            {
                Debug.LogWarning(this.projectileController.owner);
                this.manipulatorController = this.projectileController.owner.GetComponent<ManipulatorController>();
            }
        }

        public void OnDamageInflictedServer(DamageReport damageReport)
        {
            this.hitCount++;
        }

        public void FixedUpdate()
        {
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= reportDelay)
            {
                if (this.manipulatorController)
                {
                    this.manipulatorController.ElementalBonus(hitCount, (int)targetSkill);

                    UnityEvent unityEvent = this.onReport;
                    if (unityEvent == null)
                    {
                        return;
                    }
                    unityEvent.Invoke();
                }
            }
        }
    }
}
