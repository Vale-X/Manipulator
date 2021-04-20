using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using ManipulatorMod.Modules.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace ManipulatorMod.Modules
{
    internal static class Orbs
    {
        internal static GameObject chainLightningOrb;

        public static void RegisterOrbs()
        {
           //chain lightning orb
            CreateLightningOrbEffect();
            if (chainLightningOrb) PrefabAPI.RegisterNetworkPrefab(chainLightningOrb);
            EffectAPI.AddEffect(chainLightningOrb);
            OrbAPI.AddOrb(typeof(ChainLightningOrb));
        }
        public static void CreateLightningOrbEffect()
        {
            chainLightningOrb = CloneOrbEffectPrefab("LightningOrbEffect", "chainLightningOrbEffect");

            //UnityEngine.GameObject.Destroy(chainLightningOrb.GetComponent<OrbEffect>());
            //ManipulatorOrbEffectNoBezier chainOrbEffect = chainLightningOrb.AddComponent<ManipulatorOrbEffectNoBezier>();
            OrbEffect chainOrbEffect = chainLightningOrb.GetComponent<OrbEffect>();
            chainOrbEffect.startVelocity1 = new Vector3(0f, 0f, 0f);
            chainOrbEffect.startVelocity2 = new Vector3(0f, 0f, 0f);
            chainOrbEffect.endVelocity1 = new Vector3(0f, 0f, 0f);
            chainOrbEffect.endVelocity2 = new Vector3(0f, 0f, 0f);
            chainOrbEffect.faceMovement = true;
            chainOrbEffect.callArrivalIfTargetIsGone = false;
            chainOrbEffect.endEffect = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXLightning");
            chainOrbEffect.endEffectScale = 2f;
        }
        private static GameObject CloneOrbEffectPrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/" + prefabName), newPrefabName);

            // only add NetworkIdentity if there isn't one already.
            if (newPrefab.GetComponent<NetworkIdentity>() != null)
            {
            }
            else
            {
                newPrefab.AddComponent<NetworkIdentity>();
            }

            //Modules.Assets.ConvertAllRenderersToHopooShader(newPrefab);

            return newPrefab;
        }
    }
}
