using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class CreateAircraftHarness : TemporaryUnitHarness<ReactiveAircraft>
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CDoConfirm_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ReactiveAircraft> _003C_003Et__builder;

		public CreateAircraftHarness _003C_003E4__this;

		private string _003CtmpGcaDirPath_003E5__2;

		private string _003CimportGcaPath_003E5__3;

		private AircraftListData _003ClistData_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_029b: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CreateAircraftHarness createAircraftHarness = _003C_003E4__this;
			ReactiveAircraft result2 = default(ReactiveAircraft);
			try
			{
				if ((uint)num > 4u)
				{
					_003CtmpGcaDirPath_003E5__2 = Path.Combine(createAircraftHarness.TemporaryUnit.LowLevel.Deployment.PerProcessTempDirectory(), "CreateHarness");
					string text = _003CtmpGcaDirPath_003E5__2;
					Guid val = Guid.NewGuid();
					_003CimportGcaPath_003E5__3 = Path.Combine(text, ((object)(Guid)(ref val)).ToString());
				}
				try
				{
					ConfiguredTaskAwaitable val2;
					ConfiguredTaskAwaiter awaiter3;
					ConfiguredTaskAwaiter<AircraftListData> awaiter2;
					object obj;
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
					AircraftSnapshotData result3;
					AircraftListData result4;
					int num2;
					switch (num)
					{
					default:
						val2 = createAircraftHarness.TemporaryUnit.FlushEverything().ConfigureAwait(false);
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDoConfirm_003Ed__12>(ref awaiter3, ref this);
							return;
						}
						goto IL_00dc;
					case 0:
						awaiter3 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00dc;
					case 1:
						awaiter3 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_017b;
					case 2:
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftListData>);
						num = (_003C_003E1__state = -1);
						goto IL_01ed;
					case 3:
						try
						{
							if (num != 3)
							{
								awaiter = createAircraftHarness.ActualUnit.Aircraft.ImportAircaft(_003ClistData_003E5__4, _003CimportGcaPath_003E5__3).ConfigureAwait(false).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 3);
									_003C_003Eu__3 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CDoConfirm_003Ed__12>(ref awaiter, ref this);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
								num = (_003C_003E1__state = -1);
							}
							AircraftSnapshotData result = awaiter.GetResult();
							result2 = new ReactiveAircraft(createAircraftHarness.ActualUnit, result);
						}
						catch (DuplicateNameException val3)
						{
							obj = (object)val3;
							num2 = 1;
							goto IL_02a0;
						}
						goto end_IL_005c;
					case 4:
						{
							awaiter = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
							num = (_003C_003E1__state = -1);
							goto IL_0345;
						}
						IL_00dc:
						((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
						createAircraftHarness.TemporaryUnit.LowLevel.FileSystem.CreateDirectory(_003CtmpGcaDirPath_003E5__2);
						val2 = createAircraftHarness.TemporaryUnit.Aircraft.ExportAircraft(createAircraftHarness.TemporaryAircraft.AircraftIdentifier, _003CimportGcaPath_003E5__3).ConfigureAwait(false);
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDoConfirm_003Ed__12>(ref awaiter3, ref this);
							return;
						}
						goto IL_017b;
						IL_02a0:
						if (num2 != 1)
						{
							break;
						}
						if (createAircraftHarness.CanGenerateNewId)
						{
							_003ClistData_003E5__4 = _003ClistData_003E5__4.WithNewIdentifier(default(AircraftIdentifier));
							awaiter = createAircraftHarness.ActualUnit.Aircraft.ImportAircaft(_003ClistData_003E5__4, _003CimportGcaPath_003E5__3).ConfigureAwait(false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 4);
								_003C_003Eu__3 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CDoConfirm_003Ed__12>(ref awaiter, ref this);
								return;
							}
							goto IL_0345;
						}
						ExceptionDispatchInfo.Capture((obj as System.Exception) ?? throw obj).Throw();
						break;
						IL_017b:
						((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
						awaiter2 = createAircraftHarness.TemporaryAircraft.ListData.GetUpdatedValue().ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CDoConfirm_003Ed__12>(ref awaiter2, ref this);
							return;
						}
						goto IL_01ed;
						IL_0345:
						result3 = awaiter.GetResult();
						result2 = new ReactiveAircraft(createAircraftHarness.ActualUnit, result3);
						goto end_IL_005c;
						IL_01ed:
						result4 = awaiter2.GetResult();
						_003ClistData_003E5__4 = result4;
						num2 = 0;
						goto case 3;
					}
					_003ClistData_003E5__4 = null;
					goto IL_03be;
					end_IL_005c:;
				}
				finally
				{
					if (num < 0)
					{
						createAircraftHarness.TemporaryUnit.LowLevel.FileSystem.DeleteIfExists(_003CimportGcaPath_003E5__3);
						createAircraftHarness.TemporaryUnit.LowLevel.FileSystem.DeleteDirectoryIfEmpty(_003CtmpGcaDirPath_003E5__2);
					}
				}
				goto end_IL_000e;
				IL_03be:
				_003CtmpGcaDirPath_003E5__2 = null;
				_003CimportGcaPath_003E5__3 = null;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CtmpGcaDirPath_003E5__2 = null;
				_003CimportGcaPath_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtmpGcaDirPath_003E5__2 = null;
			_003CimportGcaPath_003E5__3 = null;
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
	private struct _003CStart_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<CreateAircraftHarness> _003C_003Et__builder;

		public IUnitOfWork unitOfWork;

		public Func<IUnitOfWork, System.Threading.Tasks.Task<AircraftSnapshotData>> startingAircraftFactory;

		private TemporaryUnitOfWork _003Ctemporary_003E5__2;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CreateAircraftHarness result2;
			try
			{
				if (num != 0)
				{
					_003Ctemporary_003E5__2 = new TemporaryUnitOfWork(unitOfWork);
				}
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
					if (num != 0)
					{
						awaiter = startingAircraftFactory.Invoke((IUnitOfWork)_003Ctemporary_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CStart_003Ed__10>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
						num = (_003C_003E1__state = -1);
					}
					AircraftSnapshotData result = awaiter.GetResult();
					result2 = new CreateAircraftHarness(_003Ctemporary_003E5__2, new ReactiveAircraft(_003Ctemporary_003E5__2, result));
				}
				catch (System.Exception)
				{
					_003Ctemporary_003E5__2.Dispose();
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Ctemporary_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Ctemporary_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private const string TEMP_DIR_NAME = "CreateHarness";

	[field: CompilerGenerated]
	public bool CanGenerateNewId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = true;


	[field: CompilerGenerated]
	public ReactiveAircraft TemporaryAircraft
	{
		[CompilerGenerated]
		get;
	}

	private CreateAircraftHarness(TemporaryUnitOfWork tempUnit, ReactiveAircraft startingPoint)
		: base(tempUnit)
	{
		TemporaryAircraft = startingPoint;
	}

	public static System.Threading.Tasks.Task<CreateAircraftHarness> Start(IUnitOfWork unitOfWork, AircraftListData? startingListData = null)
	{
		AircraftListData startingListData2 = startingListData;
		startingListData2 = startingListData2 ?? new AircraftListData(default(AircraftIdentifier), "Temporary Aircraft", "", AirframeCategoryClassType.Unconfigured, null, isHidden: false, canBeDeleted: true);
		return Start(unitOfWork, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_0._003C_003CStart_003Eb__0_003Ed))] async (IUnitOfWork temporary) => await temporary.Aircraft.CreateNewAircraft(startingListData2).DetachContext());
	}

	[AsyncStateMachine(typeof(_003CStart_003Ed__10))]
	public static async System.Threading.Tasks.Task<CreateAircraftHarness> Start(IUnitOfWork unitOfWork, Func<IUnitOfWork, System.Threading.Tasks.Task<AircraftSnapshotData>> startingAircraftFactory)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TemporaryUnitOfWork temporary = new TemporaryUnitOfWork(unitOfWork);
		try
		{
			return new CreateAircraftHarness(temporary, new ReactiveAircraft(temporary, await startingAircraftFactory.Invoke((IUnitOfWork)temporary).DetachContext()));
		}
		catch (System.Exception)
		{
			temporary.Dispose();
			throw;
		}
	}

	public static System.Threading.Tasks.Task<CreateAircraftHarness> StartFromExistingAircraft(IUnitOfWork unitOfWork, AircraftIdentifier exportAircraftToCopy)
	{
		IUnitOfWork unitOfWork2 = unitOfWork;
		return Start(unitOfWork2, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_0._003C_003CStartFromExistingAircraft_003Eb__0_003Ed))] async (IUnitOfWork temporary) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string text = unitOfWork2.LowLevel.Deployment.PerProcessTempDirectory();
			Guid val = Guid.NewGuid();
			string tempGcaPath = Path.Combine(text, "CreateHarness", ((object)(Guid)(ref val)).ToString());
			try
			{
				AircraftListData listData = await unitOfWork2.Aircraft.GetAircraftListDataById(exportAircraftToCopy).DetachContext();
				await unitOfWork2.Aircraft.ExportAircraft(exportAircraftToCopy, tempGcaPath).DetachContext();
				return await temporary.Aircraft.ImportAircaft(listData, tempGcaPath).DetachContext();
			}
			finally
			{
				unitOfWork2.LowLevel.FileSystem.DeleteAnythingNotLocked(tempGcaPath);
			}
		});
	}

	[AsyncStateMachine(typeof(_003CDoConfirm_003Ed__12))]
	protected override async System.Threading.Tasks.Task<ReactiveAircraft> DoConfirm()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string tmpGcaDirPath = Path.Combine(base.TemporaryUnit.LowLevel.Deployment.PerProcessTempDirectory(), "CreateHarness");
		Guid val = Guid.NewGuid();
		string importGcaPath = Path.Combine(tmpGcaDirPath, ((object)(Guid)(ref val)).ToString());
		ReactiveAircraft result = default(ReactiveAircraft);
		try
		{
			await base.TemporaryUnit.FlushEverything().ConfigureAwait(false);
			base.TemporaryUnit.LowLevel.FileSystem.CreateDirectory(tmpGcaDirPath);
			await base.TemporaryUnit.Aircraft.ExportAircraft(TemporaryAircraft.AircraftIdentifier, importGcaPath).ConfigureAwait(false);
			AircraftListData listData2 = await TemporaryAircraft.ListData.GetUpdatedValue().ConfigureAwait(false);
			object obj;
			int num;
			try
			{
				AircraftSnapshotData startingSnapshot = await base.ActualUnit.Aircraft.ImportAircaft(listData2, importGcaPath).ConfigureAwait(false);
				result = new ReactiveAircraft(base.ActualUnit, startingSnapshot);
				return result;
			}
			catch (DuplicateNameException val2)
			{
				obj = (object)val2;
				num = 1;
			}
			if (num == 1)
			{
				if (CanGenerateNewId)
				{
					listData2 = listData2.WithNewIdentifier(default(AircraftIdentifier));
					AircraftSnapshotData startingSnapshot2 = await base.ActualUnit.Aircraft.ImportAircaft(listData2, importGcaPath).ConfigureAwait(false);
					result = new ReactiveAircraft(base.ActualUnit, startingSnapshot2);
					return result;
				}
				ExceptionDispatchInfo.Capture((obj as System.Exception) ?? throw obj).Throw();
			}
		}
		finally
		{
			base.TemporaryUnit.LowLevel.FileSystem.DeleteIfExists(importGcaPath);
			base.TemporaryUnit.LowLevel.FileSystem.DeleteDirectoryIfEmpty(tmpGcaDirPath);
		}
		return result;
	}
}
