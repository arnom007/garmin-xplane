using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class UnknownTrackedEventType : TrackedEventError
{
	[field: CompilerGenerated]
	public System.Type TargetType
	{
		[CompilerGenerated]
		get;
	}

	public UnknownTrackedEventType(System.Type targetType)
	{
		TargetType = targetType;
	}

	public static UnknownTrackedEventType<T> Create<T>(T value)
	{
		return Create(typeof(T), value);
	}

	public static UnknownTrackedEventType<T> Create<TTarget, T>(T value) where T : TTarget
	{
		return Create(typeof(TTarget), value);
	}

	public virtual string ToString()
	{
		return $"Error: unknown tracked event of type {TargetType}";
	}

	private static UnknownTrackedEventType<T> Create<T>(System.Type targetType, T value)
	{
		return new UnknownTrackedEventType<T>(targetType, value);
	}
}
public class UnknownTrackedEventType<T> : UnknownTrackedEventType
{
	public T UnknownValue
	{
		[CompilerGenerated]
		get;
	}

	public UnknownTrackedEventType(System.Type targetType, T unknownValue)
		: base(targetType)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (typeof(T).IsAssignableFrom(base.TargetType))
		{
			throw new ArgumentException($"{typeof(T)} does not inherit from {base.TargetType}");
		}
		UnknownValue = unknownValue;
	}
}
