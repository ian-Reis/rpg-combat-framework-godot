using System.ComponentModel;
using Godot;
using RPGFramework.Resources;

namespace RPGFramework.Core;

public partial class AnimationController
{
    [ExportGroup("Pistol")]
    [Export] public float RollBlendSpeed  { get; set; } = 8f;   // suavidade do blend desarmado↔armado
    [Export] public StringName RollAnimationName { get; set; } = "RollAnimation";
    
    private bool _rollJustPressed = false;

    private void RequestRoll() => _rollJustPressed = true;

    private void UpdateRoll(float dt)
    {
        if (!_rollJustPressed || Pawn.State.IsRolling || !Pawn.State.CanRoll) 
            return;

        if (Parameters.OneShotRequest.TryGetValue("roll", out var rollOSPath))
        {
            AnimTree.Set(rollOSPath, (int)AnimationNodeOneShot.OneShotRequest.Fire);
            Pawn.State.IsRolling = true;
        }
    }

    private void RollCallbacks()
    {
        OnAnimFinished(RollAnimationName, () => Pawn.State.IsRolling = false);
    }
}
