using Godot;
using System;
using System.Collections.Generic;

namespace RPGFramework.Core;

public partial class DefaultControllerAnimation
{
    private readonly Dictionary<StringName, List<Action>> _animCallbacks = new();

    public void OnAnimFinished(StringName animName, Action callback)
    {
        if (!_animCallbacks.ContainsKey(animName))
            _animCallbacks[animName] = new List<Action>();
        _animCallbacks[animName].Add(callback);
    }

    private void HandleAnimationFinished(StringName animName)
    {
        if (_animCallbacks.TryGetValue(animName, out var callbacks))
            foreach (var cb in callbacks)
                cb?.Invoke();
    }
}
