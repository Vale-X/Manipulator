using R2API;
using System;

namespace ManipulatorMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Manipulator
            string prefix = ManipulatorPlugin.developerPrefix + "_MANIPULATOR_BODY_";

            string desc = "Manipulator can swap between elements to adapt to the situation, using the power of Fire, Lightning or Ice.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Get close to enemies with Cross to deal additional damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Each invoke is valuable for different situations. Ardent Ring is best to destroy larger targets. Surge is great for clearing out groups of enemies. Cryospire can keep grouped threats at bay." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Venting step teleports you in the direction you're travelling, not facing, allowing you to retreat or advanced with ease." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > While out of danger, consume all three charges of the ECE Cycle to fully empower your next Invoke or Venting Step." + Environment.NewLine + Environment.NewLine;

            LanguageAPI.Add(prefix + "NAME", StatValues.characterName);
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", StatValues.characterSubtitle);
            LanguageAPI.Add(prefix + "LORE", StatValues.characterLore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", StatValues.characterOutro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", StatValues.characterOutroFaliure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Prototype");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "V1");
            LanguageAPI.Add(prefix + "CAT_SKIN_NAME", "Nyapulator");
            #endregion

            // ColorRefs
            // <color=#ec8122> = Fire
            // <color=#98509f> = Lightning
            // <color=#35bbe2> = Ice

            #region Keywords
            LanguageAPI.Add("KEYWORD_FIRESPELL", $"<color=#ec8122><style=cKeywordName>Ardent Ring</style></color><style=cSub><color=#ec8122>Fire Invoke</color>. Launch a flaming ring, dealing <style=cIsDamage>{100f * StatValues.fireSpellCoefficient}% damage</style> and <style=cIsUtility>attaching</style>, dealing <style=cIsDamage>{StatValues.fireAttachTickMax}x{100f * StatValues.fireAttachCoefficient}% damage</style> over time.");
            LanguageAPI.Add("KEYWORD_LIGHTNINGSPELL", $"<color=#98509f><style=cKeywordName>Surge</style><style=cSub><color=#98509f>Lightning Invoke</color>. Discharge a bolt that <style=cIsUtility>chains to up to {StatValues.lightningBounceCount} targets</style>, dealing <style=cIsDamage>{100f * StatValues.lightningCoefficient}% damage</style> to each.");
            LanguageAPI.Add("KEYWORD_ICESPELL", $"<color=#35bbe2><style=cKeywordName>Cryospire</style></color><style=cSub><color=#35bbe2>Ice Invoke</color>. Create a pillar that deals <style=cIsDamage>{100f * StatValues.icePillarExplosionCoefficient}% damage</style> and <style=cIsUtility>freeezes</style>. The pillar explodes, dealing <style=cIsDamage>{100f * StatValues.icePillarExplosionCoefficient}% damage</style> and <style=cIsUtility>freezing</style> again.");

            LanguageAPI.Add("KEYWORD_FIREEFFECT", $"<color=#ec8122><style=cKeywordName>Fire</style></color><style=cSub><style=cIsDamage>Ignite enemies</style> on hit");
            LanguageAPI.Add("KEYWORD_LIGHTNINGEFFECT", $"<color=#98509f><style=cKeywordName>Lightning</style></color><style=cSub>Create a <style=cIsDamage>burst of lightning</style>.");
            LanguageAPI.Add("KEYWORD_ICEEFFECT", $"<color=#35bbe2><style=cKeywordName>Ice</style></color><style=cSub><style=cIsUtility>Slow enemies</style> on hit.");

            LanguageAPI.Add("KEYWORD_FIREBONUS", $"<color=#ec8122><style=cKeywordName>Fire</style></color><style=cSub><style=cIsDamage>Increases damage.</style>");
            LanguageAPI.Add("KEYWORD_LIGHTNINGBONUS", $"<color=#98509f><style=cKeywordName>Lightning</style></color><style=cSub><style=cIsUtility>Reduces cooldown</style> for each enemy hit.");
            LanguageAPI.Add("KEYWORD_ICEBONUS", $"<color=#35bbe2><style=cKeywordName>Ice</style></color><style=cSub><style=cIsHealing>Temporary barrier</style> for each enemy hit.");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Manipulator passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "He dun' have one (yet?).");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Cross");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Slice forward for <style=cIsDamage>{100f * StatValues.attackDamageCoefficient}% damage</style>. Fire a wave <style=cIsUtility>based on the current element</style>, dealing <style=cIsDamage>{100f * StatValues.waveDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_NAME", "Invoke");
            LanguageAPI.Add(prefix + "SECONDARY_SPELL_DESCRIPTION", $"Cast a different ability determined by the current element: <color=#ec8122>Ardent Ring</color>, <color=#98509f>Surge</color> or <color=#35bbe2>Cryospire</color>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_VENT_NAME", "Venting Step");
            LanguageAPI.Add(prefix + "UTILITY_VENT_DESCRIPTION", $"<style=cIsUtility>Blink</style>, leaving behind a delayed burst <style=cIsUtility>based on the current element</style> that deals <style=cIsDamage>{100f * StatValues.explosionDamage}% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SWITCH_NAME", "ECE Cycle");
            LanguageAPI.Add(prefix + "SPECIAL_SWITCH_DESCRIPTION", $"Cycle to the next element: <color=#ec8122>Fire</color>, <color=#98509f>Lightning</color> or <color=#35bbe2>Ice</color>. <style=cIsUtility>Grant an effect to the next Secondary or Utility cast</style>.");
            #endregion

            #region Elements
            LanguageAPI.Add(prefix + "ELEMENT_FIRE_NAME", "Initiate: Fire");
            LanguageAPI.Add(prefix + "ELEMENT_FIRE_DESCRIPTION", $"Begin stage attuned to <color=#ec8122>Fire</color>.");

            LanguageAPI.Add(prefix + "ELEMENT_LIGHTNING_NAME", "Initiate: Lightning");
            LanguageAPI.Add(prefix + "ELEMENT_LIGHTNING_DESCRIPTION", $"Begin stage attuned to <color=#98509f>Lightning</color>.");

            LanguageAPI.Add(prefix + "ELEMENT_ICE_NAME", "Initiate: Ice");
            LanguageAPI.Add(prefix + "ELEMENT_ICE_DESCRIPTION", $"Begin stage attuned to <color=#35bbe2>Ice</color>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "Prelude");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Enter Titanic Plains.");
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "Prelude");

            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Manipulator: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Manipulator, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Manipulator: Mastery");

            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_NAME", "Manipulator: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Manipulator, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_UNLOCKABLE_NAME", "Manipulator: Grand Mastery");
            #endregion
            #endregion

        }
    }
}