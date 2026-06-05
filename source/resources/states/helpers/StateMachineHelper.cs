using Interfaces;

// ReSharper disable once CheckNamespace
namespace Helpers;

public static class StateMachineHelper
{
    public static void ChangeState(IHasStateMachineComponent owner, string state) => owner.StateMachineComponent.ChangeState(state);
}