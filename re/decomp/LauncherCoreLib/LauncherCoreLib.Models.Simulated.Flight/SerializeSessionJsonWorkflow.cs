using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.Persistence;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class SerializeSessionJsonWorkflow : PreflightOnlyWorkflow
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CPreFlight_003Ed__6 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public ISessionEnvironment sessionEnvironment;

		public SerializeSessionJsonWorkflow _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SerializeSessionJsonWorkflow serializeSessionJsonWorkflow = _003C_003E4__this;
			ISessionEnvironment result;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					string sessionPath = GetSessionPath(sessionEnvironment);
					if (JsonPathIsInvalid(sessionPath))
					{
						throw new InvalidOperationException((sessionPath ?? "<NULL>") + " is not a valid path to store JSON for this session.");
					}
					SimSessionData persistenceModel = new SimSessionData
					{
						Configuration_name = sessionEnvironment.Inputs.ConfigurationName,
						Session_number = sessionEnvironment.Inputs.SessionIndex,
						Environment_settings = BuildEnvironmentSettings(sessionEnvironment, serializeSessionJsonWorkflow._sessionMode),
						Trainers = BuildTrainerDictionary(sessionEnvironment),
						Install_slots = BuildInstallSlots(sessionEnvironment.Data.TrainerData)
					};
					ConfiguredTaskAwaitable val = serializeSessionJsonWorkflow._persistence.Commit(persistenceModel, sessionPath).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPreFlight_003Ed__6>(ref awaiter, ref this);
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
				result = sessionEnvironment;
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

	private readonly IPersistArbitraryObjects _persistence;

	private readonly GetAircraftSessionBehaviorMode _sessionMode;

	[field: CompilerGenerated]
	public static GetAircraftSessionBehaviorMode DefaultAircraftSessionBehaviorMode
	{
		[CompilerGenerated]
		get;
	} = (AircraftSnapshotData _) => SessionBehaviorMode.ALaCarteTrainers;


	public SerializeSessionJsonWorkflow(IPersistArbitraryObjects fileSystem, GetAircraftSessionBehaviorMode sessionMode)
	{
		_persistence = fileSystem;
		_sessionMode = sessionMode;
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__6))]
	public override async System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string sessionPath = GetSessionPath(sessionEnvironment);
		if (JsonPathIsInvalid(sessionPath))
		{
			throw new InvalidOperationException((sessionPath ?? "<NULL>") + " is not a valid path to store JSON for this session.");
		}
		SimSessionData persistenceModel = new SimSessionData
		{
			Configuration_name = sessionEnvironment.Inputs.ConfigurationName,
			Session_number = sessionEnvironment.Inputs.SessionIndex,
			Environment_settings = BuildEnvironmentSettings(sessionEnvironment, _sessionMode),
			Trainers = BuildTrainerDictionary(sessionEnvironment),
			Install_slots = BuildInstallSlots(sessionEnvironment.Data.TrainerData)
		};
		await _persistence.Commit(persistenceModel, sessionPath).DetachContext();
		return sessionEnvironment;
	}

	private static Dictionary<string, JToken> BuildJTokenDictionary(TrainerSessionData trainerData)
	{
		return Enumerable.ToDictionary<KeyValuePair<string, string>, string, JToken>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)trainerData.Settings, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> s) => s.Key), (Func<KeyValuePair<string, string>, JToken>)((KeyValuePair<string, string> s) => JToken.FromObject(s.Value, SerializationUtil.JSON_SERIALIZER.Value)));
	}

	private static IDictionary<string, JToken> BuildExtraEnvironmentSettings()
	{
		return (IDictionary<string, JToken>)(object)new Dictionary<string, JToken>();
	}

	private static bool CheckIfPrebuilt(ISessionEnvironment sessionEnvironment, GetAircraftSessionBehaviorMode sessionMode)
	{
		return sessionMode(sessionEnvironment.Inputs.CreationRequestOrThrow().AircraftSnapShot) == SessionBehaviorMode.PurposeBuiltAircraft;
	}

	private static bool JsonPathIsInvalid(string simSessionPath)
	{
		return string.IsNullOrEmpty(simSessionPath);
	}

	private static Mode ResolveMode(SessionData data)
	{
		Mode result = default(Mode);
		if (((IReadOnlyDictionary<string, string>)(object)data.EnvironmentSettings).ContainsKey("Mode") && System.Enum.TryParse<Mode>(((IReadOnlyDictionary<string, string>)(object)data.EnvironmentSettings)["Mode"], ref result))
		{
			return result;
		}
		return Mode.DEMO;
	}

	private static Trainer_settings BuildSettings(TrainerSessionData trainerData)
	{
		return new Trainer_settings
		{
			Instance_id = trainerData.InstallSlot.InstanceId,
			Model = trainerData.PackageInfo.ModelInfo.ModelId,
			AdditionalProperties = (IDictionary<string, JToken>)(object)BuildJTokenDictionary(trainerData)
		};
	}

	private static Environment_settings BuildEnvironmentSettings(ISessionEnvironment sessionEnvironment, GetAircraftSessionBehaviorMode sessionMode)
	{
		return new Environment_settings
		{
			Airframe = SimSessionData.ConvertActualToSerial(sessionEnvironment.Inputs.AirframeCCT),
			Mode = ResolveMode(sessionEnvironment.Data),
			Auto_poweron = true,
			AdditionalProperties = BuildExtraEnvironmentSettings(),
			Pbt_aircraft = CheckIfPrebuilt(sessionEnvironment, sessionMode)
		};
	}

	private static Dictionary<string, Dictionary<string, string>> BuildInstallSlots(IImmutableDictionary<TrainerIdentifier, TrainerSessionData> trainerData)
	{
		return Enumerable.ToDictionary<IGrouping<AvionicsType, ValueTuple<InstallationSlot, TrainerIdentifier>>, string, Dictionary<string, string>>(Enumerable.GroupBy<ValueTuple<InstallationSlot, TrainerIdentifier>, AvionicsType>(Enumerable.Select<KeyValuePair<TrainerIdentifier, TrainerSessionData>, ValueTuple<InstallationSlot, TrainerIdentifier>>((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerIdentifier, TrainerSessionData>>)trainerData, (Func<KeyValuePair<TrainerIdentifier, TrainerSessionData>, ValueTuple<InstallationSlot, TrainerIdentifier>>)((KeyValuePair<TrainerIdentifier, TrainerSessionData> tsd) => new ValueTuple<InstallationSlot, TrainerIdentifier>(tsd.Value.InstallSlot, tsd.Value.EndpointName))), (Func<ValueTuple<InstallationSlot, TrainerIdentifier>, AvionicsType>)((ValueTuple<InstallationSlot, TrainerIdentifier> trainerSlot) => trainerSlot.Item1.Category)), (Func<IGrouping<AvionicsType, ValueTuple<InstallationSlot, TrainerIdentifier>>, string>)((IGrouping<AvionicsType, ValueTuple<InstallationSlot, TrainerIdentifier>> slotsByCategory) => ((object)SimSessionData.ConvertActualToSerial(slotsByCategory.Key)).ToString()), (Func<IGrouping<AvionicsType, ValueTuple<InstallationSlot, TrainerIdentifier>>, Dictionary<string, string>>)((IGrouping<AvionicsType, ValueTuple<InstallationSlot, TrainerIdentifier>> slotsByCategory) => Enumerable.ToDictionary<ValueTuple<InstallationSlot, TrainerIdentifier>, string, string>((System.Collections.Generic.IEnumerable<ValueTuple<InstallationSlot, TrainerIdentifier>>)slotsByCategory, (Func<ValueTuple<InstallationSlot, TrainerIdentifier>, string>)((ValueTuple<InstallationSlot, TrainerIdentifier> slot) => slot.Item1.InstanceId.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)), (Func<ValueTuple<InstallationSlot, TrainerIdentifier>, string>)((ValueTuple<InstallationSlot, TrainerIdentifier> slot) => ((object)slot.Item2).ToString()))));
	}

	private static Dictionary<string, Trainer> BuildTrainerDictionary(ISessionEnvironment sessionEnvironment)
	{
		return Enumerable.ToDictionary<Trainer, string>(Enumerable.Select<TrainerSessionData, Trainer>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, Trainer>)((TrainerSessionData trainerData) => new Trainer
		{
			Family_name = trainerData.PackageInfo.FamilyId,
			Id = trainerData.EndpointName,
			Slot = SimSessionData.ConvertActualToSerial(trainerData.InstallSlot),
			Settings = BuildSettings(trainerData)
		})), (Func<Trainer, string>)((Trainer t) => t.Id));
	}

	private static string GetSessionPath(ISessionEnvironment sessionEnvironment)
	{
		return sessionEnvironment.Inputs.SessionJsonPath;
	}
}
