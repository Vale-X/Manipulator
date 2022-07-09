using RoR2;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    [RequireComponent(typeof(TemporaryVisualEffect))]
    class TemporaryVisualEffectInstanceController : MonoBehaviour
    {
        public float startDuration;
        public TemporaryVisualEffect temporaryVisualEffect;

        private float timer;

        public void Awake()
        {
        }

        public void Start()
        {
            this.timer = this.startDuration;
            if (temporaryVisualEffect) Debug.LogWarning("TVEIC: No TVE linked!");
        }

        public void RefreshDuration()
        {
            this.timer = this.startDuration;
        }

        public void AddDuration(float addDuration)
        {
            this.timer += addDuration;
        }

        public void LateUpdate()
        {
            if (!this.temporaryVisualEffect)
            {
                Destroy(this);
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
