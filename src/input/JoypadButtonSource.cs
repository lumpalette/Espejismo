using Godot;
using System.Diagnostics.CodeAnalysis;

namespace Spectrum.Input;

/// <summary>
///		Represents a button from a joypad device.
/// </summary>
/// <param name="button">
///		The joypad button to assign.
///	</param>
public class JoypadButtonSource(JoyButton button) : InputSource<JoypadButtonSource>
{
	/// <summary>
	///		Gets the button identifier.
	/// </summary>
	public JoyButton Button { get; } = button;

	public override bool TryParseEvent(InputEvent? e, float deadzone, out float value)
	{
		if (e is not InputEventJoypadButton jbutton || Button != jbutton.ButtonIndex)
		{
			value = 0f;
			return false;
		}

		value = jbutton.Pressed ? 1f : 0f;
		return true;
	}

	public override bool Equals([NotNullWhen(true)] JoypadButtonSource? other)
	{
		return other is not null && Button == other.Button;
	}

	public override int GetHashCode()
	{
		return Button.GetHashCode();
	}
}
