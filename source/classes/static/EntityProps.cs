// ReSharper disable once CheckNamespace
namespace Classes.statics;

// Numeric stats were moved to Data.CharacterStats (a typed Godot Resource).
// This file keeps only the string keys used to read node references
// from the Pawn via pawn.Get(), which are set by GDScript on that node.
public static class EntityProps
{
    // Planet runtime state (set by the planet's GDScript)
    public const string CurrentPlanet = "_current_planet";
    public const string NewPlanet     = "_new_planet";

    // Node references on the Pawn (set by GDScript or exported props)
    public const string RayFloor  = "ray_floor";
    public const string Model     = "model";
    public const string SpringArm = "spring_arm";
}
