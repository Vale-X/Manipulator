using ManipulatorMod.Modules;
using ManipulatorMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using EntityStates;
using UnityEngine;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSpellIce : BaseElementalSpell
    {
        protected GameObject goodCrosshairInstance;
        protected GameObject badCrosshairInstance;
        protected GameObject areaIndicatorInstance;

        private GameObject cachedCrosshair;

        protected bool goodPlacement;

        public override void OnEnter()
        {
            base.OnEnter();

            this.cachedCrosshair = base.characterBody.crosshairPrefab;

            this.goodCrosshairInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.goodCrosshairPrefab);
            this.badCrosshairInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.badCrosshairPrefab);
            this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.areaIndicatorPrefab);
            this.areaIndicatorInstance.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

            this.UpdateAreaIndicator();
        }

        public override void Update()
        {
            base.Update();
            this.UpdateAreaIndicator();
        }

        private void UpdateAreaIndicator()
        {
            if (!hasFired)
            {
                float outNum = 0f;
                this.goodPlacement = false;
                this.areaIndicatorInstance.SetActive(true);
                if (this.areaIndicatorInstance)
                {
                    Ray aimray = base.GetAimRay();
                    RaycastHit raycast;
                    if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimray, base.gameObject, out outNum), out raycast, StaticValues.icePillarMaxRange + outNum, LayerIndex.world.mask))
                    {
                        this.areaIndicatorInstance.transform.position = raycast.point;
                        this.areaIndicatorInstance.transform.up = Vector3.up;
                        this.areaIndicatorInstance.transform.forward = -aimray.direction;
                        this.goodPlacement = (Vector3.Angle(Vector3.up, raycast.normal) < StaticValues.icePillarMaxSlope);
                    }
                    base.characterBody.crosshairPrefab = (this.goodPlacement ? this.goodCrosshairInstance : this.badCrosshairInstance);
                }
                this.areaIndicatorInstance.SetActive(this.goodPlacement);
            }
        }

        public override void FixedUpdate()
        {
            if (base.fixedAge >= this.attackStartTime && !base.inputBank.skill2.down && base.isAuthority)
            {
                this.FireIcePillar();
                this.outer.SetNextStateToMain();
            }
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            base.characterBody.crosshairPrefab = this.cachedCrosshair;
            UnityEngine.GameObject.Destroy(this.goodCrosshairInstance);
            UnityEngine.GameObject.Destroy(this.badCrosshairInstance);
            base.OnExit();
        }

        private void FireIcePillar()
        {
            if (!this.outer.destroying)
            {
                if (this.goodPlacement)
                {
                    if (this.areaIndicatorInstance && base.isAuthority && this.projectilePrefab)
                    {
                        this.hasFiredAttempted = true;
                        Vector3 forward = this.areaIndicatorInstance.transform.forward;
                        forward.y = 0f;
                        forward.Normalize();
                        Vector3 vector = Vector3.Cross(Vector3.up, forward);
                        bool crit = base.RollCrit();

                        FireProjectileInfo iceWallInfo = new FireProjectileInfo
                        {
                            projectilePrefab = this.projectilePrefab,
                            position = this.areaIndicatorInstance.transform.position + Vector3.up,
                            rotation = Util.QuaternionSafeLookRotation(vector),
                            owner = this.gameObject,
                            damage = this.damageStat * StaticValues.icePillarDamageCoefficient * this.fireDamageBonus,
                            force = 0f,
                            crit = crit,
                            damageColorIndex = DamageColorIndex.Default,
                            target = null,
                            speedOverride = -1f,
                        };
                        ProjectileManager.instance.FireProjectile(iceWallInfo);
                        this.hasFired = true;
                    }
                }
                else
                {
                    base.activatorSkillSlot.AddOneStock();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
