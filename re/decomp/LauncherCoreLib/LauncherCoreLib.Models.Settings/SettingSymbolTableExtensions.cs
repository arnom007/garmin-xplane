using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Settings;

public static class SettingSymbolTableExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CLoadSettingConstraints_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ISettingSymbolTable table;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			try
			{
				AsyncSubject<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> asyncSubject;
				if (num != 0)
				{
					asyncSubject = Enumerable.Select<IActiveSetting, IObservable<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>(table.Settings, (Func<IActiveSetting, IObservable<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>>)((IActiveSetting s) => s.WhenAvailableOptionsChange.Take<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>(1))).Merge().LastOrDefaultAsync<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>()
						.GetAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, _003CLoadSettingConstraints_003Ed__0>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				asyncSubject.GetResult();
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

	[AsyncStateMachine(typeof(_003CLoadSettingConstraints_003Ed__0))]
	public static System.Threading.Tasks.Task LoadSettingConstraints(this ISettingSymbolTable table)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CLoadSettingConstraints_003Ed__0 _003CLoadSettingConstraints_003Ed__ = default(_003CLoadSettingConstraints_003Ed__0);
		_003CLoadSettingConstraints_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CLoadSettingConstraints_003Ed__.table = table;
		_003CLoadSettingConstraints_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CLoadSettingConstraints_003Ed__._003C_003Et__builder)).Start<_003CLoadSettingConstraints_003Ed__0>(ref _003CLoadSettingConstraints_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CLoadSettingConstraints_003Ed__._003C_003Et__builder)).Task;
	}
}
