using Godot;
using System.Diagnostics.CodeAnalysis;

namespace Spectrum.Input;

/// <summary>
///   Represents a button from a mouse device.
/// </summary>
/// <param name="button">
///   The mouse button to assign.
/// </param>
public class MouseButtonSource(MouseButton button) : InputSource<MouseButtonSource>
{
	/// <summary>
	///   Gets the identifier of the assigned button.
	/// </summary>
	public MouseButton Button { get; } = button;

	public override bool TryParseEvent(InputEvent? e, float deadzone, out float value)
	{
		if (e is not InputEventMouseButton mbutton || Button != mbutton.ButtonIndex)
		{
			value = 0f;
			return false;
		}

		value = mbutton.Pressed ? 1f : 0f;
		return true;
	}

	public override bool Equals([NotNullWhen(true)] MouseButtonSource? other)
	{
		return other is not null && Button == other.Button;
	}

	public override int GetHashCode()
	{
		return Button.GetHashCode();
	}
}
