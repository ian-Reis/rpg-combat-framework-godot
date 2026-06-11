using System;
using Classes.statics;
using Godot;
using Interfaces;

namespace Helpers;

public static class PlanetsHelper
{
    public static bool ChangedPlanet(ISystemLogicContext context)
    {
        var pawn = context?.Pawn;
        if (pawn == null) return false;

        Node3D currentPlanet = (Node3D)pawn.Get(EntityProps.CurrentPlanet);
        Node3D newPlanet     = (Node3D)pawn.Get(EntityProps.NewPlanet);
        return currentPlanet != newPlanet;
    }
}