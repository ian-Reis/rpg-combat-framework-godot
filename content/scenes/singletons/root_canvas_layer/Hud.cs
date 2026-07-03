using Godot;
using System;

public partial class Hud : Control
{
    [Export] public HSlider HealthSlider { get; set; }
    [Export] public HSlider StaminaSlider { get; set; }
    [Export] public Label AmmunationLabel { get; set; }

    public void SetHealthMaxMinValue(float maxValue, float minValue)
    {
        HealthSlider.MaxValue = maxValue;
        HealthSlider.MinValue = minValue;

        HealthSlider.Value = maxValue;
    }
    public void SetStaminaMaxMinValue(float maxValue, float minValue)
    {
        StaminaSlider.MaxValue = maxValue;
        StaminaSlider.MinValue = minValue;

        StaminaSlider.Value = maxValue;
    }

    public void HealthChangeValue(float newValue)
    {
        HealthSlider.Value = newValue;

    }
    public void StaminaChangeValue(float newValue)
    {
        StaminaSlider.Value = newValue;

    }


}
