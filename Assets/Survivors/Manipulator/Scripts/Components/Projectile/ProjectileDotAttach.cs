using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
    public class ProjectileDotAttach : MonoBehaviour, IProjectileImpactBehavior
    {
        [Tooltip("How long should the projectile be attached for?")]
        public float attachDuration = 3f;
        [Tooltip("Damage coeffient of tick attacks.")]
        public float attachDamageCoefficient = 1f;
        [Tooltip("How many times should damage occur over attachLifetime")]
        public int attachHitCount = 3;
        [Tooltip("Sound to play on tick hit.")]
        public string attachTickSoundString;
        public bool destroyWhenNotAlive;
        [Tooltip("whether to scale the ProjectileController's ghost by the scale of the attached Body or not.")]
        public bool useGhostScaling;

        private bool attached;
        private bool alive = true;
        private HurtBox mainHurtBox;
        private Collider hitCollider;
        private Collider bodyCollider;
        private CharacterBody characterBody;
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        internal Vector3 ringScale;
        internal float ringRadius;
        private float timer;
        private float fixedAge;
        private float nextAttackTimer;
        private HealthComponent attachedHealth;


        public void Awake()
        {
            this.projectileController = base.GetComponent<ProjectileController>();
            this.projectileDamage = base.GetComponent<ProjectileDamage>();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (this.attached || !this.alive)
            {
                return;
            }

            hitCollider = impactInfo.collider;
            if (hitCollider)
            {
                HurtBox hurtbox = hitCollider.GetComponent<HurtBox>();
                if (hurtbox)
                {
                    this.AttachToHurtBox(hurtbox);
                }
            }
        }

        public void AttachToCollider(Collider collider)
        {
            if (this.attached || !this.alive)
            {
                return;
            }

            if (collider)
            {
                HurtBox hurtbox = collider.GetComponent<HurtBox>();
                if (hurtbox)
                {
                    this.AttachToHurtBox(hurtbox);
                }
            }
        }

        private void AttachToHurtBox(HurtBox target)
        {
            if (this.attached || !this.alive)
            {
                return;
            }

            this.mainHurtBox = target.hurtBoxGroup.mainHurtBox;

            if (this.hitCollider.transform.root.GetComponent<Collider>())
            this.bodyCollider = this.hitCollider.transform.root.GetComponent<Collider>();
            //Debug.LogWarning(target.transform.root);
            this.characterBody = target.transform.root.GetComponent<CharacterBody>();
            this.attachedHealth = target.healthComponent;

            this.attached = true;
            this.UpdateScale();
        }

        private void UpdateScale()
        {
            if (!this.attached || !this.alive)
            {
                return;
            }

            if (useGhostScaling)
            {
                this.ringRadius = this.characterBody.radius;
                if (bodyCollider)
                {
                    if (bodyCollider.GetType() == typeof(CapsuleCollider))
                    {
                        CapsuleCollider capsule = (CapsuleCollider)bodyCollider;
                        Vector3 newScale = new Vector3(capsule.radius, capsule.radius, capsule.radius);
                        this.ringScale = newScale;
                    }
                    if (bodyCollider.GetType() == typeof(SphereCollider))
                    {
                        SphereCollider sphere = (SphereCollider)bodyCollider;
                        Vector3 newScale = new Vector3(sphere.radius, sphere.radius, sphere.radius);
                        this.ringScale = newScale;
                    }
                    if (bodyCollider.GetType() == typeof(BoxCollider))
                    {
                        BoxCollider box = (BoxCollider)bodyCollider;
                        this.ringScale = box.size;
                    }
                }
            }
        }

        public void Update()
        {
            if (!this.attached || !this.alive)
            {
                return;
            }

            if (this.attached)
            {
                base.transform.rotation = Quaternion.Euler(Vector3.left);
                if (this.mainHurtBox)
                {
                    base.transform.position = mainHurtBox.transform.position;
                }
            }
        }

        public void FixedUpdate()
        {
            this.fixedAge += Time.fixedDeltaTime;

            if (this.fixedAge >= 0.2f && !this.attached) this.alive = false;
            if (!this.attachedHealth.alive) this.alive = false;

            if (this.attached)
            {
                this.timer += Time.fixedDeltaTime;
                if (this.timer >= this.attachDuration)
                {
                    this.alive = false;
                }
                else
                {
                    if (this.attachHitCount > 0)
                    {
                        this.nextAttackTimer += Time.fixedDeltaTime;
                        if (this.nextAttackTimer >= this.attachDuration / (float)this.attachHitCount)
                        {
                            this.nextAttackTimer -= this.attachDuration / (float)this.attachHitCount;
                            if (mainHurtBox)
                            {
                                HealthComponent health = mainHurtBox.healthComponent;
                                DamageInfo damageInfo = new DamageInfo();
                                if (this.projectileDamage)
                                {
                                    damageInfo.damage = this.projectileDamage.damage * this.attachDamageCoefficient;
                                    damageInfo.crit = this.projectileDamage.crit;
                                    damageInfo.attacker = this.projectileController.owner;
                                    damageInfo.inflictor = base.gameObject;
                                    damageInfo.position = mainHurtBox.transform.position;
                                    damageInfo.force = new Vector3(0f, 0f, 0f);
                                    damageInfo.procChainMask = this.projectileController.procChainMask;
                                    damageInfo.procCoefficient = this.projectileController.procCoefficient;
                                    damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
                                    damageInfo.damageType = this.projectileDamage.damageType;
                                }
                                else
                                {
                                    Debug.Log("No projectile damage component!");
                                }

                                if (mainHurtBox.healthComponent.gameObject == this.projectileController.owner)
                                {
                                    return;
                                }
                                if (FriendlyFireManager.ShouldDirectHitProceed(health, this.projectileController.teamFilter.teamIndex))
                                {
                                    Util.PlaySound(this.attachTickSoundString, base.gameObject);
                                    if (NetworkServer.active)
                                    {
                                        damageInfo.ModifyDamageInfo(mainHurtBox.damageModifier);
                                        health.TakeDamage(damageInfo);
                                        GlobalEventManager.instance.OnHitEnemy(damageInfo, mainHurtBox.healthComponent.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!this.alive)
            {
                if (this.destroyWhenNotAlive)
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
            }
        }
    }
}
