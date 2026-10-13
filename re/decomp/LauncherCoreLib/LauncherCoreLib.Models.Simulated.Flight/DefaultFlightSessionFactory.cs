using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Tasks;
using LauncherCoreLib.Models.CrashReporting;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class DefaultFlightSessionFactory : IFlightSessionFactory
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateSession_003Ed__6 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IFlightSession> _003C_003Et__builder;

		public FlightSessionRequestInfo sessionRequest;

		public DefaultFlightSessionFactory _003C_003E4__this;

		private string _003CconfigurationName_003E5__2;

		private AirframeCategoryClassType _003Cairframe_003E5__3;

		private ConfiguredTaskAwaiter<int> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultFlightSessionFactory defaultFlightSessionFactory = _003C_003E4__this;
			IFlightSession result2;
			try
			{
				ConfiguredTaskAwaiter<int> awaiter;
				if (num != 0)
				{
					_003CconfigurationName_003E5__2 = BuildConfigurationName(sessionRequest);
					_003Cairframe_003E5__3 = sessionRequest.AircraftSnapShot.ListData.AirframeCCT;
					awaiter = defaultFlightSessionFactory._sessionCounter.GetAndIncrementSessionCount().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<int>, _003CCreateSession_003Ed__6>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<int>);
					num = (_003C_003E1__state = -1);
				}
				int result = awaiter.GetResult();
				string baseDir = defaultFlightSessionFactory._deployment.SessionBaseDirectoryPath();
				DateTimeOffset now = DateTimeOffset.Now;
				string text = BuildSessionFolderPath(baseDir, now, result);
				result2 = new DefaultFlightSession(new DefaultSessionEnvironment(new SessionInfo
				{
					SessionIndex = result,
					SessionPathRoot = text,
					SessionJsonPath = BuildSessionJsonFilePath(text),
					StartedAt = now,
					AirframeCCT = _003Cairframe_003E5__3,
					ConfigurationName = _003CconfigurationName_003E5__2,
					CreationRequest = sessionRequest
				}), defaultFlightSessionFactory._workflowProvider.Workflow, defaultFlightSessionFactory._simSessionFactory);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CconfigurationName_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CconfigurationName_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly IFlightSessionCounter _sessionCounter;

	private readonly IDeploymentPathProvider _deployment;

	private readonly ISessionWorkflowProvider _workflowProvider;

	private readonly IFileSystem _fileSystem;

	private readonly ISimSessionFactory _simSessionFactory;

	public DefaultFlightSessionFactory(IFlightSessionCounter counter, IDeploymentPathProvider deploymentPathProvider, ISessionWorkflowProvider workflowProvider, IFileSystem fileSystem, ISimSessionFactory simSessionFactory)
	{
		_sessionCounter = counter;
		_deployment = deploymentPathProvider;
		_workflowProvider = workflowProvider;
		_fileSystem = fileSystem;
		_simSessionFactory = simSessionFactory;
	}

	[AsyncStateMachine(typeof(_003CCreateSession_003Ed__6))]
	public async System.Threading.Tasks.Task<IFlightSession> CreateSession(FlightSessionRequestInfo sessionRequest, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string configurationName = BuildConfigurationName(sessionRequest);
		AirframeCategoryClassType airframe = sessionRequest.AircraftSnapShot.ListData.AirframeCCT;
		int num = await _sessionCounter.GetAndIncrementSessionCount().DetachContext();
		string baseDir = _deployment.SessionBaseDirectoryPath();
		DateTimeOffset now = DateTimeOffset.Now;
		string text = BuildSessionFolderPath(baseDir, now, num);
		return new DefaultFlightSession(new DefaultSessionEnvironment(new SessionInfo
		{
			SessionIndex = num,
			SessionPathRoot = text,
			SessionJsonPath = BuildSessionJsonFilePath(text),
			StartedAt = now,
			AirframeCCT = airframe,
			ConfigurationName = configurationName,
			CreationRequest = sessionRequest
		}), _workflowProvider.Workflow, _simSessionFactory);
	}

	private static string BuildConfigurationName(FlightSessionRequestInfo request)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		AircraftSnapshotData aircraftSnapShot = request.AircraftSnapShot;
		if (((System.Collections.Generic.IReadOnlyCollection<SimInstallKey>)aircraftSnapShot.Panel.ActiveSimKeys).Count == 0)
		{
			throw new InvalidOperationException("The aircraft must have at least one active simulator to power up.");
		}
		SimInstallData simInstallData = Enumerable.First<SimInstallData>(aircraftSnapShot.Panel.ActiveSims);
		string text = SessionConfigurationNameFormatter.Format(primaryPackage: request.Packages[simInstallData.FullPackageId ?? throw new InvalidOperationException("An active sim being powered up must have a package reference.")], aircraftDisplayName: aircraftSnapShot.ListData.DisplayName);
		if (text.Length > 35)
		{
			text = text.Substring(0, 35);
		}
		return text;
	}

	private static string BuildSessionFolderPath(string baseDir, DateTimeOffset now, int sess_count)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return SessionCrashScope.BuildFolderPath(baseDir, now, sess_count);
	}

	private static string BuildSessionJsonFilePath(string sessionRoot)
	{
		return Path.Combine(sessionRoot, "sim_session.json");
	}
}
