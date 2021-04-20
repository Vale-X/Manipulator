using System.Collections.Generic;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using System.Collections;

namespace ManipulatorMod.Modules
{
    internal class ContentPacks : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => StatValues.MODUID;

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders; ;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = identifier;
            contentPack.artifactDefs.Add(new ArtifactDef[0]);
            contentPack.bodyPrefabs.Add(Modules.Prefabs.bodyPrefabs.ToArray());
            contentPack.buffDefs.Add(Modules.Buffs.buffDefs.ToArray());
            contentPack.effectDefs.Add(Modules.Assets.effectDefs.ToArray());
            contentPack.eliteDefs.Add(new EliteDef[0]);
            contentPack.entityStateConfigurations.Add(new EntityStateConfiguration[0]);
            contentPack.entityStateTypes.Add(Modules.States.entityStates.ToArray());
            contentPack.equipmentDefs.Add(new EquipmentDef[0]);
            contentPack.gameEndingDefs.Add(new GameEndingDef[0]);
            //contentPack.gameModePrefabs.Add(new Run[0]);
            contentPack.itemDefs.Add(new ItemDef[0]);
            contentPack.masterPrefabs.Add(Modules.Prefabs.masterPrefabs.ToArray());
            contentPack.musicTrackDefs.Add(new MusicTrackDef[0]);
            contentPack.networkedObjectPrefabs.Add(new GameObject[0]);
            contentPack.networkSoundEventDefs.Add(Modules.Assets.networkSoundEventDefs.ToArray());
            contentPack.projectilePrefabs.Add(Modules.Prefabs.projectilePrefabs.ToArray());
            contentPack.sceneDefs.Add(new SceneDef[0]);
            contentPack.skillDefs.Add(Modules.Skills.skillDefs.ToArray());
            contentPack.skillFamilies.Add(Modules.Skills.skillFamilies.ToArray());
            contentPack.surfaceDefs.Add(new SurfaceDef[0]);
            contentPack.survivorDefs.Add(Modules.Prefabs.survivorDefinitions.ToArray());
            //contentPack.unlockableDefs.Add(Modules.Unlockables.unlockableDefs.ToArray());
            args.ReportProgress(1f);

            yield break;
        }
    }
}