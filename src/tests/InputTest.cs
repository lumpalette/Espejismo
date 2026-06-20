using Godot;
using Spectrum.Input;

namespace Spectrum.Tests;

internal sealed partial class InputTest : Node
{
	public override void _Ready()
	{
		var player = new PlayerInput
		{
			ActionMap = new InputActionMap
			{
				{ "accept", new InputAction(
					new KeySource(Key.Z, isVirtual: false),
					new KeySource(Key.Enter, isVirtual: false),
					new JoypadButtonSource(JoyButton.B)
				) },
				{ "decline", new InputAction(
					new KeySource(Key.X, isVirtual: false),
					new KeySource(Key.Shift, isVirtual: false),
					new JoypadButtonSource(JoyButton.A)
				) },
				{ "context", new InputAction(
					new KeySource(Key.C, isVirtual: false),
					new KeySource(Key.I, isVirtual: false),
					new JoypadButtonSource(JoyButton.Start)
				) }
			}
		};

		Game.Input.Add(0, player);
		Game.Input[0].AddDevice(0L);
		Game.Input[0].AddDevice(InputEvent.DeviceIdKeyboard);
		Game.Input[0].AddDevice(InputEvent.DeviceIdMouse);
	}

	public override void _Process(double delta)
	{
		if (Game.Input[0].WasPressed("accept")
			|| Game.Input[0].WasPressed("decline")
			|| Game.Input[0].WasPressed("context"))
		{
			GD.Print("pressed from process");
		}

		if (Game.Input[0].WasReleased("accept"))
		{
			GD.Print("released from process");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Game.Input[0].WasPressed("accept"))
		{
			GD.Print("pressed from physics");
		}

		if (Game.Input[0].WasReleased("accept")
			|| Game.Input[0].WasReleased("decline")
			|| Game.Input[0].WasReleased("context"))
		{
			GD.Print("released from physics");
		}
	}
}
