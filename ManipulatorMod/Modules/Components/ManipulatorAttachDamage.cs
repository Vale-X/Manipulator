using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    public class ManipulatorAttachDamage : MonoBehaviour
    {
        public void EnableAttach()
        {
            this.enableDamage = true;
            this.tickRate = attachDuration / tickMax;
            //Debug.LogWarning("Enabled AttachDamage");
        }

        void FixedUpdate()
        {
            if (enableDamage)
            {
                this.stopwatch += Time.fixedDeltaTime;
                if (this.stopwatch >= tickRate)
                {
                    //Debug.LogWarning("AttachDamage: FixedUpdate stopwatch ticked");
                    this.stopwatch = 0f;
                    this.tickCounter++;
                    this.AttachHit();
                }
            }
        }

        private void AttachHit()
        {
            //Debug.LogWarning("AttachDamage: hit");
            this.attachDamageInfo.position = attachHurtBox.transform.position;
            this.attachHurtBox.healthComponent.TakeDamage(this.attachDamageInfo);
            GlobalEventManager.instance.OnHitEnemy(this.attachDamageInfo, this.attachHurtBox.healthComponent.gameObject);
            if (!this.attachHurtBox.healthComponent.alive || !this.attachHurtBox.healthComponent)
            {
                //Debug.LogWarning("AttachDamage: enemy not alive!");
                this.AttachExit();
            }
            else if (this.tickCounter >= tickMax)
            {
                //Debug.LogWarning("AttachDamage: TickHitMax!");
                this.AttachExit();
            }
        }

        private void AttachExit()
        {
            this.projectileOwner.DestroyFromAttach();
            Destroy(this.gameObject);
        }

        private float stopwatch;
        public int tickCounter;
        private bool enableDamage;
        public GameObject attachGhost;
        public DamageInfo attachDamageInfo;
        public HurtBox attachHurtBox;
        public ManipulatorProjectileAttachOnImpact projectileOwner;
        public int tickMax = 3;
        public float attachDuration = 1.5f;
        public float tickRate;



    }
}
