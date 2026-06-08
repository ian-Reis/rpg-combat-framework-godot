using Godot;
using System;
using Components;

// ReSharper disable once CheckNamespace
namespace Interfaces;

public interface IHasStateMachineComponent
{
    StateMachineComponent StateMachineComponent { get; }
}