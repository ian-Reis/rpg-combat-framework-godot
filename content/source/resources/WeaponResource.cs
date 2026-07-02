using Godot;
using RPGFramework.Entities;

namespace RPGFramework.Resources;

// Uma arma = um arquivo .tres. Criar arma nova = criar um Resource, sem código.
[GlobalClass]
public partial class WeaponResource : Resource
{
    [Export] public string      DisplayName     { get; set; } = "Weapon";
    [Export] public WeaponMode  CurrentMode     { get; set; } = WeaponMode.None;
    [Export] public PackedScene Prefab          { get; set; }

    // Ajuste fino de como a prefab encaixa na mão (relativo ao BoneAttachment).
    [ExportGroup("Grip (ajuste na mão)")]
    [Export] public Vector3 GripPosition { get; set; } = Vector3.Zero;
    [Export] public Vector3 GripRotation { get; set; } = Vector3.Zero; // em graus
    [Export] public Vector3 GripScale    { get; set; } = Vector3.One;

    [ExportGroup("Animations")]
    // Clips desta arma, com NOMES PADRONIZADOS (ex: Attack_A, Attack_B, Heavy, poses armadas).
    // Equipar troca esta library no AnimationPlayer; a CombatSM (que referencia esses nomes) não muda.
    [Export] public AnimationLibrary Animations { get; set; }

    // Sequência do combo. O índice casa com os estados da CombatSM (A/B/C...).
    [Export] public AttackData[] Combo { get; set; } = System.Array.Empty<AttackData>();
    [Export] public AttackData Heavy { get; set; }

    // Dano do golpe nº 'index' do combo (0 = A, 1 = B...). Lido pelo combate ao ligar o hitbox.
    public float DamageForCombo(int index) => (index >= 0 && index < Combo.Length && Combo[index] != null) ? Combo[index].Damage : 0f;
}
