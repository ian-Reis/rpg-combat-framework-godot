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
        var playback = GetPlayback(context);
        if (playback == null) return false;

        float length = playback.GetCurrentLength();
        if (length <= 0f) return false;

        return playback.GetCurrentPlayPosition() >= length - endThreshold;
    }

    // Returns 0..1 — how far through the current animation we are.
    public static float GetNormalizedPlayPosition(IHasAnimationTree context)
    {
        var playback = GetPlayback(context);
        if (playback == null) return 0f;

        float length = playback.GetCurrentLength();
        if (length <= 0f) return 0f;

        return Mathf.Clamp(playback.GetCurrentPlayPosition() / length, 0f, 1f);
    }

    // Tells the AnimationTree StateMachine to travel to a specific node (plays its animation).
    public static void Travel(IHasAnimationTree context, string stateName)
    {
        if (context?.AnimationTree == null)
        {
            GD.PrintErr("[AnimationTreeHelper] AnimationTree is null");
            return;
        }

        if (!context.AnimationTree.Active)
        {
            GD.PrintErr("[AnimationTreeHelper] AnimationTree is not active");
            return;
        }

        var playback = GetPlayback(context);
        if (playback == null)
        {
            GD.PrintErr("[AnimationTreeHelper] Playback not found at 'parameters/playback'");
            return;
        }

        GD.Print($"[AnimationTreeHelper] Travel → {stateName} (current: {playback.GetCurrentNode()})");
        playback.Travel(stateName);
    }

    // Converts root motion displacement (local space) to world-space velocity.
    // Call once per frame — GetRootMotionPosition() returns the delta since last animation step.
    public static Vector3 GetRootMotionVelocity(IHasAnimationTree context, Basis worldBasis, float delta)
    {
        if (context?.AnimationTree == null || delta <= 0f) return Vector3.Zero;
        Vector3 local = context.AnimationTree.GetRootMotionPosition();
        // Godot forward is -Z; animations exported from Blender use +Z forward — flip Z to correct.
        local.Z = -local.Z;
        return (worldBasis * local) / delta;
    }

    private static AnimationNodeStateMachinePlayback GetPlayback(IHasAnimationTree context)
    {
        if (context?.AnimationTree == null) return null;
        return context.AnimationTree.Get((StringName)context.PlaybackPath).As<AnimationNodeStateMachinePlayback>();
    }
}
