using System;
using System.Runtime.CompilerServices;
using LanguageExt;

namespace LauncherCoreLib.Models.EventLogging;

public sealed class AppCenterEventName : IEquatable<AppCenterEventName>
{
	private const int MAX_CHARS = 256;

	private static readonly ValidatorUtil.AppCenterSemanticStringValidator VALIDATOR = ValidatorUtil.Combine(ValidatorUtil.AppCenterIdentifierRegex, 256);

	[field: CompilerGenerated]
	private string EventName
	{
		[CompilerGenerated]
		get;
	}

	private AppCenterEventName(string eventName)
	{
		EventName = eventName;
	}

	public static Validation<AppCenterSemanticsError, AppCenterEventName> Parse(string eventName)
	{
		return VALIDATOR(eventName).Map<AppCenterEventName>((string s) => new AppCenterEventName(s));
	}

	public static AppCenterEventName ParseAndThrowOnErrors(string propertyName)
	{
		return ValidatorUtil.ValidateAndThrow(Parse, propertyName);
	}

	public override int GetHashCode()
	{
		return EventName.GetHashCode((StringComparison)4);
	}

	public override bool Equals(object? obj)
	{
		if (obj is AppCenterEventName other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(AppCenterEventName? other)
	{
		return other?.EventName == EventName;
	}

	public override string ToString()
	{
		return EventName;
	}
}
