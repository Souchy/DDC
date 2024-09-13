using Core.DataCenter.Metadata.Enums;
using Core.DataCenter.Metadata.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Extractor.Converters;
internal class ItemTypeConverter : IConverter<ItemTypes, Models.ItemType>
{
    public Models.ItemType Convert(ItemTypes data) => new()
    {
        id = data.id,
        nameId = data.nameId,
        superTypeId = data.superTypeId,
        categoryId = data.categoryId,
        isInEncyclopedia = data.isInEncyclopedia,
        gender = data.gender,
        mimickable = data.mimickable,
        name = data.name,
        possiblePositions = data.possiblePositions.ToList(),
        plural = data.plural,
        //superType = data.superType,
        zoneMinSize = data.zoneMinSize,
        zoneShape = data.zoneShape,
        zoneSize = data.zoneSize,
        rawZone = data.rawZone,
        evolutiveTypeId = data.evolutiveTypeId,
        craftXpRatio = data.craftXpRatio,
        adminSelectionTypeName = data.adminSelectionTypeName,
    };
}
