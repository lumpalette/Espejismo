namespace Espejismo.Core.NeoInput;

/// <summary>
/// Specifies the phases of an <see cref="InputAction"/> in a given frame.
/// </summary>
public enum InputActionPhase
{
    /// <summary>
    /// The action's strength is zero, and it was not released in the current frame.
    /// </summary>
    Released,

    /// <summary>
    /// The action's strength became greater than zero in the current frame. This phase lasts one frame.
    /// </summary>
    JustPressed,

    /// <summary>
    /// The action's strength is greater than zero, and it was not pressed in the current frame.
    /// </summary>
    Pressed,

    /// <summary>
    /// The action's strength became zero in the current frame. This phase lasts one frame.
    /// </summary>
    JustReleased
}
