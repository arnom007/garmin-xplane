using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class HangarUtil
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPrepareStartingInfoWithUniqueId_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftListData> _003C_003Et__builder;

		public AircraftListData requestedStart;

		public System.Collections.Generic.IEnumerable<AircraftIdentifier> idsToCheck;

		public Func<AircraftIdentifier, System.Threading.Tasks.Task<bool>> reserveId;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AircraftListData result2;
			try
			{
				ConfiguredTaskAwaiter<AircraftIdentifier> awaiter;
				if (num != 0)
				{
					if (requestedStart.AircraftIdentifier.IsValid)
					{
						goto IL_009e;
					}
					awaiter = ReserveFirstUnusedIdFrom(idsToCheck, reserveId).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003CPrepareStartingInfoWithUniqueId_003Ed__0>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftIdentifier>);
					num = (_003C_003E1__state = -1);
				}
				AircraftIdentifier result = awaiter.GetResult();
				requestedStart = requestedStart.WithNewIdentifier(result);
				goto IL_009e;
				IL_009e:
				result2 = requestedStart;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CReserveFirstUnusedIdFrom_003Ed__1 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftIdentifier> _003C_003Et__builder;

		public System.Collections.Generic.IEnumerable<AircraftIdentifier> possibleIds;

		public Func<AircraftIdentifier, System.Threading.Tasks.Task<bool>> reserveId;

		private System.Collections.Generic.IEnumerator<AircraftIdentifier> _003C_003E7__wrap1;

		private AircraftIdentifier _003Cid_003E5__3;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AircraftIdentifier result;
			try
			{
				if (num != 0)
				{
					_003C_003E7__wrap1 = possibleIds.GetEnumerator();
				}
				try
				{
					if (num != 0)
					{
						goto IL_00ba;
					}
					ConfiguredTaskAwaiter<bool> awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<bool>);
					num = (_003C_003E1__state = -1);
					goto IL_009c;
					IL_009c:
					if (!awaiter.GetResult())
					{
						_003Cid_003E5__3 = default(AircraftIdentifier);
						goto IL_00ba;
					}
					result = _003Cid_003E5__3;
					goto end_IL_0007;
					IL_00ba:
					if (((System.Collections.IEnumerator)_003C_003E7__wrap1).MoveNext())
					{
						_003Cid_003E5__3 = _003C_003E7__wrap1.Current;
						awaiter = reserveId.Invoke(_003Cid_003E5__3).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CReserveFirstUnusedIdFrom_003Ed__1>(ref awaiter, ref this);
							return;
						}
						goto IL_009c;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						((System.IDisposable)_003C_003E7__wrap1).Dispose();
					}
				}
				_003C_003E7__wrap1 = null;
				throw new ArgumentOutOfRangeException("possibleIds", "The possible ids are all taken.");
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CPrepareStartingInfoWithUniqueId_003Ed__0))]
	public static async System.Threading.Tasks.Task<AircraftListData> PrepareStartingInfoWithUniqueId(AircraftListData requestedStart, System.Collections.Generic.IEnumerable<AircraftIdentifier> idsToCheck, Func<AircraftIdentifier, System.Threading.Tasks.Task<bool>> reserveId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!requestedStart.AircraftIdentifier.IsValid)
		{
			requestedStart = requestedStart.WithNewIdentifier(await ReserveFirstUnusedIdFrom(idsToCheck, reserveId).DetachContext());
		}
		return requestedStart;
	}

	[AsyncStateMachine(typeof(_003CReserveFirstUnusedIdFrom_003Ed__1))]
	public static async System.Threading.Tasks.Task<AircraftIdentifier> ReserveFirstUnusedIdFrom(System.Collections.Generic.IEnumerable<AircraftIdentifier> possibleIds, Func<AircraftIdentifier, System.Threading.Tasks.Task<bool>> reserveId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IEnumerator<AircraftIdentifier> enumerator = possibleIds.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				AircraftIdentifier id = enumerator.Current;
				if (await reserveId.Invoke(id).DetachContext())
				{
					return id;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		throw new ArgumentOutOfRangeException("possibleIds", "The possible ids are all taken.");
	}

	public static IObservable<DataChangeNotification<SimInstallKey>> WhenSimulatorIsChangedOnAircraft(IObservable<DataChangeNotification<SimAircraftKey>> whenSimulatorChanges, AircraftIdentifier aircraftIdentifier)
	{
		return whenSimulatorChanges.Where<DataChangeNotification<SimAircraftKey>>((Func<DataChangeNotification<SimAircraftKey>, bool>)((DataChangeNotification<SimAircraftKey> change) => change.Value.AircraftIdentifier == aircraftIdentifier)).Select<DataChangeNotification<SimAircraftKey>, DataChangeNotification<SimInstallKey>>((Func<DataChangeNotification<SimAircraftKey>, DataChangeNotification<SimInstallKey>>)((DataChangeNotification<SimAircraftKey> change) => DataChangeNotification.Create(change.Value.InstallationKey, change.Change)));
	}

	public static System.Collections.Generic.IEnumerable<DataChangeNotification<SimAircraftKey>> ToAircraftKeysChanged(this System.Collections.Generic.IEnumerable<DataChangeNotification<SimInstallKey>> simChanges, AircraftIdentifier aircaftIdentifier)
	{
		return Enumerable.Select<DataChangeNotification<SimInstallKey>, DataChangeNotification<SimAircraftKey>>(simChanges, (Func<DataChangeNotification<SimInstallKey>, DataChangeNotification<SimAircraftKey>>)((DataChangeNotification<SimInstallKey> simChange) => DataChangeNotification.Create(new SimAircraftKey(aircaftIdentifier, simChange.Value), simChange.Change)));
	}
}
