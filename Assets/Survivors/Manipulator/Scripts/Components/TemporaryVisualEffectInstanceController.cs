using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;

namespace ManipulatorMod.Modules.Components
{
    [RequireComponent(typeof(TemporaryVisualEffect))]
    class TemporaryVisualEffectInstanceController : MonoBehaviour
    {
        public float duration;
        public TemporaryVisualEffect temporaryVisualEffect;

        private float timer;

        public void Awake()
        {
            this.temporaryVisualEffect = this.gameObject.GetComponent<TemporaryVisualEffect>();
        }

        public void Start()
        {
            this.timer = this.duration;
        }

        public void LateUpdate()
        {
            if (!this.temporaryVisualEffect)
            {
                return;
            }
            if (this.temporaryVisualEffect.visualState == TemporaryVisualEffect.VisualState.Enter)
            {
                this.timer -= Time.fixedDeltaTime;
                if (this.timer <= 0f)
                {
                    this.temporaryVisualEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
                    this.timer = 0f;
                }
            }
            if (!this.temporaryVisualEffect.GetComponent<DestroyOnTimer>() && this.temporaryVisualEffect.visualState == TemporaryVisualEffect.VisualState.Exit)
            {
                Debug.LogWarning(this.gameObject + ": TemporaryVisualEffectInstanceController: GameObject does not have a DestroyOnTimer, destroying instantly!");
                UnityEngine.GameObject.Destroy(this.gameObject);
            }
        }
    }
}
