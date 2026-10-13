using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Settings;

public static class ActiveSettingExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public string displayName;

		internal bool _003CHasOptionWithDisplayName_003Eb__0(ISettingValueDefinition v)
		{
			return v.DisplayText == displayName;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		public string simulatorValue;

		internal bool _003CSelectBySimulatorValue_003Eb__0(ISettingValueDefinition v)
		{
			return v.SimulatorValue == simulatorValue;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		public string displayName;

		internal bool _003CSelectByDisplayName_003Eb__0(ISettingValueDefinition v)
		{
			return v.DisplayText == displayName;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetAvailableOptions_003Ed__19 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> _003C_003Et__builder;

		public IActiveSetting setting;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> result;
			try
			{
				AsyncSubject<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> asyncSubject;
				if (num != 0)
				{
					asyncSubject = setting.WhenAvailableOptionsChange.FirstAsync<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>().GetAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, _003CGetAvailableOptions_003Ed__19>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				result = asyncSubject.GetResult();
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CHasOptionWithDisplayName_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public string displayName;

		public IActiveSetting setting;

		private _003C_003Ec__DisplayClass3_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> _003C_003Eu__1;

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
			bool result;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0();
					_003C_003E8__1.displayName = displayName;
					awaiter = GetAvailableOptions(setting).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, _003CHasOptionWithDisplayName_003Ed__3>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>);
					num = (_003C_003E1__state = -1);
				}
				result = Enumerable.Any<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)awaiter.GetResult(), (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.DisplayText == _003C_003E8__1.displayName));
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CSelectByDisplayName_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISettingValueDefinition> _003C_003Et__builder;

		public string displayName;

		public IActiveSetting setting;

		private _003C_003Ec__DisplayClass9_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ISettingValueDefinition result;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass9_0();
					_003C_003E8__1.displayName = displayName;
					awaiter = GetAvailableOptions(setting).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, _003CSelectByDisplayName_003Ed__9>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>);
					num = (_003C_003E1__state = -1);
				}
				ISettingValueDefinition settingValueDefinition = Enumerable.FirstOrDefault<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)awaiter.GetResult(), (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.DisplayText == _003C_003E8__1.displayName));
				if (settingValueDefinition == null)
				{
					throw new InvalidOperationException(((object)setting)?.ToString() + " has no matching option with display name: '" + _003C_003E8__1.displayName + "'");
				}
				setting.Select(settingValueDefinition);
				result = settingValueDefinition;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CSelectBySimulatorValue_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISettingValueDefinition> _003C_003Et__builder;

		public string simulatorValue;

		public IActiveSetting setting;

		private _003C_003Ec__DisplayClass8_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ISettingValueDefinition result;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass8_0();
					_003C_003E8__1.simulatorValue = simulatorValue;
					awaiter = GetAvailableOptions(setting).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>, _003CSelectBySimulatorValue_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>);
					num = (_003C_003E1__state = -1);
				}
				ISettingValueDefinition settingValueDefinition = Enumerable.FirstOrDefault<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)awaiter.GetResult(), (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.SimulatorValue == _003C_003E8__1.simulatorValue));
				if (settingValueDefinition == null)
				{
					throw new InvalidOperationException("No matching setting to the simulator value");
				}
				setting.Select(settingValueDefinition);
				result = settingValueDefinition;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	public static string DisplayName(this IActiveSetting setting)
	{
		return setting.Definition.DisplayText;
	}

	public static string Description(this IActiveSetting setting)
	{
		return setting.Definition.Description;
	}

	public static string SelectedDisplayName(this IActiveSetting setting)
	{
		IActiveSetting setting2 = setting;
		ISettingValueDefinition settingValueDefinition = Enumerable.FirstOrDefault<ISettingValueDefinition>(setting2.Definition.Values, (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.ValueId == setting2.CurrentValue));
		if (settingValueDefinition == null)
		{
			return "Unknown Option";
		}
		return settingValueDefinition.DisplayText;
	}

	[AsyncStateMachine(typeof(_003CHasOptionWithDisplayName_003Ed__3))]
	public static async System.Threading.Tasks.Task<bool> HasOptionWithDisplayName(this IActiveSetting setting, string displayName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string displayName2 = displayName;
		return Enumerable.Any<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)(await GetAvailableOptions(setting).DetachContext()), (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.DisplayText == displayName2));
	}

	public static void Select(this IActiveSetting setting, ISettingValueDefinition valueDefinition)
	{
		ThrowIfNotIdentifierControl(setting, "Select");
		setting.CurrentValue = valueDefinition.ValueId;
	}

	public static ISettingValueDefinition? GetOptionByLocalValueId(this IActiveSetting setting, SettingIdentifier valueLocalId)
	{
		SettingIdentifier valueLocalId2 = valueLocalId;
		ThrowIfNotIdentifierControl(setting, "GetOptionByLocalValueId");
		return Enumerable.FirstOrDefault<ISettingValueDefinition>(setting.Definition.Values, (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition o) => o.ValueId.LocalId == valueLocalId2));
	}

	public static void SelectByLocalValueId(this IActiveSetting setting, SettingIdentifier valueLocalId)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfNotIdentifierControl(setting, "SelectByLocalValueId");
		ISettingValueDefinition optionByLocalValueId = setting.GetOptionByLocalValueId(valueLocalId);
		if (optionByLocalValueId == null)
		{
			throw new KeyNotFoundException("'" + valueLocalId + "' is not an option in the definition of '" + setting.LocalId + "'.");
		}
		setting.CurrentValue = optionByLocalValueId.ValueId;
	}

	public static void SelectByFullValueId(this IActiveSetting setting, SettingIdentifier valueId)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		SettingIdentifier valueId2 = valueId;
		ThrowIfNotIdentifierControl(setting, "SelectByFullValueId");
		if (Enumerable.FirstOrDefault<ISettingValueDefinition>(setting.Definition.Values, (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition o) => o.ValueId == valueId2)) == null)
		{
			throw new KeyNotFoundException("'" + valueId2 + "' is not an option in the definition of '" + setting.LocalId + "'.");
		}
		setting.CurrentValue = valueId2;
	}

	[AsyncStateMachine(typeof(_003CSelectBySimulatorValue_003Ed__8))]
	public static async System.Threading.Tasks.Task<ISettingValueDefinition> SelectBySimulatorValue(this IActiveSetting setting, string simulatorValue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string simulatorValue2 = simulatorValue;
		ISettingValueDefinition settingValueDefinition = Enumerable.FirstOrDefault<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)(await GetAvailableOptions(setting).DetachContext()), (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.SimulatorValue == simulatorValue2));
		if (settingValueDefinition == null)
		{
			throw new InvalidOperationException("No matching setting to the simulator value");
		}
		setting.Select(settingValueDefinition);
		return settingValueDefinition;
	}

	[AsyncStateMachine(typeof(_003CSelectByDisplayName_003Ed__9))]
	public static async System.Threading.Tasks.Task<ISettingValueDefinition> SelectByDisplayName(this IActiveSetting setting, string displayName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string displayName2 = displayName;
		ISettingValueDefinition settingValueDefinition = Enumerable.FirstOrDefault<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)(await GetAvailableOptions(setting).DetachContext()), (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.DisplayText == displayName2));
		if (settingValueDefinition == null)
		{
			throw new InvalidOperationException(((object)setting)?.ToString() + " has no matching option with display name: '" + displayName2 + "'");
		}
		setting.Select(settingValueDefinition);
		return settingValueDefinition;
	}

	public static SettingIdentifier SelectedLocalId(this IActiveSetting setting)
	{
		return ((SettingIdentifier)setting.CurrentValue).LocalId;
	}

	public static string SelectedSimulatorValue(this IActiveSetting setting)
	{
		IActiveSetting setting2 = setting;
		if (setting2.Definition.ControlType.ControlUsesIdentifiers())
		{
			return Enumerable.First<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)setting2.CachedAvailableOptions, (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition o) => o.ValueId == setting2.CurrentValue)).SimulatorValue;
		}
		return setting2.CurrentValue;
	}

	public static IObservable<string> SettingValuesOverTime(this IActiveSetting setting)
	{
		return Observable.Return(setting.CurrentValue).Concat<string>(setting.WhenValueChanges().Select<SettingValueChangedEventArgs, string>((Func<SettingValueChangedEventArgs, string>)((SettingValueChangedEventArgs svc) => svc.NewValue)));
	}

	public static IObservable<SettingValueChangedEventArgs> WhenValueChanges(this IActiveSetting setting)
	{
		IActiveSetting setting2 = setting;
		return Observable.FromEventPattern<SettingValueChangedEventArgs>(delegate(EventHandler<SettingValueChangedEventArgs> h)
		{
			setting2.ValueChanged += h;
		}, delegate(EventHandler<SettingValueChangedEventArgs> h)
		{
			setting2.ValueChanged -= h;
		}).Select<EventPattern<SettingValueChangedEventArgs>, SettingValueChangedEventArgs>((Func<EventPattern<SettingValueChangedEventArgs>, SettingValueChangedEventArgs>)((EventPattern<SettingValueChangedEventArgs> ea) => ea.EventArgs));
	}

	public static IObservable<SettingHasOptionsChanged> WhenSettingOptionsChange(this IActiveSetting setting)
	{
		IActiveSetting setting2 = setting;
		return setting2.WhenAvailableOptionsChange.Select<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>, SettingHasOptionsChanged>((Func<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>, SettingHasOptionsChanged>)((System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> availableOptions) => new SettingHasOptionsChanged(setting2, availableOptions)));
	}

	public static bool SettingHasOptions(this IActiveSetting setting)
	{
		return setting.SettingHasOptions((System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition>)setting.CachedAvailableOptions);
	}

	public static bool SettingHasOptions(this IActiveSetting setting, System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition> options)
	{
		return SettingHasOptions(setting.Definition, options);
	}

	public static bool SettingHasOptions(ISettingDefinition definition, System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition> options)
	{
		if (definition.ControlType == SettingUIControlType.Picklist)
		{
			return options.Count >= 2;
		}
		return false;
	}

	public static bool LoadPersistedOption(this IActiveSetting setting, KeyValuePair<SettingIdentifier, string> persistedOption)
	{
		if (SettingIdentifier.IsValidFullId(persistedOption.Value))
		{
			SettingIdentifier settingIdentifier = new SettingIdentifier(persistedOption.Value);
			if (setting.GetOptionByLocalValueId(settingIdentifier.LocalId) != null)
			{
				setting.SelectByLocalValueId(settingIdentifier.LocalId);
				return true;
			}
			Logger.Log(LogLevel.Warning, "Could not match persisted option: " + settingIdentifier);
		}
		else
		{
			Logger.Log(LogLevel.Error, "An invalid setting identifier was persisted: " + persistedOption.Value);
		}
		return false;
	}

	[AsyncStateMachine(typeof(_003CGetAvailableOptions_003Ed__19))]
	private static async System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> GetAvailableOptions(IActiveSetting setting)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await setting.WhenAvailableOptionsChange.FirstAsync<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>();
	}

	private static void ThrowIfNotIdentifierControl(IActiveSetting setting, [CallerMemberName] string? methodName = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!setting.Definition.ControlType.ControlUsesIdentifiers())
		{
			throw new InvalidOperationException(methodName + " can only be used for settings that use identifiers as their values.");
		}
	}
}
