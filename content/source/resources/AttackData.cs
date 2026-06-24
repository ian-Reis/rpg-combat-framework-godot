using Godot;

namespace RPGFramework.Resources;

// Dados de UM golpe. É aqui que cresce o "etc" por ataque (hitstop, efeitos, status...).
[GlobalClass]
public partial class AttackData : Resource
{
    [Export] public float Damage     { get; set; } = 10f;
    [Export] public float LungeForce { get; set; } = 4f;  // avanço (se a arma usar)
}
