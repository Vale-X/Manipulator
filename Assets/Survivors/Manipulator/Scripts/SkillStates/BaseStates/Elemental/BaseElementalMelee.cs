using System.Collections.Generic;
using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseElementalMelee : BaseSkillElement
    {
        [SerializeField]
        public GameObject wavePrefab;
        [SerializeField]
        public NetworkSoundEventDef impactSound;
        [SerializeField]
        public GameObject swingEffectPrefab;
        [SerializeField]
        public GameObject hitEffectPrefab;
        [SerializeField]
        public string swingSoundString = "";
        [SerializeField]
        public string hitSoundString = "";

        public int swingIndex;
        protected string hitboxName = "Sword";
        protected string muzzleString = "SwingCenter";
        protected Vector3 bonusForce = Vector3.zero;
        protected float damageCoefficient = StaticValues.crossDamageCoefficient;
        protected float procCoefficient = StaticValues.crossProcCoefficient;
        protected float pushForce = StaticValues.crossPushForce;
        protected float baseDuration = StaticValues.crossDuration;
        protected float attackStartTime = StaticValues.crossStartTime;
        protected float attackEndTime = StaticValues.crossEndTime;
        protected float baseEarlyExitTime = StaticValues.crossEarlyExitTime;
        protected float hitStopDuration = StaticValues.crossHitStopDuration;
        protected float attackRecoil = StaticValues.crossAttackRecoil;
        protected float hitHopVelocity = StaticValues.crossHitHopVelocity;
        protected float baseLaunchTime = StaticValues.waveLaunchTime;
        protected bool cancelled = false;
        protected NetworkSoundEventIndex impactSoundIndex;

        private float earlyExitTime;
        protected float launchTime;
        public float duration;
        public bool hasFiredAttack;
        public bool hasFiredWave;
        private float hitPauseTimer;
        private OverlapAttack attack;
        private bool hasHopped;
        public bool inHitPause;
        private float meleeStopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        #region OnEnter
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.launchTime = this.baseLaunchTime / this.attackSpeedStat;
            this.earlyExitTime = this.baseEarlyExitTime / this.attackSpeedStat;
            this.hasFiredAttack = false;
            this.animator = base.GetModelAnimator();
            base.StartAimMode(0.5f + this.duration, false);
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);
            //this.impactSoundIndex = impactSound.index;

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            this.PlayAttackAnimation();

            this.attack = new OverlapAttack();
            this.attack.damageType = this.GetDamageType();
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            //this.attack.impactSound = this.impactSoundIndex;
            //
        }

        protected virtual void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), "Slash.playbackRate", this.duration, 0.05f);
        }

        protected virtual DamageType GetDamageType()
        {
            return DamageType.Generic;
        }
        #endregion

        #region FixedUpdate
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
                base.characterMotor.velocity = this.storedVelocity;
            }
            if (!this.inHitPause)
            {
                this.meleeStopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Swing.playbackRate", 0f);
            }

            if (this.meleeStopwatch >= (this.duration * this.attackStartTime) && this.meleeStopwatch <= (this.duration * this.attackEndTime))
            {
                this.FireAttack();
            }

            if (this.meleeStopwatch >= (this.duration * this.launchTime) && this.meleeStopwatch <= (this.duration * this.attackEndTime))
            {
                this.FireProjectile();
            }

            if (this.meleeStopwatch >= (this.duration - this.earlyExitTime) && base.isAuthority)
            {
                if (base.inputBank.skill1.down)
                {
                    if (!this.hasFiredAttack) this.FireAttack();
                    if (!this.hasFiredWave) this.FireProjectile();
                    this.SetNextState();
                    return;
                }
            }
            if (this.meleeStopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        #endregion

        #region FireAttack
        protected virtual void FireAttack()
        {
            if (!this.hasFiredAttack)
            {
                this.hasFiredAttack = true;
                Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

                if (base.isAuthority)
                {
                    this.PlaySwingEffect();
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                }
            }

            if (base.isAuthority)
            {
                if (this.attack.Fire())
                {
                    this.OnHitEnemyAuthority(this.GetHitHealth());
                }
            }
        }

        protected virtual void FireProjectile()
        {

            if (!this.hasFiredWave && (this.meleeStopwatch >= this.launchTime))
            {
                this.hasFiredWave = true;
                if (base.isAuthority)
                {
                    GameObject tempPrefab = this.wavePrefab;
                    SetChildRotation rot = tempPrefab.GetComponent<SetChildRotation>();
                    rot.SetRotation("RotatedCollider", Quaternion.Euler(this.swingIndex == 0 ? new Vector3(0f, 0f, -30f) : new Vector3(0f, 0f, 30f)));

                    Ray aimRay = base.GetAimRay();
                    ProjectileManager.instance.FireProjectile(tempPrefab, //(this.swingIndex == 1 ? this.wavePrefab : this.wavePrefabAlt)
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        StaticValues.waveDamageCoefficient * this.damageStat,
                        StaticValues.waveForce,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null);
                }
            }
        }

        protected virtual void OnHitEnemyAuthority(List<HealthComponent> healthList)
        {
            Util.PlaySound(this.hitSoundString, base.gameObject);

            if (!this.hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded && this.hitHopVelocity > 0f)
                {
                    base.SmallHop(base.characterMotor, this.hitHopVelocity);
                }

                this.hasHopped = true;
            }

            if (!this.inHitPause && this.hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
                this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
                this.inHitPause = true;
            }
        }

        private List<HealthComponent> GetHitHealth(List<HurtBox> hitResults = null)
        {
            List<HealthComponent> healthList = new List<HealthComponent>();
            HitBox[] hitBoxes = this.attack.hitBoxGroup.hitBoxes;
            for (int i = 0; i < hitBoxes.Length; i++)
            {
                Transform transform = hitBoxes[i].transform;
                Vector3 position = transform.position;
                Vector3 vector = transform.lossyScale * 0.5f;
                Quaternion rotation = transform.rotation;
                Collider[] array = Physics.OverlapBox(position, vector, rotation, LayerIndex.entityPrecise.mask);
                for (int j = 0; j < array.Length; j++)
                {
                    HurtBox component = array[j].GetComponent<HurtBox>();
                    if (component && !healthList.Contains(component.healthComponent) && this.HurtBoxPassesFilter(component))
                    {
                        healthList.Add(component.healthComponent);
                    }
                    if (0 >= this.attack.maximumOverlapTargets)
                    {
                        break;
                    }
                }
            }
            return healthList;
        }

        private bool HurtBoxPassesFilter(HurtBox hurtBox)
        {
            return !hurtBox.healthComponent || ((!(hurtBox.healthComponent.gameObject == this.attack.attacker) || this.attack.attackerFiltering != AttackerFiltering.NeverHit) && (!(this.attack.attacker == null) || !(hurtBox.healthComponent.gameObject.GetComponent<MaulingRock>() != null)) && FriendlyFireManager.ShouldDirectHitProceed(hurtBox.healthComponent, this.attack.teamIndex));
        }

        protected virtual void PlaySwingEffect()
        {
            if (this.swingEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
            }
        }

        #endregion

        #region OnExit
        public override void OnExit()
        {
            if (!this.hasFiredAttack && !this.cancelled) this.FireAttack();
            if (!this.hasFiredWave && !this.cancelled) this.FireProjectile();

            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        #endregion

        protected virtual void SetNextState()
        {
            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;


            var obj = (BaseElementalMelee)Activator.CreateInstance(this.activatorSkillSlot.activationState.stateType);
            obj.swingIndex = index;
            obj.activatorSkillSlot = this.activatorSkillSlot;
            this.outer.SetNextState(obj);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.swingIndex = reader.ReadInt32();
        }
    }
}