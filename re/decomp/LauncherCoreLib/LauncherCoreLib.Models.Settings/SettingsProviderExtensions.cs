using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LOLA.Extensions;
using LOLA.Logging;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Settings;

public static class SettingsProviderExtensions
{
	public static IObservable<bool> WhenHasSettingsChanges(this ISettingsProvider settingsProvider)
	{
		ISettingsProvider settingsProvider2 = settingsProvider;
		return settingsProvider2.Settings.ToObservable().SelectMany<IActiveSetting, SettingHasOptionsChanged>((Func<IActiveSetting, IObservable<SettingHasOptionsChanged>>)((IActiveSetting s) => s.WhenSettingOptionsChange())).Select<SettingHasOptionsChanged, bool>((Func<SettingHasOptionsChanged, bool>)((SettingHasOptionsChanged soc) => soc.HasOptions || Enumerable.Any<IActiveSetting>(settingsProvider2.Settings, (Func<IActiveSetting, bool>)((IActiveSetting s) => s.SettingHasOptions()))));
	}

	public static IObservable<IObservable<IActiveSetting>> FilteredSettingStreams(this ISettingsProvider provider, ISettingsFilter filter)
	{
		ISettingsFilter filter2 = filter;
		ISettingsProvider provider2 = provider;
		return Observable.Return(filter2.FilterAppliesToProvider(provider2)).Concat<bool>(Observable.FromEventPattern(delegate(EventHandler h)
		{
			filter2.FilterChanged += h;
		}, delegate(EventHandler h)
		{
			filter2.FilterChanged -= h;
		}).Select<EventPattern<object>, bool>((Func<EventPattern<object>, bool>)((EventPattern<object> _) => filter2.FilterAppliesToProvider(provider2)))).Select<bool, System.Collections.Generic.IEnumerable<IActiveSetting>>((Func<bool, System.Collections.Generic.IEnumerable<IActiveSetting>>)delegate(bool filterApplies)
		{
			if (!filterApplies)
			{
				return provider2.Settings;
			}
			System.Collections.Generic.IEnumerable<IActiveSetting> settings2 = provider2.Settings;
			ISettingsFilter settingsFilter = filter2;
			return Enumerable.Where<IActiveSetting>(settings2, (Func<IActiveSetting, bool>)settingsFilter.IsSettingValid);
		})
			.Select<System.Collections.Generic.IEnumerable<IActiveSetting>, IObservable<IActiveSetting>>((Func<System.Collections.Generic.IEnumerable<IActiveSetting>, IObservable<IActiveSetting>>)((System.Collections.Generic.IEnumerable<IActiveSetting> settings) => settings.ToObservable()));
	}

	public static Dictionary<string, string> ToPersistenceDictionary(this ISettingsProvider settingsProvider)
	{
		return Enumerable.Select<IActiveSetting, KeyValuePair<string, string>>(settingsProvider.Settings, (Func<IActiveSetting, KeyValuePair<string, string>>)((IActiveSetting simSetting) => new KeyValuePair<string, string>(((object)simSetting.LocalId).ToString(), simSetting.CurrentValue))).ToDictionary();
	}

	public static Dictionary<SettingIdentifier, string> ToPersistenceDictionaryV2(this ISettingsProvider settingsProvider)
	{
		return Enumerable.ToDictionary<IActiveSetting, SettingIdentifier, string>(Enumerable.Where<IActiveSetting>(settingsProvider.Settings, (Func<IActiveSetting, bool>)((IActiveSetting setting) => ((System.Collections.Generic.IReadOnlyCollection<ISettingValueDefinition>)setting.CachedAvailableOptions).Count > 0)), (Func<IActiveSetting, SettingIdentifier>)((IActiveSetting setting) => setting.LocalId), (Func<IActiveSetting, string>)((IActiveSetting setting) => setting.CurrentValue));
	}

	public static System.Collections.Generic.IList<KeyValuePair<string, string>> LoadPersistedOptionsAndGetInvalids(this ISettingsProvider settingsProvider, System.Collections.Generic.IEnumerable<KeyValuePair<string, string>> persistedOptions)
	{
		return (System.Collections.Generic.IList<KeyValuePair<string, string>>)Enumerable.ToList<KeyValuePair<string, string>>(Enumerable.Select<KeyValuePair<SettingIdentifier, string>, KeyValuePair<string, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)settingsProvider.LoadPersistedOptionsAndGetInvalids(Enumerable.Select<KeyValuePair<string, string>, KeyValuePair<SettingIdentifier, string>>(persistedOptions, (Func<KeyValuePair<string, string>, KeyValuePair<SettingIdentifier, string>>)((KeyValuePair<string, string> kv) => new KeyValuePair<SettingIdentifier, string>(SettingIdentifier.FromString(kv.Key), kv.Value)))), (Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<string, string>>)((KeyValuePair<SettingIdentifier, string> inv) => new KeyValuePair<string, string>(((object)inv.Key).ToString(), inv.Value))));
	}

	public static System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>> LoadPersistedOptionsAndGetInvalids(this ISettingsProvider settingsProvider, System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>> persistedOptions)
	{
		ISettingsProvider settingsProvider2 = settingsProvider;
		return (System.Collections.Generic.IList<KeyValuePair<SettingIdentifier, string>>)Enumerable.ToList<KeyValuePair<SettingIdentifier, string>>(Enumerable.Where<KeyValuePair<SettingIdentifier, string>>(persistedOptions, (Func<KeyValuePair<SettingIdentifier, string>, bool>)delegate(KeyValuePair<SettingIdentifier, string> persistedOption)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			IActiveSetting setting = settingsProvider2.GetSetting(persistedOption.Key);
			return setting == null || !setting.LoadPersistedOption(persistedOption);
		}));
	}

	public static KeyValuePair<SettingIdentifier, string> ToUpdatedPackageReference(this KeyValuePair<SettingIdentifier, string> persistedSetting, System.Collections.Generic.IReadOnlyList<PackageReferenceUpgradeInfo> approvedUpgrades)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (SettingIdentifier.IsValidFullId(persistedSetting.Value))
		{
			string text = ((object)new SettingIdentifier(persistedSetting.Value).RelativeTo("Packages")?.Root).ToString();
			if (text != null && TrainerPackageIdentifier.CanParse(text))
			{
				TrainerPackageIdentifier tpi = TrainerPackageIdentifier.Parse(text);
				PackageReferenceUpgradeInfo packageReferenceUpgradeInfo = Enumerable.FirstOrDefault<PackageReferenceUpgradeInfo>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)approvedUpgrades, (Func<PackageReferenceUpgradeInfo, bool>)((PackageReferenceUpgradeInfo up) => up.ToUpgradeVersions().CanUpgradePackage(tpi)));
				if (packageReferenceUpgradeInfo != null)
				{
					return new KeyValuePair<SettingIdentifier, string>(persistedSetting.Key, persistedSetting.Value.Replace(text, ((object)packageReferenceUpgradeInfo.UpgradeId).ToString(), (StringComparison)4));
				}
				string text2 = string.Join(",\n\t", Enumerable.Select<PackageReferenceUpgradeInfo, string>((System.Collections.Generic.IEnumerable<PackageReferenceUpgradeInfo>)approvedUpgrades, (Func<PackageReferenceUpgradeInfo, string>)((PackageReferenceUpgradeInfo au) => ((object)au).ToString())));
				Logger.Log(LogLevel.Warning, "Tried to update package reference in a setting with no matching upgrade. Setting: " + persistedSetting.Value + "\nUpdates:\n\t" + text2);
			}
			else
			{
				Logger.Log(LogLevel.Warning, "Tried to update package reference in a setting that does not have a package reference. " + persistedSetting.Value);
			}
		}
		return persistedSetting;
	}

	public static KeyValuePair<SettingIdentifier, string> ToUpdatedPackageReference(this KeyValuePair<SettingIdentifier, string> persistedSetting, TrainerPackageIdentifier newPackageId)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (SettingIdentifier.IsValidFullId(persistedSetting.Value))
		{
			string text = ((object)new SettingIdentifier(persistedSetting.Value).RelativeTo("Packages")?.Root).ToString();
			if (text != null && TrainerPackageIdentifier.CanParse(text))
			{
				return new KeyValuePair<SettingIdentifier, string>(persistedSetting.Key, persistedSetting.Value.Replace(text, (string?)newPackageId, (StringComparison)4));
			}
			Logger.Log(LogLevel.Warning, "Tried to update package reference in a setting that does not have a package reference. " + persistedSetting.Value);
		}
		return persistedSetting;
	}
}
