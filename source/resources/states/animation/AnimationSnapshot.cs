namespace Animation;

public struct AnimationSnapshot
{
    public float HorizontalSpeed;   // velocidade horizontal real (units/s)
    public float NormalizedSpeed;   // 0..1 relativo ao RunSpeed (0=parado, 1=correndo)
    public float VerticalVelocity;  // positivo=subindo, negativo=caindo
    public bool  IsGrounded;
    public bool  HasInput;
    public float TimeInAir;         // segundos desde último grounded
}
