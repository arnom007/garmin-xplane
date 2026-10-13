using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class SessionEnvironmentExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CUpdateTrainerData_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<TrainerSessionData>> asyncUpdates;

		public ISessionEnvironment sessionEnvironment;

		private ConfiguredTaskAwaiter<TrainerSessionData[]> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ISessionEnvironment result;
			try
			{
				ConfiguredTaskAwaiter<TrainerSessionData[]> awaiter;
				if (num != 0)
				{
					awaiter = System.Threading.Tasks.Task.WhenAll<TrainerSessionData>(asyncUpdates).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TrainerSessionData[]>, _003CUpdateTrainerData_003Ed__5>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TrainerSessionData[]>);
					num = (_003C_003E1__state = -1);
				}
				ImmutableDictionary<TrainerIdentifier, TrainerSessionData> trainerData = ImmutableDictionary.ToImmutableDictionary<TrainerSessionData, TrainerIdentifier>((System.Collections.Generic.IEnumerable<TrainerSessionData>)awaiter.GetResult(), (Func<TrainerSessionData, TrainerIdentifier>)TrainerSessionData.SessionDataKeySelector);
				SessionDataBuilder sessionDataBuilder = sessionEnvironment.Data.ToBuilder();
				sessionDataBuilder.TrainerData = (IImmutableDictionary<TrainerIdentifier, TrainerSessionData>)(object)trainerData;
				result = sessionEnvironment.UpdateData(sessionDataBuilder.ToImmutable());
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

	public static ISessionEnvironment UpdateData(this ISessionEnvironment environment, SessionData newData)
	{
		if (newData == environment.Data)
		{
			return environment;
		}
		return new DefaultSessionEnvironment(environment.Inputs, newData ?? new SessionData(), environment.WorkflowAttachments);
	}

	public static ISessionEnvironment UpdateData(this ISessionEnvironment environment, Action<SessionDataBuilder> modifySessionData)
	{
		SessionDataBuilder sessionDataBuilder = environment.Data.ToBuilder();
		modifySessionData.Invoke(sessionDataBuilder);
		return environment.UpdateData(sessionDataBuilder.ToImmutable());
	}

	public static ISessionEnvironment UpdateAttachments(this ISessionEnvironment environment, ImmutableDictionary<object, object> newAttachments)
	{
		if (newAttachments == environment.WorkflowAttachments)
		{
			return environment;
		}
		return new DefaultSessionEnvironment(environment.Inputs, environment.Data, newAttachments);
	}

	public static SessionData UpdateAllTrainerData(this SessionData sessionData, Action<TrainerSessionData, TrainerSessionDataBuilder> modifyTrainer)
	{
		return sessionData.UpdateTrainerData((KeyValuePair<TrainerIdentifier, TrainerSessionData> tsd) => true, modifyTrainer);
	}

	public static ISessionEnvironment UpdateTrainerData(this ISessionEnvironment sessionEnvironment, System.Collections.Generic.IEnumerable<TrainerSessionData> trainerSessionDatas)
	{
		ImmutableDictionary<TrainerIdentifier, TrainerSessionData> trainerData = ImmutableDictionary.ToImmutableDictionary<TrainerSessionData, TrainerIdentifier>(trainerSessionDatas, (Func<TrainerSessionData, TrainerIdentifier>)TrainerSessionData.SessionDataKeySelector);
		SessionDataBuilder sessionDataBuilder = sessionEnvironment.Data.ToBuilder();
		sessionDataBuilder.TrainerData = (IImmutableDictionary<TrainerIdentifier, TrainerSessionData>)(object)trainerData;
		return sessionEnvironment.UpdateData(sessionDataBuilder.ToImmutable());
	}

	[AsyncStateMachine(typeof(_003CUpdateTrainerData_003Ed__5))]
	public static async System.Threading.Tasks.Task<ISessionEnvironment> UpdateTrainerData(this ISessionEnvironment sessionEnvironment, System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<TrainerSessionData>> asyncUpdates)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ImmutableDictionary<TrainerIdentifier, TrainerSessionData> trainerData = ImmutableDictionary.ToImmutableDictionary<TrainerSessionData, TrainerIdentifier>((System.Collections.Generic.IEnumerable<TrainerSessionData>)(await System.Threading.Tasks.Task.WhenAll<TrainerSessionData>(asyncUpdates).DetachContext()), (Func<TrainerSessionData, TrainerIdentifier>)TrainerSessionData.SessionDataKeySelector);
		SessionDataBuilder sessionDataBuilder = sessionEnvironment.Data.ToBuilder();
		sessionDataBuilder.TrainerData = (IImmutableDictionary<TrainerIdentifier, TrainerSessionData>)(object)trainerData;
		return sessionEnvironment.UpdateData(sessionDataBuilder.ToImmutable());
	}

	public static SessionData UpdateTrainerData(this SessionData sessionData, Func<KeyValuePair<TrainerIdentifier, TrainerSessionData>, bool> updateIfTrue, Action<TrainerSessionData, TrainerSessionDataBuilder> modifyTrainer)
	{
		Action<TrainerSessionData, TrainerSessionDataBuilder> modifyTrainer2 = modifyTrainer;
		System.Collections.Generic.IEnumerable<KeyValuePair<TrainerIdentifier, TrainerSessionData>> items = Enumerable.Select<KeyValuePair<TrainerIdentifier, TrainerSessionData>, KeyValuePair<TrainerIdentifier, TrainerSessionData>>(Enumerable.Where<KeyValuePair<TrainerIdentifier, TrainerSessionData>>((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerIdentifier, TrainerSessionData>>)sessionData.TrainerData, updateIfTrue), (Func<KeyValuePair<TrainerIdentifier, TrainerSessionData>, KeyValuePair<TrainerIdentifier, TrainerSessionData>>)delegate(KeyValuePair<TrainerIdentifier, TrainerSessionData> kv)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			TrainerSessionDataBuilder trainerSessionDataBuilder = kv.Value.ToBuilder();
			modifyTrainer2.Invoke(kv.Value, trainerSessionDataBuilder);
			return new KeyValuePair<TrainerIdentifier, TrainerSessionData>(kv.Key, trainerSessionDataBuilder.ToImmutable());
		});
		IImmutableDictionary<TrainerIdentifier, TrainerSessionData> trainerData = sessionData.TrainerData.SetItems(items);
		SessionDataBuilder sessionDataBuilder = sessionData.ToBuilder();
		sessionDataBuilder.TrainerData = trainerData;
		return sessionDataBuilder.ToImmutable();
	}

	public static ISessionEnvironment AttachWorkflowState(this ISessionEnvironment environment, object key, object state)
	{
		ImmutableDictionary<object, object> newAttachments = environment.WorkflowAttachments.Add(key, state);
		return environment.UpdateAttachments(newAttachments);
	}

	public static TResult? GetWorkflowState<TResult>(this ISessionEnvironment environment, object key)
	{
		object obj = default(object);
		if (environment.WorkflowAttachments.TryGetValue(key, ref obj))
		{
			return (TResult)obj;
		}
		return default(TResult);
	}

	public static ISessionEnvironment DetachWorkflowState(this ISessionEnvironment environment, object key)
	{
		ImmutableDictionary<object, object> newAttachments = environment.WorkflowAttachments.Remove(key);
		return environment.UpdateAttachments(newAttachments);
	}
}
