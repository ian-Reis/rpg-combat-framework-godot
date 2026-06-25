using Godot;
using System;

namespace RPGFramework.Helpers;

public static class SignalHelper
{
    // Adicionado 'public' e 'this' no primeiro parâmetro para virar Extension Method
    public static int TryConnect<T>(this T node, Action connectAction, StringName logName) where T : GodotObject
    {
        if (node == null) return 0;
        
        try
        {
            connectAction();
            return 1;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"{logName}: Error connecting signal on {typeof(T).Name}: {ex.Message}");
            return 0;
        }
    }
}
