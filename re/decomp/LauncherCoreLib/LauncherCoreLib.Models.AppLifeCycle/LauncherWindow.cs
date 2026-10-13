using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.SoftConfig;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.AppLifeCycle;

public sealed class LauncherWindow : IMainAppWindow, IAppWindow, System.IDisposable
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CSetWindowSizeAndLocation_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public LauncherWindow _003C_003E4__this;

		public WindowSizeAndLocation window;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			LauncherWindow launcherWindow = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00db;
				}
				BehaviorSubject<WindowSizeAndLocation> windowSizeAndLocation = launcherWindow._windowSizeAndLocation;
				bool flag = false;
				WindowSizeAndLocation value;
				try
				{
					Monitor.Enter((object)windowSizeAndLocation, ref flag);
					value = launcherWindow._windowSizeAndLocation.Value;
				}
				finally
				{
					if (num < 0 && flag)
					{
						Monitor.Exit((object)windowSizeAndLocation);
					}
				}
				if (!value.Equals(window))
				{
					string path = launcherWindow.BuildWindowFilePath();
					if (window.State != 0)
					{
						ConfiguredTaskAwaitable val = launcherWindow._lowLevel.ObjectPersistence.Commit(window, path).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CSetWindowSizeAndLocation_003Ed__12>(ref awaiter, ref this);
							return;
						}
						goto IL_00db;
					}
					launcherWindow._lowLevel.FileSystem.DeleteIfExists(path);
					goto IL_00f7;
				}
				goto end_IL_000e;
				IL_00db:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				goto IL_00f7;
				IL_00f7:
				windowSizeAndLocation = launcherWindow._windowSizeAndLocation;
				flag = false;
				try
				{
					Monitor.Enter((object)windowSizeAndLocation, ref flag);
					launcherWindow._windowSizeAndLocation.OnNext(window);
				}
				finally
				{
					if (num < 0 && flag)
					{
						Monitor.Exit((object)windowSizeAndLocation);
					}
				}
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	public const string DEFAULT_WINDOW_TITLE = "Starting...";

	private readonly ILowLevelDal _lowLevel;

	private readonly BehaviorSubject<string> _windowTitle = new BehaviorSubject<string>("Starting...");

	private readonly BehaviorSubject<WindowSizeAndLocation> _windowSizeAndLocation;

	public IObservable<string> WhenWindowTitleChanges
	{
		get
		{
			IObservable<string> val = ((IObservable<string>)(object)_windowTitle).AsObservable<string>();
			if (SoftConfigurations.IsDebug)
			{
				val = val.Select<string, string>((Func<string, string>)((string title) => title + " [DEBUG]"));
			}
			return val;
		}
	}

	public string WindowTitle
	{
		get
		{
			return _windowTitle.Value;
		}
		set
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			_windowTitle.OnNext(value);
		}
	}

	public IObservable<WindowSizeAndLocation> WhenWindowChangesSizeOrLocation => ((IObservable<WindowSizeAndLocation>)(object)_windowSizeAndLocation).AsObservable<WindowSizeAndLocation>();

	public LauncherWindow(ILowLevelDal lowLevelDal)
	{
		_lowLevel = lowLevelDal;
		_windowSizeAndLocation = new BehaviorSubject<WindowSizeAndLocation>(LoadWindowSynchronously());
	}

	[AsyncStateMachine(typeof(_003CSetWindowSizeAndLocation_003Ed__12))]
	public System.Threading.Tasks.Task SetWindowSizeAndLocation(WindowSizeAndLocation window)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CSetWindowSizeAndLocation_003Ed__12 _003CSetWindowSizeAndLocation_003Ed__ = default(_003CSetWindowSizeAndLocation_003Ed__12);
		_003CSetWindowSizeAndLocation_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CSetWindowSizeAndLocation_003Ed__._003C_003E4__this = this;
		_003CSetWindowSizeAndLocation_003Ed__.window = window;
		_003CSetWindowSizeAndLocation_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CSetWindowSizeAndLocation_003Ed__._003C_003Et__builder)).Start<_003CSetWindowSizeAndLocation_003Ed__12>(ref _003CSetWindowSizeAndLocation_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CSetWindowSizeAndLocation_003Ed__._003C_003Et__builder)).Task;
	}

	public void Dispose()
	{
		_windowSizeAndLocation.Dispose();
		_windowTitle.Dispose();
	}

	private WindowSizeAndLocation LoadWindowSynchronously()
	{
		//IL_003d: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		WindowSizeAndLocation windowSizeAndLocation = null;
		string path = BuildWindowFilePath();
		if (_lowLevel.FileSystem.PathExists(path))
		{
			try
			{
				windowSizeAndLocation = _lowLevel.ObjectPersistence.LoadSync<WindowSizeAndLocation>(path);
			}
			catch (FileNotFoundException)
			{
			}
			catch (SerializationException val2)
			{
				Logger.Log((System.Exception)val2, "Loading window state.");
			}
		}
		return windowSizeAndLocation ?? new WindowSizeAndLocation(OsWindowState.None, new Rectangle(0, 0, 0, 0));
	}

	private string BuildWindowFilePath()
	{
		return Path.Combine(_lowLevel.Deployment.UserAppDataDirPath, "window.json");
	}
}
