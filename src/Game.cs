using Godot;
using Spectrum.Input;
using System;

namespace Spectrum;

/// <summary>
/// Represents the core of the game: provides global access to the main subsystems.
/// </summary>
public partial class Game : Node
{
	[Export]
	private PlayerInputManager? _input;

	private static Game? s_instance;

	private Game() { }

	/// <summary>
	/// Gets access to the input system interface.
	/// </summary>
	public static IPlayerInputManager Input
	{
		get
		{
			if (s_instance is null || s_instance._input is null)
			{
				throw new InvalidOperationException($"Trying to access Game.Input before the Game autoload was initialized");
			}

			return s_instance._input;
		}
	}

	public override void _EnterTree()
	{
		if (s_instance is not null)
		{
			QueueFree();
			return;
		}

		s_instance = this;
	}

	public override void _Ready()
	{
		if (_input is null)
		{
			throw new InvalidOperationException($"Input node is null");
		}
	}

	public override void _ExitTree()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}
}
