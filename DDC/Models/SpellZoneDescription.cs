using Metadata.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Models;
public class SpellZoneDescription
{
    //public static byte DefaultParam1 { get; init; }
    //public static byte DefaultParam2 { get; init; }
    //public static sbyte DefaultDamageDecreaseStepPercent { get; init; }
    //public static sbyte DefaultMaxDamageDecreaseApplyCount { get; init; }
    public List<int> cellIds { get; init; }
    public SpellZoneShape shape { get; init; }
    public byte param1 { get; init; }
    public byte param2 { get; init; }
    public sbyte damageDecreaseStepPercent { get; init; }
    public sbyte maxDamageDecreaseApplyCount { get; init; }
    public bool isStopAtTarget { get; init; }
}
