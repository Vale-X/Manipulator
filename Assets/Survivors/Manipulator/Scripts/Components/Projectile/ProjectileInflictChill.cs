using UnityEngine;
using RoR2;

namespace ManipulatorMod.Modules.Components
{
    class ProjectileInflictChill : MonoBehaviour, IOnDamageInflictedServerReceiver
    {
        public void OnDamageInflictedServer(DamageReport damageReport)
        {
            CharacterBody victim = damageReport.victimBody;
            if (victim)
            {
                if (!victim.HasBuff(Buffs.chillCooldown))
                {
                    victim.AddTimedBuff(Buffs.chillDebuff, StaticValues.chillDebuffDuration);
                }
            }
        }
    }
}
