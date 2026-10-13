using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Domain.PurposeBuiltTraining;
using DynamicData;
using LOLA.Extensions;
using LOLA.Reactive;
using Prism.Mvvm;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public sealed class PbtSelectionListViewModel : BindableBase, IDisposable
{
	private readonly Action<TrainingItemKey?> _onSelected;

	private readonly CompositeDisposable _cleanup = new CompositeDisposable();

	private TrainingItemKey? _selectedItemKey;

	public ReadOnlyObservableCollection<TrainingItemListInfo> Items { get; }

	public TrainingItemKey? SelectedItemKey
	{
		get
		{
			return _selectedItemKey;
		}
		set
		{
			SetProperty(ref _selectedItemKey, value, CreateChangeHandler(value), "SelectedItemKey");
		}
	}

	public PbtSelectionListViewModel(IObservable<IChangeSet<TrainingItemListInfo>> items, IObservable<TrainingItemKey?> selectedKeysOverTime, Action<TrainingItemKey?> onSelected)
	{
		_onSelected = onSelected;
		items.ObserveOn(ViewModelSchedulers.UIThread).Bind(out ReadOnlyObservableCollection<TrainingItemListInfo> readOnlyObservableCollection).Do(delegate
		{
			RaisePropertyChanged("SelectedItemKey");
		})
			.Subscribe()
			.DisposeWith(_cleanup);
		Items = readOnlyObservableCollection;
		selectedKeysOverTime.Do(delegate(TrainingItemKey? s)
		{
			SelectedItemKey = s;
		}).Subscribe().DisposeWith(_cleanup);
	}

	public static IObservable<IChangeSet<TrainingItemListInfo>> ConvertToChangeSet(IObservable<TrainingMenu> menus)
	{
		IObservable<TrainingMenu> menus2 = menus;
		return Observable.Defer(() => Observable.Using(() => new SourceCache<TrainingItemListInfo, TrainingItemKey>((TrainingItemListInfo i) => i.Key), (SourceCache<TrainingItemListInfo, TrainingItemKey> source) => source.Connect().RemoveKey().Sort(Compari.OrderBy(Compari.Start<TrainingItemListInfo>(), (TrainingItemListInfo li) => li.Title.RawValue).ToComparer())
			.Merge(menus2.Do(delegate(TrainingMenu menu)
			{
				source.Edit(delegate(ISourceUpdater<TrainingItemListInfo, TrainingItemKey> api)
				{
					api.RemoveKeys(api.Items.Select(source.KeySelector).Except(menu.ListItems.Select(source.KeySelector)));
					api.AddOrUpdate(menu.ListItems);
				});
			}).SelectDefault<TrainingMenu, IChangeSet<TrainingItemListInfo>>(null).IgnoreElements())));
	}

	public void Dispose()
	{
		_cleanup.Dispose();
	}

	private Action CreateChangeHandler(TrainingItemKey? value)
	{
		return delegate
		{
			_onSelected(value);
		};
	}
}
