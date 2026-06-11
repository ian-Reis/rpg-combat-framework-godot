using Godot;

namespace Helpers;

public static class InputHelper
{
    public static Vector2 GetInputDirection() => Input.GetVector("left", "right", "forward", "forback");
}
