using System;
using System.Collections.Generic;

namespace Espejismo.Core.NeoInput;

/// <summary>
///   Represents the state of an individual player in the input system.
/// </summary>
public abstract class PlayerInput
{
	/// <summary>
	///   Gets the phase of the specified action in the current frame.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query.
	/// </param>
	/// <returns>
	///   One of the <see cref="InputActionState"/> enum values.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if there is no action defined with <paramref name="actionName"/>.
	/// </exception>
	public InputActionState GetState(string actionName)
	{

	}

	/// <summary>
	///   Gets the current strength or intensity of the specified action.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query.
	/// </param>
	/// <returns>
	///   A value in the range [0,1] representing the action strength.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if there is no action defined with <paramref name="actionName"/>.
	/// </exception>
	public float GetStrength(string actionName)
	{

	}

	/// <summary>
	///   Determines whether the specified action is currently active in the current frame.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the action is pressed; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if there is no action defined with <paramref name="actionName"/>.
	/// </exception>
	public bool IsPressed(string actionName)
	{
		
	}

	/// <summary>
	///   Determines whether the specified action was activated in the current frame.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the action was pressed this frame; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if there is no action defined with <paramref name="actionName"/>.
	/// </exception>
	public bool WasPressed(string actionName)
	{

	}

	/// <summary>
	///   Determines whether the specified action was deactivated in the current frame.
	/// </summary>
	/// <param name="actionName">
	///   The name of the action to query.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the action was released this frame; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="actionName"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if there is no action defined with <paramref name="actionName"/>
	/// </exception>
	public bool WasReleased(string actionName)
	{

	}

	private struct ActionFrameData()
	{
		public float CurrentStrength;

		// Strength values for each source that contributed to the action.
		public Dictionary<InputSource, float> SourceStrengths = [];

		// Timestamps used for tracking the state between frames.
		public ulong PressedProcessFrame;
		public ulong PressedPhysicsFrame;
		public ulong ReleasedProcessFrame;
		public ulong ReleasedPhysicsFrame;
	}
}
