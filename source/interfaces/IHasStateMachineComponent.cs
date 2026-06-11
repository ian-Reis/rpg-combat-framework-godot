using Godot;
using System;
using Components;

// ReSharper disable once CheckNamespace
namespace Interfaces;

public interface IHasLogicStateMachineComponent
{
    LogicStateMachineComponent LogicStateMachineComponent { get; }
}