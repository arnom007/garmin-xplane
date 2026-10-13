using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingValueChangedEventArgs : EventArgs, IEquatable<SettingValueChangedEventArgs>
{
	[field: CompilerGenerated]
	public IActiveSetting OwningSetting
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string OldValue
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string NewValue
	{
		[CompilerGenerated]
		get;
	}

	public SettingValueChangedEventArgs(IActiveSetting setting, string oldval, string newval)
	{
		OwningSetting = setting;
		OldValue = oldval;
		NewValue = newval;
	}

	public override string ToString()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0}: {1} -> {2}", (object)OwningSetting, (object)ValOrNULL(OldValue), (object)ValOrNULL(NewValue));
	}

	public bool Equals(SettingValueChangedEventArgs? other)
	{
		if (this != other)
		{
			if (other != null && OwningSetting == other.OwningSetting && OldValue == other.OldValue)
			{
				return NewValue == other.NewValue;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SettingValueChangedEventArgs other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)OwningSetting.LocalId).GetHashCode();
	}

	private static string ValOrNULL(string value)
	{
		return value ?? "<NULL>";
	}
}
