using Godot;

namespace Data;

[GlobalClass]
public partial class LootEntry : Resource
{
    [Export] public Resource Item        = null;  // game-defined item resource
    [Export] public int      MinQuantity = 1;
    [Export] public int      MaxQuantity = 1;
    [Export] public float    DropChance  = 1f;    // 0..1
}
