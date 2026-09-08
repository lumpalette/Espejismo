using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Espejismo.Core.Input;

// Implementation for IPlayerInputManager so it can be used as a singleton in Game.
internal sealed partial class PlayerInputManager : Node, IPlayerInputManager
{
	private readonly Dictionary<int, PlayerInput> _players = [];

	public event EventHandler<PlayerDisconnectionEventArgs>? DeviceDisconnected;

	public IEnumerable<int> Indexes => _players.Keys;

	public IEnumerable<PlayerInput> Players => _players.Values;

	public int Count => _players.Count;

	public PlayerInput this[int playerIndex]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfNegative(playerIndex, nameof(playerIndex));

			if (!TryGetPlayer(playerIndex, out var player))
			{
				throw new KeyNotFoundException($"Player index ({playerIndex}) is undefined");
			}

			return player;
		}
	}

	public override void _EnterTree()
	{
		Godot.Input.JoyConnectionChanged += OnJoyConnectionChanged;
	}

	public override void _ExitTree()
	{
		Godot.Input.JoyConnectionChanged -= OnJoyConnectionChanged;
	}

	public override void _Input(InputEvent @event)
	{
		foreach (var player in Players)
		{
			player.Process(@event);
		}
	}

	public void Add(int playerIndex, PlayerInput player)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(playerIndex, nameof(playerIndex));
		ArgumentNullException.ThrowIfNull(player, nameof(player));

		if (!_players.TryAdd(playerIndex, player))
		{
			throw new ArgumentException($"Player index ({playerIndex}) is already defined", nameof(player));
		}
	}

	public bool ContainsIndex(int playerIndex)
	{
		return _players.ContainsKey(playerIndex);
	}

	public bool ContainsPlayer(PlayerInput player)
	{
		return _players.ContainsValue(player);
	}

	public bool TryGetPlayer(int playerIndex, [NotNullWhen(true)] out PlayerInput? player)
	{
		return _players.TryGetValue(playerIndex, out player);
	}

	public int IndexOf(PlayerInput? player)
	{
		if (player is null)
		{
			return -1;
		}

		foreach (var entry in _players)
		{
			if (entry.Value == player)
			{
				return entry.Key;
			}
		}

		return -1;
	}

	public bool Remove(int playerIndex)
	{
		return _players.Remove(playerIndex);
	}

	private void OnJoyConnectionChanged(long device, bool connected)
	{
		if (connected)
		{
			return;
		}

		foreach (var entry in _players)
		{
			if (entry.Value.RemoveDevice(device))
			{
				// We don't break here, as two or more players can share the same input device.
				var args = new PlayerDisconnectionEventArgs(entry.Key, device, entry.Value.Devices.Count == 0);
				DeviceDisconnected?.Invoke(this, args);
			}
		}
	}
}
