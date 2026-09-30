namespace Espejismo.Core.NeoInput;

/// <summary>
///   Specifies the states or phases of a bound action during a frame.
/// </summary>
public enum InputActionState
{
	/// <summary>
	///   The action is not currently active.
	/// </summary>
	Released,

	/// <summary>
	///   The action was activated in the current frame.
	/// </summary>
	WasPressed,

	/// <summary>
	///   The action is currently active.
	/// </summary>
	Pressed,

	/// <summary>
	///   The action was deactivated in the current frame.
	/// </summary>
	WasReleased
}
