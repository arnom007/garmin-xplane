using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.DAL;

public static class UnitOfWorkExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CFlushEverything_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IUnitOfWork unitOfWork;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = FlushHelpers.FlushAll(unitOfWork.Simulators, unitOfWork.Aircraft).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CFlushEverything_003Ed__0>(ref awaiter, ref this);
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

	[AsyncStateMachine(typeof(_003CFlushEverything_003Ed__0))]
	public static System.Threading.Tasks.Task FlushEverything(this IUnitOfWork unitOfWork)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CFlushEverything_003Ed__0 _003CFlushEverything_003Ed__ = default(_003CFlushEverything_003Ed__0);
		_003CFlushEverything_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CFlushEverything_003Ed__.unitOfWork = unitOfWork;
		_003CFlushEverything_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CFlushEverything_003Ed__._003C_003Et__builder)).Start<_003CFlushEverything_003Ed__0>(ref _003CFlushEverything_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CFlushEverything_003Ed__._003C_003Et__builder)).Task;
	}

	public static void InvalidateAircraftData(this IUnitOfWork unitOfWork, AircraftIdentifier aircraftIdentifier)
	{
		System.Collections.Generic.IEnumerator<ICacheAircraftData> enumerator = Enumerable.OfType<ICacheAircraftData>((System.Collections.IEnumerable)(object)new object[2] { unitOfWork.Simulators, unitOfWork.FamilyData }).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				enumerator.Current.InvalidateAircraftData(aircraftIdentifier);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}
}
