using Core.DataCenter.Metadata.Effect;
using Core.DataCenter.Metadata.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Extractor.Converters;
internal class EffectInstanceConverter : IConverter<EffectInstance, Models.EffectInstance>
{
    public static EffectInstanceConverter instance= new();

    public Models.EffectInstance Convert(EffectInstance data) => new()
    {
        baseEffectId = data.baseEffectId,
        category = data.category,
        characteristicOperator = data.characteristicOperator,
        delay = data.delay,
        description = data.description,
        descriptionForTooltip = data.descriptionForTooltip,
        dispellable = data.dispellable,
        duration = data.duration,
        durationString = data.durationString,
        effectElement = data.effectElement,
        effectId = data.effectId,
        effectUid = data.effectUid,
        forClientOnly = data.forClientOnly,
        group = data.group,
        hideValueInTooltip = data.hideValueInTooltip,
        isInPercent = data.isInPercent,
        modificator = data.modificator,
        m_flags = data.m_flags,
        oppositeId = data.oppositeId,
        order = data.order,
        parameter0 = data.parameter0,
        parameter1 = data.parameter1,
        parameter2 = data.parameter2,
        parameter3 = data.parameter3,
        parameter4 = data.parameter4,
        priority = data.priority,
        random = data.random,
        showInSet = data.showInSet,
        spellId = data.spellId,
        targetId = data.targetId,   
        targetMask = data.targetMask,
        theoreticalDescription = data.theoreticalDescription,
        theoreticalDescriptionForTooltip = data.theoreticalDescriptionForTooltip,
        theoreticalShortDescriptionForTooltip = data.theoreticalShortDescriptionForTooltip,
        trigger = data.trigger,
        triggers = data.triggers,
        useInFight = data.useInFight,
        visibleInBuffUi = data.visibleInBuffUi,
        visibleInFightLog = data.visibleInFightLog,
        visibleInTooltip = data.visibleInTooltip,
        visibleOnTerrain = data.visibleOnTerrain,
        zoneDamageDecreaseStepPercent = data.zoneDamageDecreaseStepPercent,
        zoneDescr = data.zoneDescr.Convert(),
        zoneMaxDamageDecreaseApplyCount = data.zoneMaxDamageDecreaseApplyCount,
        zoneMinSize = data.zoneMinSize,
        zoneShape = data.zoneShape,
        zoneSize = data.zoneSize,
        zoneStopAtTarget = data.zoneStopAtTarget

    };

}
