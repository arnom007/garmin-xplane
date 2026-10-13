using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LOLA.Logging;

namespace LauncherCoreLib.Models.Settings;

public class ActiveSetting : IActiveSetting
{
	private readonly IConstraintWatcher _constraintWatcher;

	private readonly Func<string> _runtimePathResolver;

	private string _currentValue = string.Empty;

	[CompilerGenerated]
	private EventHandler<SettingValueChangedEventArgs>? m_ValueChanged;

	[field: CompilerGenerated]
	public ISettingDefinition Definition
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<ISettingValueDefinition> CachedAvailableOptions
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public string CurrentValue
	{
		get
		{
			return _currentValue;
		}
		set
		{
			string text = Interlocked.Exchange<string>(ref _currentValue, value);
			if (text != value)
			{
				string runtimeSettingPath = RuntimeSettingPath;
				Logger.Log(LogLevel.Info, "[Setting Change] id: {0}{3}old: {1}{3}new: {2}", (runtimeSettingPath.Length > 0) ? runtimeSettingPath : ((object)this).ToString(), NiceNullString(text), NiceNullString(value), Environment.NewLine + "\t");
				RaiseValueChanged(text, value);
			}
		}
	}

	public IObservable<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>> WhenAvailableOptionsChange => (IObservable<System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>>)(object)_constraintWatcher.WatchConstraints(Definition.Values).Do<ISettingValueDefinition[]>((Action<ISettingValueDefinition[]>)([CompilerGenerated] (ISettingValueDefinition[] options) =>
	{
		CachedAvailableOptions = options;
		if (Definition.ControlType.ControlUsesIdentifiers() && !Enumerable.Any<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)options, (Func<ISettingValueDefinition, bool>)([CompilerGenerated] (ISettingValueDefinition o) => o.ValueId == CurrentValue)))
		{
			ISettingValueDefinition settingValueDefinition = Enumerable.FirstOrDefault<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)options, (Func<ISettingValueDefinition, bool>)([CompilerGenerated] (ISettingValueDefinition o) => o.ValueId == Definition.DefaultValue)) ?? Enumerable.FirstOrDefault<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)options);
			if (settingValueDefinition != null)
			{
				this.Select(settingValueDefinition);
			}
		}
	}));

	public string RuntimeSettingPath => _runtimePathResolver.Invoke();

	public SettingIdentifier LocalId => Definition.SettingId.LocalId;

	public event EventHandler<SettingValueChangedEventArgs>? ValueChanged
	{
		[CompilerGenerated]
		add
		{
			EventHandler<SettingValueChangedEventArgs> val = this.m_ValueChanged;
			EventHandler<SettingValueChangedEventArgs> val2;
			do
			{
				val2 = val;
				EventHandler<SettingValueChangedEventArgs> val3 = (EventHandler<SettingValueChangedEventArgs>)(object)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler<SettingValueChangedEventArgs>>(ref this.m_ValueChanged, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<SettingValueChangedEventArgs> val = this.m_ValueChanged;
			EventHandler<SettingValueChangedEventArgs> val2;
			do
			{
				val2 = val;
				EventHandler<SettingValueChangedEventArgs> val3 = (EventHandler<SettingValueChangedEventArgs>)(object)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler<SettingValueChangedEventArgs>>(ref this.m_ValueChanged, val3, val2);
			}
			while (val != val2);
		}
	}

	public ActiveSetting(ISettingDefinition definition, IConstraintWatcher constraintResolver, Func<string>? runtimePathResolver = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (definition == null)
		{
			throw new ArgumentNullException("definition");
		}
		if (definition.SettingId == SettingIdentifier.Empty)
		{
			throw new ArgumentException("Active settings require a non empty identifier.");
		}
		Definition = definition;
		_constraintWatcher = constraintResolver ?? throw new ArgumentNullException("constraintResolver");
		_runtimePathResolver = runtimePathResolver ?? ((Func<string>)(() => string.Empty));
		CachedAvailableOptions = (System.Collections.Generic.IReadOnlyList<ISettingValueDefinition>)Enumerable.ToList<ISettingValueDefinition>(Definition.Values);
		if (Definition.ControlType.ControlUsesIdentifiers())
		{
			if (!Enumerable.Any<ISettingValueDefinition>(Definition.Values, (Func<ISettingValueDefinition, bool>)([CompilerGenerated] (ISettingValueDefinition v) => v.ValueId.LocalId == Definition.DefaultValue)))
			{
				throw new KeyNotFoundException("The default value '" + Definition.DefaultValue + "' is not a valid value in the definition.");
			}
			CurrentValue = SettingIdentifier.Concatenate(Definition.SettingId, Definition.DefaultValue);
		}
		else
		{
			CurrentValue = Definition.DefaultValue;
		}
	}

	public virtual string ToString()
	{
		return Definition.SettingId;
	}

	protected void RaiseValueChanged(string previous, string current)
	{
		this.ValueChanged?.Invoke((object)this, new SettingValueChangedEventArgs(this, previous, current));
	}

	private static string NiceNullString(string value)
	{
		return value ?? "NULL";
	}
}
