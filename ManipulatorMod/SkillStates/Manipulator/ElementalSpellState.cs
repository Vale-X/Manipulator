using EntityStates;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ManipulatorMod.Modules.Components;


namespace ManipulatorMod.SkillStates
{
    public class ElementalSpellState : BaseSkillState
    {
        private ManipulatorController.ManipulatorElement spellElement;
        private ManipulatorController manipulatorController;
        public static float baseDuration = .5f;
        private float duration = 0.1f;

        public static float lightningDamageCoefficient = StatValues.lightningCoefficient;
        public static float damageCoefficientPerBounce = StatValues.lightningCoefficientPerBounce;
        public static float lightningProcCoefficient = StatValues.LightningProcCoefficient;
        public static float lightningBounceRange = StatValues.lightningBounceRange;
        public static int maxBounceCount = StatValues.lightningBounceCount;
        private bool hasRemovedBuff;
        private static float reductionPercent = StatValues.lightningCooldownReduction;

        private static GameObject fireSpellPrefab = Modules.Projectiles.fireSpellRingPrefab;
        private float fireSpellDamageCoefficient = StatValues.fireSpellCoefficient;
        private static float fireSpellForce = StatValues.fireSpellForce;
        private float fireDamageBonus = 1f;

        public static float iceMaxDistance = StatValues.iceMaxDistance;
        public static float maxSlopeAngle = StatValues.maxSlopeAngle;
        private float iceDuration = StatValues.iceDuration;
        private static GameObject iceSpellPrefab = Modules.Projectiles.icePillarPrefab;
        public static float damageCoefficient = StatValues.icePillarExplosionCoefficient;

        public override void OnEnter()
        {
            base.OnEnter();

            this.manipulatorController = characterBody.GetComponent<ManipulatorController>();

            //increase damage if fire buff is on
            this.checkFireBuff();

            this.spellElement = manipulatorController.currentElement;

            //run correct element spell
            switch (this.spellElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    OnEnterFireSpell();
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    OnEnterLightningSpell();
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    OnEnterIceSpell();
                    break;
            }

        }

        //multiplies damage by 1.2x if fire buff is on player, checks at onEnter
        private void checkFireBuff()
        {
            if (base.HasBuff(Modules.Buffs.fireBonusBuff))
            {
                this.fireDamageBonus = 1 + StatValues.fireBuffAmount;
                //Debug.LogWarning($"Has Fire buff! {this.fireDamageBonus}");
            }
        }

        void OnEnterFireSpell()
        {
            canFireFire = true;
        }

        //lightning is based off of Huntresses glaive ability.
        void OnEnterLightningSpell()
        {
            this.stopwatch = 0f;
            this.duration = ElementalSpellState.baseDuration;
            this.modelTransform = base.GetModelTransform();
            this.animator = base.GetModelAnimator();
            this.manipulatorTracker = base.GetComponent<ManipulatorTracker>();
            //Util.PlayScaledSound(ElementalSpell.attackSoundString, base.gameObject, this.attackSpeedStat);
            if (this.manipulatorTracker && base.isAuthority)
            {
                this.initialOrbTarget = this.manipulatorTracker.GetTrackingTarget();
                //Debug.LogWarning($"Tracking target: {this.manipulatorTracker.GetTrackingTarget()}");
            }
            //base.PlayAnimation("FullBody, Override", "ThrowGlaive", "ThrowGlaive.playbackRate", this.duration);
            if (this.modelTransform)
            {
                this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    Transform transform = this.childLocator.FindChild("RightHand");
                    if (transform && ElementalSpellState.chargePrefab)
                    {
                        this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ElementalSpellState.chargePrefab, transform.position, transform.rotation);
                        this.chargeEffect.transform.parent = transform;
                    }
                }
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
            }
        }

        //Ice makes use of Artificer's Ice wall.
        void OnEnterIceSpell()
        {
            //Debug.LogWarning("Started Ice Spell");
            this.duration = this.iceDuration / this.attackSpeedStat;
            //muzzleflashEffect = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.muzzleflashEffect);
            ElementalSpellState.goodCrosshairPrefab = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.goodCrosshairPrefab);
            ElementalSpellState.badCrosshairPrefab = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.badCrosshairPrefab);
            ElementalSpellState.areaIndicatorPrefab = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Mage.Weapon.PrepWall.areaIndicatorPrefab);
            //Debug.LogWarning("Got all Mage Prefabs");

            base.characterBody.SetAimTimer(this.duration + 2f);
            this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
            //Debug.LogWarning("Cached crosshair");
            //base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.iceDuration);
            //Util.PlaySound(ElementalSpell.prepWallSoundString, base.gameObject);
            this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ElementalSpellState.areaIndicatorPrefab);
            this.areaIndicatorInstance.transform.localScale = new Vector3(3.5f, 10f, 3.5f);
            //Debug.LogWarning("Instance set");

            //Debug.LogWarning("Initial Ice spell update");
            this.UpdateAreaIndicator();
        }

        private void UpdateAreaIndicator()
        {
            //Debug.LogWarning("Ice update started");
            if (!this.hasFiredPillar)
            {
                this.goodPlacement = false;
                this.areaIndicatorInstance.SetActive(true);
                if (this.areaIndicatorInstance)
                {
                    //Debug.LogWarning("Area indicator instance true");
                    float num = ElementalSpellState.iceMaxDistance;
                    float num2 = 0f;
                    Ray aimRay = base.GetAimRay();
                    RaycastHit raycastHit;
                    if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out num2), out raycastHit, num + num2, LayerIndex.world.mask))
                    {
                        //Debug.LogWarning("Raycast hit");
                        this.areaIndicatorInstance.transform.position = raycastHit.point;
                        this.areaIndicatorInstance.transform.up = raycastHit.normal;
                        this.areaIndicatorInstance.transform.forward = -aimRay.direction;
                        this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < ElementalSpellState.maxSlopeAngle);
                        //Debug.LogWarning($"goodPlacement: {this.goodPlacement}");
                    }
                    base.characterBody.crosshairPrefab = (this.goodPlacement ? ElementalSpellState.goodCrosshairPrefab : ElementalSpellState.badCrosshairPrefab);
                }
                this.areaIndicatorInstance.SetActive(this.goodPlacement);
                //Debug.LogWarning("areaIndicatorInstance active");
            }
        }



        // Token: 0x06003B89 RID: 15241 RVA: 0x0000D472 File Offset: 0x0000B672
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }


        public override void Update()
        {
            base.Update();
            if (this.spellElement == ManipulatorController.ManipulatorElement.Ice)
                this.UpdateAreaIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //get current element
            switch (this.spellElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    FixedUpdateFire();
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    FixedUpdateLightning();
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    FixedUpdateIce();
                    break;
            }
        }

        public void FixedUpdateFire()
        {
            this.stopwatch += Time.fixedDeltaTime;
            if (!hasFiredFire)
            {
                LaunchFireRing();
            }
            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void LaunchFireRing()
        {
            if (!hasFiredFire && canFireFire)
            {
                //fire once
                hasFiredFire = true;

                //visual effects for spawning projectile.
                //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);
                //Util.PlaySound("ManipulatorBombThrow", base.gameObject);

                if (base.isAuthority)
                {
                    //get aim direction
                    Ray aimRay = base.GetAimRay();

                    //spawn fire spell projectile
                    ProjectileManager.instance.FireProjectile(fireSpellPrefab,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        this.fireSpellDamageCoefficient * this.damageStat * this.fireDamageBonus,
                        fireSpellForce,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null);
                }
                if (!this.hasRemovedBuff)
                {
                    ElementalBonus(1);
                }
            }
        }

        public void FixedUpdateLightning()
        {
            this.stopwatch += Time.fixedDeltaTime;
            if (!this.hasTriedToFireBolt)//&& this.animator.GetFloat("ThrowGlaive.fire") > 0f
            {
                if (this.chargeEffect)
                {
                    EntityState.Destroy(this.chargeEffect);
                }
                this.FireLightningSpell();
            }
            CharacterMotor characterMotor = base.characterMotor;
            characterMotor.velocity.y = characterMotor.velocity.y + ElementalSpellState.antigravityStrength * Time.fixedDeltaTime * (1f - this.stopwatch / this.iceDuration);
            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                if (!this.hasRemovedBuff && this.hasSuccessfullyCastLightning)
                {
                    ElementalBonus(lightningOrbRef.bouncedObjects.Count);
                    //Debug.LogWarning("Buff is on!");
                    /*for (int i = 0; i < lightningOrbRef.bouncedObjects.Count; i++)
					{
						base.skillLocator.secondary.rechargeStopwatch = base.skillLocator.secondary.rechargeStopwatch + (reductionPercent * (base.skillLocator.secondary.finalRechargeInterval - base.skillLocator.secondary.rechargeStopwatch));
						//Debug.LogWarning("Lightning spell hit enemy! Secondary cooldown set to: " + base.skillLocator.secondary.rechargeStopwatch);
					}*/
                }
                this.outer.SetNextStateToMain();
                return;
            }
        }

        void FireLightningSpell()
        {
            this.hasTriedToFireBolt = true;
            ManipulatorLightningOrb lightningOrb = new ManipulatorLightningOrb() { 
            lightningType = ManipulatorLightningOrb.LightningType.Ukulele,
            damageValue = this.damageStat * ElementalSpellState.lightningDamageCoefficient * this.fireDamageBonus,
            isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
            teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
            attacker = base.gameObject,
            procCoefficient = ElementalSpellState.lightningProcCoefficient,
            bouncesRemaining = ElementalSpellState.maxBounceCount,
            speed = ElementalSpellState.lightningTravelSpeed,
            bouncedObjects = new List<HealthComponent>(),
            range = ElementalSpellState.lightningBounceRange,
            damageCoefficientPerBounce = ElementalSpellState.damageCoefficientPerBounce};
            HurtBox hurtBox = this.initialOrbTarget;
            if (hurtBox)
            {
                this.hasSuccessfullyCastLightning = true;
                Transform transform = this.childLocator.FindChild("LeftHand");
                //EffectManager.SimpleMuzzleFlash(ElementalSpellState.muzzleFlashPrefab, base.gameObject, "HandL", true);
                lightningOrb.origin = transform.position;
                lightningOrb.target = hurtBox;
                OrbManager.instance.AddOrb(lightningOrb);
            }
            lightningOrbRef = lightningOrb;
        }

        public void FixedUpdateIce()
        {
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.duration && !base.inputBank.skill2.down && base.isAuthority)
            {
                if (!this.hasFiredPillar)
                {
                    this.fireIcePillar();
                    this.hasFiredPillar = true;
                }
                float extendedDuration = this.duration + 0.05f;

                if (this.stopwatch >= extendedDuration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        private void fireIcePillar()
        {
            if (!this.outer.destroying)
            {
                if (this.goodPlacement)
                {
                    if (this.areaIndicatorInstance && base.isAuthority)
                    {
                        //fire ice pillar
                        Vector3 forward = this.areaIndicatorInstance.transform.forward;
                        forward.y = 0f;
                        forward.Normalize();
                        Vector3 vector = Vector3.Cross(Vector3.up, forward);
                        bool crit = Util.CheckRoll(this.critStat, base.characterBody.master);
                        //Debug.LogWarning($"Pillar this.gameObject: {this.gameObject}");
                        //ProjectileManager.instance.FireProjectile(iceSpellPrefab, this.areaIndicatorInstance.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(Vector3.up), this.gameObject, this.damageStat * ElementalSpell.damageCoefficient * this.fireDamageBonus, 0f, crit, DamageColorIndex.Default, null, -1f);

                        FireProjectileInfo iceWallInfo = new FireProjectileInfo
                        {
                            projectilePrefab = iceSpellPrefab,
                            position = this.areaIndicatorInstance.transform.position + Vector3.up,
                            rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                            owner = this.gameObject,
                            damage = this.damageStat * ElementalSpellState.damageCoefficient * this.fireDamageBonus,
                            force = 0f,
                            crit = crit,
                            damageColorIndex = DamageColorIndex.Default,
                            target = null,
                            speedOverride = -1f
                        };
                        ProjectileManager.instance.FireProjectile(iceWallInfo);
                    }
                }
                else
                {
                    //refund if not successful
                    base.skillLocator.secondary.AddOneStock();
                }
            }
            //reset to normal
            EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
        }

        public override void OnExit()
        {
            switch (this.spellElement)
            {
                case ManipulatorController.ManipulatorElement.Fire:
                    OnExitFire();
                    break;
                case ManipulatorController.ManipulatorElement.Lightning:
                    OnExitLightning();
                    break;
                case ManipulatorController.ManipulatorElement.Ice:
                    OnExitIce();
                    break;
            }
            this.hasRemovedBuff = false;
            base.OnExit();
        }

        public void OnExitFire()
        {
            canFireFire = false;
            hasFiredFire = false;
        }

        public void OnExitLightning()
        {
            if (this.chargeEffect)
            {
                EntityState.Destroy(this.chargeEffect);
            }
            int layerIndex = this.animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                this.animator.SetLayerWeight(layerIndex, 1.5f);
                //this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
            }
            if (!this.hasTriedToFireBolt)
            {
                this.FireLightningSpell();
            }
            if (!this.hasSuccessfullyCastLightning && NetworkServer.active)
            {
                base.skillLocator.secondary.AddOneStock();
            }
        }

        public void OnExitIce()
        {
           
        }


        //triggers any buff effects and removes the buffs given from special (ElementSwitch)
        public void ElementalBonus(int enemiesHit)
        {
            //Debug.LogWarning($"BonusTriggered! {enemiesHit}");
            if (base.HasBuff(Modules.Buffs.fireBonusBuff))
            {
                this.fireDamageBonus = 1f;
                base.characterBody.RemoveBuff(Modules.Buffs.fireBonusBuff);
            }

            if (base.HasBuff(Modules.Buffs.lightningBonusBuff))
            {
                for (int i = 0; i < enemiesHit; i++)
                {
                    base.skillLocator.secondary.rechargeStopwatch = base.skillLocator.secondary.rechargeStopwatch + (reductionPercent * (base.skillLocator.secondary.finalRechargeInterval - base.skillLocator.secondary.rechargeStopwatch));
                }
                base.characterBody.RemoveBuff(Modules.Buffs.lightningBonusBuff);
            }

            if (base.HasBuff(Modules.Buffs.iceBonusBuff))
            {
                for (int i = 0; i < enemiesHit; i++)
                {

                    base.healthComponent.AddBarrier(base.healthComponent.fullHealth * StatValues.iceBarrierPercent);
                }
                base.characterBody.RemoveBuff(Modules.Buffs.iceBonusBuff);
            }
            this.hasRemovedBuff = true;
        }

        //from ProjectileGrappleController, for linking this script with the ice spell pillar projectile
        /*public void SetPillarReference(GameObject pillar)
        {
            Debug.LogWarning("Am I being used");
            this.pillarInstance = pillar;
            this.pillarExplode = pillar.GetComponent<ProjectileIcePillarExplode>();
            this.hadPillarInstance = true;
        }*/

        //lightning
        public static GameObject chargePrefab;
        public static GameObject muzzleFlashPrefab;
        public static float smallHopStrength;
        public static float antigravityStrength;
        public static float lightningTravelSpeed;
        public static string attackSoundString;
        private float stopwatch;
        private Animator animator;
        private GameObject chargeEffect;
        private Transform modelTransform;
        private ManipulatorTracker manipulatorTracker;
        private ChildLocator childLocator;
        private bool hasTriedToFireBolt;
        private bool hasSuccessfullyCastLightning;
        private HurtBox initialOrbTarget;
        private ManipulatorLightningOrb lightningOrbRef;

        //fire
        private static bool hasFiredFire;
        private static bool canFireFire;

        //ice
        public static GameObject areaIndicatorPrefab;
        public static GameObject projectilePrefab;
        public static GameObject muzzleflashEffect;
        public static GameObject goodCrosshairPrefab;
        public static GameObject badCrosshairPrefab;
        public static string prepWallSoundString;
        public static string fireSoundString;
        private bool goodPlacement;
        private GameObject areaIndicatorInstance;
        private GameObject cachedCrosshairPrefab;
        public GameObject pillarInstance;
        protected ProjectileIcePillarExplode pillarExplode;
        private bool hadPillarInstance;
        private bool hasFiredPillar;
    }
}
