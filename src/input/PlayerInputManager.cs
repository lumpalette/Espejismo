using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spectrum.Input;

internal sealed partial class PlayerInputManager : Node, IPlayerInputManager
{
	private readonly Dictionary<int, PlayerInput> _players = [];

	public IEnumerable<int> Indexes => _players.Keys;

	public IEnumerable<PlayerInput> Players => _players.Values;

	public int Count => _players.Count;

	public PlayerInput this[int playerIndex]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfNegative(playerIndex, nameof(playerIndex));

			if (!TryGetPlayer(playerIndex, out PlayerInput? player))
			{
				throw new KeyNotFoundException($"Player index ({playerIndex}) is undefined");
			}

			return player;
		}
	}

	public override void _Input(InputEvent @event)
	{
		foreach (PlayerInput player in Players)
		{
			player.HandleEvent(@event);
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

		foreach (KeyValuePair<int, PlayerInput> entry in _players)
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
}
