using Godot;
using System;
using System.Collections.Generic;

namespace Spectrum.Input;

/// <summary>
///   Contains information and the frame-based state of a logical game action.
/// </summary>
public class InputAction
{
	// for small sources this is actually slower than a list, but the performance difference is so negligible that it
	// actually doesn't matter.
	private readonly HashSet<InputSource> _sources = [];
	private readonly Dictionary<InputSource, float> _strengths = [];

	// we set those to MaxValue because otherwise an action release would be detected in the first frame of the game,
	// which is probably more common than waiting 9.74 * 10^9 years (a lot).
	private ulong _pressedProcessFrame = ulong.MaxValue;
	private ulong _pressedPhysicsFrame = ulong.MaxValue;
	private ulong _releasedProcessFrame = ulong.MaxValue;
	private ulong _releasedPhysicsFrame = ulong.MaxValue;

	private float _currentStrength;

	/// <summary>
	///   Initializes a new instance of the <see cref="InputAction"/> class that is empty.
	/// </summary>
	public InputAction()
	{
		// ñiñiñiñiñiñiñi
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="InputAction"/> class by providing a collection of
	///   <see cref="InputSource"/> instances to use as bindings.
	/// </summary>
	/// <param name="sources">
	///   The sources to bind to this action.
	/// </param>
	public InputAction(params ReadOnlySpan<InputSource> sources)
	{
		foreach (var source in sources)
		{
			Bind(source);
		}
	}

	/// <summary>
	///   Gets the current strength or intensity of the action.
	/// </summary>
	/// <value>
	///   A floating-point number in the range [0,1].
	/// </value>
	public float Strength => Math.Abs(_currentStrength);

	/// <summary>
	///   Gets the state of the action in the current frame.
	/// </summary>
	public InputActionState State
	{
		get
		{
			if (Engine.IsInPhysicsFrame())
			{
				if (_currentStrength == 0f)
				{
					return (Engine.GetPhysicsFrames() == _releasedPhysicsFrame)
						? InputActionState.WasReleased
						: InputActionState.Released;
				}

				return (Engine.GetPhysicsFrames() == _pressedPhysicsFrame)
					? InputActionState.WasPressed
					: InputActionState.Pressed;
			}
			
			if (_currentStrength == 0f)
			{
				return (Engine.GetProcessFrames() == _releasedProcessFrame)
					? InputActionState.WasReleased
					: InputActionState.Released;
			}

			return (Engine.GetProcessFrames() == _pressedProcessFrame)
				? InputActionState.WasPressed
				: InputActionState.Pressed;
		}
	}

	/// <summary>
	///   Gets a collection of <see cref="InputSource"/> instances that can trigger the action.
	/// </summary>
	public IReadOnlySet<InputSource> Sources => _sources;

	/// <summary>
	///   Binds the specified <see cref="InputSource"/> to the action.
	/// </summary>
	/// <param name="source">
	///   The source to bind.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if <paramref name="source"/> was successfully bound; <see langword="false"/> if it was
	///   already present.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="source"/> is <see langword="null"/>.
	/// </exception>
	public bool Bind(InputSource source)
	{
		ArgumentNullException.ThrowIfNull(source, nameof(source));
		return _sources.Add(source);
	}

	/// <summary>
	///   Unbinds the specified <see cref="InputSource"/> from the action.
	/// </summary>
	/// <param name="source">
	///   The source to unbind.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="source"/> was successfully unbound; <see langword="false"/> if
	///   it was not present.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="source"/> is <see langword="null"/>.
	/// </exception>
	public bool Unbind(InputSource source)
	{
		ArgumentNullException.ThrowIfNull(source, nameof(source));

		var sourceRemoved = _sources.Remove(source);
		var strengthRemoved = sourceRemoved && _strengths.Remove(source);

		if (strengthRemoved)
		{
			// We also need to recalculate the current strength in case the unbound source was the strongest.
			_currentStrength = GetCurrentMaxStrength();

			if (_currentStrength == 0f)
			{
				_releasedProcessFrame = ulong.MaxValue;
				_releasedPhysicsFrame = ulong.MaxValue;
			}
		}

		return sourceRemoved;
	}

	/// <summary>
	///   Unbinds all <see cref="InputSource"/> instances associated with the action.
	/// </summary>
	public void UnbindAll()
	{
		_sources.Clear();
		ResetState();
	}

	// Processes the specified Godot's input event and updates the state of the action. We also need to pass a deadzone
	// value because it doesn't make too much sense for actions to store independent deadzones from each other.
	internal void HandleEvent(InputEvent e, float deadzone)
	{
		foreach (var source in _sources)
		{
			if (!source.TryParseEvent(e, deadzone, out float strength))
			{
				continue;
			}

			// The final strength of the action is determined by the strongest source. If two sources are active and
			// then one is deactivated, this ensures the action strength matches the other source's strength.
			if (strength != 0)
			{
				_strengths[source] = strength;
			}
			else
			{
				_strengths.Remove(source);
			}

			var wasActive = _currentStrength != 0f;

			_currentStrength = GetCurrentMaxStrength();
			
			var nowActive = _currentStrength != 0f;

			// We offset the physics timestamps by +1 because the frame counter increases just before entering that
			// loop, after all input events have been emitted.
			if (!wasActive && nowActive)
			{
				_pressedProcessFrame = Engine.GetProcessFrames();
				_pressedPhysicsFrame = Engine.GetPhysicsFrames() + 1;
			}
			else if (wasActive && !nowActive)
			{
				_releasedProcessFrame = Engine.GetProcessFrames();
				_releasedPhysicsFrame = Engine.GetPhysicsFrames() + 1;
			}

			break;
		}
	}

	internal void ResetState()
	{
		_currentStrength = 0f;
		_strengths.Clear();
		_releasedProcessFrame = ulong.MaxValue;
		_releasedPhysicsFrame = ulong.MaxValue;
	}

	private float GetCurrentMaxStrength()
	{
		var strength = 0f;

		foreach (var s in _strengths.Values)
		{
			if (Math.Abs(s) > Math.Abs(strength))
			{
				strength = s;
			}
		}

		return strength;
	}
}
