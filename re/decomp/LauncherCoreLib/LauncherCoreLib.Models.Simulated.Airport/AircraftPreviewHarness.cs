using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;

namespace LauncherCoreLib.Models.Simulated.Airport;

public sealed class AircraftPreviewHarness : System.IDisposable
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreate_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftPreviewHarness> _003C_003Et__builder;

		public IUnitOfWork unitOfWork;

		public AircraftSnapshotData snapshot;

		private TemporaryUnitOfWork _003CinMemory_003E5__2;

		private AircraftSnapshotData _003Ctemp_003E5__3;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AircraftPreviewHarness result;
			try
			{
				if ((uint)num > 1u)
				{
					_003CinMemory_003E5__2 = new TemporaryUnitOfWork(unitOfWork);
				}
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
						num = (_003C_003E1__state = -1);
						goto IL_00c4;
					}
					ConfiguredTaskAwaiter awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0153;
					}
					if (snapshot != null)
					{
						awaiter = _003CinMemory_003E5__2.Aircraft.CreateNewAircraft(snapshot.ListData).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CCreate_003Ed__5>(ref awaiter, ref this);
							return;
						}
						goto IL_00c4;
					}
					_003CinMemory_003E5__2.Dispose();
					result = new AircraftPreviewHarness(_003CinMemory_003E5__2, null);
					goto end_IL_001d;
					IL_0153:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					ReactiveAircraft previewReadyAircraft = new ReactiveAircraft(_003CinMemory_003E5__2, _003Ctemp_003E5__3);
					result = new AircraftPreviewHarness(_003CinMemory_003E5__2, previewReadyAircraft);
					goto end_IL_001d;
					IL_00c4:
					AircraftSnapshotData result2 = awaiter.GetResult();
					_003Ctemp_003E5__3 = snapshot.WithNewIdentifier(result2.AircraftIdentifier);
					ConfiguredTaskAwaitable val = _003CinMemory_003E5__2.Aircraft.UpdateAircraft(_003Ctemp_003E5__3).DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreate_003Ed__5>(ref awaiter2, ref this);
						return;
					}
					goto IL_0153;
					end_IL_001d:;
				}
				catch (System.Exception)
				{
					_003CinMemory_003E5__2.Dispose();
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CinMemory_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CinMemory_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly TemporaryUnitOfWork _temporaryUnitOfWork;

	[field: CompilerGenerated]
	public ReactiveAircraft? Aircraft
	{
		[CompilerGenerated]
		get;
	}

	private AircraftPreviewHarness(TemporaryUnitOfWork unitOfWork, ReactiveAircraft? previewReadyAircraft)
	{
		_temporaryUnitOfWork = unitOfWork;
		Aircraft = previewReadyAircraft;
	}

	[AsyncStateMachine(typeof(_003CCreate_003Ed__5))]
	public static async System.Threading.Tasks.Task<AircraftPreviewHarness> Create(IUnitOfWork unitOfWork, AircraftSnapshotData? snapshot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TemporaryUnitOfWork inMemory = new TemporaryUnitOfWork(unitOfWork);
		try
		{
			if (snapshot == null)
			{
				inMemory.Dispose();
				return new AircraftPreviewHarness(inMemory, null);
			}
			AircraftSnapshotData temp = snapshot.WithNewIdentifier((await inMemory.Aircraft.CreateNewAircraft(snapshot.ListData).DetachContext()).AircraftIdentifier);
			await inMemory.Aircraft.UpdateAircraft(temp).DetachContext();
			ReactiveAircraft previewReadyAircraft = new ReactiveAircraft(inMemory, temp);
			return new AircraftPreviewHarness(inMemory, previewReadyAircraft);
		}
		catch (System.Exception)
		{
			inMemory.Dispose();
			throw;
		}
	}

	public void Dispose()
	{
		_temporaryUnitOfWork.Dispose();
	}
}
