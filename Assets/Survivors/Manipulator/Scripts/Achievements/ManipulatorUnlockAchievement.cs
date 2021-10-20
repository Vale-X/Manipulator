/*using RoR2;
using System;
using UnityEngine;
using ManipulatorMod.Modules;
using RoR2.Networking;
using RoR2.Achievements;
using R2API;
using ManipulatorMod.SkillStates;

namespace ManipulatorMod.Achievements
{
   internal class ManipulatorUnlock : ModdedUnlockable
    {
        // this prefix variable is ROBVALE_THUNDERHENRY_BODY_UNLOCK_ by default.
        public string Prefix => ManipulatorPlugin.developerPrefix + Tokens.maniPrefix + "UNLOCK_";

        // Requires Tokens created in tokens.cs, as they are displayed to the player.
        public override string AchievementNameToken => Prefix + "SURVIVOR_NAME";
        public override string AchievementDescToken => Prefix + "SURVIVOR_DESC";

        // Used for referencing and must be unique to the achievement.
        public override string AchievementIdentifier => Prefix + "SURVIVOR_ID";
        public override string UnlockableIdentifier => Prefix + "SURVIVOR_REWARD_ID";
        public override string UnlockableNameToken => Prefix + "SURVIVOR_UNLOCKABLE_NAME";

        // If PrerequisiteUnlockableIdentifier matches the name of an existing AchievementIdentifier, 
        // you need to have the Achievement unlocked in order to be able to unlock this achievement.
        // In this case this ID doesn't (shouldn't) match anything, so no required achievement in order to unlock this.
        public override string PrerequisiteUnlockableIdentifier => "FreeMage";
        public override Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texManipulatorAchievement");

        public override void OnInstall()
        {
            base.OnInstall();
        }
        public override void OnUninstall()
        {
            base.OnUninstall();
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