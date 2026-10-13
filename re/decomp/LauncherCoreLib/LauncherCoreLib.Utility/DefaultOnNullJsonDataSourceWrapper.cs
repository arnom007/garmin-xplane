using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Utility;

public class DefaultOnNullJsonDataSourceWrapper<TJsonModel> : TypedSyncedJsonDataSourceWrapper<TJsonModel> where TJsonModel : class
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetValue_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TJsonModel> _003C_003Et__builder;

		public DefaultOnNullJsonDataSourceWrapper<TJsonModel> _003C_003E4__this;

		private ConfiguredTaskAwaiter<TJsonModel> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultOnNullJsonDataSourceWrapper<TJsonModel> defaultOnNullJsonDataSourceWrapper = _003C_003E4__this;
			TJsonModel result2;
			try
			{
				ConfiguredTaskAwaiter<TJsonModel> awaiter;
				if (num != 0)
				{
					awaiter = ((TypedSyncedJsonDataSourceWrapper<TJsonModel>)defaultOnNullJsonDataSourceWrapper).GetValue().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TJsonModel>, _003CGetValue_003Ed__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TJsonModel>);
					num = (_003C_003E1__state = -1);
				}
				TJsonModel result = awaiter.GetResult();
				result2 = defaultOnNullJsonDataSourceWrapper.ValueOrCreateDefault(result);
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

	private readonly Func<TJsonModel> _defaultFactory;

	public DefaultOnNullJsonDataSourceWrapper(ISynchronizedJsonDataSource dataSource, string jsonPath, Func<TJsonModel> defaultFactory)
		: base(dataSource, jsonPath)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_defaultFactory = defaultFactory ?? throw new ArgumentNullException("defaultFactory");
	}

	[AsyncStateMachine(typeof(DefaultOnNullJsonDataSourceWrapper<>._003CGetValue_003Ed__2))]
	public override async System.Threading.Tasks.Task<TJsonModel?> GetValue()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ValueOrCreateDefault(await base.GetValue().DetachContext());
	}

	public override TJsonModel? GetValueSync()
	{
		return ValueOrCreateDefault(base.GetValueSync());
	}

	private TJsonModel ValueOrCreateDefault(TJsonModel? model)
	{
		if (EqualityComparer<TJsonModel>.Default.Equals(model, (TJsonModel)null))
		{
			model = _defaultFactory.Invoke();
		}
		return model;
	}
}
