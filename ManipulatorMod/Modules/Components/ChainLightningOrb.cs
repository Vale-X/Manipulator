using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    public class ChainLightningOrb : Orb
    {
        public GameObject orbTarget;
        public int orbIndex;
        public float orbOverrideDuration = 0.3f;

        public override void Begin()
        {
            if (orbTarget)
            {
                duration = orbOverrideDuration;
                EffectData effectData = new EffectData
                {
                    scale = 1,
                    origin = this.origin,
                    genericFloat = base.duration,
                    rootObject = orbTarget
                };
                //effectData.SetChildLocatorTransformReference(Target, Index);
                EffectManager.SpawnEffect(Orbs.chainLightningOrb, effectData, false);
            }    
        }
    }
}
