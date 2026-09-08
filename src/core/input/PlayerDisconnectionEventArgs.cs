using System;

namespace Espejismo.Core.Input;

/// <summary>
///   Provides data to the <see cref="IPlayerInputManager.DeviceDisconnected"/> event.
/// </summary>
/// <param name="playerIndex">
///   The zero-based player index disconnected.
/// </param>
/// <param name="deviceId">
///   The identifier of the disconnected device.
/// </param>
/// <param name="fullDisconnection">
///   Whether the disconnected device was the last device assigned to the player.
/// </param>
public class PlayerDisconnectionEventArgs(int playerIndex, long deviceId, bool fullDisconnection) : EventArgs
{
	/// <summary>
	///   Gets the zero-based player index whose device was disconnected.
	/// </summary>
	public int PlayerIndex { get; } = playerIndex;

	/// <summary>
	///   Gets the numeric identifier of the disconnected device.
	/// </summary>
	public long DeviceId { get; } = deviceId;

	/// <summary>
	///   Gets a value indicating whether the player was left wuth no other connected device following the
	///   disconnection.
	/// </summary>
	public bool IsFullyDisconnected { get; } = fullDisconnection;
}
