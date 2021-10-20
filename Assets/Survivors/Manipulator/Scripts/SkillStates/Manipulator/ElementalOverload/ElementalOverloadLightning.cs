using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace ManipulatorMod.SkillStates
{
    public class ElementalOverloadLightning : BaseElementalOverload
    {
        [SerializeField]
        public GameObject lightningBurst;

        public override void ElementBurst()
        {
            base.ElementBurst();

            BlastAttack blastAttack = MakeBlastAttack();
            FireLightningBurst(blastAttack.crit, blastAttack.baseDamage);
            blastAttack.Fire();
        }

        public void FireLightningBurst(bool crit, float blastDamage)
        {
            Ray aimRay = base.GetAimRay();
            ProjectileManager.instance.FireProjectile(this.lightningBurst, transform.position, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, blastDamage * 0.5f, 0, crit, DamageColorIndex.Default, null, -1f);
        }
    }
}