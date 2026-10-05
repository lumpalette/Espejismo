using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.NeoInput;

/// <summary>
/// Represents the state of an individual player in the input system.
/// </summary>
public class PlayerInput
{
	private readonly HashSet<long> _devices = [];

	/// <summary>
	/// Gets or sets a value indicating whether the player can receive and process incoming input events. The default
	/// value is <see langword="true"/>.
	/// </summary>
	/// <remarks>
	/// Toggling this property to <see langword="false"/> resets the strengths of all actions in the current
	/// <see cref="ActionMap"/>.
	/// </remarks>
	public bool IsEnabled
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;

				if (!value)
				{
					ResetStrengths();
				}
			}
		}
	} = true;

	/// <summary>
	/// Gets or sets the action map currently assigned to the player.
	/// </summary>
	/// <remarks>
	/// Changing the action map resets the strengths of all actions from the previous one.
	/// </remarks>
	public InputActionMap? ActionMap
	{
		get;
		set
		{
			if (field != value)
			{
				ResetStrengths();
				field = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the threshold value used for analog input sources, such as joysticks or triggers. The default
	/// value is 0.2.
	/// </summary>
	/// <value>
	/// A single-precision floating-point number in the range [0,1].
	/// </value>
	/// <remarks>
	/// The value is always clamped to the range [0,1]. Setting it outside this range will automatically clamp it.
	/// </remarks>
	public float Deadzone
	{
		get;
		set => field = Math.Clamp(value, 0f, 1f);
	} = 0.2f;

	/// <summary>
	/// Gets a collection of numeric identifiers for the input devices assigned to the player.
	/// </summary>
	public IReadOnlySet<long> Devices => _devices;

	/// <summary>
	/// Assigns a new input device to the player.
	/// </summary>
	/// <param name="deviceId">The numeric identifier of the device to add.</param>
	/// <remarks>
	/// If the <paramref name="deviceId"/> identifier is already assigned, the method call is ignored.
	/// </remarks>
	public void AddDevice(long deviceId)
	{
		_ = _devices.Add(deviceId);
	}

	/// <summary>
	/// Removes an assigned input device from the player.
	/// </summary>
	/// <param name="deviceId">The numeric identifier of the device to remove.</param>
	/// <returns>
	/// <see langword="true"/> if the device is successfully removed; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// Successfully removing a device resets the strengths of all actions in the current <see cref="ActionMap"/>.
	/// </remarks>
	public bool RemoveDevice(long deviceId)
	{
		var success = _devices.Remove(deviceId);

		if (success)
		{
			ResetStrengths();
		}

		return success;
	}

	/// <summary>
	/// Clears all input devices assigned to the player.
	/// </summary>
	/// <remarks>
	/// This method resets the strengths of all actions in the current <see cref="ActionMap"/>.
	/// </remarks>
	public void ClearDevices()
	{
		_devices.Clear();
		ResetStrengths();
	}

	/// <summary>
	/// Processes the specified <see cref="InputEvent"/> and updates the state of the player's actions accordingly.
	/// </summary>
	/// <param name="e">
	/// The event to process. If <see langword="null"/>, the method returns <see langword="false"/>.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="e"/> can be handled by the player and at least one
	/// <see cref="InputAction"/> in the action map has a source that matches it; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// An event can be handled if it is neither <see langword="null"/> nor an echo, and the player:
	/// <list type="bullet">
	///     <item>is enabled,</item>
	///     <item>has an action map assigned, and</item>
	///     <item>has the event's device assigned.</item>
	/// </list>
	/// </para>
	/// <para>
	/// For how each <see cref="InputAction"/> is updated, see <see cref="InputAction.TryUpdateFromEvent"/>.
	/// </para>
	/// </remarks>
	public bool ProcessEvent(InputEvent? e)
	{
		if (!CanHandleEvent(e))
		{
			return false;
		}

		var processed = false;

		foreach (var action in ActionMap.Actions)
		{
			processed |= action.TryUpdateFromEvent(e, Deadzone);
		}

		return processed;
	}

	/// <summary>
	/// Determines whether the specified action is currently pressed.
	/// </summary>
	/// <param name="actionName">The name of the action to check.</param>
	/// <returns><see langword="true"/> if the action is pressed; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// <para>
	/// Use this method to check for behaviour that happens across multiple frames, such as movement.
	/// </para>
	/// <para>
	/// An action is considered pressed if its strength is greater than 0, i.e. if the player's input exceeds the
	/// current <see cref="Deadzone"/> threshold.
	/// </para>
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown if an <see cref="ActionMap"/> is not assigned to the player.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	/// Thrown if an action with the specified name does not exist in the <see cref="ActionMap"/>.
	/// </exception>
	public bool IsPressed(string actionName)
	{
		return GetAction(actionName).Strength > 0f;
	}

	/// <summary>
	/// Determines whether the specified action has been pressed in the current frame.
	/// </summary>
	/// <param name="actionName">The name of the action to check.</param>
	/// <returns>
	/// <see langword="true"/> if the action went from released to pressed in this frame; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// Use this method to check for behaviour that should occur only once every time an action is pressed, such as
	/// jumping.
	/// </para>
	/// <para>
	/// For more details about the behaviour of this method, see <see cref="InputActionPhase.JustPressed"/>.
	/// </para>
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown if an <see cref="ActionMap"/> is not assigned to the player.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	/// Thrown if an action with the specified name does not exist in the <see cref="ActionMap"/>.
	/// </exception>
	public bool JustPressed(string actionName)
	{
		return GetAction(actionName).Phase == InputActionPhase.JustPressed;
	}

	/// <summary>
	/// Determines whether the specified action has been released in the current frame.
	/// </summary>
	/// <param name="actionName">The name of the action to check.</param>
	/// <returns>
	/// <see langword="true"/> if the action went from pressed to released in this frame; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// Use this method to check for behaviour that should occur only once every time an action is released, such as UI
	/// confirmation.
	/// </para>
	/// <para>
	/// For more details about the behaviour of this method, see <see cref="InputActionPhase.JustReleased"/>.
	/// </para>
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown if an <see cref="ActionMap"/> is not assigned to the player.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	/// Thrown if an action with the specified name does not exist in the <see cref="ActionMap"/>.
	/// </exception>
	public bool JustReleased(string actionName)
	{
		return GetAction(actionName).Phase == InputActionPhase.JustReleased;
	}

	private InputAction GetAction(string name)
	{
		ArgumentNullException.ThrowIfNull(name);

		if (ActionMap is null)
		{
			throw new InvalidOperationException($"Cannot get action '{name}' because no action map is assigned.");
		}

		return ActionMap[name];
	}

	private void ResetStrengths()
	{
		if (ActionMap is not null)
		{
			foreach (var action in ActionMap.Actions)
			{
				action.Strength = 0f;
			}
		}
	}

	[MemberNotNullWhen(true, nameof(ActionMap))]
	private bool CanHandleEvent(InputEvent? e)
	{
		return IsEnabled && ActionMap is not null && e is not null && _devices.Contains(e.Device) && !e.IsEcho();
	}
}
