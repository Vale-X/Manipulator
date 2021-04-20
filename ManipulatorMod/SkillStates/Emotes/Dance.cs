namespace ManipulatorMod.SkillStates.Emotes
{
    public class Dance : BaseEmote
    {
        public override void OnEnter()
        {
            this.animString = "Dance";
            this.animDuration = 0.75f;
            this.soundString = "ManipulatorDance";
            base.OnEnter();
        }
    }
}