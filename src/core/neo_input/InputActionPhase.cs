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
    /// <remarks>
    /// If an action is pressed and released in the same frame, the resulting phase will be <see cref="JustReleased"/>,
    /// completely skipping this phase.
    /// </remarks>
    JustPressed,

    /// <summary>
    /// The action's strength is greater than zero, and it was not pressed in the current frame.
    /// </summary>
    Pressed,

    /// <summary>
    /// The action's strength became zero in the current frame. This phase lasts one frame.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If an action is released and pressed in the same frame, the resulting phase will be <see cref="JustPressed"/>,
    /// completely skipping this phase.
    /// </para>
    /// <para>
    /// The release is not necessarily caused by the player. Anything that sets the action's strength to zero produces
    /// this phase, for example:
    /// <list type="bullet">
    ///     <item>Disabling the player.</item>
    ///     <item>Changing the player's action map.</item>
    ///     <item>Unbinding a source from the action.</item>
    ///     <item>Removing a device assigned to the player.</item>
    /// </list>
    /// </para>
    /// </remarks>
    JustReleased
}
