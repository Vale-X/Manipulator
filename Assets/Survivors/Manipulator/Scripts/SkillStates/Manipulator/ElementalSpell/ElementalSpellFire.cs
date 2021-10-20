using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.SkillStates.BaseStates;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSpellFire : BaseElementalSpell
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.fixedAge >= this.attackStartTime && !this.hasFired)
            {
                this.hasFiredAttempted = true;
                this.LaunchFireRing();
            }
            if (this.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void LaunchFireRing()
        {
            if (base.isAuthority && this.projectilePrefab)
            {
                Ray aimRay = base.GetAimRay();

                FireProjectileInfo fireRingInfo = new FireProjectileInfo
                {
                    projectilePrefab = this.projectilePrefab,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    owner = base.gameObject,
                    damage = this.damageStat * StaticValues.fireSpellDamageCoefficient * this.fireDamageBonus,
                    force = StaticValues.fireSpellForce,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default
                };
                ProjectileManager.instance.FireProjectile(fireRingInfo);
                this.hasFired = true;
                if (!this.hasRemovedBuff) ElementalBonus(1);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
