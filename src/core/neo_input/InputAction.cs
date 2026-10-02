using Godot;
using System;
using System.Collections.Generic;

namespace Espejismo.Core.NeoInput;

/// <summary>
/// Represents the state of a logical game action, bound to one or more <see cref="InputSource"/> instances.
/// </summary>
public class InputAction
{
    private const float Epsilon = 0.001f;

    private readonly HashSet<InputSource> _sources = [];
    private readonly Dictionary<InputSource, float> _sourceStrengths = [];

    private ulong _pressedProcessFrame = ulong.MaxValue;
    private ulong _pressedPhysicsFrame = ulong.MaxValue;
    private ulong _releasedProcessFrame = ulong.MaxValue;
    private ulong _releasedPhysicsFrame = ulong.MaxValue;

    private float _currentStrength;

    /// <summary>
    /// Creates a new <see cref="InputAction"/> instance with no bound sources.
    /// </summary>
    public InputAction()
    {
    }

    /// <summary>
    /// Creates a new <see cref="InputAction"/> instance bound to the specified <see cref="InputSource"/> instances.
    /// </summary>
    /// <param name="sources">The sources to bind.</param>
    public InputAction(params ReadOnlySpan<InputSource> sources)
    {
        foreach (var source in sources)
        {
            _ = _sources.Add(source);
        }
    }

    /// <summary>
    /// Gets or sets the current strength or intensity of the action.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value is always clamped to the range [0,1]. Setting it outside this range will automatically clamp it.
    /// </para>
    /// <para>
    /// Setting this value updates the press/release timestamps, which, along with this property, are used to compute
    /// the current <see cref="State"/>.
    /// </para>
    /// </remarks>
    /// <value>
    /// A single-precision floating-point number in the range [0,1].
    /// </value>
    public float Strength
    {
        get => _currentStrength;
        set
        {
            _sourceStrengths.Clear();
            UpdateStrength(Math.Clamp(value, 0f, 1f));
        }
    }

    /// <summary>
    /// Gets the state of the action in the current frame.
    /// </summary>
    /// <remarks>
    /// The value of this property is based on the current <see cref="Strength"/> value and press/release timestamps,
    /// which are set when the aforementioned property is written.
    /// </remarks>
    public InputActionState State
    {
        get
        {
            if (Engine.IsInPhysicsFrame())
            {
                if (Strength > 0f)
                {
                    return (Engine.GetPhysicsFrames() == _pressedPhysicsFrame)
                        ? InputActionState.JustPressed
                        : InputActionState.Pressed;
                }

                return (Engine.GetPhysicsFrames() == _releasedPhysicsFrame)
                    ? InputActionState.JustReleased
                    : InputActionState.Released;
            }

            if (Strength > 0f)
            {
                return (Engine.GetProcessFrames() == _pressedProcessFrame)
                    ? InputActionState.JustPressed
                    : InputActionState.Pressed;
            }

            return (Engine.GetProcessFrames() == _releasedProcessFrame)
                ? InputActionState.JustReleased
                : InputActionState.Released;
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="InputSource"/> instances that can trigger the action.
    /// </summary>
    public IReadOnlySet<InputSource> Sources => _sources;

    /// <summary>
    /// Binds an <see cref="InputSource"/> to the action.
    /// </summary>
    /// <param name="source">The source to bind.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="source"/> is successfully bound; <see langword="false"/> if it
    /// was already bound.
    /// </returns>
    public bool Bind(InputSource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return _sources.Add(source);
    }

    /// <summary>
    /// Unbinds an <see cref="InputSource"/> to the action.
    /// </summary>
    /// <remarks>
    /// The method recomputes the <see cref="Strength"/> value in case the unbound source was the strongest one, which
    /// is set to 0 if <paramref name="source"/> was the last one; this does not update the press/release timestamps,
    /// however.
    /// </remarks>
    /// <param name="source">The source to unbind.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="source"/> is successfully unbound; <see langword="false"/> if it
    /// was already unbound.
    /// </returns>
    public bool Unbind(InputSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var sourceRemoved = _sources.Remove(source);
        var strengthRemoved = sourceRemoved && _sourceStrengths.Remove(source);

        if (strengthRemoved)
        {
            // Recalculate max strength in case the removed source was the strongest one.
            _currentStrength = GetCurrentMaxStrength();
        }

        return sourceRemoved;
    }

    /// <summary>
    /// Unbinds every <see cref="InputSource"/> associated with the action.
    /// </summary>
    /// <remarks>
    /// The method sets the <see cref="Strength"/> value to 0 without updating the press/release timestamps.
    /// </remarks>
    public void UnbindAll()
    {
        _sources.Clear();
        _sourceStrengths.Clear();
        _currentStrength = 0f;
    }

    /// <summary>
    /// Searches for a bound <see cref="InputSource"/> that matches the specified <see cref="InputEvent"/>, and if
    /// found, updates the state of the action based on the event's data.
    /// </summary>
    /// <remarks>
    /// The event is matched against each bound <see cref="InputSource"/>. If two or more sources match, the strongest
    /// source takes precedence.
    /// </remarks>
    /// <param name="e">
    /// The event to match. If <see langword="null"/>, the method returns <see langword="false"/>.
    /// </param>
    /// <param name="deadzone">The deadzone threshold for analog inputs, normalized.</param>
    /// <returns>
    /// <see langword="true"/> if a matching source is found for <paramref name="e"/>; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool TryUpdateFromEvent(InputEvent? e, float deadzone)
    {
        if (e is null)
        {
            return false;
        }

        foreach (var source in _sources)
        {
            if (!source.TryParseEvent(e, out var strength))
            {
                continue;
            }

            strength = ApplyDeadzone(Math.Clamp(Math.Abs(strength), 0f, 1f), deadzone);

            if (strength > 0)
            {
                _ = _sourceStrengths[source] = strength;
            }
            else
            {
                _ = _sourceStrengths.Remove(source);
            }

            UpdateStrength(GetCurrentMaxStrength());
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears the state of the action, which bypasses setting the press/release timestamps.
    /// </summary>
    public void ResetState()
    {
        Strength = 0f;

        _pressedProcessFrame = ulong.MaxValue;
        _pressedPhysicsFrame = ulong.MaxValue;
        _releasedProcessFrame = ulong.MaxValue;
        _releasedPhysicsFrame = ulong.MaxValue;
    }

    private void UpdateStrength(float strength)
    {
        // Determine if we're going from inactive to active or vice versa for updating the press/release timestamps.
        var wasActive = _currentStrength > 0f;
        _currentStrength = strength;
        var nowActive = _currentStrength > 0f;

        // We offset the physics frame by 1 in case we're not in a physics frame, because the physics frame counter
        // only increments right after _process() ends and before _physics_process() begins.
        var processFrame = Engine.GetProcessFrames();
        var physicsFrame = Engine.GetPhysicsFrames() + (Engine.IsInPhysicsFrame() ? 0uL : 1uL);

        if (!wasActive && nowActive)
        {
            _pressedProcessFrame = processFrame;
            _pressedPhysicsFrame = physicsFrame;
        }
        else if (wasActive && !nowActive)
        {
            _releasedProcessFrame = processFrame;
            _releasedPhysicsFrame = physicsFrame;
        }
    }

    // strength is expected to be normalized.
    private static float ApplyDeadzone(float strength, float deadzone)
    {
        if (strength < Epsilon || strength <= deadzone)
        {
            return 0f;
        }

        return (strength - deadzone) / (1f - deadzone);
    }

    private float GetCurrentMaxStrength()
    {
        var maxStrength = 0f;

        foreach (var strength in _sourceStrengths.Values)
        {
            if (strength > maxStrength)
            {
                maxStrength = strength;
            }
        }

        return maxStrength;
    }
}
