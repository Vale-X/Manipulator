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
    class Effects
    {

        internal static GameObject ventBlinkEffect;

        public static void RegisterEffects()
        {
            CreateVentBlinkEffect();

            EffectAPI.AddEffect(ventBlinkEffect);
        }

        public static void CreateVentBlinkEffect()
        {
            ventBlinkEffect = CloneEffectPrefab("HuntressBlinkEffect","ManipulatorVentBlinkEffect");
        }

        private static GameObject CloneEffectPrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/" + prefabName), newPrefabName);
            if (!newPrefab.GetComponent<NetworkIdentity>()) newPrefab.AddComponent<NetworkIdentity>();

            return newPrefab;
        }
    }
}
