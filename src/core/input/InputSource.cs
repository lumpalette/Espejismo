using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.Input;

/// <summary>
/// Represents a specific source of input data from an input device.
/// </summary>
/// <remarks>
/// The main purpose of this class is to provide a homogeneous interface for any type of input source. Do <b>not</b>
/// extend this class directly; use <see cref="InputSource{TSource}"/> instead, which provides strongly-typed
/// equality handling automatically.
/// </remarks>
public abstract class InputSource : IEquatable<InputSource>
{
	/// <summary>
	/// Determines whether the specified <see cref="InputEvent"/> matches the source type and, if so, computes its
	/// strength value.
	/// </summary>
	/// <param name="e">
	/// The event to match. If <see langword="null"/>, the method returns <see langword="false"/>.
	/// </param>
	/// <param name="deadzone">
	/// Deadzone threshold for analog inputs, if applicable.
	/// </param>
	/// <param name="value">
	/// When this method returns, contains a value in the range [-1,1] representing the strength of the event, if it
	/// matches; otherwise, 0.0.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the type of <paramref name="e"/> matches the source type; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public abstract bool TryParseEvent(InputEvent? e, float deadzone, out float value);

	/// <summary>
	/// Determines whether the source is equal to the specified <see cref="InputSource"/>.
	/// </summary>
	/// <param name="other">
	/// The source to compare with.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the source and <paramref name="other"/> are equal; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public abstract bool Equals([NotNullWhen(true)] InputSource? other);

	/// <summary>
	/// Computes the hash code of the source.
	/// </summary>
	/// <returns>
	/// The hash code for this <see cref="InputSource"/>.
	/// </returns>
	public abstract override int GetHashCode();

	/// <summary>
	/// Determines whether the source is equal to the specified object.
	/// </summary>
	/// <param name="obj">
	/// The object to compare with.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="obj"/> is an <see cref="InputSource"/> and is equal; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return Equals(obj as InputSource);
	}

	/// <summary>
	/// Determines whether two specified <see cref="InputSource"/> instances are equal.
	/// </summary>
	/// <param name="a">
	/// The source to compare with <paramref name="b"/>.
	/// </param>
	/// <param name="b">
	/// The source to compare with <paramref name="a"/>.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public static bool operator ==(InputSource? a, InputSource? b)
		=> ReferenceEquals(a, b) || ((a is null) ? b is null : a.Equals(b));

	/// <summary>
	/// Determines whether two specified <see cref="InputSource"/> instances are not equal.
	/// </summary>
	/// <param name="a">
	/// The source to compare with <paramref name="b"/>.
	/// </param>
	/// <param name="b">
	/// The source to compare with <paramref name="a"/>.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="a"/> is not equal to <paramref name="b"/>; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public static bool operator !=(InputSource? a, InputSource? b) => !(a == b);
}

/// <summary>
/// Provides a strongly-typed template for implementing <see cref="InputSource"/> types.
/// </summary>
/// <typeparam name="TSource">
/// The type of the derived input source.
/// </typeparam>
public abstract class InputSource<TSource> : InputSource, IEquatable<TSource> where TSource : InputSource<TSource>
{
	/// <summary>
	/// Determines whether the source is equal to the specified <typeparamref name="TSource"/>.
	/// </summary>
	/// <param name="other">
	/// The source to compare with.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if the source and <paramref name="other"/> are equal; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public abstract bool Equals([NotNullWhen(true)] TSource? other);

	/// <summary>
	/// Determines whether the source is equal to the specified non-generic <see cref="InputSource"/>.
	/// </summary>
	/// <param name="other">
	/// The source to compare with.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="other"/> is a <typeparamref name="TSource"/> and is equal;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Equals([NotNullWhen(true)] InputSource? other)
	{
		return Equals(other as TSource);
	}
}
