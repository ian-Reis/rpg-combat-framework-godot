using Godot;

namespace RPGFramework.Resources;

// Uma arma = um arquivo .tres. Criar arma nova = criar um Resource, sem código.
[GlobalClass]
public partial class WeaponResource : Resource
{
    [Export] public string DisplayName { get; set; } = "Weapon";

    // Modelo 3D anexado na mão ao equipar.
    [Export] public PackedScene Model { get; set; }

    // Clips desta arma, com NOMES PADRONIZADOS (ex: Attack_A, Attack_B, Heavy, poses armadas).
    // Equipar troca esta library no AnimationPlayer; a CombatSM (que referencia esses nomes) não muda.
    [Export] public AnimationLibrary Animations { get; set; }

    // Sequência do combo. O índice casa com os estados da CombatSM (A/B/C...).
    [Export] public AttackData[] Combo { get; set; } = System.Array.Empty<AttackData>();
    [Export] public AttackData Heavy { get; set; }

    // Dano do golpe nº 'index' do combo (0 = A, 1 = B...). Lido pelo combate ao ligar o hitbox.
    public float DamageForCombo(int index)
        => (index >= 0 && index < Combo.Length && Combo[index] != null) ? Combo[index].Damage : 0f;
}
