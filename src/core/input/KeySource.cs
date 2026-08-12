using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.Input;

/// <summary>
///   Represents a key from a keyboard.
/// </summary>
/// <param name="key">
///   The key to assign.
/// </param>
/// <param name="isVirtual">
///   <see langword="true"/> whether to use the user's keyboard layout, <see langword="false"/> to use a US QWERTY
///   keyboard.
/// </param>
public class KeySource(Key key, bool isVirtual) : InputSource<KeySource>
{
	/// <summary>
	///   Gets the key identifier.
	/// </summary>
	public Key Key { get; } = key;

	/// <summary>
	///   Gets a value indicating whether the key position is based on the user's current layout or based on a standard
	///   US QWERTY layout.
	/// </summary>
	public bool IsVirtual { get; } = isVirtual;

	/// <inheritdoc/>
	public override bool TryParseEvent(InputEvent? e, float deadzone, out float value)
	{
		if (e is not InputEventKey key || Key != (IsVirtual ? key.Keycode : key.PhysicalKeycode))
		{
			value = 0f;
			return false;
		}

		value = key.Pressed ? 1f : 0f;
		return true;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] KeySource? other)
	{
		return other is not null && Key == other.Key && IsVirtual == other.IsVirtual;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(Key, IsVirtual);
	}
}
