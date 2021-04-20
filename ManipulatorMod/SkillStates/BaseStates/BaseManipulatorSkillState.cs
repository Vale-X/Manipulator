using EntityStates;
using ManipulatorMod.Modules.Components;

namespace ManipulatorMod.SkillStates.BaseStates
{
    public class BaseManipulatorSkillState : BaseSkillState
    {
        protected ManipulatorController manipulatorController;

        public override void OnEnter()
        {
            this.manipulatorController = base.GetComponent<ManipulatorController>();
            base.OnEnter();
        }
    }
}