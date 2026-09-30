using System;
using System.Collections.Generic;

namespace Espejismo.Core.NeoInput;

/// <summary>
/// Represents the state of an individual player in the input system.
/// </summary>
public class PlayerInput
{
    private readonly HashSet<long> _devices = [];

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
        _devices.Add(deviceId);
    }

    /// <summary>
    /// Removes the assigned input device from the player.
    /// </summary>
    /// <param name="deviceId">The numeric identifier of the device to remove.</param>
    /// <returns><see langword="true"/> if the device is successfully removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveDevice(long deviceId)
    {
        return _devices.Remove(deviceId);
    }

    /// <summary>
    /// Gets the phase of the specified action in the current frame.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns>One of the <see cref="InputActionState"/> enum values.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if there is no action defined with <paramref name="actionName"/>.</exception>
    public InputActionState GetState(string actionName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the current strength or intensity of the specified action.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns>A single-precision floating-point number in the range [0,1].</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if there is no action defined with <paramref name="actionName"/>.</exception>
    public float GetStrength(string actionName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified action is currently active in the current frame.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns><see langword="true"/> if the action is pressed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if there is no action defined with <paramref name="actionName"/>.</exception>
    public bool IsPressed(string actionName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified action was activated in the current frame.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns><see langword="true"/> if the action was pressed this frame; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if there is no action defined with <paramref name="actionName"/>.</exception>
    public bool WasPressed(string actionName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified action was deactivated in the current frame.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns><see langword="true"/> if the action was released this frame; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if there is no action defined with <paramref name="actionName"/>.</exception>
    public bool WasReleased(string actionName)
    {
        throw new NotImplementedException();
    }
}
