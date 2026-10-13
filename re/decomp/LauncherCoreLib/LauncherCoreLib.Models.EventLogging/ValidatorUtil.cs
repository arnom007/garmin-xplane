using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LOLA.Logging;
using LanguageExt;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;

namespace LauncherCoreLib.Models.EventLogging;

public static class ValidatorUtil
{
	public delegate Validation<AppCenterSemanticsError, string> AppCenterSemanticStringValidator(string param);

	public delegate Validation<AppCenterSemanticsError, T> AppCenterSemanticValidator<T>(string param);

	[field: CompilerGenerated]
	public static Regex AppCenterIdentifierRegex
	{
		[CompilerGenerated]
		get;
	} = new Regex("^[A-Z][a-zA-Z]*(_[A-Z][a-zA-Z]*)*(_[0-9]*)?$");


	public static string SemanticErrorMessage(string appCenterString)
	{
		return "App Center string returned a SemanticsError when parsing. Please check that the string '" + appCenterString + "' was well formed.";
	}

	public static System.Exception ArgumentError(Seq<AppCenterSemanticsError> errors, string appCenterString)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		return (System.Exception)new ArgumentException(SemanticErrorMessage(appCenterString) + " Errors: " + string.Join<AppCenterSemanticsError>(",", (System.Collections.Generic.IEnumerable<AppCenterSemanticsError>)errors));
	}

	public static AppCenterSemanticStringValidator CheckAppCenterStringAgainstRegex(Regex stringFilter)
	{
		Regex stringFilter2 = stringFilter;
		return delegate(string param)
		{
			Match val = stringFilter2.Match(param);
			return (((Group)val).Success && ((Capture)val).Value.Length == param.Length) ? ((Validation<AppCenterSemanticsError, string>)param) : ((Validation<AppCenterSemanticsError, string>)new AppCenterStringFailedRegex(param));
		};
	}

	public static AppCenterSemanticStringValidator CheckAppCenterStringOutOfRange(int maxChars)
	{
		return (string param) => (param.Length <= maxChars) ? ((Validation<AppCenterSemanticsError, string>)param) : ((Validation<AppCenterSemanticsError, string>)new AppCenterStringOutOfRange(param, maxChars));
	}

	public static AppCenterSemanticStringValidator Combine(Regex stringFilter, int maxChars)
	{
		Regex stringFilter2 = stringFilter;
		return (string str) => CheckAppCenterStringOutOfRange(maxChars)(str) | CheckAppCenterStringAgainstRegex(stringFilter2)(str);
	}

	public static T ValidateAndThrow<T>(AppCenterSemanticValidator<T> validate, string appCenterString)
	{
		string appCenterString2 = appCenterString;
		return validate(appCenterString2).IfFail((Func<Seq<AppCenterSemanticsError>, T>)delegate(Seq<AppCenterSemanticsError> errs)
		{
			throw ArgumentError(errs, appCenterString2);
		});
	}

	public static T ValidateTruncateAndLogErrors<T>(AppCenterSemanticValidator<T> validate, string appCenterString, int maxChars)
	{
		return validate((appCenterString.Length > maxChars) ? string.Concat(MemoryExtensions.AsSpan(appCenterString, 0, maxChars - 4), string.op_Implicit(" ...")) : appCenterString).IfFail(LogSupposedlyImpossibleValidationErrorAndThrow<T>(appCenterString));
	}

	public static T ValidateTruncateFromBeginningAndLogErrors<T>(AppCenterSemanticValidator<T> validate, string appCenterString, int maxChars)
	{
		string param;
		if (appCenterString.Length <= maxChars)
		{
			param = appCenterString;
		}
		else
		{
			System.ReadOnlySpan<char> readOnlySpan = string.op_Implicit("...");
			System.ReadOnlySpan<char> readOnlySpan2 = MemoryExtensions.AsSpan(appCenterString);
			int num = maxChars - 3;
			int length = readOnlySpan2.Length;
			int num2 = length - num;
			param = string.Concat(readOnlySpan, readOnlySpan2.Slice(num2, length - num2));
		}
		return validate(param).IfFail(LogSupposedlyImpossibleValidationErrorAndThrow<T>(appCenterString));
	}

	private static Func<Seq<AppCenterSemanticsError>, T> LogSupposedlyImpossibleValidationErrorAndThrow<T>(string appCenterString)
	{
		string appCenterString2 = appCenterString;
		return delegate(Seq<AppCenterSemanticsError> errors)
		{
			System.Collections.Generic.IEnumerator<string> enumerator = errors.Select<string>((AppCenterSemanticsError e) => e.ErrorMessage).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					string current = enumerator.Current;
					Logger.Log(LogLevel.Error, current);
					TrackedEvent.Log(new SemanticsErrorDataDescriptor(appCenterString2, current));
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			throw ArgumentError(errors, appCenterString2);
		};
	}
}
