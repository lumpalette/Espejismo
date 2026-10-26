using Godot;
using System;
using System.Collections.Generic;

namespace Espejismo.Core.NeoInput;

public partial class PlayerInput
{
    // Helper struct to identify a source with its originating input device.
    private readonly record struct DeviceSource(long DeviceId, InputSource Source);

    // Helper class for maintaining the state of an individual action.
    private sealed class ActionState(InputAction action)
    {
        // Holds the strength corresponding to each source currently contributing to the action.
        private readonly Dictionary<DeviceSource, double> _contributors = [];

        private double _strength;

        private int _version = action.Version;

        public InputAction Action { get; } = action;

        // Press timestamps for when the strength goes from zero to a positive value.
        public ulong PressedProcessTimestamp { get; private set; } = ulong.MaxValue;

        public ulong PressedPhysicsTimestamp { get; private set; } = ulong.MaxValue;

        // Release timestamps for when the strength goes from a positive value to zero.
        public ulong ReleasedProcessTimestamp { get; private set; } = ulong.MaxValue;

        public ulong ReleasedPhysicsTimestamp { get; private set; } = ulong.MaxValue;

        // The setter is only used for overriding the strength of an action (for example, in replays); use
        // SetContributorStrength() for input from a physical device.
        public double Strength
        {
            get
            {
                Sync();
                return _strength;
            }
            set
            {
                if (double.IsNaN(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), double.NaN,
                        "Attempting to assign Strength to NaN.");
                }

                _version = Action.Version;
                _contributors.Clear();

                UpdateStrength(Math.Clamp(value, 0d, 1d));
            }
        }

        // Returns the current action phase based on its strength and the press/release timestamps.
        public InputActionPhase Phase
        {
            get
            {
                if (Engine.IsInPhysicsFrame())
                {
                    if (Strength > 0d)
                    {
                        return (Engine.GetPhysicsFrames() == PressedPhysicsTimestamp)
                            ? InputActionPhase.JustPressed
                            : InputActionPhase.Pressed;
                    }

                    return (Engine.GetPhysicsFrames() == ReleasedPhysicsTimestamp)
                        ? InputActionPhase.JustReleased
                        : InputActionPhase.Released;
                }

                if (Strength > 0d)
                {
                    return (Engine.GetProcessFrames() == PressedProcessTimestamp)
                        ? InputActionPhase.JustPressed
                        : InputActionPhase.Pressed;
                }

                return (Engine.GetProcessFrames() == ReleasedProcessTimestamp)
                    ? InputActionPhase.JustReleased
                    : InputActionPhase.Released;
            }
        }

        // Sets the strength contribution of a specific source.
        public void SetContributorStrength(DeviceSource contributor, double strength)
        {
            Sync();

            // This already handles the case in which strength is NaN, as NaN > 0 always returns false.
            if (strength > 0d)
            {
                _contributors[contributor] = strength;
            }
            else
            {
                _ = _contributors.Remove(contributor);
            }

            UpdateStrength(GetMaxStrength());
        }

        // Removes the strength contributions from all sources belonging to the specified device.
        public void RemoveDevice(long deviceId)
        {
            Sync();
            RemoveStrengthFromContributors((c) => c.DeviceId == deviceId);
        }

        // Checks if any sources from the action have been removed, and if so, removes its strength contribution.
        private void Sync()
        {
            if (_version == Action.Version)
            {
                return;
            }

            _version = Action.Version;
            RemoveStrengthFromContributors((c) => !Action.Sources.Contains(c.Source));
        }

        // Updates the strength of the action, updating its timestamps.
        private void UpdateStrength(double strength)
        {
            if (_strength == strength)
            {
                return;
            }

            var physicsOffset = Engine.IsInPhysicsFrame() ? 0uL : 1uL;

            if (strength > 0d && _strength == 0d)
            {
                PressedProcessTimestamp = Engine.GetProcessFrames();
                PressedPhysicsTimestamp = Engine.GetPhysicsFrames() + physicsOffset;
            }
            else if (strength == 0d && _strength > 0d)
            {
                ReleasedProcessTimestamp = Engine.GetProcessFrames();
                ReleasedPhysicsTimestamp = Engine.GetPhysicsFrames() + physicsOffset;
            }

            _strength = strength;
        }

        // Removes the strength contribution of all contributors that satisfy the specified condition.
        private void RemoveStrengthFromContributors(Predicate<DeviceSource> predicate)
        {
            foreach (var c in _contributors.Keys)
            {
                if (predicate(c))
                {
                    _ = _contributors.Remove(c);
                }
            }

            UpdateStrength(GetMaxStrength());
        }

        private double GetMaxStrength()
        {
            var maxStrength = 0d;

            foreach (var strength in _contributors.Values)
            {
                if (strength > maxStrength)
                {
                    maxStrength = strength;
                }
            }

            return maxStrength;
        }
    }
}
