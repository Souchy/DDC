using Core.DataCenter.Metadata.Spell;
using DDC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Extractor.Converters;
internal class SpellZoneDescriptionConverter : IConverter<SpellZoneDescr, SpellZoneDescription>
{
    public static SpellZoneDescriptionConverter instance = new();
    public SpellZoneDescription Convert(SpellZoneDescr data) => new()
    {
        cellIds = data.cellIds.ToList(),
        damageDecreaseStepPercent = data.damageDecreaseStepPercent,
        isStopAtTarget = data.isStopAtTarget,
        maxDamageDecreaseApplyCount = data.maxDamageDecreaseApplyCount,
        param1 = data.param1,
        param2 = data.param2,
        shape = data.shape
    };
}

internal static class SpellZoneDescriptionConverterExtension
{
    public static SpellZoneDescription Convert(this SpellZoneDescr data) => SpellZoneDescriptionConverter.instance.Convert(data);
}
