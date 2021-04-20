using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using RoR2.Achievements;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManipulatorMod.Modules.Achievements
{
    internal class ManipulatorUnlockAchievement : R2API.ModdedUnlockable
    {
        public override string AchievementIdentifier => ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier => ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken => ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override string AchievementDescToken => ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override string UnlockableNameToken => ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_UNLOCKABLE_NAME";
        public override string PrerequisiteUnlockableIdentifier => "";
        public override Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texManipulatorAchievement");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                                    {
                                Language.GetString(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                                    }));

        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        public void Check(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
        }

        public override void OnInstall()
        {
            base.OnInstall();

            On.RoR2.SceneDirector.Start += this.Check;
        }
        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.SceneDirector.Start -= this.Check;
        }
    }
}
