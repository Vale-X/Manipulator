using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    class ProjectileInflictTemporaryVisualEffect : MonoBehaviour, IOnDamageInflictedServerReceiver
    {
        //public TemporaryVisualEffect temporaryVisualEffect;
        public GameObject temporaryVisualEffectPrefab;
        public float duration = 1;
        [Tooltip("End all effects early when projectile is destroyed?")]
        public bool endEffectsOnDestroy;
        [Tooltip("Use CharacterBody's bestFitRadius instead of radius?")]
        public bool useBestFitRadius;
        [Tooltip("Can multiple of the same TemporaryVisualEffect be applied to the same target?")]
        public bool canStack;
        [Tooltip("If the TVE already exists on the target, reset the duration of the TVE?")]
        public bool refreshDuration;
        [Tooltip("If the TVE already exists on the target, add the duration onto it?")]
        public bool addDuration;

        private List<TemporaryVisualEffect> visualEffects = new List<TemporaryVisualEffect>();
        private static Dictionary<TemporaryVisualEffect, TemporaryVisualEffectInstanceController> visualControllers = new Dictionary<TemporaryVisualEffect, TemporaryVisualEffectInstanceController>();

        public void OnDamageInflictedServer(DamageReport damageReport)
        {
            // Clean up the dictionary a bit.
            foreach (KeyValuePair<TemporaryVisualEffect, TemporaryVisualEffectInstanceController> entry in visualControllers)
            {
                if (entry.Key == null || entry.Value == null)
                {
                    visualControllers.Remove(entry.Key);
                }
            }
            CharacterBody victim = damageReport.victimBody;
            if (victim)
            {
                this.AddTVE(damageReport.victimBody, this.duration);
            }
        }

        private void AddTVE(CharacterBody target, float duration)
        {
            foreach (TemporaryVisualEffect effect in visualEffects)
            {
                if (effect != null)
                {
                    if (effect.parentTransform == target.coreTransform)
                    {
                        visualControllers.TryGetValue(effect, out TemporaryVisualEffectInstanceController foundController);
                        if (addDuration)
                        {
                            foundController.AddDuration(duration);
                        }
                        if (refreshDuration)
                        {
                            foundController.RefreshDuration();
                        }
                        if (!canStack)
                        {
                            return;
                        }
                    }
                }
            }
            GameObject effectObject = UnityEngine.GameObject.Instantiate<GameObject>(temporaryVisualEffectPrefab, target.corePosition, Quaternion.identity);
            if (effectObject)
            {
                var tempEffect = effectObject.GetComponent<TemporaryVisualEffect>();
                if (!tempEffect)
                {
                    Debug.LogError(base.gameObject + ": ProjectileTemporaryVisualEffect: No TemporaryVisualEffect inside TemporaryVisualEffectPrefab.");
                    UnityEngine.GameObject.Destroy(effectObject);
                    return;
                }
                tempEffect.parentTransform = target.coreTransform;
                tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
                tempEffect.healthComponent = target.healthComponent;
                tempEffect.radius = this.useBestFitRadius ? target.bestFitRadius : target.radius;
                var localCameraEffect = gameObject.GetComponent<LocalCameraEffect>();
                if (localCameraEffect)
                {
                    localCameraEffect.targetCharacter = target.gameObject;
                }
                var effectController = effectObject.AddComponent<TemporaryVisualEffectInstanceController>();
                effectController.startDuration = duration;
                effectController.temporaryVisualEffect = tempEffect;
                this.visualEffects.Add(tempEffect);
                visualControllers.Add(tempEffect, effectController);
            }
            else { Debug.LogError(base.gameObject + ": ProjectileTemporaryVisualEffect: Unable to Instantiate TemporaryVisualEffectPrefab."); return; }
        }

        public void OnDestroy()
        {
            if (this.endEffectsOnDestroy)
            {
                foreach (TemporaryVisualEffect effect in visualEffects)
                {
                    if (effect != null)
                    {
                        effect.visualState = TemporaryVisualEffect.VisualState.Exit;
                    }
                }
            }
        }
    }
}
