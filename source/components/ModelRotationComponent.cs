using Godot;
using Components;
using Classes.statics;
using Handlers;

namespace Components;

[GlobalClass] public partial class ModelRotationComponent : Node
{
    [Export] public Node3D Model;

    private SystemLogicComponents _owner;

    public override void _Ready()
    {
        _owner = GetParent<SystemLogicComponents>();
    }

    public override void _Process(double delta)
    {
        if (_owner?.Pawn == null) return;
        if (Model == null) return;

        Vector2 inputDir = MovementHandler.GetInputDirection();
        if (inputDir.Length() < 0.1f) return;

        Node3D pawn = _owner.Pawn;

        SpringArm3D cam = (SpringArm3D)pawn.Get(EntityProps.SpringArm);
        if (cam == null) return;

        Vector3 up = pawn is CharacterBody3D charBody
            ? charBody.UpDirection.Normalized()
            : pawn.GlobalTransform.Basis.Y.Normalized();

        Vector3 forward = -cam.Transform.Basis.Z;
        Vector3 right   =  cam.Transform.Basis.X;

        forward -= up * forward.Dot(up);
        right   -= up * right.Dot(up);

        forward = forward.Normalized();
        right   = right.Normalized();

        Vector3 moveDir = (forward * inputDir.Y + right * inputDir.X).Normalized();

        Vector3 currentForward = -Model.Transform.Basis.Z;
        currentForward -= up * currentForward.Dot(up);
        currentForward  = currentForward.Normalized();

        float angle = Mathf.Atan2(
            up.Dot(currentForward.Cross(moveDir)),
            currentForward.Dot(moveDir)
        );

        float turnSpeed = 10f;
        float turn      = Mathf.Clamp(angle, -turnSpeed * (float)delta, turnSpeed * (float)delta);

        Quaternion rot      = new Quaternion(up, turn);
        Vector3    newForward = rot * currentForward;
        Vector3    newRight   = newForward.Cross(up).Normalized();

        Model.Transform = new Transform3D(
            new Basis(newRight, up, -newForward).Orthonormalized(),
            Model.Transform.Origin
        );
    }
}