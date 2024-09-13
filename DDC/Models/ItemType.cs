using Core.DataCenter.Metadata.Item;
using Metadata.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Models;

public class ItemType
{

    public int id { get; init; }

    public int nameId { get; init; }

    public int superTypeId { get; init; }

    public Items.ItemCategoryEnum categoryId { get; init; }

    public bool isInEncyclopedia { get; init; }

    public bool plural { get; init; }

    public int gender { get; init; }

    public string rawZone { get; init; }

    public bool mimickable { get; init; }

    public int craftXpRatio { get; init; }

    public int evolutiveTypeId { get; init; }

    public List<int> possiblePositions { get; init; }

    public uint m_zoneSize { get; init; }

    public SpellZoneShape m_zoneShape { get; init; }

    public uint m_zoneMinSize { get; init; }

    public ItemSuperType m_superType { get; init; }

    public string m_name { get; init; }

    public string name { get; init; }

    public virtual string adminSelectionTypeName { get; init; }

    public ItemSuperType superType { get; init; }

    public uint zoneSize { get; init; }

    public SpellZoneShape zoneShape { get; init; }

    public uint zoneMinSize { get; init; }
}
