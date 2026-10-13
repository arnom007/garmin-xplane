using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AbstractUi;

public readonly struct UiAlias : IEquatable<UiAlias>, IEquatable<KeyValuePair<string, string>>
{
	public static class Key
	{
		public static string Verbatim(string key)
		{
			return key;
		}

		public static string Singular(string key)
		{
			return key + "_SINGULAR";
		}

		public static string Plural(string key)
		{
			return key + "_PLURAL";
		}
	}

	[field: CompilerGenerated]
	public static UiAlias Invalid
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string AliasKey
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string AliasValue
	{
		[CompilerGenerated]
		get;
	}

	public UiAlias(string key, string value)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentNullException("key");
		}
		if (string.IsNullOrEmpty(value))
		{
			throw new ArgumentNullException("value");
		}
		AliasKey = key;
		AliasValue = value;
	}

	public static bool IsValid(UiAlias uiAlias)
	{
		return uiAlias != Invalid;
	}

	public static bool operator ==(UiAlias lhs, UiAlias rhs)
	{
		return EqualityComparer<UiAlias>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(UiAlias lhs, UiAlias rhs)
	{
		return !EqualityComparer<UiAlias>.Default.Equals(lhs, rhs);
	}

	public UiAlias ToNewAlias(string value)
	{
		return new UiAlias(AliasKey, value);
	}

	public override bool Equals(object? obj)
	{
		if (obj is UiAlias)
		{
			return Equals((UiAlias)obj);
		}
		return false;
	}

	public bool Equals(UiAlias other)
	{
		if (other.AliasKey == AliasKey)
		{
			return other.AliasValue == AliasValue;
		}
		return false;
	}

	public bool Equals(KeyValuePair<string, string> other)
	{
		if (other.Key == AliasKey)
		{
			return other.Value == AliasValue;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<string, string>(AliasKey, AliasValue);
	}

	public override string ToString()
	{
		return AliasValue;
	}
}
