/*using RoR2;
using System;
using UnityEngine;
using ManipulatorMod.Modules;
using ManipulatorMod.Modules.Components;
using RoR2.Achievements;
using RoR2.Skills;
using R2API;

namespace ManipulatorMod.Achievements
{
    internal class ManipulatorJump : ModdedUnlockable
    {
        // this prefix variable is ROBVALE_THUNDERHENRY_BODY_UNLOCK_ by default.
        public string Prefix => ManipulatorPlugin.developerPrefix + Tokens.maniPrefix + "UNLOCK_";

        // Requires Tokens created in tokens.cs, as they are displayed to the player.
        public override string AchievementNameToken => Prefix + "JUMP_NAME";
        public override string AchievementDescToken => Prefix + "JUMP_DESC";

        // Used for referencing and must be unique to the achievement.
        public override string AchievementIdentifier => Prefix + "JUMP_ID";
        public override string UnlockableIdentifier => Prefix + "JUMP_REWARD_ID";
        public override string UnlockableNameToken => Prefix + "JUMP_UNLOCKABLE_NAME";

        // If PrerequisiteUnlockableIdentifier matches the name of an existing AchievementIdentifier, 
        // you need to have the Achievement unlocked in order to be able to unlock this achievement.
        // In this case you need to have HenryUnlockAchievement completed in order to meet the requirements for this achivement.
        public override string PrerequisiteUnlockableIdentifier => Prefix + "SURVIVOR_ID";

        // make sure this matches the NAME of the UnlockableDef you create for the achievement.
        //public override UnlockableDef UnlockableDef => Modules.Assets.mainAssetBundle.LoadAsset<UnlockableDef>("Skills.Manipulator.VentedStep");
        public override Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIconDef");

        private static readonly int requirement = 5;

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("ManipulatorBody");
        }
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            this.SetServerTracked(true);
        }
        public override void OnBodyRequirementBroken()
        {
            this.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }

        public class ManipulatorJumpServerAchievement : BaseServerAchievement
        {
            private CharacterBody trackedBody
            {
                get
                {
                    return this._trackedBody;
                }
                set
                {
                    if (this._trackedBody == value)
                    {
                        return;
                    }
                    if (this._trackedBody != null)
                    {
                        this._trackedBody.onSkillActivatedServer -= this.OnBodySkillActivatedServer;
                    }
                    this._trackedBody = value;
                    if (this._trackedBody != null)
                    {
                        this._trackedBody.onSkillActivatedServer += OnBodySkillActivatedServer;
                        this.progress = 0;
                    }
                }
            }

            private void OnBodySkillActivatedServer(GenericSkill skillSlot)
            {
                if (skillSlot.skillDef == this.requiredSkillDef && this.requiredSkillDef != null)
                {
                    this.progress = 0;
                }
            }

            public override void OnInstall()
            {
                base.OnInstall();
                this.fireProjIndex = ProjectileCatalog.FindProjectileIndex("ManipulatorBlinkFire");
                this.lightningProjIndex = ProjectileCatalog.FindProjectileIndex("ManipulatorBlinkLightning");
                this.iceProjIndex = ProjectileCatalog.FindProjectileIndex("ManipulatorBlinkIce");
                this.requiredSkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("Manipulator.Venting Step"));
                GlobalEventManager.onCharacterDeathGlobal += onCharacterDeathGlobal;
                RoR2Application.onFixedUpdate += FixedUpdate;
            }

            public override void OnUninstall()
            {
                this.trackedBody = null;
                GlobalEventManager.onCharacterDeathGlobal -= onCharacterDeathGlobal;
                RoR2Application.onFixedUpdate -= FixedUpdate;
                base.OnUninstall();
            }

            private void FixedUpdate()
            {
                this.trackedBody = base.GetCurrentBody();
            }

            private void onCharacterDeathGlobal(DamageReport report)
            {
                if (report.attackerBody == this.trackedBody && report.attackerBody)
                {
                    if (!base.IsCurrentBody(report.damageInfo.attacker))
                    {
                        return;
                    }
                    int index = ProjectileCatalog.GetProjectileIndex(report.damageInfo.inflictor);
                    if (index == this.fireProjIndex || index == this.lightningProjIndex || index == this.iceProjIndex)
                    {
                        this.progress++;
                        if (this.progress >= ManipulatorJump.requirement)
                        {
                            base.Grant();
                        }
                    }
                }
            }

            private int progress;
            private int fireProjIndex;
            private int lightningProjIndex;
            private int iceProjIndex;
            private CharacterBody _trackedBody;
            private SkillDef requiredSkillDef;
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