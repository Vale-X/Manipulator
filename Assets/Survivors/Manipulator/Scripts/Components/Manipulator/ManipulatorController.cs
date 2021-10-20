using System.Collections.Generic;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System.Diagnostics;
using System.Collections;
using ManipulatorMod.Modules.Scriptables;

namespace ManipulatorMod.Modules.Components
{
    public class ManipulatorController : MonoBehaviour
    {
        // Public Variables
        public GenericSkill elementSkill;

        // Internal Variables
        internal bool hasJetBuff;
        internal bool endJet;
        internal bool hasSwapped = false;
        internal ElementalSkillDef[] elementalSkills;
        internal Dictionary<ElementalSkillDef, GenericSkill> elementalDict = new Dictionary<ElementalSkillDef, GenericSkill>();
        public Element currentElement;
        public enum Element
        {
            None,
            Fire,
            Lightning,
            Ice
        }

        // Private Variables
        private CharacterBody characterBody;
        private CharacterModel model;
        private SkillLocator skillLocator;
        private Stopwatch jetStopwatch;
        private ModelSkinController modelSkinController;
        private ChildLocator childLocator;
        private ModelLocator modelLocator;

        private Material variantFireMaterial;
        private Material variantLightningMaterial;
        private Material variantIceMaterial;

        private void Awake()
        {
            // Get all relevant components;
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.model = this.gameObject.GetComponentInChildren<CharacterModel>();
            this.modelSkinController = this.gameObject.GetComponentInChildren<ModelSkinController>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.skillLocator = this.gameObject.GetComponentInChildren<SkillLocator>();
            this.modelLocator = this.gameObject.GetComponent<ModelLocator>();
            this.jetStopwatch = new Stopwatch();
        }

        public void Start()
        {
            StartCoroutine(SetupManipulator());
        }

        IEnumerator SetupManipulator()
        {
            yield return new WaitForEndOfFrame();
            this.FindVariantMaterials();
            this.elementalSkills = this.GetElementalSkills();
            this.currentElement = this.GetStartingElement();
            this.UpdateElement(currentElement);
            this.SetMaterialEmissive(this.currentElement);
            this.hasSwapped = false;

        }

        private void FindVariantMaterials()
        {
            int skinIndex = this.modelSkinController.currentSkinIndex;
            Material tempMat2 = this.modelSkinController.skins[skinIndex].rendererInfos[0].defaultMaterial;
            string tempName = tempMat2.name;
            string searchname = tempName.Substring(0, tempName.IndexOf("-"));
            variantFireMaterial = Shaders.GetMaterialFromStorage(searchname + "-Fire");
            variantLightningMaterial = Shaders.GetMaterialFromStorage(searchname + "-Lightning");
            variantIceMaterial = Shaders.GetMaterialFromStorage(searchname + "-Ice");
        }

        internal void UpdateElement(Element element)
        {
            this.currentElement = element;
            foreach (var item in this.elementalDict)
            {
                item.Key.SwitchElement(item.Value, element);
            }
        }

        public void FixedUpdate()
        {
            // Handle Jetpack timer
            if (hasJetBuff)
            {
                this.jetStopwatch.Start();
                if (this.jetStopwatch.Elapsed.TotalSeconds >= StaticValues.jetpackDuration) this.endJet = true;
            }
            else if (this.characterBody.characterMotor.isGrounded) this.jetStopwatch.Reset();
        }

        private ElementalSkillDef[] GetElementalSkills()
        {
            List<ElementalSkillDef> elementalSkills = new List<ElementalSkillDef>();
            foreach (GenericSkill skill in this.skillLocator.allSkills)
            {
                if (skill.skillDef.GetType().IsAssignableFrom(typeof(ElementalSkillDef)))
                {
                    if (skill.skillDef as ElementalSkillDef != null)
                    {
                        elementalSkills.Add(skill.skillDef as ElementalSkillDef);
                        this.elementalDict.Add(skill.skillDef as ElementalSkillDef, skill);
                    }
                }
            }
            return elementalSkills.ToArray();
        }

        private Element GetStartingElement()
        {
            if (elementSkill)
            {
                switch (elementSkill.skillDef.skillName)
                {
                    case "Manipulator.Start Fire":
                        return Element.Fire;
                    case "Manipulator.Start Lightning":
                        return Element.Lightning;
                    case "Manipulator.Start Ice":
                        return Element.Ice;
                    default:
                        UnityEngine.Debug.LogWarning(ManipulatorPlugin.MODNAME + ": GetStartingElement warning: Returned default!");
                        return Element.Fire;
                }
            }
            else return Element.Fire;
        }

        internal void SetMaterialEmissive(Element newElement)
        {
            switch (newElement)
            {
                case Element.Fire:
                    this.UpdateMaterial(this.model, variantFireMaterial);
                    break;
                case Element.Lightning:
                    this.UpdateMaterial(this.model, variantLightningMaterial);
                    break;
                case Element.Ice:
                    this.UpdateMaterial(this.model, variantIceMaterial);
                    break;
                default:
                    UnityEngine.Debug.LogWarning(ManipulatorPlugin.MODNAME + ": SetMaterialEmissive warning: using default, may be recursive!");
                    SetMaterialEmissive(GetStartingElement());
                    break;
            }
        }

        private void UpdateMaterial(CharacterModel model, Material newMat)
        {
            CharacterModel.RendererInfo[] rendererInfos = model.baseRendererInfos;
            CharacterModel.RendererInfo bodyInfo = rendererInfos[model.baseRendererInfos.Length -1];
            CharacterModel.RendererInfo swordInfo = rendererInfos[0];
            bodyInfo.defaultMaterial = newMat;
            swordInfo.defaultMaterial = newMat;
            rendererInfos.SetValue(bodyInfo, model.baseRendererInfos.Length - 1);
            rendererInfos.SetValue(swordInfo, 0);
            model.baseRendererInfos = rendererInfos;
        }

        public void ElementalBonus(int hitCount, int targetSkill)
        {
            GenericSkill genericTarget = null;
            switch (targetSkill)
            {
                case 0:
                    genericTarget = this.skillLocator.primary;
                    break;
                case 1:
                    genericTarget = this.skillLocator.secondary;
                    break;
                case 2:
                    genericTarget = this.skillLocator.utility;
                    break;
                case 3:
                    genericTarget = this.skillLocator.special;
                    break;
            }

            // Fire Bonus
            if (this.characterBody.HasBuff(Modules.Buffs.fireBuff))
            {
                this.characterBody.RemoveBuff(Modules.Buffs.fireBuff);
            }

            // Lightning Bonus
            if (this.characterBody.HasBuff(Modules.Buffs.lightningBuff))
            {
                for (int i = 0; i < hitCount; i++)
                {
                    if (genericTarget != null)
                    {
                        genericTarget.rechargeStopwatch = genericTarget.rechargeStopwatch + (StaticValues.lightningCooldownReduction * (genericTarget.finalRechargeInterval - genericTarget.rechargeStopwatch));
                    }
                }
                this.characterBody.RemoveBuff(Modules.Buffs.lightningBuff);
            }

            // Ice Bonus
            if (this.characterBody.HasBuff(Modules.Buffs.iceBuff))
            {
                for (int i = 0; i < hitCount; i++)
                {
                    this.characterBody.healthComponent.AddBarrier(this.characterBody.healthComponent.fullHealth * StaticValues.iceBarrierPercent);
                }
                this.characterBody.RemoveBuff(Modules.Buffs.iceBuff);
            }
        }
    }
}