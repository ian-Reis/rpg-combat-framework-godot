using Godot;

// ReSharper disable once CheckNamespace
namespace Components;

// Legacy node — kept so existing scene files that reference this script don't break.
// No longer the required parent for components. Use direct [Export] references instead.
[GlobalClass]
public partial class SystemLogicComponents : Node { }
