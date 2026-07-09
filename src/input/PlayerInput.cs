using Godot;
using System;
using System.Collections.Generic;

namespace Spectrum.Input;

/// <summary>
///   Represents an individual player in the input system.
/// </summary>
public sealed class PlayerInput
{
	private readonly List<long> _devices = [];

	/// <summary>
	///   Gets a collection of numeric identifiers for the input devices assigned to the player.
	/// </summary>
	public IReadOnlyList<long> Devices => _devices;

	/// <summary>
	///   Gets or sets the action map currently assigned to the player.
	/// </summary>
	/// <remarks>
	///   Assigning a new map will reset the state of all action in the previous map.
	/// </remarks>
	public InputActionMap? ActionMap
	{
		get;
		set
		{
			ResetAllStates();
			field = value;
		}
	}

	/// <summary>
	///   Gets or sets a value indicating whether the player can receive and process incoming input events. The default
	///   is <see langword="true"/>.
	/// </summary>
	/// <remarks>
	///   When this property is set to <see langword="false"/>, the state of all current input actions is reset.
	/// </remarks>
	public bool IsEnabled
	{
		get;
		set
		{
			field = value;

			if (!field)
			{
				ResetAllStates();
			}
		}
	} = true;

	/// <summary>
	///   Gets or sets the threshold used for analog input sources, such as joysticks or triggers. The default is 0.2.
	/// </summary>
	/// <value>
	///   A floating-point number in the range [0,1].
	/// </value>
	public float Deadzone
	{
		get;
		set => field = Mathf.Clamp(value, 0f, 1f);
	} = 0.2f;

	/// <summary>
	///   Assigns a new input device to the player.
	/// </summary>
	/// <remarks>
	///   If <paramref name="deviceId"/> is already assigned to this player, the method emits a warning and the method
	///   call is ignored.
	/// </remarks>
	/// <param name="deviceId">
	///   The identifier of the device to add.
	/// </param>
	public void AddDevice(long deviceId)
	{
		if (_devices.Contains(deviceId))
		{
			GD.PushWarning($"Device with ID {deviceId} is already assigned to this player.");
			return;
		}

		_devices.Add(deviceId);
	}

	/// <summary>
	///   Removes an assigned input device from the player.
	/// </summary>
	/// <remarks>
	///   If the device is successfully removed, the state of all current input actions is reset.
	/// </remarks>
	/// <param name="deviceId">
	///   The identifier of the device to remove.
	/// </param>
	public void RemoveDevice(long deviceId)
	{
		if (_devices.Remove(deviceId))
		{
			ResetAllStates();
		}
	}

	/// <summary>
	///   Removes all input devices assigned to this player.
	/// </summary>
	/// <remarks>
	///   When this method is called, the state of all current input actions is reset.
	/// </remarks>
	public void ClearDevices()
	{
		if (_devices.Count > 0)
		{
			_devices.Clear();
			ResetAllStates();
		}
	}

	/// <summary>
	///   Processes the specified <see cref="InputEvent"/> and updates the state of the action map accordingly.
	/// </summary>
	/// <param name="e">
	///   The input event to process.
	/// </param>
	public void Process(InputEvent e)
	{
		if (!CanHandleEvent(e))
		{
			return;
		}

		foreach (var action in ActionMap!.Actions)
		{
			action.Process(e, Deadzone);
		}
	}

	/// <summary>
	///   Gets the current strength or intensity of the specified action.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query, case-insensitive.
	/// </param>
	/// <returns>
	///   A value in the range [0,1] representing the action strength.
	/// </returns>
	/// <exception cref="ArgumentException">
	///   Thrown if <paramref name="actionName"/> is empty or consists only of white-space characters.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///   Thrown if <see cref="ActionMap"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if no action is defined with the name <paramref name="actionName"/>.
	/// </exception>
	public float GetStrength(string actionName)
	{
		return GetAction(actionName).Strength;
	}

	/// <summary>
	///   Determines whether the specified action is currently active.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query, case-insensitive.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the action is pressed; otherwise, <see langword="false"/>.
	///   </returns>
	/// <exception cref="ArgumentException">
	///   Thrown if <paramref name="actionName"/> is empty or consists only of white-space characters.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///   Thrown if <see cref="ActionMap"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if no action is defined with the name <paramref name="actionName"/>.
	/// </exception>
	public bool IsPressed(string actionName)
	{
		return GetAction(actionName).Strength > 0f;
	}

	/// <summary>
	///   Determines whether the specified action was activated in the current frame.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query, case-insensitive.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the action was pressed this frame; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentException">
	///   Thrown if <paramref name="actionName"/> is empty or consists only of white-space characters.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///   Thrown if <see cref="ActionMap"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if no action is defined with the name <paramref name="actionName"/>.
	/// </exception>
	public bool WasPressed(string actionName)
	{
		return GetAction(actionName).State == InputActionState.WasPressed;
	}

	/// <summary>
	///   Determines whether the specified action was deactivated in the current frame.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query, case-insensitive.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the action was released this frame; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentException">
	///   Thrown if <paramref name="actionName"/> is empty or consists only of white-space characters.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///   Thrown if <see cref="ActionMap"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if no action is defined with the name <paramref name="actionName"/>.
	/// </exception>
	public bool WasReleased(string actionName)
	{
		return GetAction(actionName).State == InputActionState.WasReleased;
	}

	private InputAction GetAction(string actionName)
	{
		if (ActionMap is null)
		{
			throw new InvalidOperationException("No action map is currently assigned");
		}

		return ActionMap[actionName];
	}

	private void ResetAllStates()
	{
		if (ActionMap is null)
		{
			return;
		}

		foreach (var action in ActionMap.Actions)
		{
			action.ResetState();
		}
	}

	private bool CanHandleEvent(InputEvent e)
	{
		return IsEnabled && ActionMap is not null && _devices.Contains(e.Device) && !e.IsEcho();
	}
}
