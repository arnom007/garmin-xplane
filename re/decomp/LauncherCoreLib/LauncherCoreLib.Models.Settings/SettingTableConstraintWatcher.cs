using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LOLA.Reactive;

namespace LauncherCoreLib.Models.Settings;

public class SettingTableConstraintWatcher : IConstraintWatcher
{
	private class SettingWatch
	{
		[field: CompilerGenerated]
		public ValueConstraintDefinition Constraint
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public IActiveSetting ResolvedSetting
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public string CurrentValue
		{
			get
			{
				if (!ResolvedSetting.Definition.ControlType.ControlUsesIdentifiers())
				{
					return ResolvedSetting.CurrentValue;
				}
				return ShortenToLocalId(ResolvedSetting.CurrentValue);
			}
		}

		public bool IsInBounds
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				if (Constraint.EnableOnValues.Length != 0)
				{
					return Constraint.EnableOnValues.Contains(CurrentValue);
				}
				return true;
			}
		}

		public bool IsOutofBounds => Constraint.DisableOnValues.Contains(CurrentValue);

		public SettingWatch(ValueConstraintDefinition constraint, IActiveSetting setting)
		{
			Constraint = constraint;
			ResolvedSetting = setting;
		}

		private static string ShortenToLocalId(string currentValue)
		{
			return ((SettingIdentifier)currentValue).LocalId;
		}
	}

	private sealed class SettingWatch<TContrained> : SettingWatch
	{
		public TContrained ConstrainedValue
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public SettingWatch(ValueConstraintDefinition constraint, IActiveSetting setting, TContrained value)
			: base(constraint, setting)
		{
			ConstrainedValue = value;
		}
	}

	private readonly ISettingSymbolTable _settingTable;

	public SettingTableConstraintWatcher(ISettingSymbolTable settingTable)
	{
		_settingTable = settingTable;
	}

	public IObservable<bool> AreConstraintsInBounds(System.Collections.Generic.IReadOnlyCollection<ValueConstraintDefinition> constraints)
	{
		System.Collections.Generic.IReadOnlyCollection<ValueConstraintDefinition> constraints2 = constraints;
		if (constraints2.Count == 0)
		{
			return Observable.Return(value: true);
		}
		return Observable.Create<bool>((Func<IObserver<bool>, System.IDisposable>)delegate(IObserver<bool> o)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			List<SettingWatch> watches = Enumerable.ToList<SettingWatch>(Enumerable.Select<ValueConstraintDefinition, SettingWatch>((System.Collections.Generic.IEnumerable<ValueConstraintDefinition>)constraints2, (Func<ValueConstraintDefinition, SettingWatch>)((ValueConstraintDefinition constraint) => new SettingWatch(constraint, ResolveSetting(constraint.SettingToObserve)))));
			System.Collections.Generic.IEnumerable<SettingWatch> enumerable = Enumerable.Where<SettingWatch>((System.Collections.Generic.IEnumerable<SettingWatch>)watches, (Func<SettingWatch, bool>)((SettingWatch w) => w.ResolvedSetting == null));
			if (Enumerable.Any<SettingWatch>(enumerable))
			{
				string text = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Failed to resolve constraints: {0}", (object)string.Join<SettingIdentifier>(", ", Enumerable.Select<SettingWatch, SettingIdentifier>(enumerable, (Func<SettingWatch, SettingIdentifier>)((SettingWatch ur) => ur.Constraint.SettingToObserve))));
				o.OnError((System.Exception)new KeyNotFoundException(text));
				return Disposable.Empty;
			}
			return TimingEvent.StartStreamAndConcat<SettingValueChangedEventArgs>(WhenAnyDependantSettingChanges((System.Collections.Generic.IEnumerable<SettingWatch>)watches), "Constraint Check").Select<TimingEvent, bool>((Func<TimingEvent, bool>)((TimingEvent _) => Enumerable.All<SettingWatch>((System.Collections.Generic.IEnumerable<SettingWatch>)watches, (Func<SettingWatch, bool>)((SettingWatch w) => w.IsInBounds)) && !Enumerable.Any<SettingWatch>((System.Collections.Generic.IEnumerable<SettingWatch>)watches, (Func<SettingWatch, bool>)((SettingWatch w) => w.IsOutofBounds)))).DistinctUntilChanged<bool>()
				.SubscribeSafe<bool>(o);
		});
	}

	public IObservable<ISettingValueDefinition[]> WatchConstraints(System.Collections.Generic.IEnumerable<ISettingValueDefinition> values)
	{
		ISettingValueDefinition[] unconstrainedValues = Enumerable.ToArray<ISettingValueDefinition>(Enumerable.Where<ISettingValueDefinition>(values, (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.Constraints.Length == 0)));
		ISettingValueDefinition[] constrainedValues = Enumerable.ToArray<ISettingValueDefinition>(Enumerable.Where<ISettingValueDefinition>(values, (Func<ISettingValueDefinition, bool>)((ISettingValueDefinition v) => v.Constraints.Length > 0)));
		if (constrainedValues.Length != 0)
		{
			return Observable.Create<ISettingValueDefinition[]>((Func<IObserver<ISettingValueDefinition[]>, System.IDisposable>)delegate(IObserver<ISettingValueDefinition[]> o)
			{
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Expected O, but got Unknown
				List<SettingWatch<ISettingValueDefinition>> val = Enumerable.ToList<SettingWatch<ISettingValueDefinition>>(Enumerable.SelectMany<ISettingValueDefinition, SettingWatch<ISettingValueDefinition>>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)constrainedValues, (Func<ISettingValueDefinition, System.Collections.Generic.IEnumerable<SettingWatch<ISettingValueDefinition>>>)((ISettingValueDefinition cv) => ImmutableArrayExtensions.Select<ValueConstraintDefinition, SettingWatch<ISettingValueDefinition>>(cv.Constraints, (Func<ValueConstraintDefinition, SettingWatch<ISettingValueDefinition>>)((ValueConstraintDefinition constraint) => new SettingWatch<ISettingValueDefinition>(constraint, ResolveSetting(constraint.SettingToObserve), cv))))));
				System.Collections.Generic.IEnumerable<SettingWatch<ISettingValueDefinition>> enumerable = Enumerable.Where<SettingWatch<ISettingValueDefinition>>((System.Collections.Generic.IEnumerable<SettingWatch<ISettingValueDefinition>>)val, (Func<SettingWatch<ISettingValueDefinition>, bool>)((SettingWatch<ISettingValueDefinition> w) => w.ResolvedSetting == null));
				if (Enumerable.Any<SettingWatch<ISettingValueDefinition>>(enumerable))
				{
					string text = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Failed to resolve constraints: {0}", (object)string.Join(", ", Enumerable.Select<SettingWatch<ISettingValueDefinition>, string>(enumerable, (Func<SettingWatch<ISettingValueDefinition>, string>)((SettingWatch<ISettingValueDefinition> ur) => ur.ConstrainedValue.ValueId + " -> " + ur.Constraint.SettingToObserve))));
					o.OnError((System.Exception)new KeyNotFoundException(text));
					return Disposable.Empty;
				}
				IGrouping<ISettingValueDefinition, SettingWatch<ISettingValueDefinition>>[] groupedWatches = Enumerable.ToArray<IGrouping<ISettingValueDefinition, SettingWatch<ISettingValueDefinition>>>(Enumerable.GroupBy<SettingWatch<ISettingValueDefinition>, ISettingValueDefinition>((System.Collections.Generic.IEnumerable<SettingWatch<ISettingValueDefinition>>)val, (Func<SettingWatch<ISettingValueDefinition>, ISettingValueDefinition>)((SettingWatch<ISettingValueDefinition> w) => w.ConstrainedValue)));
				return WhenAnyDependantSettingChanges((System.Collections.Generic.IEnumerable<SettingWatch>)val).Select<SettingValueChangedEventArgs, ISettingValueDefinition[]>((Func<SettingValueChangedEventArgs, ISettingValueDefinition[]>)((SettingValueChangedEventArgs _) => AvailableValues(unconstrainedValues, (System.Collections.Generic.IEnumerable<IGrouping<ISettingValueDefinition, SettingWatch>>)(object)groupedWatches))).SubscribeSafe<ISettingValueDefinition[]>(o);
			});
		}
		return Observable.Return(unconstrainedValues);
	}

	private static ISettingValueDefinition[] AvailableValues(ISettingValueDefinition[] unconstrainedValues, System.Collections.Generic.IEnumerable<IGrouping<ISettingValueDefinition, SettingWatch>> groupedWatches)
	{
		return Enumerable.ToArray<ISettingValueDefinition>(Enumerable.Concat<ISettingValueDefinition>((System.Collections.Generic.IEnumerable<ISettingValueDefinition>)unconstrainedValues, Enumerable.Select<IGrouping<ISettingValueDefinition, SettingWatch>, ISettingValueDefinition>(Enumerable.Where<IGrouping<ISettingValueDefinition, SettingWatch>>(groupedWatches, (Func<IGrouping<ISettingValueDefinition, SettingWatch>, bool>)((IGrouping<ISettingValueDefinition, SettingWatch> valueGroup) => Enumerable.All<SettingWatch>((System.Collections.Generic.IEnumerable<SettingWatch>)valueGroup, (Func<SettingWatch, bool>)((SettingWatch sw) => sw.IsInBounds)) && !Enumerable.Any<SettingWatch>((System.Collections.Generic.IEnumerable<SettingWatch>)valueGroup, (Func<SettingWatch, bool>)((SettingWatch sw) => sw.IsOutofBounds)))), (Func<IGrouping<ISettingValueDefinition, SettingWatch>, ISettingValueDefinition>)((IGrouping<ISettingValueDefinition, SettingWatch> valueGroup) => valueGroup.Key))));
	}

	private static IObservable<SettingValueChangedEventArgs?> WhenAnyDependantSettingChanges(System.Collections.Generic.IEnumerable<SettingWatch> watches)
	{
		return Observable.Return<SettingValueChangedEventArgs>(null).Concat<SettingValueChangedEventArgs>(watches.ToObservable().SelectMany<SettingWatch, SettingValueChangedEventArgs>((Func<SettingWatch, IObservable<SettingValueChangedEventArgs>>)((SettingWatch w) => w.ResolvedSetting.WhenValueChanges())));
	}

	private IActiveSetting ResolveSetting(SettingIdentifier settingToObserve)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return _settingTable.GetByScopedIdentifier(settingToObserve) ?? throw new ArgumentException("The identifier was not found.");
	}
}
