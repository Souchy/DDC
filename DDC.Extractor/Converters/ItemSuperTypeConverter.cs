using Core.DataCenter.Metadata.Item;
using DDC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Extractor.Converters;
internal class ItemSuperTypeConverter : IConverter<ItemSuperTypes, Models.ItemSuperType>
{
    public ItemSuperType Convert(ItemSuperTypes data) => new()
    {
        id = data.id,
        possiblePositions = data.possiblePositions.ToList(),
    };
}
