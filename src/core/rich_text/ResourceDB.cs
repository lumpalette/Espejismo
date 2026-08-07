using Godot;
using System;
using System.Collections.Generic;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides static, read-only access to the rich-text resources used for the game.
/// </summary>
[GlobalClass]
public partial class ResourceDB : Resource
{
	/// <summary>
	/// The resource path where the resource database is located.
	/// </summary>
	public const string Path = "res://src/core/rich_text/resources/database.tres";

	#region Export properties
	[Export]
	private StyleTemplate? _defaultStyle;

	[ExportGroup("Registries", "Registry")]
	[Export]
	private Godot.Collections.Dictionary<string, Font> RegistryFonts
	{
		get => _mapFonts.Export;
		set => _mapFonts.Export = value;
	}

	[Export]
	private Godot.Collections.Dictionary<string, Texture2D> RegistryTextures
	{
		get => _mapTextures.Export;
		set => _mapTextures.Export = value;
	}

	[Export]
	private Godot.Collections.Dictionary<string, StyleTemplate> RegistryStyles
	{
		get => _mapStyles.Export;
		set => _mapStyles.Export = value;
	}

	[Export]
	private Godot.Collections.Dictionary<string, TextTag> RegistryTags
	{
		get => _mapTags.Export;
		set => _mapTags.Export = value;
	}
	#endregion

	private readonly Map<Font> _mapFonts = new();
	private readonly Map<Texture2D> _mapTextures = new();
	private readonly Map<StyleTemplate> _mapStyles = new();
	private readonly Map<TextTag> _mapTags = new();
	
	/// <summary>
	/// Gets the style template used as a last fallback for unset <see cref="TextStyle"/> properties.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown if a style template was not specified in the editor.
	/// </exception>
	public static StyleTemplate DefaultStyle
	{
		get
		{
			if (Active._defaultStyle is null)
			{
				throw new InvalidOperationException($"A default {nameof(StyleTemplate)} was not provided");
			}

			if (Active._defaultStyle.Font is null)
			{
				throw new InvalidOperationException("Font in default style is null");
			}

			return Active._defaultStyle;
		}
	}

	private static ResourceDB Active
	{
		get
		{
			if (Engine.IsEditorHint())
			{
				return GD.Load<ResourceDB>(Path);
			}

			field ??= GD.Load<ResourceDB>(Path);
			return field;
		}
	}

	/// <summary>
	/// Gets the <typeparamref name="T"/> resource registered with the specified name.
	/// </summary>
	/// <param name="name">
	/// The name of the resource to get.
	/// </param>
	/// <returns>
	/// The requested <typeparamref name="T"/>, if registered; otherwise, <see langword="null"/>.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// Thrown if <typeparamref name="T"/> is not a supported resource type.
	/// </exception>
	public static T? GetResource<[MustBeVariant] T>(ReadOnlySpan<char> name) where T : Resource
	{
		// I don't want to write a method for every type of resource defined lol.
		object? map = null;

		if (typeof(T) == typeof(Font))
		{
			map = Active._mapFonts;
		}
		else if (typeof(T) == typeof(Texture2D))
		{
			map = Active._mapTextures;
		}
		else if (typeof(T) == typeof(StyleTemplate))
		{
			map = Active._mapStyles;
		}
		else if (typeof(T) == typeof(TextTag))
		{
			map = Active._mapTags;
		}
		
		if (map is null)
		{
			throw new ArgumentException($"Resource type '{typeof(T).FullName}' is not supported", nameof(T));
		}

		return ((Map<T>)map).GetValue(name);
	}

	private readonly struct Map<[MustBeVariant] T> where T : Resource
	{
		private readonly Dictionary<string, T>.AlternateLookup<ReadOnlySpan<char>> _data;

		public Map()
		{
			_data = new Dictionary<string, T>().GetAlternateLookup<ReadOnlySpan<char>>();
		}

		public Godot.Collections.Dictionary<string, T> Export
		{
			get => new(_data.Dictionary);
			set
			{
				_data.Dictionary.Clear();

				foreach (var item in value)
				{
					_data.Dictionary[item.Key] = item.Value;
				}
			}
		}

		public T? GetValue(ReadOnlySpan<char> key)
		{
			_data.TryGetValue(key, out T? value);
			return value;
		}
	}
}
