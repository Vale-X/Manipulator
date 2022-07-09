using System;
using System.Collections.Generic;
using System.IO;
using Path = System.IO.Path;
using RoR2.ContentManagement;
using System.Collections;
using System.Reflection;
using UnityEngine;
using System.Linq;
using R2API;
using RoR2;
using EntityStates;
using StubbedConverter;

namespace ManipulatorMod.Modules
{
    internal static class Assets
    {
		//The file name of your asset bundle
		internal const string assetBundleName = "manipulatorassetbundle";

		//Should be the same name as your SerializableContentPack in the asset bundle
		internal const string contentPackName = "ManipulatorContentPack";

		//Name of the your soundbank file, if any.
		internal const string soundBankName = "SB_Manipulator"; //HenryBank

		internal static AssetBundle mainAssetBundle = null;
		internal static ContentPack mainContentPack = null;
		internal static SerializableContentPack serialContentPack = null;

		internal static string assemblyDir
		{
			get
			{
				return Path.GetDirectoryName(ManipulatorPlugin.pluginInfo.Location);
			}
		}

		internal static List<EffectDef> effectDefs = new List<EffectDef>();

		internal static List<Material> materialStorage = new List<Material>();

        internal static void Init()
        {
            if (assetBundleName == "thunderassetbundle")
            {
                Debug.LogError(ManipulatorPlugin.MODNAME + ": AssetBundle name hasn't been changed. Not loading any assets to avoid conflicts.");
				ManipulatorPlugin.cancel = true;
                return;
            }

			if (Modules.Config.characterEnabled.Value == false) 
			{
				Debug.LogFormat(ManipulatorPlugin.MODNAME + ": Character enabled config value is false. Not loading mod.");
				ManipulatorPlugin.cancel = true; 
				return; 
			}

            LoadAssetBundle();
            PopulateAssets();
        }

		// Any extra asset stuff not handled or loaded by the Asset Bundle should be sorted here.
		// This is also a good place to set up any references, if you need to.
		// References within SkillState scripts can be done through EntityStateConfigs instead.
		internal static void PopulateAssets()
		{
			if (!mainAssetBundle)
			{
				Debug.LogError(ManipulatorPlugin.MODNAME + ": AssetBundle not found. Unable to Populate Assets.");
				ManipulatorPlugin.cancel = true;
				return;
			}

			OrbAPI.AddOrb(typeof(Components.ChainLightningOrb));
		}

		// Loads the AssetBundle, which includes the Content Pack.
		internal static void LoadAssetBundle()
        {
            if (mainAssetBundle == null)
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(path, assetBundleName));

				if (!mainAssetBundle) 
				{
					Debug.LogError(ManipulatorPlugin.MODNAME + ": AssetBundle not found. File missing or assetBundleName is incorrect.");
					ManipulatorPlugin.cancel = true;
					return;
				}
				LoadContentPack();
			}
        }

		// Sorts out ContentPack related shenanigans.
		// Sets up variables for reference throughout the mod and initializes a new content pack based on the SerializableContentPack.
		internal static void LoadContentPack()
        {
			serialContentPack = mainAssetBundle.LoadAsset<SerializableContentPack>(contentPackName);
			mainContentPack = serialContentPack.CreateContentPack();
			AddEntityStateTypes();
			AddEntityStateConfigs();
			CreateEffectDefs();
			ContentPackProvider.contentPack = mainContentPack;
		}

		// Gathers all GameObjects with VFXAttributes attached and creates an EffectDef for each one.
		// Without this, the Effect is unable to be spawned.
		// Any VFX elements must have a NetWorkIdentity, VFXAttributes and EffectComponent on the base in order to be usable.
		internal static void CreateEffectDefs()
        {
			List<GameObject> effects = new List<GameObject>();

			GameObject[] assets = mainAssetBundle.LoadAllAssets<GameObject>();
			foreach (GameObject g in assets)
            {
				if (g.GetComponent<EffectComponent>())
                {
					effects.Add(g);
                }
            }
			foreach (GameObject g in effects)
            {
				EffectDef def = new EffectDef();
				def.prefab = g;

				effectDefs.Add(def);
            }

			mainContentPack.effectDefs.Add(effectDefs.ToArray());
        }



		// Finds all Entity State Types within the mod and adds them to the content pack.
		// Saves fuss of having to add them manually. Credit to KingEnderBrine for this code.
		internal static void AddEntityStateTypes()
        {
			Debug.LogWarning("Adding types...");
			try
			{
				mainContentPack.entityStateTypes.Add(((IEnumerable<System.Type>)Assembly.GetExecutingAssembly().GetTypes()).Where<System.Type>
					((Func<System.Type, bool>)(type => typeof(EntityState).IsAssignableFrom(type))).ToArray<System.Type>());
			}
			catch (ReflectionTypeLoadException ex)
            {
				foreach (Exception x in ex.LoaderExceptions)
                {
					Debug.LogError(x.Message);
                }
            }

			if (ManipulatorPlugin.debug)
			{
				foreach (Type t in mainContentPack.entityStateTypes)
				{
					Debug.Log(ManipulatorPlugin.MODNAME + ": Added EntityStateType: " + t);
				}
			}
		}

		internal static void AddEntityStateConfigs()
        {
			mainContentPack.entityStateConfigurations.Add(mainAssetBundle.LoadAllAssets<EntityStateConfiguration>());

			if (ManipulatorPlugin.debug) {
				foreach (EntityStateConfiguration t in mainContentPack.entityStateConfigurations)
				{
					Debug.LogWarning(ManipulatorPlugin.MODNAME + ": Added EntityStateConfiguration: " + t);
				}
			}
        }
    }

	public class ContentPackProvider : IContentPackProvider
	{
		public static SerializableContentPack serializedContentPack;
		public static ContentPack contentPack;

		public static string contentPackName = null;
		public string identifier
		{
			get
			{
				return ManipulatorPlugin.MODNAME;
			}
		}

		internal static void Initialize()
		{
			contentPackName = Assets.contentPackName;
			//contentPack = serializedContentPack.CreateContentPack();
			ContentManager.collectContentPackProviders += AddCustomContent;
		}

		private static void AddCustomContent(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
		{
			addContentPackProvider(new ContentPackProvider());
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
		{
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}
	}

	public static class SoundBankManager
    {
		//Hey thanks Kevin for the sound setup stuff
		public static string soundBankDirectory
        {
            get
            {
				return Path.Combine(Assets.assemblyDir, "soundbanks/" + Assets.soundBankName + ".bnk");
            }
        }

		public static void Init()
        {
			byte[] bank = File.ReadAllBytes(soundBankDirectory);
			SoundAPI.SoundBanks.Add(bank);
			/*AkSoundEngine.AddBasePath(soundBankDirectory);
			AkSoundEngine.LoadFilePackage("ManipulatorSounds.pck", out var packageID, -1);
			AkSoundEngine.LoadBank("SB_Manipulator", -1, out var bankID);
			AkSoundEngine.LoadBank("ManiInit", -1, out var initID);*/
        }
    }
}
