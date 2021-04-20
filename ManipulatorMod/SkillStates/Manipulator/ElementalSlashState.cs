using ManipulatorMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using ManipulatorMod.Modules.Survivors;
using UnityEngine;

namespace ManipulatorMod.SkillStates
{
    public class ElementalSlashState : BaseMeleeAttack
    {
        //create element var, set to fire by default.
        public static DamageType MeleeElement = DamageType.IgniteOnHit;

        //set other vars
        private float fireTime = StatValues.waveFireTime;
        private bool hasFired;
        private bool waveRequireStock = false;
        public static float waveDamageCoefficient = StatValues.waveDamageCoefficient;
        public static float waveForce = StatValues.waveForce;
        public static bool canFire = true;
        public GameObject wavePrefab = Modules.Projectiles.waveFirePrefab;
        //public static GameObject wavePrefabAlt = Modules.Projectiles.waveFirePrefabAlt;
        private float attackDuration = StatValues.attackDuration;
        private ManipulatorController manipulatorController;

        public override void OnEnter()
        {
            manipulatorController = characterBody.GetComponent<ManipulatorController>();

            //element check, set different wave and damage type based on current element. Can this be made better?
            switch (manipulatorController.currentElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    MeleeElement = DamageType.IgniteOnHit;
                    this.wavePrefab = Modules.Projectiles.waveFirePrefab;
                    //wavePrefabAlt = Modules.Projectiles.waveFirePrefabAlt;
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    MeleeElement = DamageType.Generic;
                    this.wavePrefab = Modules.Projectiles.waveLightningPrefab;
                    //wavePrefabAlt = Modules.Projectiles.waveLightningPrefabAlt;
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    MeleeElement = DamageType.SlowOnHit;
                    this.wavePrefab = Modules.Projectiles.waveIcePrefab;
                    //wavePrefabAlt = Modules.Projectiles.waveIcePrefabAlt;
                    break;
            }


            //setup melee attack details.
            this.hitboxName = "Sword";
            this.damageType = MeleeElement;
            this.damageCoefficient = StatValues.attackDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = StatValues.attackPushForce;
            this.bonusForce = Vector3.zero;
            this.baseDuration = this.attackDuration;
            this.attackStartTime = StatValues.attackStartTime;
            this.attackEndTime = StatValues.attackEndTime;
            this.baseEarlyExitTime = StatValues.attackEndTime;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;
            this.swingSoundString = "ManipulatorSwordSwing";
            this.hitSoundString = "";
            this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = Modules.Assets.swordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.swordHitImpactEffect;
            this.impactSound = Modules.Assets.swordHitSoundEvent.index;

            //ChangeRotation();

            base.OnEnter();
        }

        private void ChangeRotation()
        {
            //match rotation of wave to angle of swing
            switch (this.swingIndex)
            {
                case 0:
                    GameObject child = wavePrefab.transform.GetChild(0).gameObject;
                    var rotation = Quaternion.Euler(StatValues.waveRotation1);
                    child.transform.localRotation = rotation;
                    var controller = child.GetComponent<ProjectileController>();
                    controller.transform.localRotation = rotation;
                    Debug.LogWarning("case 0");
                    break;
                case 1:
                    var rotation2 = Quaternion.Euler(StatValues.waveRotation2);
                    this.wavePrefab.transform.localRotation = rotation2;
                    var controller2 = this.wavePrefab.GetComponent<ProjectileController>();
                    controller2.transform.localRotation = rotation2;
                    Debug.LogWarning("case 1");
                    break;
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAttackAnimation();
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        private void SlashWave()
        {
            if (!this.hasFired && canFire)
            {
                //fire once
                this.hasFired = true;
                
                //visual effects for spawning projectile.
                //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);
                //Util.PlaySound("ManipulatorBombThrow", base.gameObject);

                if (base.isAuthority)
                {
                    //get aim direction
                    Ray aimRay = base.GetAimRay();
                    //aimRay.direction += StatValues.waveRotation1;

                    //spawn wave projectile
                    ProjectileManager.instance.FireProjectile(this.wavePrefab,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        waveDamageCoefficient * this.damageStat,
                        waveForce,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null);
                }
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void SetNextState()
        {
            //combo for second slash type
            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            this.outer.SetNextState(new ElementalSlashState
            {
                swingIndex = index
            });
        }

        public override void FixedUpdate()
        {
           //fire wave
            base.FixedUpdate();
            if (base.fixedAge >= (this.fireTime / this.attackSpeedStat))
            {
                if (!this.waveRequireStock)
                    this.SlashWave();
                else if (!this.hasFired && base.skillLocator.primary.stock > 0)
                {
                    this.SlashWave();
                    base.skillLocator.primary.stock -= 1;
                }
            }

            if (ManipulatorMain.attackReset)
            {
                this.baseDuration = 0.025f;
                this.baseEarlyExitTime = 0.2f;
                this.attackEndTime = 0.2f;
                this.fireTime = 0.1f;
                ManipulatorMain.attackReset = false;
            }
            else
            {
                this.baseDuration = this.attackDuration;
                this.baseEarlyExitTime = StatValues.attackEndTime;
                this.attackEndTime = StatValues.attackEndTime;
                this.fireTime = 0.35f;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}