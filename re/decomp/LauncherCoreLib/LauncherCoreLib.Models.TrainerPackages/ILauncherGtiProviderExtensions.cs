using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class ILauncherGtiProviderExtensions
{
	public static IObservable<ILauncherGtiRunner> GtiExeAsSingleValueInStream(this ILauncherGtiProvider gtiProvider)
	{
		ILauncherGtiProvider gtiProvider2 = gtiProvider;
		return Observable.FromAsync<ILauncherGtiRunner>((Func<System.Threading.Tasks.Task<ILauncherGtiRunner>>)(() => gtiProvider2.GtiExe)).SingleAsync<ILauncherGtiRunner>();
	}
}
