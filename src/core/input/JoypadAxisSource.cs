using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.Input;

/// <summary>
///   An input source driven by the movement of a joypad axis or trigger in a specific direction.
/// </summary>
/// <param name="axis">
///   The joypad axis to assign.
/// </param>
/// <param name="isPositive">
///   <see langword="true"/> whether the axis direction is positive, <see langword="false"/> if it's negative.
/// </param>
public class JoypadAxisSource(JoyAxis axis, bool isPositive) : InputSource<JoypadAxisSource>
{
	/// <summary>
	///   Gets the identifier of the axis or trigger.
	/// </summary>
	public JoyAxis Axis { get; } = axis;

	/// <summary>
	///   Gets a value indicating whether the axis direction is positive.
	/// </summary>
	public bool IsPositive { get; } = isPositive;

	/// <inheritdoc/>
	public override bool TryParseEvent(InputEvent? e, float deadzone, out float value)
	{
		value = 0f;

		if (e is not InputEventJoypadMotion jmotion || Axis != jmotion.Axis)
		{
			return false;
		}

		var raw = jmotion.AxisValue;
		var abs = Math.Abs(raw);

		if (abs < 0.001f || IsPositive != raw > 0f)
		{
			return true;
		}

		value = (abs > deadzone) ? Mathf.Sign(raw) * (abs - deadzone) / (1f - deadzone) : 0f;
		return true;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] JoypadAxisSource? other)
	{
		return other is not null
			&& Axis == other.Axis
			&& IsPositive == other.IsPositive;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(Axis, IsPositive);
	}
}
