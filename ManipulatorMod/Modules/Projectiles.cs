using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.Modules
{
    internal static class Projectiles
    {
        //internal static GameObject prefabNameHERE;

        //wave
        internal static GameObject waveFirePrefab;
        internal static GameObject waveFirePrefabAlt;
        internal static GameObject waveLightningPrefab;
        internal static GameObject waveLightningPrefabAlt;
        internal static GameObject waveIcePrefab;
        internal static GameObject waveIcePrefabAlt;

        static Vector3 waveCoreSize = StatValues.waveCoreSize;
        static Vector3 waveSize = StatValues.waveOuterSize;

        //blink explosion
        internal static GameObject explosionFirePrefab;
        internal static GameObject explosionLightningPrefab;
        internal static GameObject explosionIcePrefab;

        internal static float explosionDelay = StatValues.explosionDelay;
        internal static float explosionBlastRadius = StatValues.explosionRadius;
        internal static float explosionDamage = StatValues.explosionDamage;

        //FireSpell
        internal static GameObject fireSpellRingPrefab;

        internal static float fireSpellRingVelocity = StatValues.fireSpellSpeed;
        internal static int fireSpellTickMax = StatValues.fireAttachTickMax;
        internal static float fireSpellAttachDuration = StatValues.fireAttachDuration;
        internal static float fireSpellAttachCoefficient = StatValues.fireAttachCoefficient;

        //IcePillar
        internal static GameObject icePillarPrefab;

        internal static Vector3 icePillarScale = StatValues.icePillarScale;
        internal static float icePillarExplosionCoefficient = StatValues.icePillarExplosionCoefficient;

        internal static void RegisterProjectiles()
        {
            CreateWavesFire();
            CreateWavesLightning();
            CreateWavesIce();
            CreateFireExplosion();
            CreateLightningExplosion();
            CreateIceExplosion();
            CreateFireSpellRing();
            CreateIcePillar();

            Modules.Prefabs.projectilePrefabs.Add(waveFirePrefab);
            //Modules.Prefabs.projectilePrefabs.Add(waveFirePrefabAlt);
            Modules.Prefabs.projectilePrefabs.Add(waveIcePrefab);
            //Modules.Prefabs.projectilePrefabs.Add(waveIcePrefabAlt);
            Modules.Prefabs.projectilePrefabs.Add(waveLightningPrefab);
            //Modules.Prefabs.projectilePrefabs.Add(waveLightningPrefabAlt);
            Modules.Prefabs.projectilePrefabs.Add(explosionFirePrefab);
            Modules.Prefabs.projectilePrefabs.Add(explosionLightningPrefab);
            Modules.Prefabs.projectilePrefabs.Add(explosionIcePrefab);
            Modules.Prefabs.projectilePrefabs.Add(fireSpellRingPrefab);
            Modules.Prefabs.projectilePrefabs.Add(icePillarPrefab);
            //Modules.Prefabs.projectilePrefabs.Add(prefabNameHERE);
        }

        private static GameObject CreateNewWave(string waveName, DamageType damageType, string ghostPrefabName, Vector3 waveCoreSize, Vector3 waveOuterSize, Vector3 rotation)
        {
            //clone fire projectile, fix size (original size is 0.05, 0.05, 1)
            GameObject newWavePrefab = CloneProjectilePrefab("Fireball", waveName);
            newWavePrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            newWavePrefab.transform.localRotation = Quaternion.Euler(StatValues.waveRotation1);

            //set ghost, damage and owner
            ProjectileController newWaveController = newWavePrefab.GetComponent<ProjectileController>();
            newWaveController.ghostPrefab = CreateGhostPrefab(ghostPrefabName);
            newWaveController.GetComponent<ProjectileDamage>().damageType = damageType;
            newWaveController.owner = Survivors.Manipulator.characterPrefab;
            newWaveController.procCoefficient = StatValues.waveProcCoefficient;

            TeamFilter newWaveTeam = newWavePrefab.GetComponent<TeamFilter>();
            newWaveTeam.defaultTeam = TeamIndex.Player;

            //replace single target impact with custom impact script
            UnityEngine.Object.Destroy(newWavePrefab.GetComponent<ProjectileSingleTargetImpact>());
            ManipulatorProjectileWaveImpact newWaveImpact = newWavePrefab.AddComponent<ManipulatorProjectileWaveImpact>();
            newWaveImpact.destroyOnWorld = true;
            newWaveImpact.destroyWhenNotAlive = true;

            ProjectileSimple newWaveSimple = newWavePrefab.GetComponent<ProjectileSimple>();
            newWaveSimple.lifetime = StatValues.waveLifetime;
            //newWaveSimple.velocity = waveVelocity;
            newWaveSimple.desiredForwardSpeed = StatValues.waveSpeed;

            //replace sphere with core box (smaller middle, collides with both terrain and enemies)
            UnityEngine.Object.Destroy(newWavePrefab.GetComponent<SphereCollider>());
            BoxCollider newWaveBox = newWavePrefab.AddComponent<BoxCollider>();
            newWaveBox.size = waveCoreSize;

            //add outer box collider (collides with enemies only)
            GameObject childObject = new GameObject("OuterBox");
            childObject.layer = 14;
            childObject.transform.parent = newWavePrefab.gameObject.transform;
            ManipulatorProjectileChildCollisionDetection childScript = childObject.AddComponent<ManipulatorProjectileChildCollisionDetection>();
            childScript.AddBoxCollider(waveOuterSize, childObject);

            //Quaternion quaternion = Quaternion.Euler(rotation);
            //newWavePrefab.transform.rotation = quaternion;
            //newWaveController.ghostPrefab.transform.rotation = quaternion;
            //newWaveController.ghostTransformAnchor.rotation = quaternion;
            //newWaveController.ghostTransformAnchor = newWavePrefab.transform;

            //create 'container' to enable local rotation of the wave
            //GameObject transformBox = new GameObject();
            //newWavePrefab.transform.parent = transformBox.transform;

            //return result
            return newWavePrefab;
        }

        private static void CreateWavesFire()
        {
            waveFirePrefab = CreateNewWave("ManipulatorWaveFireProjectile", DamageType.IgniteOnHit, "ManipulatorGhostFireWave", waveCoreSize, waveSize, StatValues.waveRotation1);
            //waveFirePrefabAlt = CreateNewWave("ManipulatorWaveFireProjectileAlt", DamageType.IgniteOnHit, "ManipulatorGhostFireWave", waveCoreSize, waveSize, StatValues.waveRotation2);
        }

        private static void CreateWavesLightning()
        {
            waveLightningPrefab = CreateNewWave("ManipulatorWaveLightningProjectile", DamageType.Generic, "ManipulatorGhostLightningWave", waveCoreSize, waveSize, StatValues.waveRotation1);
            AddLightningWave(waveLightningPrefab);
            //waveLightningPrefabAlt = CreateNewWave("ManipulatorWaveLightningProjectileAlt", DamageType.Generic, "ManipulatorGhostLightningWave", waveCoreSize, waveSize, StatValues.waveRotation2);
            //AddLightningWave(waveLightningPrefabAlt);
        }

        private static void AddLightningWave(GameObject wavePrefab)
        {
            ManipulatorProjectileWaveImpact lightningWaveChild = wavePrefab.GetComponentInChildren  <ManipulatorProjectileWaveImpact>();
            lightningWaveChild.childrenProjectilePrefab = CreateLightningWaveExplosion(StatValues.lightningBlastRadius, StatValues.lightningBlastLifetime);
            lightningWaveChild.fireChildren = true;
            lightningWaveChild.childrenCount = 1;
            lightningWaveChild.impactEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/OmniExplosionVFXQuick");
            lightningWaveChild.childrenDamageCoefficient = 0.5f;
        }

        private static GameObject CreateLightningWaveExplosion(float blastRadius, float blastDelay)
        {
            GameObject lightningExplosionPrefab = CloneProjectilePrefab("StickyBomb", "ManipulatorLightningExplosion");

            ProjectileImpactExplosion lightningExplosionImpact = lightningExplosionPrefab.GetComponent<ProjectileImpactExplosion>();
            ProjectileController lightningExplosionController = lightningExplosionPrefab.GetComponent<ProjectileController>();
            lightningExplosionController.ghostPrefab = CreateGhostPrefab("ManipulatorInvisibleSticky");
            lightningExplosionImpact.impactEffect = Resources.Load<GameObject>("Prefabs/Effects/LightningStakeNova");
            lightningExplosionImpact.impactEffect.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            lightningExplosionImpact.lifetime = blastDelay;
            lightningExplosionImpact.lifetimeAfterImpact = StatValues.lightningBlastAfterLifetime;
            lightningExplosionImpact.blastRadius = blastRadius;
            lightningExplosionImpact.blastDamageCoefficient = StatValues.lightningBlastCoefficient;
            lightningExplosionImpact.blastProcCoefficient = 0f;
            lightningExplosionImpact.falloffModel = BlastAttack.FalloffModel.None;

            UnityEngine.Object.Destroy(lightningExplosionPrefab.GetComponent<LoopSound>());
            return lightningExplosionPrefab;
        }

        private static void CreateWavesIce()
        {
            waveIcePrefab = CreateNewWave("ManipulatorWaveIceProjectile", DamageType.SlowOnHit, "ManipulatorGhostIceWave", waveCoreSize, waveSize, StatValues.waveRotation1);
            //waveIcePrefabAlt = CreateNewWave("ManipulatorWaveIceProjectile", DamageType.SlowOnHit, "ManipulatorGhostIceWave", waveCoreSize, waveSize, StatValues.waveRotation2);
        }

        private static GameObject CreateBlinkExplosion(string newExplosionName, float newExplosionDelay, float newBlastRadius, float newBlastDamage, DamageType damageType)
        {
            GameObject newBlinkPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", newExplosionName);
            UnityEngine.GameObject.Destroy(newBlinkPrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileBlinkExplosion newBlinkExplosion = newBlinkPrefab.AddComponent<ProjectileBlinkExplosion>();
            ProjectileController newBlinkController = newBlinkPrefab.GetComponent<ProjectileController>();
            Rigidbody newBlinkBody = newBlinkPrefab.GetComponent<Rigidbody>();
            ProjectileSimple newBlinkProjectile = newBlinkPrefab.GetComponent<ProjectileSimple>();
            newBlinkPrefab.GetComponent<ProjectileDamage>().damageType = damageType;
            newBlinkBody.useGravity = false;
            newBlinkProjectile.desiredForwardSpeed = 0f;
            newBlinkExplosion.lifetime = newExplosionDelay;
            newBlinkExplosion.timerAfterImpact = false;
            newBlinkExplosion.blastRadius = newBlastRadius;
            newBlinkExplosion.blastDamageCoefficient = newBlastDamage;

            return newBlinkPrefab;

        }

        private static void CreateFireExplosion()
        {
            explosionFirePrefab = CreateBlinkExplosion("ManipulatorFireBlink", explosionDelay, explosionBlastRadius, explosionDamage, DamageType.IgniteOnHit);
        }

        private static void CreateLightningExplosion()
        {
            explosionLightningPrefab = CreateBlinkExplosion("ManipulatorLightningBlink", explosionDelay, explosionBlastRadius, explosionDamage, DamageType.Generic);

            ProjectileBlinkExplosion lightningWaveChild = explosionLightningPrefab.GetComponent<ProjectileBlinkExplosion>();
            lightningWaveChild.childrenProjectilePrefab = CreateLightningWaveExplosion(StatValues.blinkLightningRadius, StatValues.blinkLightningDelay);
            lightningWaveChild.fireChildren = true;
            lightningWaveChild.childrenCount = 1;
            lightningWaveChild.impactEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/OmniExplosionVFXQuick");
            lightningWaveChild.childrenDamageCoefficient = 0.5f;
        }

        private static void CreateIceExplosion()
        {
            explosionIcePrefab = CreateBlinkExplosion("ManipulatorIceBlink", explosionDelay, explosionBlastRadius, explosionDamage, DamageType.SlowOnHit);
        }

        private static void CreateFireSpellRing()
        {
            fireSpellRingPrefab = CloneProjectilePrefab("Fireball", "ManipulatorFireRing");

            UnityEngine.GameObject.Destroy(fireSpellRingPrefab.GetComponent<ProjectileSingleTargetImpact>());
            ManipulatorProjectileAttachOnImpact manipulatorAttach = fireSpellRingPrefab.AddComponent<ManipulatorProjectileAttachOnImpact>();
            ProjectileDamage manipulatorDamage = fireSpellRingPrefab.GetComponent<ProjectileDamage>();
            manipulatorAttach.destroyOnWorld = true;
            manipulatorAttach.destroyWhenNotAlive = false;
            manipulatorAttach.attachTickMax = fireSpellTickMax;
            manipulatorAttach.attachDuration = fireSpellAttachDuration;
            manipulatorAttach.attachDamageCoefficient = fireSpellAttachCoefficient;
            manipulatorAttach.attachPrefab = CreateAttachRing();
            manipulatorAttach.ownerController = fireSpellRingPrefab.GetComponent<ProjectileController>();
            manipulatorAttach.attachPrefab.transform.SetParent(fireSpellRingPrefab.transform);
            //Debug.LogWarning("Manipulator Attach Prefab: " + manipulatorAttach.attachPrefab);

            ProjectileController ringController = fireSpellRingPrefab.GetComponent<ProjectileController>();
            ringController.procCoefficient = StatValues.fireSpellCoefficient;

            ProjectileSimple fireRingSimple = fireSpellRingPrefab.GetComponent<ProjectileSimple>();
            //fireRingSimple.velocity = fireSpellRingVelocity;
            fireRingSimple.desiredForwardSpeed = fireSpellRingVelocity;
            fireRingSimple.lifetime = 3f;
        }

        private static GameObject CreateAttachRing()
        {
            GameObject newRingPrefab = new GameObject("ManipulatorAttachRingPrefab", typeof(ManipulatorAttachDamage));
            ManipulatorAttachDamage newRingAttach = newRingPrefab.GetComponent<ManipulatorAttachDamage>();
            newRingAttach.attachGhost = CreateGhostPrefab("ManipulatorGhostFireRing");
            return newRingPrefab;
        }

        private static void CreateIcePillar()
        {
            icePillarPrefab = CloneProjectilePrefab("MageIcewallPillarProjectile", "ManipulatorIcePillar");

            UnityEngine.GameObject.Destroy(icePillarPrefab.transform.Find("ProximityDetonator").gameObject);
            icePillarPrefab.transform.localScale = Projectiles.icePillarScale;

            ProjectileController icePillarController = icePillarPrefab.GetComponent<ProjectileController>();
            icePillarController.procCoefficient = StatValues.icePillarProcCoefficient;
            //icePillarController.owner = Manipulator.characterPrefab;
            GameObject icePillarGhost = CloneGhostPrefab("MageIcePillarGhost", "ManipulatorIcePillarGhost");
            icePillarGhost.transform.localScale = Projectiles.icePillarScale;

            GameObject icePillarParticleObject = icePillarGhost.transform.Find("Mesh").gameObject;
            ParticleSystem.MainModule icePillarParticle = icePillarParticleObject.GetComponent<ParticleSystem>().main;
            icePillarParticle.startSizeX = 1f;
            icePillarParticle.startSizeY = 1f;
            icePillarParticle.startSizeZ = 1f;

            ProjectileDamage icePillarDamage = icePillarPrefab.GetComponent<ProjectileDamage>();
            icePillarDamage.damageType = DamageType.Freeze2s;
            icePillarDamage.damage = 20f;

            UnityEngine.GameObject.Destroy(icePillarPrefab.GetComponent<ProjectileImpactExplosion>());
            ProjectileIcePillarExplode icePillarExplosion = icePillarPrefab.AddComponent<ProjectileIcePillarExplode>();
            icePillarExplosion.bonusBlastForce = StatValues.icePillarBlastForce;
            icePillarExplosion.blastDamageCoefficient = icePillarExplosionCoefficient;
            icePillarExplosion.blastRadius = 5f;
            icePillarExplosion.lifetime = 4f;
            icePillarExplosion.falloffModel = BlastAttack.FalloffModel.None;
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }

        private static GameObject CloneGhostPrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectileghosts/" + prefabName), newPrefabName);
            if (!newPrefab.GetComponent<NetworkIdentity>()) newPrefab.AddComponent<NetworkIdentity>();
            if (!newPrefab.GetComponent<ProjectileGhostController>()) newPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(newPrefab);
            return newPrefab;
        }
    }
}