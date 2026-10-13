using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Globbing;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Winfs;

internal class WinfsSessionWorkflow : ISessionWorkflow
{
	private static class DatabaseFiltering
	{
		public static bool FilterDatabasesForThisSession(AirframeCategoryClassType airframe, SupportedWinfsDatabase currentNavDatabase, WinfsDatabaseLinkInstruction dbLink)
		{
			if (dbLink.Database.IsNavigationDatabase())
			{
				return IsCurrentNavigationDatabase(dbLink, currentNavDatabase);
			}
			if (dbLink.Database.IsObstacleDatabase())
			{
				return dbLink.Database.ObstacleDatabaseMatchesAirframe(airframe);
			}
			return true;
		}

		private static bool IsCurrentNavigationDatabase(WinfsDatabaseLinkInstruction dbLink, SupportedWinfsDatabase currentNavDb)
		{
			return dbLink.Database == currentNavDb;
		}
	}

	private sealed class SourcedTransformReport
	{
		[field: CompilerGenerated]
		public WinfsTransformReport Report
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public BatchTransformSource Source
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public SourcedTransformReport(WinfsTransformReport report, BatchTransformSource source)
		{
			Report = report;
			Source = source;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		public WinfsSessionWorkflow _003C_003E4__this;

		public ISessionEnvironment sessionEnvironment;

		public CancellationToken token;

		public WinfsSessionWorkflowBehaviorMode mode;

		internal System.Threading.Tasks.Task<List<SourcedTransformReport>> _003CPreFlight_003Eb__0(TrainerSessionData s)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			return _003C_003E4__this.PrepareSimulator(sessionEnvironment, s, mode, token);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_0
	{
		public WinfsSessionWorkflow _003C_003E4__this;

		public TrainerSessionData tsd;

		public List<SourcedTransformReport> transformReports;

		internal IObservable<SourcedBatchTransform> _003CPrepareSimulator_003Eb__0(JToken s)
		{
			return _003C_003E4__this._sourcedBatchTransformProvider.SourcedTransforms(new ManifestDataSource(s), tsd.PackageInfo.BaseInfo.PackageRootPath);
		}

		internal IObservable<SourcedBatchTransform> _003CPrepareSimulator_003Eb__1()
		{
			return _003C_003E4__this._sourcedBatchTransformProvider.SourcedTransforms(tsd.PackageInfo);
		}

		internal void _003CPrepareSimulator_003Eb__4(SourcedTransformReport report)
		{
			transformReports.Add(report);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPreFlight_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public WinfsSessionWorkflow _003C_003E4__this;

		public ISessionEnvironment sessionEnvironment;

		public CancellationToken token;

		private _003C_003Ec__DisplayClass11_0 _003C_003E8__1;

		private List<List<SourcedTransformReport>> _003CsuccessfulTransforms_003E5__2;

		private List<System.Threading.Tasks.Task<List<SourcedTransformReport>>> _003CtransformTasks_003E5__3;

		private ConfiguredTaskAwaiter<List<SourcedTransformReport>[]> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			WinfsSessionWorkflow winfsSessionWorkflow = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass11_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.sessionEnvironment = sessionEnvironment;
					_003C_003E8__1.token = token;
					_003CsuccessfulTransforms_003E5__2 = new List<List<SourcedTransformReport>>();
				}
				try
				{
					if (num != 0)
					{
						_003C_003E8__1.mode = winfsSessionWorkflow._modeSelector(_003C_003E8__1.sessionEnvironment.Inputs.CreationRequestOrThrow().AircraftSnapShot);
						if (_003C_003E8__1.mode != 0)
						{
							Logger.Log(LogLevel.Info, $"Using alternate WINFS session transform mode: {_003C_003E8__1.mode}");
						}
						_003CtransformTasks_003E5__3 = Enumerable.ToList<System.Threading.Tasks.Task<List<SourcedTransformReport>>>(Enumerable.Select<TrainerSessionData, System.Threading.Tasks.Task<List<SourcedTransformReport>>>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)_003C_003E8__1.sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, System.Threading.Tasks.Task<List<SourcedTransformReport>>>)((TrainerSessionData s) => _003C_003E8__1._003C_003E4__this.PrepareSimulator(_003C_003E8__1.sessionEnvironment, s, _003C_003E8__1.mode, _003C_003E8__1.token))));
					}
					try
					{
						ConfiguredTaskAwaiter<List<SourcedTransformReport>[]> awaiter;
						if (num != 0)
						{
							awaiter = System.Threading.Tasks.Task.WhenAll<List<SourcedTransformReport>>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<List<SourcedTransformReport>>>)_003CtransformTasks_003E5__3).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<List<SourcedTransformReport>[]>, _003CPreFlight_003Ed__11>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<List<SourcedTransformReport>[]>);
							num = (_003C_003E1__state = -1);
						}
						awaiter.GetResult();
					}
					catch (System.Exception)
					{
						TrackSuccessfulTransforms(_003CsuccessfulTransforms_003E5__2, _003CtransformTasks_003E5__3);
						throw;
					}
					TrackSuccessfulTransforms(_003CsuccessfulTransforms_003E5__2, _003CtransformTasks_003E5__3);
					_003C_003E8__1.sessionEnvironment = _003C_003E8__1.sessionEnvironment.AttachWorkflowState(winfsSessionWorkflow._attachmentKey, _003CsuccessfulTransforms_003E5__2);
					result = _003C_003E8__1.sessionEnvironment;
				}
				catch (System.Exception)
				{
					winfsSessionWorkflow.CleanupNonUserFiles(_003CsuccessfulTransforms_003E5__2);
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CsuccessfulTransforms_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CsuccessfulTransforms_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPrepareSimulator_003Ed__17 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<List<SourcedTransformReport>> _003C_003Et__builder;

		public WinfsSessionWorkflow _003C_003E4__this;

		public TrainerSessionData tsd;

		public ISessionEnvironment session;

		public WinfsSessionWorkflowBehaviorMode mode;

		private _003C_003Ec__DisplayClass17_0 _003C_003E8__1;

		public CancellationToken token;

		private IWinfsCreationContext _003CcreationContext_003E5__2;

		private bool _003CwinfsMountAlreadyExists_003E5__3;

		private ISettingsBinder<SimAircraftKey> _003CsimulatorSetting_003E5__4;

		private ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>> _003C_003Eu__1;

		private object _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_03d1: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			WinfsSessionWorkflow winfsSessionWorkflow = _003C_003E4__this;
			List<SourcedTransformReport> transformReports;
			try
			{
				ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_011c;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass17_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.tsd = tsd;
					_003CcreationContext_003E5__2 = winfsSessionWorkflow._creationContextFactory.CreateContext(session.Inputs, _003C_003E8__1.tsd);
					_003CwinfsMountAlreadyExists_003E5__3 = winfsSessionWorkflow._fileSystem.PathExists(_003CcreationContext_003E5__2.WinfsDirectoryPath);
					awaiter = winfsSessionWorkflow._unitOfWork.Settings.BindSimulator(_003C_003E8__1.tsd.InstallData.AircraftSimKey).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>>, _003CPrepareSimulator_003Ed__17>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ISettingsBinder<SimAircraftKey>>);
					num = (_003C_003E1__state = -1);
				}
				ISettingsBinder<SimAircraftKey> result = awaiter.GetResult();
				_003CsimulatorSetting_003E5__4 = result;
				goto IL_011c;
				IL_011c:
				try
				{
					ISettingSymbolTable table = default(ISettingSymbolTable);
					IObservable<SourcedBatchTransform> source2 = default(IObservable<SourcedBatchTransform>);
					if (num != 1)
					{
						table = _003CsimulatorSetting_003E5__4.Table;
						IObservable<SourcedBatchTransform> source;
						if (mode == WinfsSessionWorkflowBehaviorMode.PurposeBuiltAircraft)
						{
							IReadOnlyJsonDataSource aircraftModuleData = session.Inputs.CreationRequestOrThrow().AircraftSnapShot.HeavyData.AircraftModuleData;
							string jsonPath = "winfs_transforms." + _003C_003E8__1.tsd.InstallSlot.SerializedSlot;
							source = (aircraftModuleData.IsPathValid(jsonPath) ? Option<JToken>.Some(aircraftModuleData.CopyData(jsonPath)) : Option<JToken>.None).Match<IObservable<SourcedBatchTransform>>((Func<JToken, IObservable<SourcedBatchTransform>>)((JToken s) => _003C_003E8__1._003C_003E4__this._sourcedBatchTransformProvider.SourcedTransforms(new ManifestDataSource(s), _003C_003E8__1.tsd.PackageInfo.BaseInfo.PackageRootPath)), (Func<IObservable<SourcedBatchTransform>>)(() => _003C_003E8__1._003C_003E4__this._sourcedBatchTransformProvider.SourcedTransforms(_003C_003E8__1.tsd.PackageInfo)));
						}
						else
						{
							source = winfsSessionWorkflow._sourcedBatchTransformProvider.SourcedTransforms(_003C_003E8__1.tsd.PackageInfo);
						}
						ISourcedBatchFilter[] filters = winfsSessionWorkflow._filterFactory.CreateFilters(session.Inputs, table);
						source2 = AggregateSourcedTransformFilter.Filter(source, filters);
						_003C_003E8__1.transformReports = new List<SourcedTransformReport>();
					}
					try
					{
						AsyncSubject<SourcedTransformReport> asyncSubject;
						if (num != 1)
						{
							IObservable<SourcedBatchTransform> transformsToApply = source2.Where<SourcedBatchTransform>((Func<SourcedBatchTransform, bool>)((SourcedBatchTransform st) => st.Source == BatchTransformSource.Essentials));
							IObservable<SourcedTransformReport> first = winfsSessionWorkflow.ApplyTransforms(_003C_003E8__1.tsd, _003CcreationContext_003E5__2, table, transformsToApply, token);
							if (!_003CwinfsMountAlreadyExists_003E5__3)
							{
								IObservable<SourcedBatchTransform> transformsToApply2 = source2.Where<SourcedBatchTransform>((Func<SourcedBatchTransform, bool>)((SourcedBatchTransform st) => st.Source == BatchTransformSource.StartingUserData));
								first = first.Concat<SourcedTransformReport>(winfsSessionWorkflow.ApplyTransforms(_003C_003E8__1.tsd, _003CcreationContext_003E5__2, table, transformsToApply2, token));
							}
							first = first.Concat<SourcedTransformReport>(winfsSessionWorkflow.ApplyDatabaseTransforms(session, _003C_003E8__1.tsd, _003CcreationContext_003E5__2, table, token));
							asyncSubject = first.Do<SourcedTransformReport>((Action<SourcedTransformReport>)delegate(SourcedTransformReport report)
							{
								_003C_003E8__1.transformReports.Add(report);
							}).Where<SourcedTransformReport>((Func<SourcedTransformReport, bool>)((SourcedTransformReport report) => report.Report.Result != WinfsOperationResult.Succeeded)).Do<SourcedTransformReport>((Action<SourcedTransformReport>)delegate(SourcedTransformReport errorReport)
							{
								//IL_0035: Unknown result type (might be due to invalid IL or missing references)
								//IL_0018: Unknown result type (might be due to invalid IL or missing references)
								throw (errorReport.Report.Error != null) ? new IOException(errorReport.Report.FormattedReport, errorReport.Report.Error) : new IOException(errorReport.Report.FormattedReport);
							})
								.LastOrDefaultAsync<SourcedTransformReport>()
								.GetAwaiter<SourcedTransformReport>();
							if (!asyncSubject.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = asyncSubject;
								_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<SourcedTransformReport>, _003CPrepareSimulator_003Ed__17>(ref asyncSubject, ref this);
								return;
							}
						}
						else
						{
							asyncSubject = (AsyncSubject<SourcedTransformReport>)_003C_003Eu__2;
							_003C_003Eu__2 = null;
							num = (_003C_003E1__state = -1);
						}
						asyncSubject.GetResult();
						winfsSessionWorkflow._fileSystem.CreateDirectory(_003CcreationContext_003E5__2.WinfsDirectoryPath);
					}
					catch (IOException val)
					{
						IOException val2 = val;
						if (_003CwinfsMountAlreadyExists_003E5__3)
						{
							winfsSessionWorkflow.CleanupNonUserFiles((System.Collections.Generic.IEnumerable<SourcedTransformReport>)_003C_003E8__1.transformReports);
						}
						else
						{
							winfsSessionWorkflow._fileSystem.DeleteIfExists(_003CcreationContext_003E5__2.WinfsDirectoryPath);
						}
						if (val2 is FileNotFoundException || val2 is DirectoryNotFoundException)
						{
							throw new FileNotFoundException((string?)_003C_003E8__1.tsd.EndpointName + " is missing required files. Repair installation to resolve this.", (System.Exception)(object)val2);
						}
						throw new InvalidOperationException("Cannot start " + (string?)_003C_003E8__1.tsd.EndpointName + " because an error occurred during winfs construction.", (System.Exception)(object)val2);
					}
					transformReports = _003C_003E8__1.transformReports;
				}
				finally
				{
					if (num < 0 && _003CsimulatorSetting_003E5__4 != null)
					{
						((System.IDisposable)_003CsimulatorSetting_003E5__4).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CcreationContext_003E5__2 = null;
				_003CsimulatorSetting_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CcreationContext_003E5__2 = null;
			_003CsimulatorSetting_003E5__4 = null;
			_003C_003Et__builder.SetResult(transformReports);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ICreationContextFactory _creationContextFactory;

	private readonly ISourcedBatchTransformProvider _sourcedBatchTransformProvider;

	private readonly ISourcedBatchFilterFactory _filterFactory;

	private readonly IWinfsTransformProcessor _processor;

	private readonly IFileSystem _fileSystem;

	private readonly IUnitOfWork _unitOfWork;

	private readonly GetAircraftWinFsSessionWorkflowMode _modeSelector;

	private readonly object _attachmentKey = new object();

	public static GetAircraftWinFsSessionWorkflowMode DefaultWorkflowModeSelector => (AircraftSnapshotData _) => WinfsSessionWorkflowBehaviorMode.Normal;

	public WinfsSessionWorkflow(ICreationContextFactory creationContextFactory, ISourcedBatchTransformProvider batchTransformProvider, ISourcedBatchFilterFactory filterFactory, IWinfsTransformProcessor transformProcessor, IFileSystem fileSystem, IUnitOfWork unitOfWork, GetAircraftWinFsSessionWorkflowMode modeSelector)
	{
		_creationContextFactory = creationContextFactory;
		_sourcedBatchTransformProvider = batchTransformProvider;
		_filterFactory = filterFactory;
		_processor = transformProcessor;
		_fileSystem = fileSystem;
		_unitOfWork = unitOfWork;
		_modeSelector = modeSelector;
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__11))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ISessionEnvironment sessionEnvironment2 = sessionEnvironment;
		List<List<SourcedTransformReport>> successfulTransforms = new List<List<SourcedTransformReport>>();
		try
		{
			WinfsSessionWorkflowBehaviorMode mode = _modeSelector(sessionEnvironment2.Inputs.CreationRequestOrThrow().AircraftSnapShot);
			if (mode != 0)
			{
				Logger.Log(LogLevel.Info, $"Using alternate WINFS session transform mode: {mode}");
			}
			List<System.Threading.Tasks.Task<List<SourcedTransformReport>>> transformTasks = Enumerable.ToList<System.Threading.Tasks.Task<List<SourcedTransformReport>>>(Enumerable.Select<TrainerSessionData, System.Threading.Tasks.Task<List<SourcedTransformReport>>>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment2.Data.TrainerData).Values, (Func<TrainerSessionData, System.Threading.Tasks.Task<List<SourcedTransformReport>>>)((TrainerSessionData s) => PrepareSimulator(sessionEnvironment2, s, mode, token))));
			try
			{
				await System.Threading.Tasks.Task.WhenAll<List<SourcedTransformReport>>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<List<SourcedTransformReport>>>)transformTasks).DetachContext();
			}
			catch (System.Exception)
			{
				TrackSuccessfulTransforms(successfulTransforms, transformTasks);
				throw;
			}
			TrackSuccessfulTransforms(successfulTransforms, transformTasks);
			sessionEnvironment2 = sessionEnvironment2.AttachWorkflowState(_attachmentKey, successfulTransforms);
			return sessionEnvironment2;
		}
		catch (System.Exception)
		{
			CleanupNonUserFiles(successfulTransforms);
			throw;
		}
	}

	public System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		List<List<SourcedTransformReport>> workflowState = sessionEnvironment.GetWorkflowState<List<List<SourcedTransformReport>>>(_attachmentKey);
		CleanupNonUserFiles(workflowState ?? throw new InvalidOperationException("transforms were not attached to session."));
		return System.Threading.Tasks.Task.FromResult<ISessionEnvironment>(sessionEnvironment.DetachWorkflowState(_attachmentKey));
	}

	private static void TrackSuccessfulTransforms(List<List<SourcedTransformReport>> successfulTransforms, List<System.Threading.Tasks.Task<List<SourcedTransformReport>>> transformTasks)
	{
		System.Collections.Generic.IEnumerator<System.Threading.Tasks.Task<List<SourcedTransformReport>>> enumerator = Enumerable.Where<System.Threading.Tasks.Task<List<SourcedTransformReport>>>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<List<SourcedTransformReport>>>)transformTasks, (Func<System.Threading.Tasks.Task<List<SourcedTransformReport>>, bool>)((System.Threading.Tasks.Task<List<SourcedTransformReport>> t) => !((System.Threading.Tasks.Task)t).IsFaulted && !((System.Threading.Tasks.Task)t).IsCanceled)).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				System.Threading.Tasks.Task<List<SourcedTransformReport>> current = enumerator.Current;
				successfulTransforms.Add(current.Result);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private static IObservable<System.Reactive.Unit> Cancelled(TrainerSessionData tsd, CancellationToken token)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		TrainerSessionData tsd2 = tsd;
		return token.ToObservable().Do<System.Reactive.Unit>((Action<System.Reactive.Unit>)delegate
		{
			Logger.Log(LogLevel.Info, "Preflight canceled while preparing: {0}", tsd2.EndpointName);
		});
	}

	private static string Substitute(string inputString, ISettingSymbolTable simulatorSettings)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IEnumerable<TrainerIdentifier> substitutionIds = SettingSubstitution.GetSubstitutionIds(inputString);
		string text = inputString;
		System.Collections.Generic.IEnumerator<TrainerIdentifier> enumerator = substitutionIds.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				TrainerIdentifier current = enumerator.Current;
				string replacement = (simulatorSettings.GetByScopedIdentifier(SettingIdentifier.FromTrainerIdentifier(current)) ?? throw new FormatException("Invalid package manifest. " + SettingSubstitution.Wrap(current) + " could not be resolved.")).SelectedSimulatorValue();
				text = SettingSubstitution.ReplaceSubstitution(text, current, replacement);
			}
			return text;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private static SourcedBatchTransform ApplySubstitutions(SourcedBatchTransform transform, ISettingSymbolTable simulatorSettings)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		ISettingSymbolTable simulatorSettings2 = simulatorSettings;
		return new SourcedBatchTransform(new BatchTransformation(ImmutableArrayExtensions.Select<WinfsCopyInstruction, WinfsCopyInstruction>(transform.Transform.CopyInstructions, (Func<WinfsCopyInstruction, WinfsCopyInstruction>)((WinfsCopyInstruction wci) => new WinfsCopyInstruction(Substitute(wci.PackageBaseDirectory, simulatorSettings2), Substitute(wci.WinFsBaseDirectory, simulatorSettings2), (System.Collections.Generic.IEnumerable<Glob>)wci.Globs))), ImmutableArrayExtensions.Select<WinfsLinkInstruction, WinfsLinkInstruction>(transform.Transform.LinkInstructions, (Func<WinfsLinkInstruction, WinfsLinkInstruction>)((WinfsLinkInstruction wli) => new WinfsLinkInstruction(Substitute(wli.PackagePath, simulatorSettings2), Substitute(wli.WinfsPath, simulatorSettings2)))), (System.Collections.Generic.IEnumerable<ValueConstraintDefinition>)(object)transform.Transform.Constraints), transform.Source);
	}

	[AsyncStateMachine(typeof(_003CPrepareSimulator_003Ed__17))]
	private async System.Threading.Tasks.Task<List<SourcedTransformReport>> PrepareSimulator(ISessionEnvironment session, TrainerSessionData tsd, WinfsSessionWorkflowBehaviorMode mode, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		TrainerSessionData tsd2 = tsd;
		IWinfsCreationContext creationContext = _creationContextFactory.CreateContext(session.Inputs, tsd2);
		bool winfsMountAlreadyExists = _fileSystem.PathExists(creationContext.WinfsDirectoryPath);
		using ISettingsBinder<SimAircraftKey> simulatorSetting = await _unitOfWork.Settings.BindSimulator(tsd2.InstallData.AircraftSimKey).DetachContext();
		ISettingSymbolTable table = simulatorSetting.Table;
		IObservable<SourcedBatchTransform> source;
		if (mode == WinfsSessionWorkflowBehaviorMode.PurposeBuiltAircraft)
		{
			IReadOnlyJsonDataSource aircraftModuleData = session.Inputs.CreationRequestOrThrow().AircraftSnapShot.HeavyData.AircraftModuleData;
			string jsonPath = "winfs_transforms." + tsd2.InstallSlot.SerializedSlot;
			source = (aircraftModuleData.IsPathValid(jsonPath) ? Option<JToken>.Some(aircraftModuleData.CopyData(jsonPath)) : Option<JToken>.None).Match<IObservable<SourcedBatchTransform>>((Func<JToken, IObservable<SourcedBatchTransform>>)((JToken s) => _sourcedBatchTransformProvider.SourcedTransforms(new ManifestDataSource(s), tsd2.PackageInfo.BaseInfo.PackageRootPath)), (Func<IObservable<SourcedBatchTransform>>)(() => _sourcedBatchTransformProvider.SourcedTransforms(tsd2.PackageInfo)));
		}
		else
		{
			source = _sourcedBatchTransformProvider.SourcedTransforms(tsd2.PackageInfo);
		}
		ISourcedBatchFilter[] filters = _filterFactory.CreateFilters(session.Inputs, table);
		IObservable<SourcedBatchTransform> source2 = AggregateSourcedTransformFilter.Filter(source, filters);
		List<SourcedTransformReport> transformReports = new List<SourcedTransformReport>();
		try
		{
			IObservable<SourcedBatchTransform> transformsToApply = source2.Where<SourcedBatchTransform>((Func<SourcedBatchTransform, bool>)((SourcedBatchTransform st) => st.Source == BatchTransformSource.Essentials));
			IObservable<SourcedTransformReport> first = ApplyTransforms(tsd2, creationContext, table, transformsToApply, token);
			if (!winfsMountAlreadyExists)
			{
				IObservable<SourcedBatchTransform> transformsToApply2 = source2.Where<SourcedBatchTransform>((Func<SourcedBatchTransform, bool>)((SourcedBatchTransform st) => st.Source == BatchTransformSource.StartingUserData));
				first = first.Concat<SourcedTransformReport>(ApplyTransforms(tsd2, creationContext, table, transformsToApply2, token));
			}
			first = first.Concat<SourcedTransformReport>(ApplyDatabaseTransforms(session, tsd2, creationContext, table, token));
			await first.Do<SourcedTransformReport>((Action<SourcedTransformReport>)delegate(SourcedTransformReport report)
			{
				transformReports.Add(report);
			}).Where<SourcedTransformReport>((Func<SourcedTransformReport, bool>)((SourcedTransformReport report) => report.Report.Result != WinfsOperationResult.Succeeded)).Do<SourcedTransformReport>((Action<SourcedTransformReport>)delegate(SourcedTransformReport errorReport)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				throw (errorReport.Report.Error != null) ? new IOException(errorReport.Report.FormattedReport, errorReport.Report.Error) : new IOException(errorReport.Report.FormattedReport);
			})
				.LastOrDefaultAsync<SourcedTransformReport>();
			_fileSystem.CreateDirectory(creationContext.WinfsDirectoryPath);
		}
		catch (IOException val)
		{
			IOException val2 = val;
			if (winfsMountAlreadyExists)
			{
				CleanupNonUserFiles((System.Collections.Generic.IEnumerable<SourcedTransformReport>)transformReports);
			}
			else
			{
				_fileSystem.DeleteIfExists(creationContext.WinfsDirectoryPath);
			}
			if (val2 is FileNotFoundException || val2 is DirectoryNotFoundException)
			{
				throw new FileNotFoundException((string?)tsd2.EndpointName + " is missing required files. Repair installation to resolve this.", (System.Exception)(object)val2);
			}
			throw new InvalidOperationException("Cannot start " + (string?)tsd2.EndpointName + " because an error occurred during winfs construction.", (System.Exception)(object)val2);
		}
		return transformReports;
	}

	private void CleanupNonUserFiles(List<List<SourcedTransformReport>> successfulTransforms)
	{
		if (successfulTransforms != null)
		{
			CleanupNonUserFiles(Enumerable.SelectMany<List<SourcedTransformReport>, SourcedTransformReport>((System.Collections.Generic.IEnumerable<List<SourcedTransformReport>>)successfulTransforms, (Func<List<SourcedTransformReport>, System.Collections.Generic.IEnumerable<SourcedTransformReport>>)((List<SourcedTransformReport> st) => (System.Collections.Generic.IEnumerable<SourcedTransformReport>)st)));
		}
	}

	private void CleanupNonUserFiles(System.Collections.Generic.IEnumerable<SourcedTransformReport> transformReports)
	{
		System.Collections.Generic.IEnumerator<WinfsTransformRequest> enumerator = Enumerable.Select<SourcedTransformReport, WinfsTransformRequest>(Enumerable.Where<SourcedTransformReport>(transformReports, (Func<SourcedTransformReport, bool>)((SourcedTransformReport sr) => sr.Source == BatchTransformSource.Essentials || sr.Source == BatchTransformSource.Databases)), (Func<SourcedTransformReport, WinfsTransformRequest>)((SourcedTransformReport sr) => sr.Report.Request)).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				WinfsTransformRequest current = enumerator.Current;
				string text = current.WinfsPath;
				if (current.Operation == WinfsTranformOperation.Link)
				{
					text += ".lnk";
				}
				_fileSystem.DeleteIfExists(text);
				string text2 = LauncherPathHelpers.NonRootPathParentDirectory(text);
				if (!Enumerable.Any<FileSystemEntry>(_fileSystem.EntriesIn(text2)))
				{
					_fileSystem.DeleteIfExists(text2);
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private IObservable<SourcedTransformReport> ApplyDatabaseTransforms(ISessionEnvironment sessionEnvironment, TrainerSessionData tsd, IWinfsCreationContext creationContext, ISettingSymbolTable simulatorSettings, CancellationToken token)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		IWinfsCreationContext creationContext2 = creationContext;
		ISessionEnvironment sessionEnvironment2 = sessionEnvironment;
		ISettingSymbolTable simulatorSettings2 = simulatorSettings;
		System.Collections.Generic.IEnumerator<string> enumerator = Enumerable.Select<RelativePath, string>((tsd.PackageInfo.BaseInfo.Manifest.GetDatabaseMap() ?? throw new FormatException("The 'Winfs.Databases' is required for Database Winfs transformations.")).WinfsRelativePaths, (Func<RelativePath, string>)((RelativePath rp) => rp.AppendTo(creationContext2.WinfsDirectoryPath))).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				string current = enumerator.Current;
				_fileSystem.DeleteIfExists(current + ".lnk");
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		IActiveSetting activeSetting = simulatorSettings2.GetNavigationDatabaseSettingFromParent() ?? throw new KeyNotFoundException("NavigationDatabase");
		SupportedWinfsDatabase currentNavigationDatabase = NavigationDatabaseSetting.CurrentlySelectedDatabaseForAircraftType(activeSetting, sessionEnvironment2.Inputs.AirframeCCT);
		IObservable<WinfsTransformRequest> source = _sourcedBatchTransformProvider.DatabaseTransforms(tsd.PackageInfo.BaseInfo.Manifest, tsd.PackageInfo.BaseInfo.PackageRootPath).Where<WinfsDatabaseLinkInstruction>((Func<WinfsDatabaseLinkInstruction, bool>)((WinfsDatabaseLinkInstruction dbLink) => DatabaseFiltering.FilterDatabasesForThisSession(sessionEnvironment2.Inputs.AirframeCCT, currentNavigationDatabase, dbLink))).SelectMany<WinfsDatabaseLinkInstruction, WinfsTransformRequest>((Func<WinfsDatabaseLinkInstruction, IObservable<WinfsTransformRequest>>)((WinfsDatabaseLinkInstruction dbTransform) => RequestBuilder.CreateLinkRequests(ApplySubstitutions(dbTransform.ToSourcedTransform(), simulatorSettings2).Transform.LinkInstructionStream, creationContext2)));
		IWinfsTransformProcessor processor = _processor;
		return source.SelectMany<WinfsTransformRequest, WinfsTransformReport>((Func<WinfsTransformRequest, System.Threading.Tasks.Task<WinfsTransformReport>>)processor.ProcessRequest).Select<WinfsTransformReport, SourcedTransformReport>((Func<WinfsTransformReport, SourcedTransformReport>)((WinfsTransformReport report) => new SourcedTransformReport(report, BatchTransformSource.Databases))).TakeUntil<SourcedTransformReport, System.Reactive.Unit>(Cancelled(tsd, token));
	}

	private IObservable<SourcedTransformReport> ApplyTransforms(TrainerSessionData tsd, IWinfsCreationContext creationContext, ISettingSymbolTable simulator, IObservable<SourcedBatchTransform> transformsToApply, CancellationToken token)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ISettingSymbolTable simulator2 = simulator;
		IWinfsCreationContext creationContext2 = creationContext;
		return transformsToApply.Select<SourcedBatchTransform, SourcedBatchTransform>((Func<SourcedBatchTransform, SourcedBatchTransform>)((SourcedBatchTransform transform) => ApplySubstitutions(transform, simulator2))).SelectMany<SourcedBatchTransform, SourcedTransformReport>((Func<SourcedBatchTransform, IObservable<SourcedTransformReport>>)((SourcedBatchTransform st) => BatchProcessor.ProcessBatch(st.Transform, creationContext2, _processor).Select<WinfsTransformReport, SourcedTransformReport>((Func<WinfsTransformReport, SourcedTransformReport>)((WinfsTransformReport report) => new SourcedTransformReport(report, st.Source))))).TakeUntil<SourcedTransformReport, System.Reactive.Unit>(Cancelled(tsd, token));
	}
}
