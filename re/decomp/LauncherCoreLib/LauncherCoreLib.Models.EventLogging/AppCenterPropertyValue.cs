using System;
using System.Runtime.CompilerServices;
using LanguageExt;

namespace LauncherCoreLib.Models.EventLogging;

public sealed class AppCenterPropertyValue : IEquatable<AppCenterPropertyValue>
{
	public const int MAX_CHARS = 125;

	private static readonly ValidatorUtil.AppCenterSemanticStringValidator VALIDATOR = ValidatorUtil.CheckAppCenterStringOutOfRange(125);

	[field: CompilerGenerated]
	public string PropertyValue
	{
		[CompilerGenerated]
		get;
	}

	private AppCenterPropertyValue(string propertyValue)
	{
		PropertyValue = propertyValue;
	}

	public static Validation<AppCenterSemanticsError, AppCenterPropertyValue> Parse(string propertyValue)
	{
		string propertyValue2 = propertyValue;
		return VALIDATOR(propertyValue2).Map<AppCenterPropertyValue>((string _) => new AppCenterPropertyValue(propertyValue2));
	}

	public static AppCenterPropertyValue ParseTruncateAndHandleErrors(string propertyValue)
	{
		return ValidatorUtil.ValidateTruncateAndLogErrors(Parse, propertyValue, 125);
	}

	public static AppCenterPropertyValue ParseTruncateFromBeginningAndHandleErrors(string propertyValue)
	{
		return ValidatorUtil.ValidateTruncateFromBeginningAndLogErrors(Parse, propertyValue, 125);
	}

	public static AppCenterPropertyValue ParseAndThrowOnErrors(string propertyValue)
	{
		return ValidatorUtil.ValidateAndThrow(Parse, propertyValue);
	}

	public override int GetHashCode()
	{
		return PropertyValue.GetHashCode((StringComparison)4);
	}

	public override bool Equals(object? obj)
	{
		if (obj is AppCenterPropertyValue other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(AppCenterPropertyValue? other)
	{
		return other?.PropertyValue == PropertyValue;
	}

	public override string ToString()
	{
		return PropertyValue;
	}
}
