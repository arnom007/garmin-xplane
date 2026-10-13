using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using IO.Win32.Events;
using LOLA.Logging;
using LOLA.Reactive;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class WindowWatcher
{
	public static IObservable<TrainerWindowScreenCoordinates> SelectCurrentWindow<T>(this IObservable<T> stream, IOsWindowOperations windowing, SimProcessSession session)
	{
		SimProcessSession session2 = session;
		IOsWindowOperations windowing2 = windowing;
		TrainerWindowScreenCoordinates initialLocation = session2.Simulator.InstallData.ParseWindowRect();
		return stream.TakeUntil<T, Unit>(((System.IDisposable)session2.Process.Exited).ToObservable()).SkipWhile<T>((Func<T, bool>)((T _) => session2.Process.MainWindowHandle == System.IntPtr.Zero)).Select<T, OsWindow>((Func<T, OsWindow>)((T _) => windowing2.GetCurrentWindow(session2.Process.MainWindowHandle)))
			.Select<OsWindow, TrainerWindowScreenCoordinates>((Func<OsWindow, TrainerWindowScreenCoordinates>)delegate(OsWindow osWindow)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				Rectangle rectangle = osWindow.Rectangle;
				int x = ((Rectangle)(ref rectangle)).X;
				rectangle = osWindow.Rectangle;
				int y = ((Rectangle)(ref rectangle)).Y;
				rectangle = osWindow.Rectangle;
				int width = ((Rectangle)(ref rectangle)).Width;
				rectangle = osWindow.Rectangle;
				return new TrainerWindowScreenCoordinates(x, y, width, ((Rectangle)(ref rectangle)).Height);
			})
			.SkipWhile<TrainerWindowScreenCoordinates>((Func<TrainerWindowScreenCoordinates, bool>)((TrainerWindowScreenCoordinates twsc) => twsc == initialLocation));
	}

	public static WatchTrainerWindowRectangle CreatePollingWatcher(IScheduler scheduler, IOsWindowOperations windowing, TimeSpan interval)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		IScheduler scheduler2 = scheduler;
		IOsWindowOperations windowing2 = windowing;
		return (SimProcessSession session) => TimingEvent.StartStreamAndConcat<long>(Observable.Interval(interval, scheduler2)).SelectCurrentWindow<TimingEvent>(windowing2, session);
	}

	public static WatchTrainerWindowRectangle CreateEventWatcher(IMessagePumpThreadDispatcher messagePumpThread, IOsWindowOperations windowing)
	{
		return Win32EventsAdapter.CreateWindowWatcher(messagePumpThread, windowing);
	}

	public static WatchTrainerWindowRectangle FallbackOn(this WatchTrainerWindowRectangle primary, WatchTrainerWindowRectangle secondary)
	{
		WatchTrainerWindowRectangle primary2 = primary;
		WatchTrainerWindowRectangle secondary2 = secondary;
		return delegate(SimProcessSession session)
		{
			SimProcessSession session2 = session;
			return primary2(session2).Catch<TrainerWindowScreenCoordinates, System.Exception>(delegate(System.Exception e)
			{
				Logger.LogFormattedException(e, $"[Primary window watcher][{session2.Simulator.InstallData.InstallationKey}]");
				return secondary2(session2);
			});
		};
	}
}
