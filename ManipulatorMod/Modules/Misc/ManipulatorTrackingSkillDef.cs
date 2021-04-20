using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.Modules.Misc
{
    public class ManipulatorTrackingSkillDef : SkillDef
    {
        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new ManipulatorTrackingSkillDef.InstanceData
            {
                manipulatorTracker = skillSlot.GetComponent<ManipulatorTracker>()
            };
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            ManipulatorTracker manipulatorTracker = ((ManipulatorTrackingSkillDef.InstanceData)skillSlot.skillInstanceData).manipulatorTracker;
            return (manipulatorTracker != null) ? manipulatorTracker.GetTrackingTarget() : null;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return ManipulatorTrackingSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && ManipulatorTrackingSkillDef.HasTarget(skillSlot);
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public ManipulatorTracker manipulatorTracker;
        }
    }
}