using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Models;
public class Effect
{
    public int id { get; init; }
    public int descriptionId { get; init; }
    public int iconId { get; init; }
    public int characteristic { get; init; }
    public int category { get; init; }
    public string characteristicOperator { get; init; }
    public bool showInTooltip { get; init; }
    public bool useDice { get; init; }
    public bool forceMinMax { get; init; }
    public bool boost { get; init; }
    public bool active { get; init; }
    public int oppositeId { get; init; }
    public string theoreticalDescriptionId { get; init; }
    public int theoreticalPattern { get; init; }
    public bool showInSet { get; init; }
    public sbyte bonusType { get; init; }
    public bool useInFight { get; init; }
    public int effectPriority { get; init; }
    public float effectPowerRate { get; init; }
    public int elementId { get; init; }
    public bool isInPercent { get; init; }
    public bool hideValueInTooltip { get; init; }
    public string m_description { get; init; }
    public string m_theoreticalDescription { get; init; }
    public string description { get; init; }
    public string theoreticalDescription { get; init; }
}
