using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2.Skills;
using EntityStates;
using ManipulatorMod.Modules.Components;
using JetBrains.Annotations;
using RoR2;
using UnityEngine;
using HG;

namespace ManipulatorMod.Modules.Scriptables
{
    [CreateAssetMenu(menuName = "RoR2/SkillDef/ElementalSkillDef")]
    class ElementalSkillDef : SkillDef
    {
        public bool shouldUseTracker;
        public ManipulatorController.Element trackerElement;
        private bool useTracker;

        public Sprite[] icons;
        public SerializableEntityStateType[] elementalEntityStates;
        public bool useElementalTokens = false;
        public string[] elementalNames;
        public string[] elementalDescriptions;

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new ElementalSkillDef.InstanceData
            {
                manipulatorTracker = skillSlot.GetComponent<ManipulatorTracker>()
            };
        }
        public override void OnUnassigned([NotNull] GenericSkill skillSlot)
        {
            base.OnUnassigned(skillSlot);
        }
        public override string GetCurrentNameToken([NotNull] GenericSkill skillSlot)
        {
            if (!this.useElementalTokens) return base.GetCurrentNameToken(skillSlot);

            ElementalSkillDef.InstanceData instanceData = (ElementalSkillDef.InstanceData)skillSlot.skillInstanceData;
            int index = (instanceData != null) ? (int)instanceData.currentElement : 0;
            return ArrayUtils.GetSafe<string>(this.elementalNames, index);
        }

        public override string GetCurrentDescriptionToken([NotNull] GenericSkill skillSlot)
        {
            if (!this.useElementalTokens) return base.GetCurrentDescriptionToken(skillSlot);

            ElementalSkillDef.InstanceData instanceData = (ElementalSkillDef.InstanceData)skillSlot.skillInstanceData;
            int index = (instanceData != null) ? (int)instanceData.currentElement : 0;
            return ArrayUtils.GetSafe<string>(this.elementalDescriptions, index);
        }

        public override Sprite GetCurrentIcon([NotNull] GenericSkill skillSlot)
        {
            ElementalSkillDef.InstanceData instanceData = (ElementalSkillDef.InstanceData)skillSlot.skillInstanceData;
            int index = (instanceData != null) ? (int)instanceData.currentElement : 0;
            return ArrayUtils.GetSafe<Sprite>(this.icons, index);
        }

        private bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            if (useTracker)
            {
                ManipulatorTracker manipulatorTracker = ((ElementalSkillDef.InstanceData)skillSlot.skillInstanceData).manipulatorTracker;
                return (manipulatorTracker != null) ? manipulatorTracker.GetTrackingTarget() : false;
            }
            else return true;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            if (shouldUseTracker) { return this.HasTarget(skillSlot) && base.CanExecute(skillSlot); }
            else return base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            ElementalSkillDef.InstanceData instanceData = (ElementalSkillDef.InstanceData)skillSlot.skillInstanceData;
            if (instanceData.currentElement == trackerElement)
            {
                useTracker = true;
            }
            else useTracker = false;
            if (shouldUseTracker && useTracker) return base.IsReady(skillSlot) && this.HasTarget(skillSlot);
            else return base.IsReady(skillSlot);
        }
        public virtual ManipulatorController.Element SwitchElement([NotNull] GenericSkill skillSlot, [NotNull] ManipulatorController.Element inElement)
        {
            ElementalSkillDef.InstanceData instanceData = (ElementalSkillDef.InstanceData)skillSlot.skillInstanceData;
            instanceData.currentElement = inElement;
            if (this.elementalEntityStates.Length != 4) { return instanceData.currentElement; }
            int index = (instanceData != null) ? (int)instanceData.currentElement : 0;
            this.activationState = elementalEntityStates[index];
            return instanceData.currentElement;
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public ManipulatorController.Element currentElement;
            public ManipulatorTracker manipulatorTracker;
        }
    }
}
