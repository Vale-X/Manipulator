/*using RoR2;
using System;
using UnityEngine;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using R2API;

namespace ManipulatorMod.Achievements
{
    internal class ManipulatorOverload : ModdedUnlockable
    {
        // this prefix variable is ROBVALE_THUNDERHENRY_BODY_UNLOCK_ by default.
        public string Prefix => ManipulatorPlugin.developerPrefix + Tokens.maniPrefix + "UNLOCK_";

        // Requires Tokens created in tokens.cs, as they are displayed to the player.
        public override string AchievementNameToken => Prefix + "OVERLOAD_NAME";
        public override string AchievementDescToken => Prefix + "OVERLOAD_DESC";

        // Used for referencing and must be unique to the achievement.
        public override string AchievementIdentifier => Prefix + "OVERLOAD_ID";
        public override string UnlockableIdentifier => Prefix + "OVERLOAD_REWARD_ID";
        public override string UnlockableNameToken => Prefix + "OVERLOAD_UNLOCKABLE_NAME";

        // If PrerequisiteUnlockableIdentifier matches the name of an existing AchievementIdentifier, 
        // you need to have the Achievement unlocked in order to be able to unlock this achievement.
        // In this case you need to have HenryUnlockAchievement completed in order to meet the requirements for this achivement.
        public override string PrerequisiteUnlockableIdentifier => Prefix + "SURVIVOR_ID";

        // make sure this matches the NAME of the UnlockableDef you create for the achievement.
        //public override UnlockableDef UnlockableDef => Modules.Assets.mainAssetBundle.LoadAsset<UnlockableDef>("Skills.Manipulator.Overload");
        public override Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconDef");

        public override void OnInstall()
        {
            base.OnInstall();
            TeleporterInteraction.onTeleporterFinishGlobal += SwitchCheckTeleport;
        }
        public override void OnUninstall()
        {
            base.OnUninstall();
            TeleporterInteraction.onTeleporterFinishGlobal -= SwitchCheckTeleport;
        }

        private void SwitchCheckTeleport(TeleporterInteraction obj)
        {
            if (base.meetsBodyRequirement)
            {
                ManipulatorController maniController = base.localUser.cachedBodyObject.GetComponent<ManipulatorController>();
                if (maniController)
                {
                    if (!maniController.hasSwapped)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(Prefabs.bodyPrefabs[0]);
        }
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }
        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }

        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString(AchievementNameToken),
            Language.GetString(AchievementDescToken)
        });
        public override Func<string> GetUnlocked => () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString(AchievementNameToken),
            Language.GetString(AchievementDescToken)
        });


    }
}*/