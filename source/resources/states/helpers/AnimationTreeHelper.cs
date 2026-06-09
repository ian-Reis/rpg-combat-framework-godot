using Godot;
// ReSharper disable once CheckNamespace
using Interfaces;
using Components;

namespace Helpers;

public static class AnimationTreeHelper
{
    public static void SetBlendPosition(StateMachineComponent context, Godot.StringName path, Godot.Variant value)
    {
        if (context?.AnimationTree == null)
        {
            GD.Print("State machine animation tree is null");
            return;
        }
        
        context.AnimationTree.Set(path, value);
    }

    public static void SetTreeCondition(StateMachineComponent context, Godot.StringName path, bool condition)
    {
        if (context?.AnimationTree == null)
        {
            GD.Print("State machine animation tree is null");
            return;
        }

        context.AnimationTree.Set(path, condition);
    }

    public static bool AnimationFinish(StateMachineComponent context, float endThreshold = 0.05f)
    {
        if (context?.AnimationTree == null)
        {
            GD.Print("State machine animation tree is null");
            return false;
        }

        var playback = context.AnimationTree.Get((Godot.StringName)"parameters/playback").As<Godot.AnimationNodeStateMachinePlayback>();
        if (playback != null)
        {
            float length = playback.GetCurrentLength();
            if (length > 0.0f)
            {
                float pos = playback.GetCurrentPlayPosition();
                return pos >= length - endThreshold;
            }
        }
        return false;
    }
}