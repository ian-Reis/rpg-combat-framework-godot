using System.Collections.Generic;
using Godot;

namespace Data;

[GlobalClass]
public partial class LootTable : Resource
{
    [Export] public LootEntry[] Entries  = [];
    [Export] public int         MaxDrops = 3;

    // Returns every entry that passes its chance roll, up to MaxDrops.
    public List<(Resource item, int quantity)> Roll()
    {
        var result = new List<(Resource, int)>();

        foreach (var entry in Entries)
        {
            if (result.Count >= MaxDrops) break;
            if (entry?.Item == null) continue;
            if (GD.Randf() > entry.DropChance) continue;

            int qty = (int)GD.RandRange(entry.MinQuantity, entry.MaxQuantity);
            result.Add((entry.Item, qty));
        }

        return result;
    }
}
