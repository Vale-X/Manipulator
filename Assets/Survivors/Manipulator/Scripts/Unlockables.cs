using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ManipulatorMod.Achievements;
using R2API;
using RoR2.UI.LogBook;
using HG;

namespace ManipulatorMod.Modules
{
    /*internal class Unlockables
    {
        // Unlockables are added at runtime (do not add them to the content pacK). One for each script that inherits from ThunderHenryAchievement.
        public static List<UnlockableDef> loadedUnlockableDefs = new List<UnlockableDef>();

        public static void Init()
        {
            AddUnlockables();
            HiddenSkin.Init();
        }
        private static void AddUnlockables()
        {
            if (!Config.forceUnlock.Value) {
                loadedUnlockableDefs.Add(UnlockableAPI.AddUnlockable<ManipulatorUnlock>(FindUnlockable("Characters.Manipulator"))); 
            }
            if (!Config.forceUnlockSkills.Value) {
                loadedUnlockableDefs.Add(UnlockableAPI.AddUnlockable<ManipulatorOverload>(FindUnlockable("Skills.Manipulator.Overload")));
                loadedUnlockableDefs.Add(UnlockableAPI.AddUnlockable<ManipulatorJump>(typeof(ManipulatorJump.ManipulatorJumpServerAchievement), FindUnlockable("Skills.Manipulator.VentedRise")));
            }
            loadedUnlockableDefs.Add(UnlockableAPI.AddUnlockable<ManipulatorMastery>(FindUnlockable("Skins.Manipulator.Alt1")));
            loadedUnlockableDefs.Add(UnlockableAPI.AddUnlockable<ManipulatorNya>(FindUnlockable("Skins.Manipulator.Nya")));
        }

        private static UnlockableDef FindUnlockable(string name)
        {
            return Modules.Assets.mainAssetBundle.LoadAsset<UnlockableDef>(name);
        }
    }*/

    internal class HiddenSkin
    {
        // Keep this all secret please! I'd rather people found this naturally on their own. Ya know?

        public static List<SkinDef> hiddenIfLockedSkins = new List<SkinDef>();

        public static void Init()
        {
            HideSkin(GetSkin("skinManipulatorNya"));

            IL.RoR2.UI.LoadoutPanelController.Row.FromSkin += HideSkinIfLocked;
            IL.RoR2.UI.LogBook.LogBookController.BuildEntriesPage += HideAchievementIfUnlockableHidden;
        }

        public static SkinDef GetSkin(string name)
        {
            return Modules.Assets.mainAssetBundle.LoadAsset<SkinDef>(name);
        }

        public static void HideSkin(SkinDef skin)
        {
            if (!hiddenIfLockedSkins.Contains(skin))
            {
                hiddenIfLockedSkins.Add(skin);
            }
        }

        private static void HideSkinIfLocked(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var skinDefVariable = -1;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(2),
                    x => x.MatchLdcI4(out _),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(2)))
            {
                var labelLoopStart = c.MarkLabel();

                c.GotoPrev(MoveType.After,
                    x => x.MatchLdloc(1),
                    x => x.MatchLdloc(2),
                    x => x.MatchLdelemRef(),
                    x => x.MatchStloc(out skinDefVariable));

                c.Emit(OpCodes.Ldloc, skinDefVariable);
                c.EmitDelegate<Func<SkinDef, bool>>((skinDef) =>
                {
                    if (hiddenIfLockedSkins.Contains(skinDef))
                    {
                        if (skinDef.unlockableDef != null)
                        {
                            if (LocalUserManager.isAnyUserSignedIn)
                            {
                                var localUser = LocalUserManager.GetFirstLocalUser();
                                if (localUser != null)
                                {
                                    // shhhh just pretend this isn't a thing
                                    return !AchievementManager.GetUserAchievementManager(localUser).userProfile.HasUnlockable(skinDef.unlockableDef);
                                }
                            }
                        }
                        return true;
                    }
                    else return false;
                });
                c.Emit(OpCodes.Brtrue, labelLoopStart);
            }
        }

        private static void HideAchievementIfUnlockableHidden(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var entriesField = -1;

            c.GotoNext(MoveType.After,
                x => x.MatchLdarg(out entriesField),
                x => x.MatchLdfld<LogBookController.NavigationPageInfo>(nameof(LogBookController.NavigationPageInfo.entries)),
                x => x.MatchStloc(out _));

            c.Emit(OpCodes.Ldloc, entriesField);
            c.EmitDelegate<Action<Entry[]>>((entries) =>
            {
                if (entries[0].category.nameToken == "LOGBOOK_CATEGORY_ACHIEVEMENTS")
                {
                    int num = entries.Length;
                    int i = 0;
                    while (i < num)
                    {
                        entries[i].getStatusImplementation = new Entry.GetStatusDelegate(ReturnHidden);
                        i++;
                    }
                }
            });
        }

        public static EntryStatus ReturnHidden(in Entry entry, UserProfile userProfile)
        {
            AchievementDef achievement = (AchievementDef)entry.extraData;
            if (achievement != null) {
                UnlockableDef unlock = UnlockableCatalog.GetUnlockableDef(achievement.unlockableRewardIdentifier);
                if (unlock != null) {
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    if (localUser != null) {
                        if (unlock.hidden && !AchievementManager.GetUserAchievementManager(localUser).userProfile.HasUnlockable(unlock) && unlock.nameToken == Modules.Assets.mainAssetBundle.LoadAsset<UnlockableDef>("Skins.Manipulator.Nya").nameToken)
                        { 
                            return EntryStatus.Locked; 
                        }
                    }
                }
            }
            return LogBookController.GetAchievementStatus(entry, userProfile);
        }
    }
}
