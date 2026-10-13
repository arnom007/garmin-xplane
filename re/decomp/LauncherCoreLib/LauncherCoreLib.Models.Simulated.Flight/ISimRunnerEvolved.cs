using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LOLA.Logging;
using LOLA.Reactive;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Properties;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISimRunnerEvolved
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CSimActions_003Eg__Abort_007C9_3_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public ISimRunnerEvolved _003C_003E4__this;

		private ConfiguredTaskAwaiter<ISimSessionEvolved?> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ISimRunnerEvolved simRunnerEvolved = _003C_003E4__this;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<ISimSessionEvolved> awaiter;
					if (num != 0)
					{
						awaiter = simRunnerEvolved.Sessions.GetUpdatedValue().ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISimSessionEvolved>, _003C_003CSimActions_003Eg__Abort_007C9_3_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISimSessionEvolved>);
						num = (_003C_003E1__state = -1);
					}
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

	ISimSessionCoordinator Coordinator { get; }

	IReactiveEntity<PowerStage> Stage { get; }

	IReactiveEntity<bool> CanPowerOn { get; }

	IReactiveEntity<ISimSessionEvolved?> Sessions { get; }

	ISimSessionEvolved StartSession();

	IObservable<IUserAction> SimActions()
	{
		return (IObservable<IUserAction>)(object)Observable.CombineLatest<PowerStage, bool, ValueTuple<PowerStage, bool>>(Stage.GetCachedThenUpdatedAndThenContinualValues(), CanPowerOn.GetCachedThenUpdatedAndThenContinualValues(), (PowerStage stage, bool canRun) => new ValueTuple<PowerStage, bool>(stage, canRun)).Select<ValueTuple<PowerStage, bool>, UserAction>((Func<ValueTuple<PowerStage, bool>, UserAction>)([CompilerGenerated] (ValueTuple<PowerStage, bool> combined) => combined.Item1 switch
		{
			PowerStage.PoweredDown => new UserAction(Resources.POWER_ON_BUTTON_TEXT, "Turn on current aircraft's avionics.", new Action(Launch), ActionStyling.Green, "POWER_SIM_ON").ToCanDoOverride(combined.Item2), 
			PowerStage.PoweringUp => new UserAction(Resources.POWER_OFF_BUTTON_TEXT, "Avionics are powering on.", null, ActionStyling.Default, "POWERING_SIM_ON"), 
			PowerStage.PoweredUp => new UserAction(Resources.POWER_OFF_BUTTON_TEXT, "Turn off current aircraft's avionics.", new Action(Abort), ActionStyling.Red, "POWER_SIM_OFF"), 
			PowerStage.PoweringDown => new UserAction(Resources.POWER_ON_BUTTON_TEXT, "Avionics are powering off.", null, ActionStyling.Default, "POWERING_SIM_OFF"), 
			_ => throw new NotSupportedException(((object)combined.Item1).ToString()), 
		}));
		[AsyncStateMachine(typeof(_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed))]
		[CompilerGenerated]
		private void Abort()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed _003C_003CSimActions_003Eg__Abort_007C9_3_003Ed = default(_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed);
			_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
			_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed._003C_003E4__this = this;
			_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed._003C_003E1__state = -1;
			((AsyncVoidMethodBuilder)(ref _003C_003CSimActions_003Eg__Abort_007C9_3_003Ed._003C_003Et__builder)).Start<_003C_003CSimActions_003Eg__Abort_007C9_3_003Ed>(ref _003C_003CSimActions_003Eg__Abort_007C9_3_003Ed);
		}
		[CompilerGenerated]
		private void Launch()
		{
			try
			{
				StartSession();
			}
			catch (System.Exception e)
			{
				Logger.LogFormattedException(e, "Attempting to start a sim session.");
				throw;
			}
		}
	}
}
