using Components;
using Interfaces;

// ReSharper disable once CheckNamespace
namespace Helpers;

public static class LogicStateMachineHelper
{
    public static void ChangeState(LogicStateMachineComponent stateMachineComponent, string newStateName) => stateMachineComponent.ChangeState(newStateName);
}