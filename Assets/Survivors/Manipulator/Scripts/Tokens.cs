using R2API;
using System;

namespace ManipulatorMod.Modules
{
    internal static class Tokens
    {
        // Used everywhere within tokens. Format is DeveloperPrefix + BodyPrefix + unique per token
        // A full example token for ThunderHenry would be ROBVALE_THUNDERHENRY_BODY_UNLOCK_SURVIVOR_NAME.
        internal const string maniPrefix = "_MANIPULATOR_BODY_";
        internal const string prefix = ManipulatorPlugin.developerPrefix + maniPrefix;

        internal static void Init()
        {
            #region Manipulator
            #region info

            string desc = "Manipulator can swap between elements to adapt to the situation, using the power of Fire, Lightning or Ice.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > ACE Pack grants a speed buff. Use this to get close to enemies and deal additional damage with Cross." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Each Invoke is valuable for different situations. Ardent Ring is best to destroy larger targets. Surge is great for clearing out groups of enemies. Cryospire can keep grouped threats at bay." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Venting step teleports you in the direction you're travelling, not facing, allowing you to retreat or advanced with ease." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > While out of danger, consume all three charges of Conduit Cycle to fully empower your next Invoke or Venting Step." + Environment.NewLine + Environment.NewLine;

            string outro = "...and so he left, on order of the court.";
            string outroFailure = "..and so he vanished, scrubbed from the records.";

            string lore ="<style=cStack>/-- BY ORDER OF: HIGH COURT CARDINALS, UNANIMOUS: THIS ACT IS MARKED AS ></style><style=cDeath>HIGHLY RESTRICTED</style><style=cStack>< --/</style>\n" +
                "ACT NO.: 6,421\n" +
                "TYPE: RECOVERY/RESCUE\n" +
                "COMMISSIONER: House Beyond, Cardinal: [REDACTED]\n" +
                "CONTRACTED: House Advent, Manipulator: [REDACTED]\n" +
                "TARGET: House Beyond, Artificer Expeditionary Squad: High Haven\n" +
                "DATED: [REDACTED]\n" +
                "\n" + 
                "ACT NOTES:\n" +
                "Target last transmitted location is corrupted, presumed interference.\n" +
                "Cooperation with UESC authorized, ship designation 'UES Safe Travels'.\n" +
                "\n" + 
                "COM NOTES:\n" +
                "Black Box's last transmission has critical data recorded in Zone 5, planet PETRICHOR V. System local bodies marked as INTEREST. Minimum requirement is target's Black Box. Prioritise data over bodies. Last Auto Log Report is as follows:\n" +
                "\n <style=cStack>" +
                ">- HC SYSTEM: INCOMING REPORT: #ABB-A1839-E\n" +
                ">- ENV AUTO LOG: EMERGENCY SITUATION LOG - SIGNAL STATUS: LONG-DISTANCE BROADCASTING\n" +
                ">- ENV AUTO LOG: LOCATION DATA CORRUPTED. SCENARIO ANALYSIS INITIATED:\n" +
                ">- ENV AUTO LOG: UNRECOGNISED MATTER COMPOSITION - ANALYSIS REQUIRED\n" +
                ">- ENV AUTO LOG: VITAL SYSTEM REPORT: CRITICAL CONDITION: 1\n" +
                ">- ENV AUTO LOG: DANGER: HIGH ENERGY FUSION DETECTED\n" +
                ">- ENV AUTO LOG: VITAL SYSTEM REPORT: CRITICAL CONDITION: 3\n" +
                ">- ENV AUTO LOG: WARNING: DISRUPTION OF LOCAL MATTER SPACETIME DETECTED\n" +
                ">- ENV AUTO LOG: WARNING: UNKNOWN ENTITY DETECTED\n" +
                ">- ENV AUTO LOG: DANGER: UNIDENTIFIED CRYSTALLINE HAZARD DETECTED\n" +
                ">- ENV AUTO LOG: VITAL SYSTEM REPORT: LOST CONNECTION: 2\n" +
                ">- ENV AUTO LOG: ANALYSIS COMPLETE: IMMEDIATE CARDINAL ACTION RE-\n" +
                ">- HC SYSTEM: SIGNAL LOST. LOG STATUS: EMERGENCY.</style>\n" +
                "\n" +
                "Recover the data. Consequence on failure.";

            // I wish there were ways to add miscellanious lore to the game cause I wanna write more stuff.
            // Considering making an item just to add more lore lmao
            string itemLore = "The House Beyond Black Box barely holds itself together, plating and connections so loosened over degradation and damage. The information inside is even less stable. Fragmented log pieces and corrupted data are difficult to work with, but one who is technically minded would be able to barely piece together what this box has witnessed. The High Court's advanced intelligence systems, however, are more than sufficient.\n\n" +
                "<style=cStack>" +
                ">- HC SYSTEM: ATTEMPTING ACCESS: ABB-A1839\n" +
                ">- HC SYSTEM: .........\n" + 
                ">- HC SYSTEM: DATA FALIURE. BEGINNING DEFRAG PROCEDURES\n";
            #endregion

            LanguageAPI.Add(prefix + "NAME", "Manipulator");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "On Court's Order");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Prototype");
            LanguageAPI.Add(prefix + "NYA_SKIN_NAME", "Nyapulator");
            #endregion

            // ColorRefs
            // <color=#ec8122> = Fire
            // <color=#98509f> = Lightning
            // <color=#35bbe2> = Ice

            #region Keywords
            LanguageAPI.Add("KEYWORD_MANI_FIRESPELL", $"<color=#ec8122><style=cKeywordName>Ardent Ring</style></color><style=cSub><color=#ec8122>Fire Invoke</color>. Launch a flaming ring, dealing <style=cIsDamage>{100f * StaticValues.fireSpellDamageCoefficient}% damage</style> and <style=cIsUtility>attaching</style>, dealing <style=cIsDamage>{StaticValues.fireAttachTickMax}x{100f * StaticValues.fireTickDamageCoefficient}% damage</style> over time.");
            LanguageAPI.Add("KEYWORD_MANI_LIGHTNINGSPELL", $"<color=#98509f><style=cKeywordName>Surge</style><style=cSub><color=#98509f>Lightning Invoke</color>. Discharge a bolt that <style=cIsUtility>chains to up to {StaticValues.lightningBounceCount} targets</style>, dealing <style=cIsDamage>{100f * StaticValues.lightningDamageCoefficient}% damage</style> to each.");
            LanguageAPI.Add("KEYWORD_MANI_ICESPELL", $"<color=#35bbe2><style=cKeywordName>Cryospire</style></color><style=cSub><color=#35bbe2>Ice Invoke</color>. <style=cIsUtility>Freezing.</style> Create a pillar that deals <style=cIsDamage>{100f * StaticValues.icePillarDamageCoefficient}% damage</style>. The pillar then shatters, dealing <style=cIsDamage>{100f * StaticValues.iceExplosionDamageCoefficient}% damage</style>.");

            LanguageAPI.Add("KEYWORD_MANI_FIREEFFECT", $"<color=#ec8122><style=cKeywordName>Fire</style></color><style=cSub><style=cIsDamage>Ignite enemies</style> on hit.");
            LanguageAPI.Add("KEYWORD_MANI_LIGHTNINGEFFECT", $"<color=#98509f><style=cKeywordName>Lightning</style></color><style=cSub>Create a <style=cIsDamage>burst of lightning</style>.");
            LanguageAPI.Add("KEYWORD_MANI_ICEEFFECT", $"<color=#35bbe2><style=cKeywordName>Ice</style></color><style=cSub>A <style=cIsUtility>stacking Slow on enemies</style> that <style=cIsUtility>freezes at maximum stacks</style>.");

            LanguageAPI.Add("KEYWORD_MANI_FIREBONUS", $"<color=#ec8122><style=cKeywordName>Fire</style></color><style=cSub><style=cIsDamage>Increases damage.</style>");
            LanguageAPI.Add("KEYWORD_MANI_LIGHTNINGBONUS", $"<color=#98509f><style=cKeywordName>Lightning</style></color><style=cSub><style=cIsUtility>Reduces cooldown</style> for each enemy hit.");
            LanguageAPI.Add("KEYWORD_MANI_ICEBONUS", $"<color=#35bbe2><style=cKeywordName>Ice</style></color><style=cSub><style=cIsHealing>Temporary barrier</style> for each enemy hit.");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "ACE Pack");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Holding the Jump key causes Manipulator to <style=cIsUtility>hover in the air</style> for a short duration.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Cross");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Slice forward for <style=cIsDamage>{100f * StaticValues.crossDamageCoefficient}% damage</style>. Fire a wave <style=cIsUtility>based on the current element</style>, dealing up to <style=cIsDamage>{100f * StaticValues.waveDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_NAME", "Invoke");
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_DESCRIPTION", $"Cast a different ability determined by the current element: <color=#ec8122>Ardent Ring</color>, <color=#98509f>Surge</color> or <color=#35bbe2>Cryospire</color>.");

            LanguageAPI.Add(prefix + "SECONDARY_SPELL_FIRE_NAME", "Ardent Ring");
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_FIRE_DESCRIPTION", $"Launch a flaming ring, dealing <style=cIsDamage>{100f * StaticValues.fireSpellDamageCoefficient}% damage</style> and <style=cIsUtility>attaching</style> to the first target hit. The ring then deals <style=cIsDamage>{StaticValues.fireAttachTickMax}x{100f * StaticValues.fireTickDamageCoefficient}% damage</style> over time to the attached target.");

            LanguageAPI.Add(prefix + "SECONDARY_SPELL_LIGHTNING_NAME", "Surge");
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_LIGHTNING_DESCRIPTION", $"Discharge a bolt of lightning that <style=cIsUtility>chains to up to {StaticValues.lightningBounceCount} targets</style>, dealing <style=cIsDamage>{100f * StaticValues.lightningDamageCoefficient}% damage</style> to each.");

            LanguageAPI.Add(prefix + "SECONDARY_SPELL_ICE_NAME", "Cryospire");
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_ICE_DESCRIPTION", $"<style=cIsUtility>Freezing.</style> Create a pillar that deals <style=cIsDamage>{100f * StaticValues.icePillarDamageCoefficient}% damage</style> to nearby enemies. The pillar then shatters, dealing <style=cIsDamage>{100f * StaticValues.icePillarDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_BLINK_NAME", "Venting Step");
            LanguageAPI.Add(prefix + "UTILITY_BLINK_DESCRIPTION", $"<style=cIsUtility>Teleport</style>, leaving behind a burst <style=cIsUtility>based on the current element</style> that deals <style=cIsDamage>{100f * StaticValues.explosionDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "UTILITY_JUMP_NAME", "Vented Rise");
            LanguageAPI.Add(prefix + "UTILITY_JUMP_DESCRIPTION", $"Launch into the air with a burst <style=cIsUtility>based on the current element</style> that deals <style=cIsDamage>{100f * StaticValues.jumpDamageCoefficient}% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SWITCH_NAME", "Conduit Cycle");
            LanguageAPI.Add(prefix + "SPECIAL_SWITCH_DESCRIPTION", $"<style=cIsUtility>Cycle to the next element</style>: <color=#ec8122>Fire</color>, <color=#98509f>Lightning</color> or <color=#35bbe2>Ice</color>. Grant a <style=cIsUtility>bonus effect</style> to the <style=cIsUtility>next Secondary or Utility cast</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_OVERLOAD_NAME", "Overload");
            LanguageAPI.Add(prefix + "SPECIAL_OVERLOAD_DESCRIPTION", $"<style=cIsUtility>Cycle to the next element</style>: <color=#ec8122>Fire</color>, <color=#98509f>Lightning</color> or <color=#35bbe2>Ice</color>. Overload your ACE Pack, releasing an <style=cIsDamage>explosion</style> that deals <style=cIsDamage>{100f * StaticValues.burstDamageCoefficient}% damage</style>.");
            #endregion

            #region Elements
            LanguageAPI.Add(prefix + "ELEMENT_FIRE_NAME", "Initiate: Fire");
            LanguageAPI.Add(prefix + "ELEMENT_FIRE_DESCRIPTION", $"Begin stages attuned to <color=#ec8122>Fire</color>.");

            LanguageAPI.Add(prefix + "ELEMENT_LIGHTNING_NAME", "Initiate: Lightning");
            LanguageAPI.Add(prefix + "ELEMENT_LIGHTNING_DESCRIPTION", $"Begin stages attuned to <color=#98509f>Lightning</color>.");

            LanguageAPI.Add(prefix + "ELEMENT_ICE_NAME", "Initiate: Ice");
            LanguageAPI.Add(prefix + "ELEMENT_ICE_DESCRIPTION", $"Begin stages attuned to <color=#35bbe2>Ice</color>.");
            #endregion

            #region Buffs
            if (ModCompatibility.BetterUICompat.betterUIInstalled)
            {
                //LanguageAPI.Add(prefix + "BUFF_", "");
                LanguageAPI.Add(prefix + "BUFF_CHILL_NAME", "Chill");
                LanguageAPI.Add(prefix + "BUFF_CHILL_DESC", "Chilled, slowed for each stack. At maximum stacks, freeze.");
                LanguageAPI.Add(prefix + "BUFF_CHILLCD_NAME", "Chill Cooldown");
                LanguageAPI.Add(prefix + "BUFF_CHILLCD_DESC", "Frozen by Chill, and now immune to Chill for a short while.");
                LanguageAPI.Add(prefix + "BUFF_JETPACK_NAME", "ACE Efficiency");
                LanguageAPI.Add(prefix + "BUFF_JETPACK_DESC", "The ACE Pack's aerial booster increases movement speed while hovering..");
                LanguageAPI.Add(prefix + "BUFF_OVERLOAD_NAME", "Overloaded");
                LanguageAPI.Add(prefix + "BUFF_OVERLOAD_DESC", "Pushing the ACE Pack past it's limits, increasing Cross's melee damage but disabling the wave.");
                LanguageAPI.Add(prefix + "BUFF_FIRE_NAME", "Fire Conduit");
                LanguageAPI.Add(prefix + "BUFF_FIRE_DESC", "Enhances the next Secondary or Utility with additional damage.");
                LanguageAPI.Add(prefix + "BUFF_LIGHTNING_NAME", "Lightning Conduit");
                LanguageAPI.Add(prefix + "BUFF_LIGHTNING_DESC", "Enhances the next Secondary or Utility with cooldown per hit.");
                LanguageAPI.Add(prefix + "BUFF_ICE_NAME", "Ice Conduit");
                LanguageAPI.Add(prefix + "BUFF_ICE_DESC", "Enhances the next Secondary or Utility with barrier per hit.");
            }
            #endregion

            #region Items
            /*LanguageAPI.Add(prefix + "ITEM_BLACKBOX_NAME", "Black Box");
            LanguageAPI.Add(prefix + "ITEM_BLACKBOX_PICKUP", "Gain X upon gaining a new Buff.");
            LanguageAPI.Add(prefix + "ITEM_BLACKBOX_DESC", "");
            LanguageAPI.Add(prefix + "ITEM_BLACKBOX_LORE", itemLore);*/
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCK_SURVIVOR_NAME", "By Act of The Court");
            LanguageAPI.Add(prefix + "UNLOCK_SURVIVOR_DESC", "Recover the data from the lost House Beyond Black Box.");
            LanguageAPI.Add(prefix + "UNLOCK_SURVIVOR_UNLOCKABLE_NAME", "By Act of The Court");

            /*LanguageAPI.Add(prefix + "UNLOCK_SURVIVOR_NAME2", "Driven for the Heavens");
            LanguageAPI.Add(prefix + "UNLOCK_SURVIVOR_DESC2", "As Artificer, challenge the lone king.");
            LanguageAPI.Add(prefix + "UNLOCK_SURVIVOR_UNLOCKABLE_NAME2", "Driven for the Heavens");*/

            LanguageAPI.Add(prefix + "UNLOCK_MASTERY_NAME", "Manipulator: Mastery");
            LanguageAPI.Add(prefix + "UNLOCK_MASTERY_DESC", "As Manipulator, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "UNLOCK_MASTERY_UNLOCKABLE_NAME", "Manipulator: Mastery");

            LanguageAPI.Add(prefix + "UNLOCK_OVERLOAD_NAME", "Manipulator: Sheer Commitment");
            LanguageAPI.Add(prefix + "UNLOCK_OVERLOAD_DESC", "As Manipulator, complete a stage without changing Element.");
            LanguageAPI.Add(prefix + "UNLOCK_OVERLOAD_UNLOCKABLE_NAME", "Manipulator: Sheer Commitment");

            LanguageAPI.Add(prefix + "UNLOCK_JUMP_NAME", "Manipulator: Venting Frustrations");
            LanguageAPI.Add(prefix + "UNLOCK_JUMP_DESC", "As Manipulator, defeat 5 enemies at once with Venting Step.");
            LanguageAPI.Add(prefix + "UNLOCK_JUMP_UNLOCKABLE_NAME", "Manipulator: Venting Frustrations");

            #region shhh
            LanguageAPI.Add(prefix + "UNLOCK_NYA_NAME", "Manipulator: Nyascended");
            LanguageAPI.Add(prefix + "UNLOCK_NYA_DESC", "Discover the nyasterious figure.");
            LanguageAPI.Add(prefix + "UNLOCK_NYA_UNLOCKABLE_NAME", "Manipulator: Nyascended");
            #endregion
            #endregion

            #region misc
            LanguageAPI.Add(ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BLACKBOX_CONTEXT", "Recover data");
            #endregion

            #endregion
        }
    }
}