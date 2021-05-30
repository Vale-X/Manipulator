using BepInEx.Configuration;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using ManipulatorMod.Modules.Misc;
using R2API;

namespace ManipulatorMod.Modules.Survivors
{
    public static class Manipulator
    {
        internal static GameObject characterPrefab;
        internal static GameObject displayPrefab;

        internal static Material manipulatorMatFire;
        internal static Material manipulatorMatLightning;
        internal static Material manipulatorMatIce;

        public const string bodyName = "ValeManipulatorBody";

        public static int swordRendererIndex;
        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        //skill defs
        /*public static SkillDef primarySkill;
        public static SkillDef secondarySkill;
        public static SkillDef utilitySkill;
        public static SkillDef specialSkill;*/

        // unlockabledefs
        internal static UnlockableDef characterUnlockableDef;
        internal static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef grandMasterySkinUnlockableDef;

        private static SkillStates.ManipulatorMain manipulatorMainRef;

        public static SkillDef crossDef;
        public static SkillDef[] crossElementDefs;
        public static SkillDef spellDef;
        public static SkillDef[] spellElementDefs;
        public static SkillDef ventDef;
        public static SkillDef[] ventElementDefs;
        public static SkillDef switchDef;
        public static SkillDef[] switchElementDefs;


        internal static void CreateCharacter()
        {
            if (Modules.Config.characterEnabled.Value)
            {
                //CreateUnlockables();

                #region Body
                characterPrefab = Modules.Prefabs.CreatePrefab(bodyName, "mdlManipulator", new BodyInfo
                {
                    armor = StatValues.baseArmor,
                    armorGrowth = StatValues.levelArmor,
                    bodyName = bodyName,
                    bodyNameToken = ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_NAME",
                    bodyColor = StatValues.manipulatorColor,
                    characterPortrait = Modules.Assets.LoadCharacterIcon("Manipulator"),
                    crosshair = Modules.Assets.LoadCrosshair("Standard"),
                    damage = StatValues.baseDamage,
                    healthGrowth = StatValues.levelHealth,
                    healthRegen = StatValues.baseRegen,
                    jumpCount = StatValues.jumpCount,
                    maxHealth = StatValues.baseHealth,
                    shield = StatValues.baseShield,
                    shieldGrowth = StatValues.levelShield,
                    moveSpeed = StatValues.baseMoveSpeed,
                    moveSpeedGrowth = StatValues.levelMoveSpeed,
                    acceleration = StatValues.baseAcceleration,
                    jumpPower = StatValues.baseJumpPower,
                    jumpPowerGrowth = StatValues.levelJumpPower,
                    attackSpeed = StatValues.baseAttackSpeed,
                    attackSpeedGrowth = StatValues.levelAttackSpeed,
                    crit = StatValues.baseCrit,
                    critGrowth = StatValues.levelCrit,
                    subtitleNameToken = ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_SUBTITLE",
                    podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
                });
                characterPrefab.AddComponent<Modules.Components.ManipulatorController>();
                characterPrefab.AddComponent<Modules.Components.ManipulatorTracker>();

                characterPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ManipulatorMain));
                #endregion

                #region Model

                manipulatorMatFire = Modules.Assets.CreateMaterial("matManipulator", new Color(0.6544118f, 0.6544118f, 0.6544118f), 1f, StatValues.fireColor, 0f);
                manipulatorMatLightning = Modules.Assets.CreateMaterial("matManipulator", new Color(0.6544118f, 0.6544118f, 0.6544118f), 1f, StatValues.lightningColor, 0f);
                manipulatorMatIce = Modules.Assets.CreateMaterial("matManipulator", new Color(0.6544118f, 0.6544118f, 0.6544118f), 1f, StatValues.iceColor, 0f);

                Material manipulatorJetMat = Modules.Assets.mageJetMat;


                //Material manipulatorMat = Modules.Assets.CreateMaterial("matManipulator"); // cache these as there's no reason to create more when they're all the same
                //Material boxingGloveMat = Modules.Assets.CreateMaterial("matBoxingGlove");

                bodyRendererIndex = 4;
                swordRendererIndex = 1;

                Modules.Prefabs.SetupCharacterModel(characterPrefab, new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "BeltModel",
                    material = manipulatorMatFire,
                },
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = manipulatorMatFire,
                },
                new CustomRendererInfo
                {
                    childName = "JetLeft",
                    material = manipulatorJetMat,
                },
                new CustomRendererInfo
                {
                    childName = "JetRight",
                    material = manipulatorJetMat,
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = manipulatorMatFire,
                }}, bodyRendererIndex);
                #endregion

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("ManipulatorDisplay", characterPrefab);

                Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, StatValues.manipulatorColor, "MANIPULATOR", characterUnlockableDef);

                //SetupTracker();
                CreateHitboxes();
                CreateSkills();
                CreateSkins();
                InitializeItemDisplays();
                CreateDoppelganger();
                CreateStateMachines();
                CreateJetEffect();

                //if (ManipulatorPlugin.scepterInstalled) CreateScepterSkills();
            }
        }

        public static void ResetIcons()
        {
            foreach (SkillDef i in Modules.Skills.skillDefs)
            {
                if (i is SkillDefElement tempElement)
                {
                    tempElement.SwitchElementIcon(Components.ManipulatorController.ManipulatorElement.None);
                    //Debug.LogWarning($"reset icon for {tempElement.skillName}");
                }
            }
        }

        private static void CreateJetEffect()
        {
            GameObject jetEffect = Modules.Assets.mageJetEffect;
            //Debug.LogWarning(jetEffect);

            ChildLocator childLocator = characterPrefab.GetComponentInChildren<ChildLocator>();
            //Debug.LogWarning(childLocator);

            Transform holder = childLocator.FindChild("JetHolder");
            //Debug.LogWarning(holder);
            jetEffect.transform.parent = holder;
            jetEffect.transform.localPosition = new Vector3(0.4f, -0.25f, 0f);
            jetEffect.transform.localRotation = Quaternion.Euler(11, 90, 90);
            jetEffect.transform.localScale = new Vector3(0.02f, 0.02f, 0.5f);

            Vector3 tempSize = new Vector3(1.2f, 1.2f, 3f);
            Transform jetL = jetEffect.transform.Find("JetsL");
            jetL.transform.localScale = tempSize;
            Transform jetR = jetEffect.transform.Find("JetsR");
            jetR.transform.localScale = tempSize;

            Transform light = jetEffect.transform.Find("Point Light");
            light.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            Light lightEffect = light.GetComponent<Light>();
            lightEffect.range = 1.5f;
            lightEffect.intensity = 10f;

        }

        public static void CreateStateMachines()
        {
            EntityStateMachine jetState = characterPrefab.AddComponent<EntityStateMachine>();
            jetState.customName = "Jet";
            jetState.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            jetState.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));

        }

        /*private static void CreateUnlockables()
        {
            //Modules.Unlockables.AddUnlockable<Achievements.NemryAchievement>(true);
            //characterUnlockableDef = Modules.Unlockables.AddUnlockable<Achievements.ManipulatorUnlockAchievement>(true);
            masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Achievements.MasteryAchievement>(true);
            //Modules.Unlockables.AddUnlockable<Achievements.GrandMasteryAchievement>(true);
            //danteSkinUnlockableDef = Modules.Unlockables.AddUnlockable<Achievements.DanteAchievement>(true);
            //vergilSkinUnlockableDef = Modules.Unlockables.AddUnlockable<Achievements.VergilAchievement>(true);
        }*/

        private static void CreateDoppelganger()
        {
            // helper method for creating a generic doppelganger- copies ai from an existing survivor
            // hopefully i'll get around to streamlining proper ai creation sometime
            Modules.Prefabs.CreateGenericDoppelganger(characterPrefab, "ManipulatorMonsterMaster", "Merc");
        }

        private static void CreateHitboxes()
        {
            ChildLocator childLocator = characterPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");

        }

        private static void CreateSkills()
        {
            Modules.Skills.CreateSkillFamilies(characterPrefab);

            string prefix = ManipulatorPlugin.developerPrefix;

            #region Passive
            Modules.Skills.AddPassiveSkill(characterPrefab, new PassiveSkillDefInfo
            {
                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texManiPassiveIcon"),
                skillNameToken = prefix + "_MANIPULATOR_BODY_PASSIVE_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_PASSIVE_DESCRIPTION",
                keywordToken = null
            });
            #endregion

            #region Primary
            SkillDefElement crossSkillDef = Modules.Skills.CreateElementSkillDef(new ElementSkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_PRIMARY_SLASH_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_PRIMARY_SLASH_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_PRIMARY_SLASH_DESCRIPTION",
                defIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIconDef"),
                fireIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIconFire"),
                lightningIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIconLightning"),
                iceIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIconIce"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ElementalSlashState)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
                keywordTokens = new string[] { "KEYWORD_FIREEFFECT", "KEYWORD_LIGHTNINGEFFECT", "KEYWORD_ICEEFFECT" }
            });
            Modules.Skills.AddPrimarySkills(characterPrefab, crossSkillDef);
            crossDef = crossSkillDef;
            #endregion

            #region Secondary
            SkillDefElement spellSkillDef = Modules.Skills.CreateElementSkillDef(new ElementSkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_SECONDARY_SPELL_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_SECONDARY_SPELL_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_SECONDARY_SPELL_DESCRIPTION",
                defIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconDef"),
                fireIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconFire"),
                lightningIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconLightning"),
                iceIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconIce"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ElementalSpellState)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = StatValues.spellCooldown,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_FIRESPELL", "KEYWORD_LIGHTNINGSPELL", "KEYWORD_ICESPELL" }
            });
            Modules.Skills.AddSecondarySkills(characterPrefab, spellSkillDef);
            spellDef = spellSkillDef;
            #endregion

            #region Utility
            SkillDefElement ventSkillDef = Modules.Skills.CreateElementSkillDef(new ElementSkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_UTILITY_VENT_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_UTILITY_VENT_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_UTILITY_VENT_DESCRIPTION",
                defIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIconDef"),
                fireIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIconFire"),
                lightningIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIconLightning"),
                iceIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIconIce"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ElementalBlinkState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = StatValues.blinkCooldown,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens= new string[] {"KEYWORD_FIREEFFECT", "KEYWORD_LIGHTNINGEFFECT", "KEYWORD_ICEEFFECT" }
            });
            Modules.Skills.AddUtilitySkills(characterPrefab, ventSkillDef);
            ventDef = ventSkillDef;
            #endregion

            #region Special
            SkillDefElement switchSkillDef = Modules.Skills.CreateElementSkillDef(new ElementSkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_SPECIAL_SWITCH_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_SPECIAL_SWITCH_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_SPECIAL_SWITCH_DESCRIPTION",
                defIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconDef"),
                fireIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconFire"),
                lightningIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconLightning"),
                iceIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconIce"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ElementalSwitchState)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = StatValues.switchCooldown,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_FIREBONUS", "KEYWORD_LIGHTNINGBONUS", "KEYWORD_ICEBONUS" }
            });

            Modules.Skills.AddSpecialSkills(characterPrefab, switchSkillDef);
            switchDef = switchSkillDef;
            #endregion

            #region Elements
            SkillDef fireSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_ELEMENT_FIRE_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_ELEMENT_FIRE_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_ELEMENT_FIRE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconDef"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BaseStates.BaseElementState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0
            });

            SkillDef lightningSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_ELEMENT_LIGHTNING_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_ELEMENT_LIGHTNING_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_ELEMENT_LIGHTNING_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconDef"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BaseStates.BaseElementState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0
            });

            SkillDef iceSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_ELEMENT_ICE_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_ELEMENT_ICE_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_ELEMENT_ICE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconDef"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.BaseStates.BaseElementState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0
            });
            Modules.Skills.AddElementSkills(characterPrefab, fireSkillDef, lightningSkillDef, iceSkillDef);
            #endregion
        }


        // dead for now until scepter gets fixed
        //[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void CreateScepterSkills()
        {
            #region scepter
            /*string prefix = ManipulatorPlugin.developerPrefix;

            SkillDef bombSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_SPECIAL_SCEPBOMB_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_SPECIAL_SCEPBOMB_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_SPECIAL_SCEPBOMB_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texScepterSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowBomb)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 4,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isBullets = false,
                isCombatSkill = true,
                mustKeyPress = false,
                noSprint = true,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            SkillDef bazookaSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_SPECIAL_SCEPBAZOOKA_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_SPECIAL_SCEPBAZOOKA_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_SPECIAL_SCEPBAZOOKA_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaIconScepter"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Bazooka.Scepter.BazookaEnter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                isBullets = false,
                isCombatSkill = true,
                mustKeyPress = false,
                noSprint = true,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            bazookaFireSkillDefScepter = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_PRIMARY_SCEPBAZOOKA_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_PRIMARY_SCEPBAZOOKA_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_PRIMARY_SCEPBAZOOKA_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaFireIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Bazooka.Scepter.BazookaCharge)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                isBullets = false,
                isCombatSkill = true,
                mustKeyPress = false,
                noSprint = true,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            bazookaCancelSkillDefScepter = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MANIPULATOR_BODY_PRIMARY_BAZOOKAOUT_NAME",
                skillNameToken = prefix + "_MANIPULATOR_BODY_PRIMARY_BAZOOKAOUT_NAME",
                skillDescriptionToken = prefix + "_MANIPULATOR_BODY_PRIMARY_BAZOOKAOUT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaOutIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Bazooka.Scepter.BazookaExit)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                isBullets = false,
                isCombatSkill = true,
                mustKeyPress = false,
                noSprint = true,
                rechargeStock = 1,
                requiredStock = 1,
                shootDelay = 0f,
                stockToConsume = 1
            });

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(bombSkillDef, bodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(bazookaSkillDef, bodyName, SkillSlot.Special, 1);
            */
            #endregion
        }


        private static void CreateSkins()
        {
            GameObject model = characterPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            //GameObject coatObject = childLocator.FindChild("Coat").gameObject;
            //GameObject swordTrail = childLocator.FindChild("SwordTrail").gameObject;

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_DEFAULT_SKIN_NAME",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMainIcon"),
                defaultRenderers,
                mainRenderer,
                model); //LoadoutAPI.CreateSkinIcon(StatValues.iconColor1, StatValues.iconColor2, StatValues.iconColor3, StatValues.iconColor4)

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulatorBelt"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulatorSword"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("donut5Mesh"),
                    renderer = defaultRenderers[2].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("donut5Mesh"),
                    renderer = defaultRenderers[3].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulator"),
                    renderer = defaultRenderers[bodyRendererIndex].renderer
                }
            };

            /*defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = coatObject,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = swordTrail,
                    shouldActivate = false
                }
            };*/

            skins.Add(defaultSkin);
            #endregion


            #region MasterySkin
            /*Material masteryMat = Modules.Assets.CreateMaterial("matManipulatorAlt");
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                masteryMat,
                masteryMat,
                masteryMat,
                masteryMat
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                masteryRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulatorSwordAlt"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulatorAlt"),
                    renderer = defaultRenderers[bodyRendererIndex].renderer
                }
            };

            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = coatObject,
                    shouldActivate = false
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = swordTrail,
                    shouldActivate = false
                }
            };

            skins.Add(masterySkin);
            #endregion

            #region GrandMasterySkin
            if (ManipulatorPlugin.starstormInstalled)
            {
                Material grandMasteryMat = Modules.Assets.CreateMaterial("matManipulatorNeo");
                CharacterModel.RendererInfo[] grandMasteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
                {
                    grandMasteryMat,
                    grandMasteryMat,
                    grandMasteryMat,
                    grandMasteryMat
                });

                SkinDef grandMasterySkin = Modules.Skins.CreateSkinDef(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_TYPHOON_SKIN_NAME",
                    Assets.mainAssetBundle.LoadAsset<Sprite>("texGrandMasteryAchievement"),
                    grandMasteryRendererInfos,
                    mainRenderer,
                    model,
                    grandMasterySkinUnlockableDef);

                grandMasterySkin.meshReplacements = new SkinDef.MeshReplacement[]
                {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulatorSword"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshManipulatorNeo"),
                    renderer = defaultRenderers[bodyRendererIndex].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNeoCoat"),
                    renderer = defaultRenderers[4].renderer
                }
                };

                grandMasterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
                {
                new SkinDef.GameObjectActivation
                {
                    gameObject = coatObject,
                    shouldActivate = true
                },
                new SkinDef.GameObjectActivation
                {
                    gameObject = swordTrail,
                    shouldActivate = false
                }
                };

                //skins.Add(grandMasterySkin);
            }*/
            #endregion

            skinController.skins = skins.ToArray();
        }

        

        private static void InitializeItemDisplays()
        {
            CharacterModel characterModel = characterPrefab.GetComponentInChildren<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemDisplayRuleSet.name = "idrsManipulator";

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        internal static void SetItemDisplays()
        {
            itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            //missing items:
            //Charged Perforator
            //Planula

            // add item displays here
            //  HIGHLY recommend using KingEnderBrine's ItemDisplayPlacementHelper mod for this
            #region Item Displays
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Jetpack,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
                            childName = "Spine2",
localPos = new Vector3(0.03597F, -0.30802F, -0.00353F),
localAngles = new Vector3(289.9999F, 270F, 180F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GoldGat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
childName = "Spine2",
localPos = new Vector3(-0.26496F, -0.01094F, -0.20258F),
localAngles = new Vector3(11.79551F, 162.8621F, 272.3618F),
localScale = new Vector3(0.15F, 0.15F, 0.15F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BFG,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
childName = "Spine2",
localPos = new Vector3(-0.09336F, -0.06329F, -0.08499F),
localAngles = new Vector3(270F, 44.31538F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CritGlasses,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
childName = "Head",
localPos = new Vector3(-0.045F, 0.15F, 0F),
localAngles = new Vector3(284.9999F, 90F, 0F),
localScale = new Vector3(0.21F, 0.21F, 0.21F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
childName = "Spine2",
localPos = new Vector3(-0.11526F, -0.04567F, 0.11004F),
localAngles = new Vector3(35.29946F, 183.3878F, 239.8422F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Behemoth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
childName = "Sword",
localPos = new Vector3(0.15F, 0F, 0.1F),
localAngles = new Vector3(0F, 0F, 270F),
localScale = new Vector3(0.05F, 0.065F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Missile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
childName = "Spine2",
localPos = new Vector3(-0.34915F, -0.0606F, -0.21717F),
localAngles = new Vector3(270.3966F, 149.3364F, 275.4625F),
localScale = new Vector3(0.075F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Dagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
childName = "RightShoulder",
localPos = new Vector3(0.07F, 0F, 0F),
localAngles = new Vector3(300F, 20F, 340F),
localScale = new Vector3(0.65F, 0.65F, 0.65F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Hoof,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
childName = "LeftLeg",
localPos = new Vector3(-0.398F, -0.023F, 0.021F),
localAngles = new Vector3(355.896F, 88.239F, 0.45F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.RightCalf
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.LimbMask,
                            limbMask = LimbFlags.RightCalf
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ChainLightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
childName = "Spine2",
localPos = new Vector3(0.0161F, -0.2188F, -0.15F),
localAngles = new Vector3(0F, 184F, 100F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
childName = "Head",
localPos = new Vector3(-0.05F, 0.11F, 0F),
localAngles = new Vector3(280F, 90F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Mushroom,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
childName = "RightArm",
localPos = new Vector3(-0.02753F, 0.08726F, -0.02683F),
localAngles = new Vector3(10.23647F, 90.77799F, 56.98195F),
localScale = new Vector3(0.03F, 0.03F, 0.03F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
childName = "Head",
localPos = new Vector3(-0.1F, 0.075F, 0F),
localAngles = new Vector3(280F, 90F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
childName = "LeftHand",
localPos = new Vector3(-0.08F, -0.1F, 0F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WardOnLevel,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
childName = "Spine2",
localPos = new Vector3(0.06873F, -0.31633F, 0.14204F),
localAngles = new Vector3(312.3047F, 168.8284F, 284.9509F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
childName = "Sword",
localPos = new Vector3(1.15F, 0F, 0F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealWhileSafe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
childName = "RightShoulder",
localPos = new Vector3(0.05F, -0.05F, 0.05F),
localAngles = new Vector3(310F, 0F, 210F),
localScale = new Vector3(0.06F, 0.06F, 0.06F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Clover,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
childName = "RightArm",
localPos = new Vector3(0.05F, 0F, 0.05F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
childName = "LeftForeArm",
localPos = new Vector3(0F, 0F, -0.09F),
localAngles = new Vector3(0F, 100F, 90F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GoldOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
childName = "Head",
localPos = new Vector3(-0.1F, 0F, 0F),
localAngles = new Vector3(285F, 90F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WarCryOnMultiKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
childName = "LeftArm",
localPos = new Vector3(0.00734F, 0F, -0.04161F),
localAngles = new Vector3(7.58159F, 98.72622F, 272.5609F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
childName = "RightForeArm",
localPos = new Vector3(0.13146F, 0F, 0.07455F),
localAngles = new Vector3(0F, 5F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
childName = "LeftArm",
localPos = new Vector3(-0.18F, 0F, 0.01F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.55F, 0.55F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
childName = "LeftArm",
localPos = new Vector3(-0.25F, 0F, 0.01F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.55F, 0.55F, 0.55F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
childName = "LeftArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(35F, 325F, 340F),
localScale = new Vector3(0.7F, 0.7F, 0.7F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
childName = "RightArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(35F, 325F, 340F),
localScale = new Vector3(0.7F, -0.7F, 0.7F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
childName = "Head",
localPos = new Vector3(0.42736F, -0.20666F, 0.00702F),
localAngles = new Vector3(280F, 90F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
childName = "Sword",
localPos = new Vector3(-0.4F, 0F, 0F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
childName = "Sword",
localPos = new Vector3(-0.25F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.08F, 0.08F, 0.08F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
childName = "LeftUpLeg",
localPos = new Vector3(-0.0538F, 0.0199F, 0.0704F),
localAngles = new Vector3(357.4934F, 102.6498F, 279.127F),
localScale = new Vector3(0.3143F, 0.2F, 0.2765F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CommandMissile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
childName = "Spine2",
localPos = new Vector3(0.12052F, -0.31316F, 0F),
localAngles = new Vector3(0F, 90F, 180F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Feather,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
childName = "LeftArm",
localPos = new Vector3(-0.034F, 0.015F, -0.026F),
localAngles = new Vector3(332.784F, 57.888F, 256.922F),
localScale = new Vector3(0.02F, 0.02F, 0.02F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Crowbar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
childName = "Spine2",
localPos = new Vector3(0.04F, -0.13F, 0.03F),
localAngles = new Vector3(0F, 140F, 280F),
localScale = new Vector3(0.3F, 0.35F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
childName = "LeftLeg",
localPos = new Vector3(-0.5F, 0.01F, 0.008F),
localAngles = new Vector3(0F, 0F, 100F),
localScale = new Vector3(0.15F, 0.2F, 0.15F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
childName = "RightLeg",
localPos = new Vector3(0.5F, -0.01F, -0.008F),
localAngles = new Vector3(0F, 0F, 280F),
localScale = new Vector3(0.15F, 0.2F, 0.15F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
childName = "Spine2",
localPos = new Vector3(0.275F, -0.175F, 0F),
localAngles = new Vector3(350F, 270F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EquipmentMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
childName = "Spine2",
localPos = new Vector3(-0.17F, -0.17F, 0F),
localAngles = new Vector3(350F, 270F, 0F),
localScale = new Vector3(0.225F, 0.125F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
childName = "Head",
localPos = new Vector3(-0.09F, 0F, 0.06F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
childName = "Head",
localPos = new Vector3(-0.09F, 0F, -0.06F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(-0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Infusion,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
childName = "Hips",
localPos = new Vector3(-0.07411F, -0.13473F, 0.10922F),
localAngles = new Vector3(50.4516F, 341.9917F, 75.92786F),
localScale = new Vector3(0.5253F, 0.5253F, 0.5253F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Medkit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
childName = "Spine2",
localPos = new Vector3(-0.0699F, -0.18582F, 0.12518F),
localAngles = new Vector3(350F, 270F, 90F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bandolier,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
childName = "Spine2",
localPos = new Vector3(0.04F, 0F, 0F),
localAngles = new Vector3(0F, 323F, 0F),
localScale = new Vector3(0.45F, 0.44F, 0.47F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BounceNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
childName = "Spine2",
localPos = new Vector3(-0.15F, -0.18F, 0.1F),
localAngles = new Vector3(80F, 270F, 270F),
localScale = new Vector3(0.214F, 0.214F, 0.214F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IgniteOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
childName = "Spine2",
localPos = new Vector3(0.24F, -0.235F, 0.16F),
localAngles = new Vector3(350.001F, 255F, 0F),
localScale = new Vector3(0.317F, 0.317F, 0.317F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StunChanceOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
childName = "Sword",
localPos = new Vector3(0.12F, 0F, 0.04F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.5672F, 0.5672F, 0.5672F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Firework,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
childName = "Spine2",
localPos = new Vector3(-0.2F, -0.17F, -0.1F),
localAngles = new Vector3(350F, 240F, 0F),
localScale = new Vector3(0.15F, 0.15F, 0.15F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarDagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
childName = "Spine2",
localPos = new Vector3(-0.0934F, -0.11667F, 0.06321F),
localAngles = new Vector3(0F, 170F, 5.41057F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Knurl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
childName = "LeftArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.0848F, 0.0848F, 0.0848F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BeetleGland,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
childName = "Spine2",
localPos = new Vector3(-0.11F, -0.07F, 0.1F),
localAngles = new Vector3(55.0001F, 155F, 260F),
localScale = new Vector3(0.075F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
childName = "Hips",
localPos = new Vector3(-0.06643F, -0.08069F, -0.15694F),
localAngles = new Vector3(350F, 270F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
childName = "Spine2",
localPos = new Vector3(0.05017F, -0.21733F, -0.13925F),
localAngles = new Vector3(0F, 0F, 260F),
localScale = new Vector3(0.0441F, 0.0441F, 0.0441F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
childName = "Hips",
localPos = new Vector3(0.05F, -0.05F, -0.22F),
localAngles = new Vector3(0F, 15F, 260.0243F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
childName = "RightUpLeg",
localPos = new Vector3(0.14668F, 0.0023F, -0.11388F),
localAngles = new Vector3(90F, 180F, 0F),
localScale = new Vector3(0.75F, 0.75F, 0.75F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
childName = "Hips",
localPos = new Vector3(-0.08F, 0.03F, -0.15F),
localAngles = new Vector3(0F, 100F, 90F),
localScale = new Vector3(0.35F, 0.35F, 0.35F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
childName = "RightUpLeg",
localPos = new Vector3(0.44151F, 0.00635F, -0.08234F),
localAngles = new Vector3(342.9149F, 350.0526F, 40.13524F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
childName = "Spine2",
localPos = new Vector3(-0.202F, 0.001F, -0.154F),
localAngles = new Vector3(290.22F, 257.137F, 188.117F),
localScale = new Vector3(0.175F, 0.175F, 0.175F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
childName = "Head",
localPos = new Vector3(-0.05F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 80F),
localScale = new Vector3(0.18F, 0.3F, 0.18F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
childName = "Spine2",
localPos = new Vector3(-0.1523F, -0.05682F, -0.01001F),
localAngles = new Vector3(303.4839F, 267.936F, 181.1706F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
childName = "Spine2",
localPos = new Vector3(0.02F, -0.21F, 0.11F),
localAngles = new Vector3(10F, 65F, 85F),
localScale = new Vector3(0.35F, 0.35F, 0.35F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
childName = "Head",
localPos = new Vector3(-0.1F, 0F, 0.06F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.34F, 0.34F, 0.34F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
childName = "Head",
localPos = new Vector3(-0.1F, 0F, -0.06F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.34F, 0.34F, -0.34F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
childName = "Spine2",
localPos = new Vector3(0.11332F, 0.08088F, -0.0896F),
localAngles = new Vector3(302.5854F, 174.8678F, 303.3604F),
localScale = new Vector3(0.1191F, 0.1191F, 0.1191F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
childName = "RightArm",
localPos = new Vector3(0.05342F, 0F, 0.04967F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
childName = "Spine2",
localPos = new Vector3(-0.05F, 0.08F, 0.08F),
localAngles = new Vector3(350F, 90F, 45F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
childName = "Spine2",
localPos = new Vector3(0.29212F, -0.19815F, 0.14122F),
localAngles = new Vector3(311.8731F, 16.62824F, 347.4625F),
localScale = new Vector3(0.2731F, 0.2731F, 0.0273F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
childName = "LeftUpLeg",
localPos = new Vector3(0.00294F, 0.00158F, -0.31034F),
localAngles = new Vector3(355.3929F, 149.0632F, 194.7801F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarSecondaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdClaw"),
childName = "LeftForeArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 355.9517F, 71.02618F),
localScale = new Vector3(0.9F, 0.9F, 0.9F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarSpecialReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdHeart"),
childName = "Spine2",
localPos = new Vector3(-0.80107F, 0F, 0.66965F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
childName = "RightArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 85F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
childName = "Head",
localPos = new Vector3(-0.02935F, 0.15098F, 0F),
localAngles = new Vector3(0F, 180F, 195F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ParentEgg,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayParentEgg"),
childName = "Spine1",
localPos = new Vector3(0.06725F, 0.25098F, 0F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.075F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
childName = "Head",
localPos = new Vector3(-0.06577F, -0.19568F, 0.00294F),
localAngles = new Vector3(287.3469F, 90.73288F, 348.8238F),
localScale = new Vector3(0.14F, 0.14F, 0.14F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
childName = "RightHand",
localPos = new Vector3(0.06F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.8F, 0.8F, 0.8F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
childName = "Spine2",
localPos = new Vector3(-0.16907F, -0.16016F, 0.09941F),
localAngles = new Vector3(350F, 270F, 0F),
localScale = new Vector3(0.0429F, 0.0429F, 0.0429F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
childName = "Hips",
localPos = new Vector3(-0.1F, 0F, 0.16F),
localAngles = new Vector3(0F, 340F, 90F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
childName = "RightForeArm",
localPos = new Vector3(0.09F, 0.006F, 0.08F),
localAngles = new Vector3(3.5F, 280F, 90F),
localScale = new Vector3(0.05F, 0.02F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExplodeOnDeath,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
childName = "Hips",
localPos = new Vector3(-0.029F, 0.097F, -0.172F),
localAngles = new Vector3(359.752F, 6.59F, 92.149F),
localScale = new Vector3(0.03F, 0.03F, 0.03F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Seed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
childName = "RightUpLeg",
localPos = new Vector3(0.07969F, 0.02271F, -0.11846F),
localAngles = new Vector3(333.9113F, 144.416F, 286.7039F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintOutOfCombat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
childName = "Hips",
localPos = new Vector3(0.07936F, 0F, 0.20407F),
localAngles = new Vector3(90F, 260F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CooldownOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkull"),
childName = "LeftArm",
localPos = new Vector3(0.01F, 0F, -0.02F),
localAngles = new Vector3(0F, 90F, 90F),
localScale = new Vector3(0.15F, 0.15F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Phasing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
childName = "LeftLeg",
localPos = new Vector3(-0.078F, 0.1F, 0.015F),
localAngles = new Vector3(0F, 270F, 180F),
localScale = new Vector3(0.2F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.PersonalShield,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
childName = "Spine2",
localPos = new Vector3(0.05326F, -0.31702F, 0F),
localAngles = new Vector3(10F, 90F, 0F),
localScale = new Vector3(0.125F, 0.125F, 0.125F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShockNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
childName = "Spine2",
localPos = new Vector3(-0.15F, -0.17F, 0F),
localAngles = new Vector3(0F, 0F, 109.1125F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShieldOnly,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
childName = "Head",
localPos = new Vector3(-0.15F, 0F, -0.075F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.3521F, 0.3521F, 0.3521F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
childName = "Head",
localPos = new Vector3(-0.15F, 0F, 0.075F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.3521F, 0.3521F, -0.3521F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
childName = "Spine2",
localPos = new Vector3(-0.177F, -0.04F, 0.13F),
localAngles = new Vector3(25F, 275F, 189F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
childName = "Spine",
localPos = new Vector3(0.022F, 0.01F, 0F),
localAngles = new Vector3(280F, 270F, 180F),
localScale = new Vector3(0.45F, 0.14F, 0.12F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
childName = "Hips",
localPos = new Vector3(-0.0771F, -0.108F, -0.1271F),
localAngles = new Vector3(320F, 45F, 38.5F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
childName = "Spine1",
localPos = new Vector3(0.028F, -0.074F, -0.111F),
localAngles = new Vector3(36.514F, 170.937F, 180.874F),
localScale = new Vector3(0.087F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
childName = "Head",
localPos = new Vector3(0.12211F, 0.089F, -0.00111F),
localAngles = new Vector3(315F, 270F, 180F),
localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshSmall1"),
childName = "Head",
localPos = new Vector3(0.11843F, 0.06242F, -0.05116F),
localAngles = new Vector3(310F, 270F, 180F),
localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshSmall1"),
childName = "Head",
localPos = new Vector3(0.11843F, 0.06242F, 0.04388F),
localAngles = new Vector3(310F, 270F, 180F),
localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshSmall2"),
childName = "Head",
localPos = new Vector3(0.0928F, 0.02113F, -0.08911F),
localAngles = new Vector3(315F, 270F, 180F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshSmall2"),
childName = "Head",
localPos = new Vector3(0.0928F, 0.02113F, 0.08911F),
localAngles = new Vector3(315F, 270F, 180F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });


            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
childName = "RightForeArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.075F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
childName = "RightForeArm",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
childName = "Hips",
localPos = new Vector3(-0.02F, 0F, -0.2F),
localAngles = new Vector3(0F, 190F, 90F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
childName = "RightUpLeg",
localPos = new Vector3(0.10609F, -0.07811F, -0.06769F),
localAngles = new Vector3(34.20052F, 0F, 180F),
localScale = new Vector3(0.035F, 0.035F, 0.035F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Icicle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
childName = "Hips",
localPos = new Vector3(-0.902F, -0.204F, 0.352F),
localAngles = new Vector3(26.328F, 354.711F, 358.762F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Talisman,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
childName = "Hips",
localPos = new Vector3(-1.45775F, -0.17697F, -0.21304F),
localAngles = new Vector3(285.0968F, 139.694F, 312.7802F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LightningStrikeOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayChargedPerforator"),
childName = "Head",
localPos = new Vector3(-0.02935F, 0.15098F, 0F),
localAngles = new Vector3(0F, 180F, 195F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
        }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LaserTurbine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
childName = "RightArm",
localPos = new Vector3(0.1F, 0F, 0.05F),
localAngles = new Vector3(0F, 9F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
childName = "Hips",
localPos = new Vector3(-1.021F, -0.328F, -0.466F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Incubator,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
childName = "Spine2",
localPos = new Vector3(-0.068F, -0.084F, -0.108F),
localAngles = new Vector3(0F, 330F, 112.766F),
localScale = new Vector3(0.025F, 0.025F, 0.025F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
childName = "Sword",
localPos = new Vector3(0.25F, 0F, -0.02F),
localAngles = new Vector3(0F, 100F, 270F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
childName = "Hips",
localPos = new Vector3(-0.00053F, 0F, 0.1952F),
localAngles = new Vector3(0F, 170F, 270F),
localScale = new Vector3(0.075F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
childName = "RightForeArm",
localPos = new Vector3(0.02F, 0F, 0.025F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.075F, 0.075F, 0.075F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
childName = "RightUpLeg",
localPos = new Vector3(0.05265F, 0.07464F, -0.07571F),
localAngles = new Vector3(37.67522F, 152.8772F, 249.1097F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
childName = "RightForeArm",
localPos = new Vector3(0.23182F, 0.0621F, 0.02733F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.04F, 0.03F, 0.04F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Fruit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
childName = "Spine2",
localPos = new Vector3(0.17445F, -0.20585F, -0.01755F),
localAngles = new Vector3(308.6619F, 181.0245F, 280.9262F),
localScale = new Vector3(0.2118F, 0.2118F, 0.2118F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixRed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
childName = "Head",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(315F, 70F, 0F),
localScale = new Vector3(0.1036F, 0.1036F, 0.1036F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
childName = "Head",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(315F, 110F, 0F),
localScale = new Vector3(-0.1036F, 0.1036F, 0.1036F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixBlue,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
childName = "Head",
localPos = new Vector3(-0.12267F, 0F, 0F),
localAngles = new Vector3(0F, 270F, 180F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
childName = "Head",
localPos = new Vector3(-0.15256F, -0.05195F, 0F),
localAngles = new Vector3(0F, 270F, 180F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixWhite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
childName = "Head",
localPos = new Vector3(-0.19653F, 0.04465F, 0F),
localAngles = new Vector3(355F, 270F, 180F),
localScale = new Vector3(0.02F, 0.02F, 0.02F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixPoison,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
childName = "Head",
localPos = new Vector3(-0.15F, 0F, 0F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(0.03F, 0.03F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixHaunted,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
childName = "Head",
localPos = new Vector3(-0.18F, 0.05F, 0F),
localAngles = new Vector3(350F, 270F, 180F),
localScale = new Vector3(0.04F, 0.04F, 0.04F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CritOnUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
childName = "Head",
localPos = new Vector3(-0.02433F, 0.29364F, 0F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DroneBackup,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
childName = "Hips",
localPos = new Vector3(-0.07158F, 0F, -0.17797F),
localAngles = new Vector3(355F, 187.9442F, 296.9974F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Lightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLightningArmRight"),
childName = "RightArm",
localPos = new Vector3(0.23087F, -0.04134F, -0.01058F),
localAngles = new Vector3(19.49962F, 92.41315F, 145.1646F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.RightArm
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.LimbMask,
                            limbMask = LimbFlags.RightArm
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BurnNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
childName = "Hips",
localPos = new Vector3(0.04831F, -0.00079F, -0.26798F),
localAngles = new Vector3(15.90734F, 37.6746F, 70.45788F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CrippleWard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
childName = "Spine2",
localPos = new Vector3(-0.18884F, -0.21381F, 0F),
localAngles = new Vector3(80.00003F, 90.00002F, 180F),
localScale = new Vector3(0.6F, 0.6F, 0.6F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
childName = "Spine2",
localPos = new Vector3(0.1155F, -0.39181F, -0.00659F),
localAngles = new Vector3(274.7721F, 90F, 270F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GainArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
childName = "RightLeg",
localPos = new Vector3(0.35302F, -0.03952F, -0.14542F),
localAngles = new Vector3(0F, 254.6422F, 90F),
localScale = new Vector3(0.6279F, 0.6279F, 0.6279F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Recycle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
childName = "Spine2",
localPos = new Vector3(0.15571F, -0.39589F, 0F),
localAngles = new Vector3(0F, 180F, 280F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.FireBallDash,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
childName = "Hips",
localPos = new Vector3(0F, 0F, 0.22973F),
localAngles = new Vector3(0F, 256.0627F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Cleanse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
childName = "Spine2",
localPos = new Vector3(0.08444F, -0.29569F, 0F),
localAngles = new Vector3(75.00003F, 270F, 180F),
localScale = new Vector3(0.0821F, 0.0821F, 0.0821F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Tonic,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
childName = "Hips",
localPos = new Vector3(0F, 0F, -0.19681F),
localAngles = new Vector3(0F, 10F, 90F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Gateway,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
childName = "Hips",
localPos = new Vector3(-0.69741F, -0.09916F, -0.03195F),
localAngles = new Vector3(270F, 90F, 0F),
localScale = new Vector3(0.0982F, 0.0982F, 0.0982F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Meteor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
childName = "Hips",
localPos = new Vector3(-0.88081F, 0.1641F, 1.05496F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Saw,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
childName = "Hips",
localPos = new Vector3(0F, -1.7606F, -0.9431F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Blackhole,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
childName = "Hips",
localPos = new Vector3(-1.32969F, 0.01467F, 0.59264F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Scanner,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
childName = "LeftArm",
localPos = new Vector3(-0.05705F, 0F, -0.03092F),
localAngles = new Vector3(13.80236F, 140F, 180F),
localScale = new Vector3(0.175F, 0.175F, 0.175F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DeathProjectile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
childName = "Spine2",
localPos = new Vector3(0.17112F, -0.4134F, 0F),
localAngles = new Vector3(90F, 270F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.LifestealOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
childName = "Head",
localPos = new Vector3(-0.03712F, -0.00919F, -0.30598F),
localAngles = new Vector3(1.19634F, 339.2263F, 0.04973F),
localScale = new Vector3(0.1246F, 0.1246F, 0.1246F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.TeamWarCry,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
childName = "Hips",
localPos = new Vector3(-0.05284F, -0.00992F, 0.20285F),
localAngles = new Vector3(0F, 337.8478F, 270F),
localScale = new Vector3(0.05F, 0.05F, 0.05F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }

        internal static void SetItemDisplays2()
        {
            itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            #region New Item Displays
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
childName = "Spine2",
localPos = new Vector3(-0.1703F, -0.04F, -0.1258F),
localAngles = new Vector3(25.8913F, 274.7432F, 188.9742F),
localScale = new Vector3(0.6701F, 0.6701F, 0.6701F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            #endregion

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];
            newRendererInfos[1].defaultMaterial = materials[1];
            newRendererInfos[bodyRendererIndex].defaultMaterial = materials[2];
            newRendererInfos[4].defaultMaterial = materials[3];

            return newRendererInfos;
        }
    }
}