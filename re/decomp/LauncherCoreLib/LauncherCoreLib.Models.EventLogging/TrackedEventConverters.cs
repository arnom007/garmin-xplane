using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;

namespace LauncherCoreLib.Models.EventLogging;

public static class TrackedEventConverters
{
	public delegate EitherAsync<TrackedEventError, TrackedEventContainer> TrackedEventConverter(TrackedEventDataDescriptor descriptor);

	public delegate Func<TrackedEventConverter, TrackedEventConverter> ConverterFactory(IConverterFactoryApi api);

	public static ConverterFactory Identity()
	{
		return (IConverterFactoryApi _) => (TrackedEventConverter next) => next;
	}

	public static ConverterFactory AsFactory(this TrackedEventConverter converter)
	{
		TrackedEventConverter converter2 = converter;
		return (IConverterFactoryApi api) => (TrackedEventConverter _) => converter2;
	}

	public static ConverterFactory ComposeConverterFactories(params ConverterFactory[] specificToBasicFactories)
	{
		ConverterFactory[] specificToBasicFactories2 = specificToBasicFactories;
		if (specificToBasicFactories2.Length == 0)
		{
			return Identity();
		}
		if (specificToBasicFactories2.Length == 1)
		{
			return specificToBasicFactories2[0];
		}
		return delegate(IConverterFactoryApi api)
		{
			IConverterFactoryApi api2 = api;
			return Enumerable.Aggregate<Func<TrackedEventConverter, TrackedEventConverter>>((System.Collections.Generic.IEnumerable<Func<TrackedEventConverter, TrackedEventConverter>>)Enumerable.ToList<Func<TrackedEventConverter, TrackedEventConverter>>(Enumerable.Select<ConverterFactory, Func<TrackedEventConverter, TrackedEventConverter>>((System.Collections.Generic.IEnumerable<ConverterFactory>)specificToBasicFactories2, (Func<ConverterFactory, Func<TrackedEventConverter, TrackedEventConverter>>)((ConverterFactory m) => m(api2)))), (Func<Func<TrackedEventConverter, TrackedEventConverter>, Func<TrackedEventConverter, TrackedEventConverter>, Func<TrackedEventConverter, TrackedEventConverter>>)((Func<TrackedEventConverter, TrackedEventConverter> a, Func<TrackedEventConverter, TrackedEventConverter> b) => (TrackedEventConverter d) => a.Invoke(b.Invoke(d))));
		};
	}

	public static ConverterFactory Compose(this ConverterFactory outer, ConverterFactory inner)
	{
		ConverterFactory outer2 = outer;
		ConverterFactory inner2 = inner;
		return delegate(IConverterFactoryApi api)
		{
			IConverterFactoryApi api2 = api;
			return (TrackedEventConverter converter) => outer2(api2).Invoke(inner2(api2).Invoke(converter));
		};
	}

	public static TrackedEventConverter CreateDependencyFreeConverter()
	{
		return (TrackedEventDataDescriptor d) => (d is GenericTrackedEventDataDescriptor genericTrackedEventDataDescriptor) ? ((EitherAsync<TrackedEventError, TrackedEventContainer>)genericTrackedEventDataDescriptor.Instance()) : ((EitherAsync<TrackedEventError, TrackedEventContainer>)UnknownTrackedEventType.Create(d));
	}

	public static TrackedEventConverter CoreLibTimeRangeDecorator(this TrackedEventConverter inner, ITimeRangeProvider timeRangeProvider)
	{
		ITimeRangeProvider timeRangeProvider2 = timeRangeProvider;
		TrackedEventConverter inner2 = inner;
		return delegate(TrackedEventDataDescriptor d)
		{
			if (d is EndLauncherSessionDataDescriptor endLauncherSessionDataDescriptor)
			{
				return endLauncherSessionDataDescriptor.Instance(timeRangeProvider2);
			}
			if (d is EndTrainerSessionDataDescriptor endTrainerSessionDataDescriptor)
			{
				return endTrainerSessionDataDescriptor.Instance(timeRangeProvider2);
			}
			return (d is NavigationDataDescriptor navigationDataDescriptor) ? ((EitherAsync<TrackedEventError, TrackedEventContainer>)navigationDataDescriptor.Instance(timeRangeProvider2)) : inner2(d);
		};
	}
}
