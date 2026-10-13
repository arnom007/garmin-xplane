using System;
using System.Runtime.CompilerServices;
using LanguageExt;

namespace LauncherCoreLib.Models.EventLogging;

public sealed class AppCenterPropertyName : IEquatable<AppCenterPropertyName>
{
	private const int MAX_CHARS = 125;

	private static readonly ValidatorUtil.AppCenterSemanticStringValidator VALIDATOR = ValidatorUtil.Combine(ValidatorUtil.AppCenterIdentifierRegex, 125);

	[field: CompilerGenerated]
	private string PropertyName
	{
		[CompilerGenerated]
		get;
	}

	private AppCenterPropertyName(string propertyName)
	{
		PropertyName = propertyName;
	}

	public static Validation<AppCenterSemanticsError, AppCenterPropertyName> Parse(string propertyName)
	{
		return VALIDATOR(propertyName).Map<AppCenterPropertyName>((string s) => new AppCenterPropertyName(s));
	}

	public static AppCenterPropertyName ParseTruncateAndHandleErrors(string propertyName)
	{
		return ValidatorUtil.ValidateTruncateAndLogErrors(Parse, propertyName, 125);
	}

	public static AppCenterPropertyName ParseAndThrowOnErrors(string propertyName)
	{
		return ValidatorUtil.ValidateAndThrow(Parse, propertyName);
	}

	public override int GetHashCode()
	{
		return PropertyName.GetHashCode((StringComparison)4);
	}

	public override bool Equals(object? obj)
	{
		if (obj is AppCenterPropertyName other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(AppCenterPropertyName? other)
	{
		return other?.PropertyName == PropertyName;
	}

	public override string ToString()
	{
		return PropertyName;
	}
}
