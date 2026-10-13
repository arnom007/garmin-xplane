using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Properties;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class SimRunnerCoordinatorAdapter : IRunSimulators, System.IDisposable
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAbort_003Ed__21 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public SimRunnerCoordinatorAdapter _003C_003E4__this;

		private ConfiguredTaskAwaiter<ISimRunnerEvolved> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ISimSessionEvolved> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SimRunnerCoordinatorAdapter simRunnerCoordinatorAdapter = _003C_003E4__this;
			try
			{
				_ = 1;
				try
				{
					ConfiguredTaskAwaiter<ISimSessionEvolved> awaiter;
					ConfiguredTaskAwaiter<ISimRunnerEvolved> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ISimSessionEvolved>);
							num = (_003C_003E1__state = -1);
							goto IL_00ec;
						}
						awaiter2 = simRunnerCoordinatorAdapter._coordinator.CurrentlyRunning.GetUpdatedValue().DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISimRunnerEvolved>, _003CAbort_003Ed__21>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISimRunnerEvolved>);
						num = (_003C_003E1__state = -1);
					}
					awaiter = awaiter2.GetResult().Sessions.GetUpdatedValue().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISimSessionEvolved>, _003CAbort_003Ed__21>(ref awaiter, ref this);
						return;
					}
					goto IL_00ec;
					IL_00ec:
					awaiter.GetResult()?.InitiateSessionShutdown();
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Attempting to abort current sim session.");
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CLaunch_003Ed__22 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public SimRunnerCoordinatorAdapter _003C_003E4__this;

		private ConfiguredTaskAwaiter<ISimRunnerEvolved> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SimRunnerCoordinatorAdapter simRunnerCoordinatorAdapter = _003C_003E4__this;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<ISimRunnerEvolved> awaiter;
					if (num != 0)
					{
						awaiter = simRunnerCoordinatorAdapter._sharedRunnerStream.FirstAsync<ISimRunnerEvolved>().ToTask<ISimRunnerEvolved>().DetachContext()
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISimRunnerEvolved>, _003CLaunch_003Ed__22>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISimRunnerEvolved>);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult().StartSession();
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Attempting to start a sim session for the current aircraft.");
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly IUnitOfWork _unitOfWork;

	private readonly ISimSessionCoordinator _coordinator;

	private readonly ISquawkBoard _squawks;

	private readonly System.IDisposable _subscription;

	private readonly IObservable<ISimRunnerEvolved> _sharedRunnerStream;

	private readonly IObservable<PowerStage> _sharedStages;

	[field: CompilerGenerated]
	public bool CanRun
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public bool IsRunning
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public IObservable<bool> WhenCanRunChanges => _sharedRunnerStream.Select<ISimRunnerEvolved, IObservable<bool>>((Func<ISimRunnerEvolved, IObservable<bool>>)((ISimRunnerEvolved r) => r.CanPowerOn.GetCachedThenUpdatedAndThenContinualValues())).Switch<bool>().DistinctUntilChanged<bool>();

	public IObservable<bool> WhenIsRunningChanges => _sharedStages.Select<PowerStage, bool>((Func<PowerStage, bool>)((PowerStage stage) => stage != PowerStage.PoweredDown)).DistinctUntilChanged<bool>().Merge<bool>(_sharedRunnerStream.Select<ISimRunnerEvolved, IObservable<bool>>((Func<ISimRunnerEvolved, IObservable<bool>>)((ISimRunnerEvolved r) => r.Sessions.GetHotContinualValues().SelectMany<ISimSessionEvolved, bool>((Func<ISimSessionEvolved, System.Threading.Tasks.Task<bool>>)([AsyncStateMachine(typeof(_003C_003Ec._003C_003Cget_WhenIsRunningChanges_003Eb__17_2_003Ed))] async (ISimSessionEvolved s) =>
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (s != null)
		{
			await s.Completion.DetachContext();
		}
		return false;
	})).IgnoreElements<bool>())).Concat<bool>());

	public IObservable<IUserAction> WhenActionChanges => (IObservable<IUserAction>)(object)Observable.CombineLatest<PowerStage, bool, ValueTuple<PowerStage, bool>>(_sharedStages, WhenCanRunChanges, (PowerStage stage, bool canRun) => new ValueTuple<PowerStage, bool>(stage, canRun)).Select<ValueTuple<PowerStage, bool>, UserAction>((Func<ValueTuple<PowerStage, bool>, UserAction>)([CompilerGenerated] (ValueTuple<PowerStage, bool> combined) =>
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		switch (combined.Item1)
		{
		case PowerStage.PoweredDown:
			return new UserAction(Resources.POWER_ON_BUTTON_TEXT, "Turn on current aircraft's avionics.", new Action(Launch), ActionStyling.Green, "POWER_SIM_ON").ToCanDoOverride(combined.Item2);
		case PowerStage.PoweringUp:
			return new UserAction(Resources.POWER_OFF_BUTTON_TEXT, "Avionics are powering on.", null, ActionStyling.Default, "POWERING_SIM_ON");
		case PowerStage.PoweredUp:
			return new UserAction(Resources.POWER_OFF_BUTTON_TEXT, "Turn off current aircraft's avionics.", new Action(Abort), ActionStyling.Red, "POWER_SIM_OFF");
		case PowerStage.PoweringDown:
			return new UserAction(Resources.POWER_ON_BUTTON_TEXT, "Avionics are powering off.", null, ActionStyling.Default, "POWERING_SIM_OFF");
		default:
			throw new NotSupportedException(((object)combined.Item1).ToString());
		}
	}));

	public SimRunnerCoordinatorAdapter(IUnitOfWork unitOfWork, ISimSessionCoordinator coordinator, ISquawkBoard squawks)
	{
		_unitOfWork = unitOfWork;
		_coordinator = coordinator;
		_squawks = squawks;
		_sharedRunnerStream = RestartableStream.FromFactory<ISimRunnerEvolved>([CompilerGenerated] () => _unitOfWork.Aircraft.ContinualSelectedAircraftOrDefault().Select<AircraftIdentifier, ISimRunnerEvolved>((Func<AircraftIdentifier, ISimRunnerEvolved>)([CompilerGenerated] (AircraftIdentifier ai) => (ai == AircraftIdentifier.Default) ? ((ISimRunnerEvolved)new NeverRunner(_coordinator)) : ((ISimRunnerEvolved)_coordinator.GetRunnerForAircraft(new ReactiveAircraft(_unitOfWork, ai))))).Replay<ISimRunnerEvolved>(1)).RefCount();
		_sharedStages = RestartableStream.FromFactory<PowerStage>([CompilerGenerated] () => _sharedRunnerStream.Select<ISimRunnerEvolved, IObservable<PowerStage>>((Func<ISimRunnerEvolved, IObservable<PowerStage>>)((ISimRunnerEvolved r) => r.Stage.GetCachedThenUpdatedAndThenContinualValues())).Switch<PowerStage>().DistinctUntilChanged<PowerStage>()
			.Replay<PowerStage>(1)).RefCount();
		_subscription = UpdateCanRunProperty().Merge<Unit>(UpdateIsRunningProperty()).Merge<Unit>(SimRunnerBehaviors.SimPowerSquawks(_sharedStages, _sharedRunnerStream).Do<Squawk>((Action<Squawk>)([CompilerGenerated] (Squawk s) =>
		{
			_squawks.Add(s);
		})).ToSideEffectStream<Squawk>()).Subscribe<Unit>();
	}

	[AsyncStateMachine(typeof(_003CAbort_003Ed__21))]
	public void Abort()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CAbort_003Ed__21 _003CAbort_003Ed__ = default(_003CAbort_003Ed__21);
		_003CAbort_003Ed__._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		_003CAbort_003Ed__._003C_003E4__this = this;
		_003CAbort_003Ed__._003C_003E1__state = -1;
		((AsyncVoidMethodBuilder)(ref _003CAbort_003Ed__._003C_003Et__builder)).Start<_003CAbort_003Ed__21>(ref _003CAbort_003Ed__);
	}

	[AsyncStateMachine(typeof(_003CLaunch_003Ed__22))]
	public void Launch()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CLaunch_003Ed__22 _003CLaunch_003Ed__ = default(_003CLaunch_003Ed__22);
		_003CLaunch_003Ed__._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		_003CLaunch_003Ed__._003C_003E4__this = this;
		_003CLaunch_003Ed__._003C_003E1__state = -1;
		((AsyncVoidMethodBuilder)(ref _003CLaunch_003Ed__._003C_003Et__builder)).Start<_003CLaunch_003Ed__22>(ref _003CLaunch_003Ed__);
	}

	public void Dispose()
	{
		_subscription.Dispose();
	}

	private IObservable<Unit> UpdateCanRunProperty()
	{
		return WhenCanRunChanges.Do<bool>((Action<bool>)([CompilerGenerated] (bool canRun) =>
		{
			CanRun = canRun;
		})).ToSideEffectStream<bool>();
	}

	private IObservable<Unit> UpdateIsRunningProperty()
	{
		return WhenIsRunningChanges.Catch<bool, System.Exception>((System.Exception e) => Observable.Return(value: false).Concat<bool>(Observable.Throw<bool>(e))).Retry<bool>().Do<bool>((Action<bool>)([CompilerGenerated] (bool isRunning) =>
		{
			IsRunning = isRunning;
		}))
			.ToSideEffectStream<bool>();
	}
}
