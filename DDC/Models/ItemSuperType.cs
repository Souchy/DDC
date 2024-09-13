using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC.Models;

public class ItemSuperType
{
    public int id { get; init; }
    public List<int> possiblePositions { get; init; }
}
