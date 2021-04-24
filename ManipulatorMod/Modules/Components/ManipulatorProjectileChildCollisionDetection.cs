using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace ManipulatorMod.Modules.Components
{
    public class ManipulatorProjectileChildCollisionDetection : MonoBehaviour
    {
        BoxCollider outerCollider;
        //Rigidbody outerBody;
        public void AddBoxCollider(Vector3 boxSize, GameObject owner)
        {
            /*
            outerBody = owner.AddComponent<Rigidbody>();
            outerBody.mass = 1f;
            outerBody.isKinematic = true;
            outerBody.drag = 0f;
            outerBody.angularDrag = 0.05f;
            outerBody.interpolation = RigidbodyInterpolation.Interpolate;
            outerBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            outerBody.useGravity = false;
            outerBody.WakeUp();*/
            outerCollider = owner.AddComponent<BoxCollider>();
            outerCollider.size = boxSize;
            outerCollider.isTrigger = true;
            //Debug.LogWarning("AddedCollider");
            /*outerCollider.transform.position = new Vector3(0f, 0f, 0f);
            outerCollider.transform.localScale = new Vector3(1f, 1f, 1f);
            owner.transform.localScale = new Vector3(5f, 0.5f, 1.5f);*/
        }

        void OnTriggerEnter(Collider other)
        {
            ManipulatorProjectileWaveImpact parentScript = this.transform.parent.gameObject.transform.parent.GetComponent<ManipulatorProjectileWaveImpact>();
            parentScript.OnChildImpact(other);
            //Debug.LogWarning("OuterHit");
        }
        
        
        /*void OnCollisionEnter(Collision other)
        {
            Debug.LogWarning(other.collider.GetComponent<TeamComponent>()._teamIndex);
            TeamIndex enemyTeam = other.collider.GetComponent<TeamComponent>()._teamIndex;
            Debug.LogWarning(enemyTeam);
            Debug.LogWarning("Test");
            Chat.AddMessage("Collided!");

            if(enemyTeam == TeamIndex.Monster || enemyTeam == TeamIndex.Neutral)
            {
                ProjectileWaveImpact parentScript = this.transform.parent.gameObject.GetComponent<ProjectileWaveImpact>();
                parentScript.OnChildImpact(other.collider, transform.position);
            }
            else
            {
                Physics.IgnoreCollision(other.collider, GetComponent<Collider>());
                Chat.AddMessage("Ignored");
            }
        }*/
        
    }
}
