using System;
using System.Linq;

namespace RoarClient.Net;

public static class RoarResultExtensions
{
	public static bool IsTypicalSuccess(this IRoarResult result)
	{
		return result.ThrowIfNull().NativeError == ROAR_err_type.ROAR_ERR_NO_ERR;
	}

	public static bool IsSizeResult(this IRoarResult result)
	{
		return result.ThrowIfNull().NativeError >= ROAR_err_type.ROAR_ERR_NO_ERR;
	}

	public static bool IsNegativeErrorCode(this IRoarResult result)
	{
		return result.ThrowIfNull().NativeError < ROAR_err_type.ROAR_ERR_NO_ERR;
	}

	public static bool IsOneOf(this IRoarResult result, params ROAR_err_type[] errors)
	{
		return errors.Contains(result.ThrowIfNull().NativeError);
	}

	public static IRoarResult ThrowIfNot(this IRoarResult result, params ROAR_err_type[] oneOfThese)
	{
		if (!result.IsOneOf(oneOfThese))
		{
			throw result.ThrowIfNull().BuildException();
		}
		return result;
	}

	public static IRoarResult ThrowIfNotTypicalSuccess(this IRoarResult result)
	{
		if (!result.IsTypicalSuccess())
		{
			throw result.ThrowIfNull().BuildException();
		}
		return result;
	}

	public static int AsInteger(this IRoarResult result)
	{
		return (int)result.ThrowIfNull().NativeError;
	}

	private static T ThrowIfNull<T>(this T result)
	{
		if (result != null)
		{
			return result;
		}
		throw new ArgumentNullException("result");
	}
}
