using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipulatorMod.Modules;
using UnityEngine;
using EntityStates;

namespace ManipulatorMod.SkillStates
{
    class ManipulatorJetpack : BaseState
    {
        private Transform jetEffect;

        public override void OnEnter()
        {
            base.OnEnter();

            this.jetEffect = base.FindModelChild("JetHolder");
            if (this.jetEffect)
            {
                this.jetEffect.gameObject.SetActive(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                float velocityY = Mathf.MoveTowards(base.characterMotor.velocity.y, StaticValues.jetVelocity, StaticValues.jetAcceleration * Time.fixedDeltaTime);
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, velocityY, base.characterMotor.velocity.z);
            }
        }

        public override void OnExit()
        {
            if (this.jetEffect)
            {
                this.jetEffect.gameObject.SetActive(false);
            }

            base.OnExit();
        }
    }
}
