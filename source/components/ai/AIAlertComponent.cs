using Godot;

namespace Components;

public enum AlertLevel { Unaware, Suspicious, Alerted }

// Tracks the entity's awareness level.
// AIDetectionComponent can feed suspicion; BT tasks react to CurrentAlert.
[GlobalClass]
public partial class AIAlertComponent : Node
{
    [Signal] public delegate void AlertChangedEventHandler(int from, int to);

    [ExportGroup("Decay")]
    [Export] public float SuspicionDecayRate = 1.0f;  // per second when no stimulus
    [Export] public float AlertDecayRate     = 0.2f;

    [ExportGroup("Thresholds")]
    [Export] public float SuspiciousThreshold = 0.3f;  // suspicion level to become Suspicious
    [Export] public float AlertedThreshold    = 1.0f;  // suspicion level to become Alerted

    public AlertLevel CurrentAlert   { get; private set; } = AlertLevel.Unaware;
    public float      SuspicionLevel { get; private set; } = 0f;

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentAlert == AlertLevel.Unaware) return;

        float decay = CurrentAlert == AlertLevel.Alerted ? AlertDecayRate : SuspicionDecayRate;
        SuspicionLevel = Mathf.Max(SuspicionLevel - decay * (float)delta, 0f);
        UpdateAlert();
    }

    public void AddSuspicion(float amount)
    {
        SuspicionLevel = Mathf.Clamp(SuspicionLevel + amount, 0f, 1f);
        UpdateAlert();
    }

    public void SetAlerted()
    {
        SuspicionLevel = 1f;
        ChangeAlert(AlertLevel.Alerted);
    }

    public void ResetAlert()
    {
        SuspicionLevel = 0f;
        ChangeAlert(AlertLevel.Unaware);
    }

    private void UpdateAlert()
    {
        AlertLevel next = SuspicionLevel >= AlertedThreshold    ? AlertLevel.Alerted
                        : SuspicionLevel >= SuspiciousThreshold ? AlertLevel.Suspicious
                        :                                         AlertLevel.Unaware;
        ChangeAlert(next);
    }

    private void ChangeAlert(AlertLevel next)
    {
        if (next == CurrentAlert) return;
        var prev = CurrentAlert;
        CurrentAlert = next;
        EmitSignal(SignalName.AlertChanged, (int)prev, (int)next);
    }
}
