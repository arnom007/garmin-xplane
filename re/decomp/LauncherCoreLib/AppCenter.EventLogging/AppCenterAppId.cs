using System;
using System.Diagnostics.CodeAnalysis;
using LanguageExt;

namespace AppCenter.EventLogging;

public readonly struct AppCenterAppId : IEquatable<AppCenterAppId>
{
	private readonly Guid _guid;

	public string Raw => ((object)(Guid)(ref _guid)).ToString();

	private AppCenterAppId(Guid raw)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this = default(AppCenterAppId);
		_guid = raw;
	}

	public static Option<AppCenterAppId> Parse(string raw)
	{
		return Prelude.parseGuid(raw).Map<AppCenterAppId>((Guid g) => new AppCenterAppId(g));
	}

	public static bool operator ==(AppCenterAppId left, AppCenterAppId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(AppCenterAppId left, AppCenterAppId right)
	{
		return !(left == right);
	}

	public bool Equals(AppCenterAppId other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _guid == other._guid;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is AppCenterAppId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)(Guid)(ref _guid)).GetHashCode();
	}

	public override string ToString()
	{
		return Raw;
	}
}
