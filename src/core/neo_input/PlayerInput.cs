using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.NeoInput;

/// <summary>
/// Represents the state of an individual player in the input system.
/// </summary>
public sealed partial class PlayerInput
{
    private readonly Dictionary<string, ActionState> _states = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets a value indicating whether the player can receive and process incoming input events. The default
    /// value is <see langword="true"/>.
    /// </summary>
    /// <remarks>¿
    /// Toggling this property resets the strength of every action currently defined in the action map.
    /// </remarks>
    public bool IsEnabled
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            ResetStrengths();
        }
    } = true;

    /// <summary>
    /// Gets or sets the action map currently assigned to the player.
    /// </summary>
    public InputActionMap? ActionMap
    {
        get;
        set
        {
            if (field != value)
            {
                _states.Clear();
                field = value;
            }
        }
    }

    /// <summary>
    /// Updates every <see cref="InputAction"/> bound to an <see cref="InputSource"/> that matches the specified
    /// <see cref="InputEvent"/>.
    /// </summary>
    /// <param name="e">The event to process.</param>
    /// <returns>
    /// <see langword="true"/> if at least one action has a source that matches <paramref name="e"/>; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The method also returns <see langword="false"/> in the following cases:
    /// <list type="bullet">
    ///     <item>The player is disabled.</item>
    ///     <item>The player does not have an <see cref="InputActionMap"/> assigned.</item>
    ///     <item>The event is <see langword="null"/>.</item>
    ///     <item>The event is an echo; see <see cref="InputEvent.IsEcho"/> for details.</item>
    /// </list>
    /// The method does not use <see cref="InputEvent.Device"/> for filtering events.
    /// </remarks>
    public bool ProcessEvent(InputEvent? e)
    {
        if (!CanHandleEvent(e))
        {
            return false;
        }

        var processed = false;

        foreach (var (name, action) in ActionMap)
        {
            ActionState? state = null;

            foreach (var source in action.Sources)
            {
                if (!source.TryParseEvent(e, out var strength))
                {
                    continue;
                }

                var normalized = Math.Clamp(strength, 0d, 1d);
                var output = ApplyDeadzone(normalized, ActionMap.Deadzone);

                // If output is NaN for some stupid reason, SetContributorStrength already accounts for it.
                state ??= GetState(name, action);
                state.SetContributorStrength(new DeviceSource(e.Device, source), output);

                processed = true;
            }
        }

        return processed;
    }

    /// <summary>
    /// Gets the current strength or intensity of the specified <see cref="InputAction"/>.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns>The strength of the action, as a value in the range [0,1].</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="actionName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an <see cref="ActionMap"/> is not assigned to the player.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if an action with the specified name does not exist in the <see cref="ActionMap"/>.
    /// </exception>
    public double GetStrength(string actionName)
    {
        return GetState(actionName).Strength;
    }

    /// <summary>
    /// Sets the current strength or intensity of the specified <see cref="InputAction"/>, ignoring input events.
    /// </summary>
    /// <param name="actionName">The name of the action to mutate.</param>
    /// <param name="strength">The new strength, values outside the range [0,1] are clamped to it.</param>
    /// <remarks>
    /// Input data from a real input device cannot be used along with this method, as the injected strength would be
    /// immediately overwritten by incoming input events; disable the player first using <see cref="IsEnabled"/> before
    /// calling this method.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="actionName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="strength"/> is <see cref="double.NaN"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the player is enabled, or if an <see cref="ActionMap"/> is not assigned to the player.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if an action with the specified name does not exist in the <see cref="ActionMap"/>.
    /// </exception>
    public void SetStrength(string actionName, double strength)
    {
        if (IsEnabled)
        {
            throw new InvalidOperationException(
                "Cannot directly set the strength of an action while the player is enabled.");
        }

        GetState(actionName).Strength = strength;
    }

    /// <summary>
    /// Determines whether the specified action is currently pressed.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns><see langword="true"/> if the action is pressed; otherwise, <see langword="false"/>.</returns>
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
        return GetState(actionName).Strength > 0d;
    }

    /// <summary>
    /// Determines whether the specified action has been pressed in the current frame.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns>
    /// <see langword="true"/> if the action went from released to pressed in this frame; otherwise,
    /// <see langword="false"/>.
    /// </returns>
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
        return GetState(actionName).Phase == InputActionPhase.JustPressed;
    }

    /// <summary>
    /// Determines whether the specified action has been released in the current frame.
    /// </summary>
    /// <param name="actionName">The name of the action to query.</param>
    /// <returns>
    /// <see langword="true"/> if the action went from pressed to released in this frame; otherwise,
    /// <see langword="false"/>.
    /// </returns>
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
        return GetState(actionName).Phase == InputActionPhase.JustReleased;
    }

    [MemberNotNullWhen(true, nameof(ActionMap))]
    private bool CanHandleEvent([NotNullWhen(true)] InputEvent? e)
    {
        return IsEnabled && ActionMap is not null && e is not null && !e.IsEcho();
    }

    // Gets the state of an action with the specified name.
    private ActionState GetState(string actionName)
    {
        ArgumentNullException.ThrowIfNull(actionName);

        if (ActionMap is null)
        {
            throw new InvalidOperationException($"Cannot get action '{actionName}' because no action map is assigned.");
        }

        if (!ActionMap.TryGetAction(actionName, out var action))
        {
            _ = _states.Remove(actionName);
            throw new KeyNotFoundException($"An action with the name '{actionName}' could not be found in the map.");
        }

        return GetState(actionName, action);
    }

    // Same as above, but used when the caller has the action already, since this avoids searching in the map twice.
    private ActionState GetState(string actionName, InputAction action)
    {
        if (!_states.TryGetValue(actionName, out var state) || !ReferenceEquals(state.Action, action))
        {
            _states[actionName] = state = new ActionState(action);
        }

        return state;
    }

    private void ResetStrengths()
    {
        foreach (var state in _states.Values)
        {
            state.Strength = 0d;
        }
    }

    // Normalizes the raw strength based on the specified deadzone value. Relies on deadzone being in [0,1].
    private static double ApplyDeadzone(double strength, double deadzone)
    {
        if (strength <= deadzone)
        {
            return 0d;
        }

        return (strength - deadzone) / (1d - deadzone);
    }
}
