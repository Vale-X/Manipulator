# ManipulatorMod
A modded Risk of Rain 2 survivor, Manipulator.

This github project is currently a complete _mess_. So is the code. Please know that I do plan to go through everything and fix it up, remove anything unecessary and just make it nice. Manipulator is based on the [HenryMod by rob](https://thunderstore.io/package/rob/HenryMod/), and because of that there's a lot of leftover code from him in there - especially in the unity project! Thanks a ton to rob for the HenryMod, I never would have made this project without it.

## Extended Changelog
This changelog is an extended version of the one found on the store, as to keep out the minor changes.

  `0.3.3`
:   - __Added Passive Icon. Other icons in the works.__
- __Added vanilla item displays (might do modded items in the future).__
- __Changes to skills:__
    - Reduced maximum Chill (Ice debuff) stacks from 10 to 8.
	- Lightning Cross's (Lightning Primary) lightning explosion now has the damage falloff of the wave.
- __Changes to stats:__
    - Actually increased armor from 0 to 12.

  `0.3.2`
  
- __Added ACE Pack (Passive).__
__Added element based materials on the model (Thanks KomradeSpectre for helping set this up!).__
- __Changes to skills:__
	- Ice's effect (for Primary and Utility) has been updated. Now a stacking slow that freezes upon reaching maximum stacks.
    - Cross (Primary) is now Agile (animation system has been remade, so doesn't look as bad anymore).
	- Cross's (Primary) wave now has damage falloff down to half damage (150%-75%).
	- Increased Lightning Cross's (Lightning Primary) lightning burst radius from 2 to 2.5.
	- Removed Venting Step's (Utility) damage falloff.
	- *Developer Notes: People have been saying that they want him to be more melee, and are relying too much on the wave. I'm adding damage falloff to the wave, and increasing armor to MUL-T levels to make melee combat a bit more forgiving.*
- __Changes to stats:__
    - Increased Armor from 0 to 12.
- __Fixes:__
    - Fixed: Lightning Cross (Lightning Primary) was doing less damage than intended.
	- Fixed: Venting Step (Utility) was doing more damage than intended.
	- Fixed: Lightning Ventiny Step (Utility) bonus was reducing the cooldown of Invoke (Secondary) instead of Venting Step (Utility).

  `0.3.1`
  
- __Added some animations, tweaked others (still WIP).__
- __Updated loadout skin icon.__
- __Removed Henry related content from the asset bundle.__
- __Disabled item displays (unintentionally was using Henry's item display rules).__
- __Changes to skills:__
    - Increased Cross's (Primary) Melee attack size.
    - Decreased Cross's (Primary) wave damage from 200% to 150%.
	- Increased Cross's (Primary) wave speed from 80 to 100.
	- Cross projectiles are now rotated to match the slash animation.
	- Decreased Venting Step's (Utility) cooldown from 8 to 6.
	- Decreased venting Step's (Utility) radius from 10 to 5 (Lighting blast radius is still 5).
- __Fixes:__
    - Fixed: Could start sprinting while using Cross (Primary). Reported by: HaalfNHaalf.
	- Fixed: Cryospire refunded utility instead of secondary on miss. Reported by: HaalfNHaalf.

  `0.3.0`
  
- __Added melee attack animations (still WIP).__
- __Added body collisions and ragdoll components.__
- __Changes to skills:__
    - Increased Lightning Cross's (Primary) wave proc coefficient from 0 to 1.
	- Set Lightning Cross's (Primary) secondary blast proc coefficient to 0.
    - Increased Ardent Ring's (Fire Secondary) initial damage from 400% to 600%.
	- Decreased Ardent Ring's (Fire Secondary) attach proc coefficient from 1 to 0.4.
	- Decreased Ardent Ring's (Fire Secondary) attach tick count from 6 to 5.
    - Increased Surge's (Lightning Secondary) bounce range from 25 to 35.
	- Increased Surge's (Lightning Secondary) proc coeffient from 0 to 0.5.
	- Decreased Surge's (Lightning Secondary) damage from 400% to 300%.
	- Set Cryospire's (Ice Secondary) proc coefficient to 1.
	- Added Venting Step (Utility) elemental effects. The elemental effects are the same as Cross (Primary).
	- Flagged Venting Step (Utility) as a combat skill.
	- Updated element switching mechanics internally (no noticable difference in-game).
    - Removed *Agile* from Cross (Primary).
	- *Developer Notes: I'm doing a rough balance pass on Manipulator, giving his secondary (hopefully) more appropriate proc values and number changes. Cross is no longer Aglie to fit with animations better.*
- __Changes to stats:__
    - Decreased base health from 100 to 90.
    - Increased level health from 26 to 27.
    - Increased base regen from 1.25/s to 2.5/s.
    - Increase level regen from 0.2 to 0.5.
    - Reduced armor from 20 to 0.
    - *Developer Notes: Manipulator is effectively a Ranged/Melee hybrid, so I'm giving him stats to match that of melee and ranged vanilla survivors. Higher regen (like melee) but lower health (like ranged). Armor is removed as for now, as melee is meant to be risk-reward for him.*
- __Fixes:__
    - Fixed: 'Cross's (Primary) melee attack is missing.'
    - Fixed: 'Cross's (Primary) wave doesn't spawn if attack speed is too high. Reported by: Clom'
	- Fixed: Holding down Secondary fired Ardent Ring (Fire Secondary) quickly if Backup Magazines were held.
    - Fixed: 'Surge (Lightning Secondary) cannot be used after using Venting Step (Utility) while attuned to Lightning.'
    - Fixed: 'Surge's (Lightning Secondary) targeting indicator doesn't hide correctly.'
    - Fixed: 'Surge (Lightning Secondary) may not fire correctly if cast shorty after (or during) *ECE Cycle* (Special). Reported by: Jame'
	- Fixed: Icon reset occurs if you press 'Quit to Menu' but don't quit.

  `0.2.1`
  
- __Updated for the latest RoR2 update (v1.1.1.2).__

  `0.2.0`

- __Updated for the Anniversary Update.__
- __Changed thunderstore team name to ValeX.__
- __Added custom model. Animations are still a WIP.__
- __Added custom survivor icon.__
- __Added initial element selection in loadout.__
- __Changes to skills:__
    - Increased delay for Lightning Cross (Primary) explosion. Suggested by Jame.
    - Increased delay for Venting Step (Utility) explosion.
    - Decreased proc coefficient for Lightning Cross (Primary) from 1 to 0.
    - Increased lifetime of Cross (Primary) wave from 0.7s to 1s.
    - Changed Lightning Cross (Primary) burst VFX (still a temporary effect).
- __Fixes:__ 
  - Fixed: 'Current element icons don't reset after quitting to menu.'

  `0.1.2`
  
- __Removed 'unlock' achievement (temporarily, until a proper achievement is made).__

  `0.1.1`
  
- __Added tips.__
- __Fixes:__
    - Fixed: Artificer's Pillars in Ice Wall being the same size as Manipulator's Cryospire.

  `0.1.0`
  
- __Initial release.__
