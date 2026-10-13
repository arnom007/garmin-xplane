using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Utility;

public static class DataChangeNotification
{
	public static DataChangeNotification<T> Create<T>(T val, DataChangeType change = DataChangeType.Updated)
	{
		return new DataChangeNotification<T>(val, change);
	}

	public static System.Collections.Generic.IEnumerable<DataChangeNotification<T>> FilterByChangeType<T>(this System.Collections.Generic.IEnumerable<DataChangeNotification<T>> changes, params DataChangeType[] changeTypes)
	{
		DataChangeType[] changeTypes2 = changeTypes;
		if (changeTypes2.Length == 1)
		{
			DataChangeType changeType = changeTypes2[0];
			return Enumerable.Where<DataChangeNotification<T>>(changes, (Func<DataChangeNotification<T>, bool>)((DataChangeNotification<T> c) => c.Change == changeType));
		}
		return Enumerable.Where<DataChangeNotification<T>>(changes, (Func<DataChangeNotification<T>, bool>)((DataChangeNotification<T> c) => Enumerable.Contains<DataChangeType>((System.Collections.Generic.IEnumerable<DataChangeType>)changeTypes2, c.Change)));
	}

	public static System.Collections.Generic.IEnumerable<DataChangeType> FilterByChangeType(this System.Collections.Generic.IEnumerable<DataChangeType> changes, params DataChangeType[] changeTypes)
	{
		DataChangeType[] changeTypes2 = changeTypes;
		if (changeTypes2.Length == 1)
		{
			DataChangeType changeType = changeTypes2[0];
			return Enumerable.Where<DataChangeType>(changes, (Func<DataChangeType, bool>)((DataChangeType c) => c == changeType));
		}
		return Enumerable.Where<DataChangeType>(changes, (Func<DataChangeType, bool>)((DataChangeType c) => Enumerable.Contains<DataChangeType>((System.Collections.Generic.IEnumerable<DataChangeType>)changeTypes2, c)));
	}

	public static System.Collections.Generic.IEnumerable<T> OnCreateOrUpdate<T>(this System.Collections.Generic.IEnumerable<DataChangeNotification<T>> changes)
	{
		return changes.FilterByChangeType(DataChangeType.Updated, DataChangeType.Created).StripChangeType();
	}

	public static System.Collections.Generic.IEnumerable<TResult> StripChangeType<TResult>(this System.Collections.Generic.IEnumerable<DataChangeNotification<TResult>> changes)
	{
		return Enumerable.Select<DataChangeNotification<TResult>, TResult>(changes, (Func<DataChangeNotification<TResult>, TResult>)((DataChangeNotification<TResult> c) => c.Value));
	}

	public static IObservable<DataChangeNotification<T>> FilterByChangeType<T>(this IObservable<DataChangeNotification<T>> changes, params DataChangeType[] changeTypes)
	{
		DataChangeType[] changeTypes2 = changeTypes;
		if (changeTypes2.Length == 1)
		{
			DataChangeType changeType = changeTypes2[0];
			return changes.Where<DataChangeNotification<T>>((Func<DataChangeNotification<T>, bool>)((DataChangeNotification<T> c) => c.Change == changeType));
		}
		return changes.Where<DataChangeNotification<T>>((Func<DataChangeNotification<T>, bool>)((DataChangeNotification<T> c) => Enumerable.Contains<DataChangeType>((System.Collections.Generic.IEnumerable<DataChangeType>)changeTypes2, c.Change)));
	}

	public static IObservable<DataChangeType> FilterByChangeType(this IObservable<DataChangeType> changes, params DataChangeType[] changeTypes)
	{
		DataChangeType[] changeTypes2 = changeTypes;
		if (changeTypes2.Length == 1)
		{
			DataChangeType changeType = changeTypes2[0];
			return changes.Where<DataChangeType>((Func<DataChangeType, bool>)((DataChangeType c) => c == changeType));
		}
		return changes.Where<DataChangeType>((Func<DataChangeType, bool>)((DataChangeType c) => Enumerable.Contains<DataChangeType>((System.Collections.Generic.IEnumerable<DataChangeType>)changeTypes2, c)));
	}

	public static IObservable<T> OnCreateOrUpdate<T>(this IObservable<DataChangeNotification<T>> changes)
	{
		return changes.FilterByChangeType<T>(new DataChangeType[2]
		{
			DataChangeType.Updated,
			DataChangeType.Created
		}).StripChangeType<T>();
	}

	public static IObservable<TResult> StripChangeType<TResult>(this IObservable<DataChangeNotification<TResult>> changes)
	{
		return changes.Select<DataChangeNotification<TResult>, TResult>((Func<DataChangeNotification<TResult>, TResult>)((DataChangeNotification<TResult> c) => c.Value));
	}
}
public readonly struct DataChangeNotification<T> : IEquatable<DataChangeNotification<T>>
{
	public T Value
	{
		[CompilerGenerated]
		get;
	}

	public DataChangeType Change
	{
		[CompilerGenerated]
		get;
	}

	public DataChangeNotification(T val, DataChangeType change = DataChangeType.Updated)
	{
		Value = val;
		Change = change;
	}

	public static bool operator ==(DataChangeNotification<T> lhs, DataChangeNotification<T> rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(DataChangeNotification<T> lhs, DataChangeNotification<T> rhs)
	{
		return !lhs.Equals(rhs);
	}

	public bool Equals(DataChangeNotification<T> other)
	{
		if (Change == other.Change)
		{
			return EqualityComparer<T>.Default.Equals(Value, other.Value);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is DataChangeNotification<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int hashCode = ((object)Change).GetHashCode();
		T value = Value;
		return hashCode ^ ((value != null) ? ((object)value).GetHashCode() : 1753);
	}

	public override string ToString()
	{
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		object obj = Change;
		T value = Value;
		return string.Format((IFormatProvider)(object)invariantCulture, "{0} - {1}", obj, (object)(((value != null) ? ((object)value).ToString() : null) ?? "<NULL>"));
	}
}
