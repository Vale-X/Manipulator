using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace ManipulatorMod.Modules.Components
{
    [RequireComponent(typeof(CharacterBody))]
    class ManipulatorElementController : NetworkBehaviour
    {
        private void Awake()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
        }

        private CharacterBody characterBody;
        public GenericSkill elementSkill;
    }
}
