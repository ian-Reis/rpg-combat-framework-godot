using Godot;
// ReSharper disable once CheckNamespace
using Interfaces;

namespace Helpers;

public static class AnimationTreeHelper
{
    public static void SetBlendPosition(IHasAnimationTree owner, Godot.StringName path, Godot.Variant value)
    {
        owner.AnimationTree.Set(path, value);
    }
    
    public static void SetTreeCondition(IHasAnimationTree owner, Godot.StringName path, bool condition)
    {
        owner.AnimationTree.Set(path, condition);
    }
    
    public static bool AnimationFinish(IHasAnimationTree owner, float endThreshold = 0.05f)
    {
        var playback = owner.AnimationTree.Get((Godot.StringName)"parameters/playback").As<Godot.AnimationNodeStateMachinePlayback>();
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