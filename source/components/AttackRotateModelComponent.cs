using Godot;
using Classes.statics;
using Constants;

namespace Components;

[GlobalClass]
public partial class AttackRotateModelComponent : Node
{
    [ExportGroup("References")]
    [Export] public Node3D      Model     { get; set; }
    [Export] public SpringArm3D SpringArm { get; set; }

    [ExportGroup("Settings")]
    [Export] public float AttackTurnSpeed = 20f;
    [Export] public bool  InvertY         = false;

    private Pawn _pawn;

    public override void _Ready()
    {
        _pawn = GetParent<Pawn>();
    }

    public override void _Process(double delta)
    {
        if (_pawn == null || Model == null) return;

        bool isAttacking = _pawn.LogicSM?.CurrentStateName == LogicStateNames.Attack;
        if (!isAttacking) return;

        SpringArm3D cam = SpringArm ?? (SpringArm3D)_pawn.Get(EntityProps.SpringArm);
        if (cam == null) return;

        Vector3 up = _pawn.UpDirection.Normalized();

        Vector3 camForward = InvertY ? cam.Transform.Basis.Z : -cam.Transform.Basis.Z;
        camForward -= up * camForward.Dot(up);
        camForward  = camForward.Normalized();

        Vector3 currentForward = -Model.Transform.Basis.Z;
        currentForward -= up * currentForward.Dot(up);
        currentForward  = currentForward.Normalized();

        float angle = Mathf.Atan2(
            up.Dot(currentForward.Cross(camForward)),
            currentForward.Dot(camForward)
        );

        float turn     = Mathf.Clamp(angle, -AttackTurnSpeed * (float)delta, AttackTurnSpeed * (float)delta);
        Quaternion rot = new Quaternion(up, turn);
        Vector3 newFwd   = rot * currentForward;
        Vector3 newRight = newFwd.Cross(up).Normalized();

        Model.Transform = new Transform3D(
            new Basis(newRight, up, -newFwd).Orthonormalized(),
            Model.Transform.Origin
        );
    }
}
