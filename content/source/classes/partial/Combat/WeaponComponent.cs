using Godot;
using RPGFramework.Entitys;
using RPGFramework.Resources;

namespace RPGFramework.Core;

// Gerencia a arma equipada de um Pawn: modelo na mão, animações e estado.
// Trocar de arma = trocar o WeaponResource. Modular e flexível.
[GlobalClass]
public partial class WeaponComponent : Node
{
    [Signal] public delegate void WeaponChangedEventHandler(WeaponResource weapon);

    [ExportGroup("References")]
    [Export] public Pawn3D Pawn { get; set; }
    [Export] public AnimationPlayer AnimPlayer { get; set; }
    [Export] public Node3D HandAttachment { get; set; } // BoneAttachment3D da mão

    [ExportGroup("Inventory")]
    [Export] public WeaponResource[] Inventory { get; set; } = System.Array.Empty<WeaponResource>();
    [Export] public int StartIndex { get; set; } = -1; // -1 = começa desarmado

    // Nome do slot de library trocado a cada arma (clips padronizados ficam aqui).
    [Export] public string AnimLibrarySlot { get; set; } = "weapon";

    public WeaponResource CurrentWeapon { get; private set; }
    public bool IsEquipped => CurrentWeapon != null;

    private int _index = -1;
    private int _lastIndex = 0; // lembra a última arma pra re-sacar
    private Node _spawnedModel;

    public override void _Ready()
    {
        if (StartIndex >= 0 && StartIndex < Inventory.Length)
            EquipIndex(StartIndex);
    }

    public void EquipIndex(int index)
    {
        if (index < 0 || index >= Inventory.Length) { Unequip(); return; }
        _index = index;
        _lastIndex = index;
        Equip(Inventory[index]);
    }

    public void Equip(WeaponResource weapon)
    {
        if (weapon == null) return;
        CurrentWeapon = weapon;

        SwapModel(weapon.Model);
        SwapAnimations(weapon.Animations);
        Pawn?.State?.SetArmed(true);

        EmitSignal(SignalName.WeaponChanged, weapon);
    }

    public void Unequip()
    {
        CurrentWeapon = null;
        _index = -1;
        SwapModel(null);
        if (AnimPlayer != null && AnimPlayer.HasAnimationLibrary(AnimLibrarySlot))
            AnimPlayer.RemoveAnimationLibrary(AnimLibrarySlot);
        Pawn?.State?.SetArmed(false);

        EmitSignal(SignalName.WeaponChanged, (WeaponResource)null);
    }

    // Saca / guarda (input de equipar). Re-saca a última arma usada.
    public void ToggleEquip()
    {
        if (IsEquipped) Unequip();
        else EquipIndex(_lastIndex);
    }

    // Cicla pra próxima arma do inventário (input de troca).
    public void CycleNext()
    {
        if (Inventory.Length == 0) return;
        EquipIndex((_index + 1) % Inventory.Length);
    }

    private void SwapModel(PackedScene model)
    {
        _spawnedModel?.QueueFree();
        _spawnedModel = null;
        if (HandAttachment != null && model != null)
        {
            _spawnedModel = model.Instantiate();
            HandAttachment.AddChild(_spawnedModel);
        }
    }

    private void SwapAnimations(AnimationLibrary lib)
    {
        if (AnimPlayer == null || lib == null) return;
        if (AnimPlayer.HasAnimationLibrary(AnimLibrarySlot))
            AnimPlayer.RemoveAnimationLibrary(AnimLibrarySlot);
        AnimPlayer.AddAnimationLibrary(AnimLibrarySlot, lib);
    }
}
