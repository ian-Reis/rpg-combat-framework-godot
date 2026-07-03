using Godot;

public partial class Projectile3D : RigidBody3D
{
    [Export] public float Velocidade { get; set; } = 50.0f;
    private Vector3 _direcaoDisparo = Vector3.Zero;

    public void Setup(Vector3 direcaoGlobal)
    {
        // Garante que a direção recebida seja guardada de forma global e normalizada
        _direcaoDisparo = direcaoGlobal.Normalized();
        // CORREÇÃO FÍSICA: Define a velocidade linear diretamente na direção correta
        LinearVelocity = _direcaoDisparo * Velocidade;
    }
}
