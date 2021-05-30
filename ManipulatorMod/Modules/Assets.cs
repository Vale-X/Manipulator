using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using RoR2.Audio;
using System.Collections.Generic;
using RoR2.UI;

namespace ManipulatorMod.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        // tracker
        internal static GameObject trackerPrefab;

        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        internal static List<EffectDef> effectDefs = new List<EffectDef>();

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            //Shader internal name                  Path name
            {"stubbed_Hopoo Games/Deferred/Standard Proxy", "shaders/deferred/hgstandard"},
            {"stubbed_Hopoo Games/FX/Cloud Remap Proxy",    "shaders/fx/hgcloudremap"}
        };

        // cache these and use to create our own materials
        public static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        
        public static Material mageMat;
        public static Material mageJetMat;
        public static Material mageFireMat;
        internal static GameObject mageJetEffect;
        public static GameObject waveImpactEffectFire;
        public static GameObject waveImpactEffectFireAlt;

        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ManipulatorMod.manipulatorassetbundle"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("ManipulatorMod.ManipulatorBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            mageJetMat = GetMageJetMaterial();
            mageJetEffect = GetMageJetEffect();
            mageFireMat = GetMageFireMaterial();

            ConvertAllBundleShaders();

            waveImpactEffectFire = LoadEffect("ManipulatorWaveImpactFire", 1.5f);
            waveImpactEffectFireAlt = LoadEffect("ManipulatorWaveImpactFireAlt", 1.5f);

            trackerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"), "ManipulatorTrackerPrefab", false);
            trackerPrefab.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            trackerPrefab.transform.Find("Core Pip").localScale = new Vector3(0.15f, 0.15f, 0.15f);

            trackerPrefab.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            trackerPrefab.transform.Find("Core, Dark").localScale = new Vector3(0.1f, 0.1f, 0.1f);

            foreach (SpriteRenderer i in trackerPrefab.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (i)
                {
                    i.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                    i.color = Color.white;
                }
            }
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            /*NetworkSoundEventCatalog.getSoundEventDefs += delegate (List<NetworkSoundEventDef> list)
            {
                list.Add(networkSoundEventDef);
            };*/
            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        //Thank you Bleppy (and KomradeSpectre) for this!
        internal static void ConvertAllBundleShaders()
        {
            Debug.Log("Manipulator: Attempting to convert shaders...");
            var materialAssets = mainAssetBundle.LoadAllAssets<Material>();
            Debug.LogWarning(materialAssets);
            foreach (Material material in materialAssets)
            {
                //Debug.Log("Manipulator: Replacing " + material.shader.name);
                if (!material.shader.name.StartsWith("stubbed_")) { continue; }
                var replacementShader = Resources.Load<Shader>(ShaderLookup[material.shader.name]);
                if (replacementShader) { material.shader = replacementShader; }
                Debug.Log("Manipulator: Succesfully replaced " + material.name + "'s shaders with Hopoo shaders");
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false, 12f);
        }
        private static GameObject LoadEffect(string resourceName, float lifetime)
        {
            return LoadEffect(resourceName, "", false, lifetime);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false, 12f);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform, 12f);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            return LoadEffect(resourceName, soundName, parentToTransform, 12f);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform, float lifetime)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);
            //EffectAPI.AddEffect(newEffect);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, Color matColor, float emission, Color emissionColor, float normalStrength)
        {
            if (!mageMat) mageMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/MageBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[3].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(mageMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return mageMat;

            mat.name = materialName;

            mat.SetColor("_Color", matColor);
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);
            mat.SetFloat("_SpecularStrength", 0.659f);
            mat.SetFloat("_SpecularExponent", 8.37f);

            return mat;
        }

        public static Material GetMageJetMaterial()
        {
            if (!mageJetMat) mageJetMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/MageBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(mageJetMat);

            return mat;
        }

        public static Material GetMageFireMaterial()
        {
            if (!mageFireMat) mageFireMat = Resources.Load<GameObject>("Prefabs/ProjectileGhosts/MageFireboltGhost").GetComponentInChildren<MeshRenderer>().materials[0];

            Material mat = UnityEngine.Object.Instantiate<Material>(mageFireMat);

            return mat;
        }

        private static GameObject GetMageJetEffect()
        {
            if (!mageJetEffect) mageJetEffect = Resources.Load<GameObject>("Prefabs/CharacterBodies/MageBody").GetComponentInChildren<ChildLocator>().FindChild("JetOn").gameObject;

            GameObject obj = UnityEngine.Object.Instantiate<GameObject>(mageJetEffect);
            obj.SetActive(true);

            return obj;
        }

        public static Material CreateMaterial(string materialName, Color matColor)
        {
            return Assets.CreateMaterial(materialName, matColor, 0f);
        }

        public static Material CreateMaterial(string materialName, Color matColor, float emission)
        {
            return Assets.CreateMaterial(materialName, matColor, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, Color matColor, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, matColor, emission, emissionColor, 0f);
        }
    }
}