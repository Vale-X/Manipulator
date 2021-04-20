using RoR2;
using RoR2.Skills;
using R2API.Utils;
using UnityEngine;
using ManipulatorMod.Modules.Misc;

namespace ManipulatorMod.Modules.Components
{
    // just a class to run some custom code for things like weapon models
    public class ManipulatorController : MonoBehaviour
    {
        //public bool hasBazookaReady;

        public bool attackReset;
        public bool gotStartingElement;

        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;
        public ManipulatorTracker tracker;
        private Animator modelAnimator;
        private SkillLocator skillLocator;

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
            //this.hasBazookaReady = false;

            //Invoke("CheckWeapon", 0.2f);
        }

        public void Start()
        {
            this.currentElement = GetStartingElement();
            this.SetElementSkillIcons(this.currentElement);
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