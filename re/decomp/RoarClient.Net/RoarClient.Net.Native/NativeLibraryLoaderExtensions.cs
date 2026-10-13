using System;

namespace RoarClient.Net.Native;

internal static class NativeLibraryLoaderExtensions
{
	public static bool SupportsDelegate(this IDynamicNativeLibraryLoader loader, Type delegateType)
	{
		return loader.HasFunctionName(delegateType.ThrowIfNotDelegate().Name);
	}

	public static bool SupportsDelegate<T>(this IDynamicNativeLibraryLoader loader) where T : Delegate
	{
		return loader.HasFunctionName(typeof(T).Name);
	}

	public static T GetFunctionDelegate<T>(this IDynamicNativeLibraryLoader loader, string functionName) where T : Delegate
	{
		return (T)loader.GetFunctionDelegate(functionName, typeof(T));
	}

	public static Delegate GetFunctionDelegateByTypeName(this IDynamicNativeLibraryLoader loader, Type delegateType)
	{
		return loader.GetFunctionDelegate(delegateType.Name, delegateType);
	}

	public static T GetFunctionDelegateByTypeName<T>(this IDynamicNativeLibraryLoader loader) where T : Delegate
	{
		return (T)loader.GetFunctionDelegateByTypeName(typeof(T));
	}

	public static bool IsDelegateType(this Type t)
	{
		return typeof(Delegate).IsAssignableFrom(t);
	}

	public static Type ThrowIfNotDelegate(this Type t)
	{
		if (!t.IsDelegateType())
		{
			throw new ArgumentException("Must be a delegate type.");
		}
		return t;
	}
}
