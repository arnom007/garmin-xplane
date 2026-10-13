using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class CompositeSimResetter : IResetSimulators
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		public ReactiveSimulator simulator;

		internal System.Threading.Tasks.Task _003CResetSimulator_003Eb__0(IResetSimulators r)
		{
			return r.ResetSimulator(simulator);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CResetSimulator_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator simulator;

		public CompositeSimResetter _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CompositeSimResetter compositeSimResetter = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass4_0
					{
						simulator = simulator
					};
					ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<IResetSimulators, System.Threading.Tasks.Task>(compositeSimResetter._resetters, (Func<IResetSimulators, System.Threading.Tasks.Task>)((IResetSimulators r) => r.ResetSimulator(CS_0024_003C_003E8__locals0.simulator)))).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetSimulator_003Ed__4>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

	private readonly System.Collections.Generic.IEnumerable<IResetSimulators> _resetters;

	public CompositeSimResetter(System.Collections.Generic.IEnumerable<IResetSimulators> resetters)
	{
		_resetters = resetters;
	}

	public CompositeSimResetter(params IResetSimulators[] resetters)
		: this((System.Collections.Generic.IEnumerable<IResetSimulators>)resetters)
	{
	}

	public IObservable<bool> CanBeReset(ReactiveSimulator simulator)
	{
		ReactiveSimulator simulator2 = simulator;
		return Enumerable.Select<IResetSimulators, IObservable<bool>>(_resetters, (Func<IResetSimulators, IObservable<bool>>)((IResetSimulators r) => r.CanBeReset(simulator2))).CombineLatestOrEmpty().Select<System.Collections.Generic.IList<bool>, bool>((Func<System.Collections.Generic.IList<bool>, bool>)((System.Collections.Generic.IList<bool> allResults) => Enumerable.Any<bool>((System.Collections.Generic.IEnumerable<bool>)allResults, (Func<bool, bool>)((bool canReset) => canReset))));
	}

	[AsyncStateMachine(typeof(_003CResetSimulator_003Ed__4))]
	public System.Threading.Tasks.Task ResetSimulator(ReactiveSimulator simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CResetSimulator_003Ed__4 _003CResetSimulator_003Ed__ = default(_003CResetSimulator_003Ed__4);
		_003CResetSimulator_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CResetSimulator_003Ed__._003C_003E4__this = this;
		_003CResetSimulator_003Ed__.simulator = simulator;
		_003CResetSimulator_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CResetSimulator_003Ed__._003C_003Et__builder)).Start<_003CResetSimulator_003Ed__4>(ref _003CResetSimulator_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CResetSimulator_003Ed__._003C_003Et__builder)).Task;
	}
}
