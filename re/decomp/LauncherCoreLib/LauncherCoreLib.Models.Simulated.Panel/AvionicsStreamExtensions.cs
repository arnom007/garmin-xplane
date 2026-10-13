using System;
using System.Reactive.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public static class AvionicsStreamExtensions
{
	public static IObservable<AvionicsModel> SupportedNavigators(this IObservable<AvionicsModel> supportedSims)
	{
		return supportedSims.FilterByAvionicsType(AvionicsType.Navigator);
	}

	public static IObservable<AvionicsModel> SupportedDisplays(this IObservable<AvionicsModel> supportedSims)
	{
		return supportedSims.FilterByAvionicsType(AvionicsType.Display);
	}

	public static IObservable<AvionicsModel> FilterByAvionicsType(this IObservable<AvionicsModel> models, AvionicsType avionicsType)
	{
		return models.Where<AvionicsModel>((Func<AvionicsModel, bool>)((AvionicsModel av) => av.AvionicsType == avionicsType));
	}
}
