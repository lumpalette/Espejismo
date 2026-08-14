using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
///   Provides static, read-only access to the rich-text resources used for the game.
/// </summary>
[GlobalClass]
public partial class ResourceDB : Resource
{
	/// <summary>
	///   The resource path where the resource database is located.
	/// </summary>
	public const string Path = "res://src/core/rich_text/resources/database.tres";

	#region Export properties
	[Export]
	private StyleTemplate? _defaultStyle;

	[ExportGroup("Registries", "Registry")]
	[Export]
	private Godot.Collections.Dictionary<string, Font> RegistryFonts
	{
		get => _fonts.Export;
		set => _fonts.Export = value;
	}

	[Export]
	private Godot.Collections.Dictionary<string, Texture2D> RegistryTextures
	{
		get => _textures.Export;
		set => _textures.Export = value;
	}

	[Export]
	private Godot.Collections.Dictionary<string, StyleTemplate> RegistryStyles
	{
		get => _templates.Export;
		set => _templates.Export = value;
	}

	[Export]
	private Godot.Collections.Dictionary<string, TextTag> RegistryTags
	{
		get => _tags.Export;
		set => _tags.Export = value;
	}
	#endregion

	private readonly Registry<Font> _fonts = new();
	private readonly Registry<Texture2D> _textures = new();
	private readonly Registry<StyleTemplate> _templates = new();
	private readonly Registry<TextTag> _tags = new();

	/// <summary>
	///   Gets the style template used as a last fallback for unset <see cref="TextStyle"/> properties.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///   Thrown if a style template was not specified in the editor.
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
	///   Gets the <see cref="Font"/> registered with the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the <see cref="Font"/> to get.
	/// </param>
	/// <param name="font">
	///   When this method returns, contains the font associated with <paramref name="name"/>, if registered;
	///   otherwise, <see langword="null"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if a font named <paramref name="name"/> exists; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool TryGetFont(ReadOnlySpan<char> name, [NotNullWhen(true)] out Font? font)
	{
		font = Active._fonts.GetValue(name);
		return font is not null;
	}

	/// <summary>
	///   Gets the <see cref="Texture2D"/> registered with the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the <see cref="Texture2D"/> to get.
	/// </param>
	/// <param name="texture">
	///   When this method returns, contains the texture associated with <paramref name="name"/>, if registered;
	///   otherwise, <see langword="null"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if a texture named <paramref name="name"/> exists; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool TryGetTexture(ReadOnlySpan<char> name, [NotNullWhen(true)] out Texture2D? texture)
	{
		texture = Active._textures.GetValue(name);
		return texture is not null;
	}

	/// <summary>
	///   Gets the <see cref="StyleTemplate"/> registered with the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the <see cref="StyleTemplate"/> to get.
	/// </param>
	/// <param name="template">
	///   When this method returns, contains the template associated with <paramref name="name"/>, if registered;
	///   otherwise, <see langword="null"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if a template named <paramref name="name"/> exists; otherwise,
	///   <see langword="false"/>.
	/// </returns>
	public static bool TryGetStyle(ReadOnlySpan<char> name, [NotNullWhen(true)] out StyleTemplate? template)
	{
		template = Active._templates.GetValue(name);
		return template is not null;
	}

	/// <summary>
	///   Gets the <see cref="TextTag"/> registered with the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the <see cref="TextTag"/> to get.
	/// </param>
	/// <param name="tag">
	///   When this method returns, contains the tag associated with <paramref name="name"/>, if registered; otherwise,
	///   <see langword="null"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if a tag named <paramref name="name"/> exists; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool TryGetTag(ReadOnlySpan<char> name, [NotNullWhen(true)] out TextTag? tag)
	{
		tag = Active._tags.GetValue(name);
		return tag is not null;
	}

	private readonly struct Registry<[MustBeVariant] T> where T : Resource
	{
		private readonly Dictionary<string, T>.AlternateLookup<ReadOnlySpan<char>> _data;

		public Registry()
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
