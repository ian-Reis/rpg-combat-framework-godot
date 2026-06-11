using Godot;
using Interfaces;

namespace Helpers;

public static class AnimationTreeHelper
{
    public static void SetBlendPosition(IHasAnimationTree context, StringName path, Variant value)
    {
        if (context?.AnimationTree == null)
        {
            GD.Print("Animation tree is null");
            return;
        }

        context.AnimationTree.Set(path, value);
    }

    public static void SetTreeCondition(IHasAnimationTree context, StringName path, bool condition)
    {
        if (context?.AnimationTree == null)
        {
            GD.Print("Animation tree is null");
            return;
        }

        context.AnimationTree.Set(path, condition);
    }

    public static bool AnimationFinish(IHasAnimationTree context, float endThreshold = 0.05f)
    {
        if (context?.AnimationTree == null)
        {
            GD.Print("Animation tree is null");
            return false;
        }

        var playback = context.AnimationTree.Get((StringName)"parameters/playback").As<AnimationNodeStateMachinePlayback>();
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
