using Godot;
using Data;

// ReSharper disable once CheckNamespace
namespace Interfaces;

public interface ISystemLogicContext : IComponentOwner, IHasStateMachineComponent, IHasAnimationStateMachineComponent
{
    Node3D Pawn { get; }
    CharacterStats Stats { get; }
}
