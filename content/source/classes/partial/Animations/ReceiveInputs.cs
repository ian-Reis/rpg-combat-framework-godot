using Godot;

namespace RPGFramework.Core;

public partial class AnimationController
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!ListenToInput) return;

        System.Action requestAction = @event switch
        {
            _ when @event.IsActionPressed("attack") => RequestAttack,
            _ when @event.IsActionPressed("heavy_attack") => RequestHeavyAttack,
            _ when @event.IsActionPressed("draw_weapon") => () => Weapon?.ToggleEquip(),
            _ => null // Nenhuma ação correspondente
        };

        requestAction?.Invoke();
    }
}