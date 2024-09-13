using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Spell;
using Metadata.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Models;
public class EffectInstance
{

    //public static sbyte UninitializedBonusType { get; init; }
    public EffectInstanceFlags m_flags { get; init; }
    public int effectUid { get; init; }
    public short baseEffectId { get; init; }
    public ActionId effectId { get; init; }
    public int order { get; init; }
    public short targetId { get; init; }
    public string targetMask { get; init; }
    public sbyte duration { get; init; }
    public float random { get; init; }
    public short group { get; init; }
    public short modificator { get; init; }
    public byte dispellable { get; init; }
    public byte delay { get; init; }
    public string triggers { get; init; }
    public sbyte effectElement { get; init; }
    public short spellId { get; init; }
    public SpellZoneDescription zoneDescr { get; init; }
    //public static string UnknownName { get; init; }
    //public static string UndefinedDescription { get; init; }
    //public MemoizedValues m_memoizedValues { get; init; }
    //public static string UiPetWeightNominal { get; init; }
    //public static string UiTooltipMonsterXpAlone { get; init; }
    //public static string UiCommonMaximum { get; init; }
    //public static string UiCommonNone { get; init; }
    //public static string UiPetWeightLean { get; init; }
    //public static string UiPetWeightFat { get; init; }
    //public static string UiItemTooltipGiveSpellCategory { get; init; }
    //public static string UiEffectBoostedSpellComplement { get; init; }
    //public static string UiCommonRandom { get; init; }
    //public static string UiEffectRandomProbability { get; init; }
    //public static string UiEffectUnknownMonster { get; init; }
    public bool visibleInTooltip { get; init; }
    public bool visibleInBuffUi { get; init; }
    public bool visibleInFightLog { get; init; }
    public bool visibleOnTerrain { get; init; }
    public bool forClientOnly { get; init; }
    public bool trigger { get; init; }
    public byte zoneSize { get; init; }
    public SpellZoneShape zoneShape { get; init; }
    public byte zoneMinSize { get; init; }
    public int zoneDamageDecreaseStepPercent { get; init; }
    public int zoneMaxDamageDecreaseApplyCount { get; init; }
    public bool zoneStopAtTarget { get; init; }
    public virtual Object parameter0 { get; init; }
    public virtual Object parameter1 { get; init; }
    public virtual Object parameter2 { get; init; }
    public virtual Object parameter3 { get; init; }
    public virtual Object parameter4 { get; init; }
    public bool useInFight { get; init; }
    public string description { get; init; }
    public string characteristicOperator { get; init; }
    public string theoreticalDescription { get; init; }
    public string descriptionForTooltip { get; init; }
    public string theoreticalDescriptionForTooltip { get; init; }
    public string theoreticalShortDescriptionForTooltip { get; init; }
    public bool showInSet { get; init; }
    public bool hideValueInTooltip { get; init; }
    public bool isInPercent { get; init; }
    public int oppositeId { get; init; }
    public int category { get; init; }
    public string durationString { get; init; }
    public int priority { get; init; }
}
