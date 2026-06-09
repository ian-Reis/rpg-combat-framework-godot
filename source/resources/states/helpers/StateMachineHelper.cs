using Components;
using Interfaces;

// ReSharper disable once CheckNamespace
namespace Helpers;

public static class StateMachineHelper
{
    public static void ChangeState(StateMachineComponent stateMachineComponent, string newStateName) => stateMachineComponent.ChangeState(newStateName);
}