using System;
using System.ComponentModel;
using Godot;

namespace RPGFramework.Helpers;

public static class RegisterGroup
{
    public static void Add(Node3D nodeTarget, StringName nameGroup)
    {
        nodeTarget.AddToGroup(nameGroup);
    }
}
