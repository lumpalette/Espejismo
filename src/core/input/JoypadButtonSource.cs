using Godot;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.Input;

/// <summary>
///   An input source driven by a physical button on a joypad.
/// </summary>
/// <param name="button">
///   The joypad button to assign.
/// </param>
public class JoypadButtonSource(JoyButton button) : InputSource<JoypadButtonSource>
{
	/// <summary>
	///   Gets the button identifier.
	/// </summary>
	public JoyButton Button { get; } = button;

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] JoypadButtonSource? other)
	{
		return other is not null && Button == other.Button;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return Button.GetHashCode();
	}
}
