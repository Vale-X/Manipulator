using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using R2API;
using ManipulatorMod.Modules;

namespace ManipulatorMod.Modules.Components
{
    class ProjectileInflictDot : MonoBehaviour, IOnDamageInflictedServerReceiver
    {
        public DotController.DotIndex dotIndex;
        public string dotName;
        public float duration; 
        public float damageCoefficient;

        public void OnDamageInflictedServer(DamageReport damageReport)
        {
            CharacterBody victim = damageReport.victimBody;
            if (victim)
            {
                InflictDotInfo inflictDotInfo = default(InflictDotInfo);
                inflictDotInfo.dotIndex = String.IsNullOrEmpty(dotName) ? dotIndex : FindDot(dotName);
                inflictDotInfo.attackerObject = damageReport.attacker;
                inflictDotInfo.damageMultiplier = this.damageCoefficient;
                inflictDotInfo.duration = this.duration;
                inflictDotInfo.victimObject = damageReport.victimBody.gameObject;
            }
        }

        public DotController.DotIndex FindDot(string name)
        {
            DotController.DotIndex returnIndex = DotController.DotIndex.None;
            foreach (DotController.DotIndex dotIndex in (DotController.DotIndex[]) Enum.GetValues(typeof(DotController.DotIndex)))
            {
                if (dotIndex.ToString() == name)
                {
                    returnIndex = dotIndex;
                }
            }
            
            return returnIndex;
        }
    }
}
