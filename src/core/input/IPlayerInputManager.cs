using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.Input;

/// <summary>
///   Defines a mechanism for managing the state and access of multiple <see cref="PlayerInput"/> instances, mapping
///   them to unique, zero-based player indexes.
/// </summary>
public interface IPlayerInputManager
{
	/// <summary>
	///   Gets a collection containing all the player indexes currently defined.
	/// </summary>
	IEnumerable<int> Indexes { get; }

	/// <summary>
	///   Gets a collection containing all the <see cref="PlayerInput"/> instances assigned to places indexes.
	/// </summary>
	IEnumerable<PlayerInput> Players { get; }

	/// <summary>
	///   Gets the total number of defined player indexes.
	/// </summary>
	int Count { get; }

	/// <summary>
	///   Gets the <see cref="PlayerInput"/> assigned to the specified player index.
	/// </summary>
	/// <param name="playerIndex">
	///   The zero-based player index to get.
	/// </param>
	/// <returns>
	///   The <see cref="PlayerInput"/> assigned at <paramref name="playerIndex"/>.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	///   Thrown if <paramref name="playerIndex"/> is negative.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if <paramref name="playerIndex"/> is undefined.
	/// </exception>
	PlayerInput this[int playerIndex] { get; }

	/// <summary>
	///   Defines a new player index and assigns it the specified <see cref="PlayerInput"/>.
	/// </summary>
	/// <param name="playerIndex">
	///   A unique, zero-based player index to define.
	/// </param>
	/// <param name="player">
	///   The player to assign to the new player index.
	/// </param>
	/// <exception cref="ArgumentException">
	///   Thrown if <paramref name="playerIndex"/> is already defined.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="player"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	///   Thrown if <paramref name="playerIndex"/> is negative.
	/// </exception>
	void Add(int playerIndex, PlayerInput player);

	/// <summary>
	///   Determines whether the specified player index is currently defined.
	/// </summary>
	/// <param name="playerIndex">
	///   The zero-based player index to query.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="playerIndex"/> is defined; otherwise, <see langword="false"/>.
	/// </returns>
	bool ContainsIndex(int playerIndex);

	/// <summary>
	///   Determines whether the specified <see cref="PlayerInput"/> is currently assigned to any player index.
	/// </summary>
	/// <param name="player">
	///   The player to search.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="player"/> is assigned to a player index; otherwise,
	///   <see langword="false"/>.
	/// </returns>
	bool ContainsPlayer(PlayerInput player);

	/// <summary>
	///   Gets the <see cref="PlayerInput"/> assigned to the specified player index.
	/// </summary>
	/// <param name="playerIndex">
	///   The zero-based player index to get.
	/// </param>
	/// <param name="player">
	///   When this method returns, contains the player assigned to the specified player index, if defined; otherwise,
	///   <see langword="null"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if a player is assigned to <paramref name="playerIndex"/>; otherwise,
	///   <see langword="false"/>.
	/// </returns>
	bool TryGetPlayer(int playerIndex, [NotNullWhen(true)] out PlayerInput? player);

	/// <summary>
	///   Returns the player index that the specified <see cref="PlayerInput"/> is currently assigned to.
	/// </summary>
	/// <param name="player">
	///   The player to search. Can be <see langword="null"/>.
	/// </param>
	/// <returns>
	///   The zero-based player index of <paramref name="player"/> if found; otherwise, -1.
	/// </returns>
	int IndexOf(PlayerInput? player);

	/// <summary>
	///   Undefines the specified player index, removing its assigned player.
	/// </summary>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="playerIndex"/> was successfully undefined; otherwise,
	///   <see langword="false"/>.
	/// </returns>
	/// <param name="playerIndex">
	///   The zero-based player index to undefine.
	/// </param>
	bool Remove(int playerIndex);
}
