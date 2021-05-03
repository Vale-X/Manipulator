using System;
using UnityEngine;
using RoR2;
using EntityStates;

namespace ManipulatorMod.SkillStates
{
	// Token: 0x02000A9B RID: 2715
	public class ManipulatorJetpack : BaseState
	{
		public static float hoverVelocity = StatValues.jetVelocity;
		public static float hoverAcceleration = StatValues.jetAcceleration;
		private Transform jetOnEffect;
		private ChildLocator childLocator;

		public override void OnEnter()
		{
			base.OnEnter();

			this.jetOnEffect = base.FindModelChild("JetHolder");
			if (this.jetOnEffect)
			{
				this.jetOnEffect.gameObject.SetActive(true);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				float num = base.characterMotor.velocity.y;
				num = Mathf.MoveTowards(num, ManipulatorJetpack.hoverVelocity, ManipulatorJetpack.hoverAcceleration * Time.fixedDeltaTime);
				base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
			}
		}

		public override void OnExit()
		{
			base.OnExit();

			if (this.jetOnEffect)
			{
				this.jetOnEffect.gameObject.SetActive(false);
			}
		}
	}
}
