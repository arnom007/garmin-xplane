using System;
using System.Collections.Generic;
using System.Linq;

namespace LauncherCoreLib.Utility;

public static class LauncherLinqExtensions
{
	public static bool AllEqual<T>(this System.Collections.Generic.IEnumerable<T> elements, T operand, IEqualityComparer<T>? comparer = null)
	{
		IEqualityComparer<T> comparer2 = comparer;
		T operand2 = operand;
		IEqualityComparer<T> val = comparer2;
		comparer2 = (IEqualityComparer<T>)(((object)val) ?? ((object)EqualityComparer<T>.Default));
		return Enumerable.All<T>(elements, (Func<T, bool>)((T e) => comparer2.Equals(e, operand2)));
	}

	public static System.Collections.Generic.IEnumerable<T> WhereEqual<T>(this System.Collections.Generic.IEnumerable<T> stream, T operand, IEqualityComparer<T>? comparer = null)
	{
		IEqualityComparer<T> comparer2 = comparer;
		T operand2 = operand;
		IEqualityComparer<T> val = comparer2;
		comparer2 = (IEqualityComparer<T>)(((object)val) ?? ((object)EqualityComparer<T>.Default));
		return Enumerable.Where<T>(stream, (Func<T, bool>)((T e) => comparer2.Equals(e, operand2)));
	}
}
