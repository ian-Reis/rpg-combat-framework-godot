using System;
using Classes.statics;
using Components;
using Godot;

namespace Helpers;

public static class PlanetsHelper
{
    public static bool ChangedPlanet(SystemLogicComponents slc)
    {
        if (slc?.Pawn is not CharacterBody3D pawn) return false;
        Node3D currentPlanet = (Node3D)pawn.Get(EntityProps.CurrentPlanet);
        Node3D newPlanet = (Node3D)pawn.Get(EntityProps.NewPlanet);
        return currentPlanet != newPlanet;
    }
}