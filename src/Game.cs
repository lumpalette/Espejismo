using Espejismo.Core.Input;
using Godot;
using System;

namespace Espejismo;

/// <summary>
/// Represents the core of the game: provides global access to the main subsystems.
/// </summary>
public partial class Game : Node
{
    private static Game? s_instance;

    [ExportGroup("Systems")]
    [Export]
    private PlayerInputManager? _input;

    private Game()
    {
    }

    /// <summary>
    /// Gets access to the input system interface.
    /// </summary>
    public static IPlayerInputManager Input
    {
        get
        {
            if (s_instance?._input is null)
            {
                throw new InvalidOperationException($"The Game autoload cannot be accessed before it initializes.");
            }

            return s_instance._input;
        }
    }

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        if (s_instance is not null)
        {
            QueueFree();
            return;
        }

        s_instance = this;
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (_input is null)
        {
            throw new InvalidOperationException("The Game's input subsystem is missing.");
        }
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }
}
