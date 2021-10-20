using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using RoR2.Networking;
using UnityEngine.Networking;
using StubbedConverter;

namespace ManipulatorMod.Modules
{
    internal static class Prefabs
    {
        // The order of your SurvivorDefs in your SerializableContentPack determines the order of body/displayPrefab variables here.
        // This lets you reference any bodyPrefabs or displayPrefabs throughout your code.

        internal static List<GameObject> bodyPrefabs = new List<GameObject>();
        internal static List<BodyIndex> bodyIndexes = new List<BodyIndex>();
        internal static List<GameObject> displayPrefabs = new List<GameObject>();

        private static PhysicMaterial ragdollMaterial;

        internal static void Init()
        {
            GetContent();
            AddPrefabReferences();
        }

        internal static void AddPrefabReferences()
        {
            ForEachReferences();

            //If you want to change the 'defaults' set in ForEachReferences, then set them for individual bodyPrefabs here.
            //This is if you want to use a custom crosshair or other stuff.

            // bodyPrefabs[0].GetComponent<CharacterBody>().crosshairPrefab = ...whatever you wanna set here.
        }

        // Some variables have to be set and reference assets we don't have access to in Thunderkit.
        // So instead we set them here.
        private static void ForEachReferences()
        {
            foreach (GameObject g in bodyPrefabs)
            {
                var cb = g.GetComponent<CharacterBody>();
                cb.crosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/StandardCrosshair");
                cb.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/SurvivorPod");

                var fs = g.GetComponentInChildren<FootstepHandler>();
                fs.footstepDustPrefab = Resources.Load<GameObject>("prefabs/GenericFootstepDust");

                SetupRagdoll(g);
            }
        }

        // Code from the original henry to setup Ragdolls for you.
        // This is so you dont have to manually set the layers for each object in the bones list.
        private static void SetupRagdoll(GameObject model)
        {
            RagdollController ragdollController = model.GetComponent<RagdollController>();

            if (!ragdollController) return;

            if (ragdollMaterial == null) ragdollMaterial = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<RagdollController>().bones[1].GetComponent<Collider>().material;

            foreach (Transform i in ragdollController.bones)
            {
                if (i)
                {
                    i.gameObject.layer = LayerIndex.ragdoll.intVal;
                    Collider j = i.GetComponent<Collider>();
                    if (j)
                    {
                        j.material = ragdollMaterial;
                        j.sharedMaterial = ragdollMaterial;
                    }
                }
            }
        }

        // Find all relevant prefabs within the content pack, per SurvivorDefs.
        private static void GetContent()
        {
            var survs = Assets.serialContentPack.survivorDefs;
            foreach (SurvivorDef s in survs)
            {
                bodyPrefabs.Add(s.bodyPrefab);
                bodyIndexes.Add(BodyCatalog.FindBodyIndex(s.bodyPrefab));
                displayPrefabs.Add(s.displayPrefab);
            }
        }

        // YO SPOILERS
        // don't tell anyone about this pls, I want this to be as secret as possible for as long as possible.
        // I want it to be discovered naturally :c
        internal static void PrefabHook()
        {
            On.RoR2.EscapeSequenceController.EscapeSequenceMainState.OnEnter += UpdateStatue;
            
        }

        internal static void StatueHook(Stage stage)
        {
            if (stage.sceneDef.cachedName == "moon2")
            {
                foreach (PlayerCharacterMasterController i in PlayerCharacterMasterController.instances)
                {
                    if (i.master.bodyPrefab == Modules.Prefabs.bodyPrefabs[0])
                    {
                        Modules.Prefabs.PrefabHook();
                        break;
                    }
                }
            }
        }

        internal static void AddBlackBox(string sceneName)
        {
            if (sceneName == "moon2")
            {
                GameObject BlackBox = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("ManiBoxScene"), new Vector3(115.72f, -182.62f, -13.99f), Quaternion.Euler(Vector3.zero));

                StubbedConverter.MaterialController.AddMaterialController(BlackBox);
            }
        }

        private static void UpdateStatue(On.RoR2.EscapeSequenceController.EscapeSequenceMainState.orig_OnEnter orig, EscapeSequenceController.EscapeSequenceMainState self)
        {
            orig(self);
            GameObject statue = UnityEngine.GameObject.Find("SmoothFrog");
            MeshFilter mesh = statue.GetComponent<MeshFilter>();
            mesh.mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBanjoCat");

            if (NetworkServer.active)
            {
                GameObject NyaZone = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("NyaZone"), statue.transform, false);
                NetworkServer.Spawn(NyaZone);
                return;
            }
        }

    }
}
