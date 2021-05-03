using RoR2;
using RoR2.Skills;
using R2API.Utils;
using UnityEngine;
using System.Diagnostics;
using ManipulatorMod.Modules.Misc;

namespace ManipulatorMod.Modules.Components
{
    // just a class to run some custom code for things like weapon models
    public class ManipulatorController : MonoBehaviour
    {
        //public bool hasBazookaReady;

        public bool attackReset;
        public bool gotStartingElement;
        public bool hasJetBuff;
        public bool endJet;
        private Stopwatch jetStopwatch;


        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;
        public ManipulatorTracker tracker;
        private Animator modelAnimator;
        private SkillLocator skillLocator;
        private Material modelMaterial;
        private Material matVariantFire;
        private Material matVariantLightning;
        private Material matVariantIce;

        public enum ManipulatorElement
        {
            None,
            Fire,
            Lightning,
            Ice
        }

        public ManipulatorElement currentElement;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.gameObject.GetComponentInChildren<CharacterModel>();
            this.tracker = this.gameObject.GetComponent<ManipulatorTracker>();
            this.modelAnimator = this.gameObject.GetComponentInChildren<Animator>();
            this.skillLocator = this.gameObject.GetComponentInChildren<SkillLocator>();
            this.jetStopwatch = new Stopwatch();
        }

        public void Start()
        {
            this.currentElement = GetStartingElement();
            this.SetElementSkillIcons(this.currentElement);
            this.SetMaterialEmissive(this.currentElement);
        }

        public void FixedUpdate()
        {
            if (hasJetBuff)
            {
                this.jetStopwatch.Start();
                if (this.jetStopwatch.Elapsed.TotalSeconds >= StatValues.jetDuration) this.endJet = true;
            }
            else if (this.characterBody.characterMotor.isGrounded) this.jetStopwatch.Reset();
        }

        private ManipulatorElement GetStartingElement()
        {
            ManipulatorElementController elementController = this.gameObject.GetComponent<ManipulatorElementController>();
            switch (elementController.elementSkill.skillDef.skillName)
            {
                case "VALE_MANIPULATOR_BODY_ELEMENT_FIRE_NAME":
                    return ManipulatorElement.Fire;
                case "VALE_MANIPULATOR_BODY_ELEMENT_LIGHTNING_NAME":
                    return ManipulatorElement.Lightning;
                case "VALE_MANIPULATOR_BODY_ELEMENT_ICE_NAME":
                    return ManipulatorElement.Ice;
                default:
                    return ManipulatorElement.Fire;
            }
        }

        public void SetElementSkillIcons(ManipulatorElement newElement)
        {
            foreach (GenericSkill i in skillLocator.allSkills)
            {
                if (i.skillDef is SkillDefElement tempElement)
                {
                    tempElement.SwitchElementIcon(newElement);
                }
            }
        }

        public void SetMaterialEmissive(ManipulatorElement newElement)
        {
            //UnityEngine.Debug.LogWarning("Settting mat emissive");
            switch (newElement)
            {
                case ManipulatorElement.Fire:
                    this.UpdateMaterial(this.model, Modules.Survivors.Manipulator.manipulatorMatFire);
                    //UnityEngine.Debug.LogWarning("Settting mat emissive fire");
                    break;
                case ManipulatorElement.Lightning:
                    this.UpdateMaterial(this.model, Modules.Survivors.Manipulator.manipulatorMatLightning);
                    //UnityEngine.Debug.LogWarning("Settting mat emissive lightning");
                    break;
                case ManipulatorElement.Ice:
                    this.UpdateMaterial(this.model, Modules.Survivors.Manipulator.manipulatorMatIce);
                    //UnityEngine.Debug.LogWarning("Settting mat emissive ice");
                    break;
                default:
                    UnityEngine.Debug.LogWarning("SetMaterialEmissive warning: using default, may be recursive!");
                    SetMaterialEmissive(GetStartingElement());
                    break;
            }
        }

        private void UpdateMaterial(CharacterModel model, Material newMat)
        {
            CharacterModel.RendererInfo[] rendererInfos = model.baseRendererInfos;
            CharacterModel.RendererInfo bodyInfo = rendererInfos[Modules.Survivors.Manipulator.bodyRendererIndex];
            CharacterModel.RendererInfo swordInfo = rendererInfos[Modules.Survivors.Manipulator.swordRendererIndex];
            bodyInfo.defaultMaterial = newMat;
            swordInfo.defaultMaterial = newMat;
            rendererInfos.SetValue(bodyInfo, Modules.Survivors.Manipulator.bodyRendererIndex);
            rendererInfos.SetValue(swordInfo, Modules.Survivors.Manipulator.swordRendererIndex);
            model.baseRendererInfos = rendererInfos;
        }

        /*private void CheckWeapon()
        {
            switch (this.characterBody.skillLocator.primary.skillDef.skillNameToken)
            {
                default:
                    this.childLocator.FindChild("SwordModel").gameObject.SetActive(true);
                    this.childLocator.FindChild("BoxingGloveL").gameObject.SetActive(false);
                    this.childLocator.FindChild("BoxingGloveR").gameObject.SetActive(false);
                    this.childLocator.FindChild("AltGun").gameObject.SetActive(false);
                    this.modelAnimator.SetLayerWeight(this.modelAnimator.GetLayerIndex("Body, Alt"), 0f);
                    break;
                case ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_PRIMARY_PUNCH_NAME":
                    this.childLocator.FindChild("SwordModel").gameObject.SetActive(false);
                    this.childLocator.FindChild("BoxingGloveL").gameObject.SetActive(true);
                    this.childLocator.FindChild("BoxingGloveR").gameObject.SetActive(true);
                    this.childLocator.FindChild("AltGun").gameObject.SetActive(false);
                    this.modelAnimator.SetLayerWeight(this.modelAnimator.GetLayerIndex("Body, Alt"), 1f);
                    break;
                case ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_PRIMARY_GUN_NAME":
                    this.childLocator.FindChild("SwordModel").gameObject.SetActive(false);
                    this.childLocator.FindChild("BoxingGloveL").gameObject.SetActive(false);
                    this.childLocator.FindChild("BoxingGloveR").gameObject.SetActive(false);
                    this.childLocator.FindChild("AltGun").gameObject.SetActive(true);
                    this.modelAnimator.SetLayerWeight(this.modelAnimator.GetLayerIndex("Body, Alt"), 0f);
                    break;
            }

            bool hasTrackingSkill = false;

            if (this.characterBody.skillLocator.secondary.skillDef.skillNameToken == ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_SECONDARY_STINGER_NAME")
            {
                this.childLocator.FindChild("GunModel").gameObject.SetActive(false);
                this.childLocator.FindChild("Gun").gameObject.SetActive(false);

                this.characterBody.crosshairPrefab = Modules.Assets.LoadCrosshair("SimpleDot");
                hasTrackingSkill = true;
            }
            else if (this.characterBody.skillLocator.secondary.skillDef.skillNameToken == ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_SECONDARY_UZI_NAME")
            {
                this.childLocator.FindChild("GunModel").GetComponent<SkinnedMeshRenderer>().sharedMesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshUzi");
            }

            if (!hasTrackingSkill && this.tracker) Destroy(this.tracker);
        }*/

        public void UpdateCrosshair()
        {
            GameObject desiredCrosshair = Modules.Assets.LoadCrosshair("Standard");

            if (this.characterBody.skillLocator.secondary.skillDef.skillNameToken == ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_SECONDARY_STINGER_NAME")
            {
                desiredCrosshair = Modules.Assets.LoadCrosshair("SimpleDot");
            }

            /*if (this.hasBazookaReady)
            {
                desiredCrosshair = Modules.Assets.bazookaCrosshair;
            }*/

            this.characterBody.crosshairPrefab = desiredCrosshair;
        }
    }
}