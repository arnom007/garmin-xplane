using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using LauncherCoreLib.Models.Settings;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class TrainerPackageExtensions
{
	public static System.Collections.Generic.IEnumerable<TrainerModelKey> FullModelKeys(this ITrainerPackage package)
	{
		ITrainerPackage package2 = package;
		return Enumerable.Select<ITrainerModelInfo, TrainerModelKey>((System.Collections.Generic.IEnumerable<ITrainerModelInfo>)package2.Models, (Func<ITrainerModelInfo, TrainerModelKey>)((ITrainerModelInfo modelInfo) => new TrainerModelKey(package2.TrainerPackageId, modelInfo.ModelId.IdentStringChecked)));
	}

	public static ITrainerModelInfo? GetModel(this ITrainerPackage package, IHaveModelIdentity modelIdentityProvider)
	{
		return package.GetModel(modelIdentityProvider.ModelId);
	}

	public static ITrainerModelInfo? GetModel(this ITrainerPackage package, TrainerModelKey tmk)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (package.TrainerPackageId != tmk.TrainerPackageId)
		{
			throw new InvalidOperationException("Package identity must match.");
		}
		return package.GetModel(tmk);
	}

	public static bool WasPublishedLaterThan(this ITrainerPackage lhs, ITrainerPackage rhs)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return lhs.BaseInfo.PublishedAt > rhs.BaseInfo.PublishedAt;
	}

	public static bool IsNewerEmbeddedReleaseThan(this ITrainerPackage lhs, ITrainerPackage rhs)
	{
		return lhs.TrainerPackageId.IsNewerEmbeddedReleaseThan(rhs.TrainerPackageId);
	}

	public static bool IsSuccessorOf(this ITrainerPackage lhs, ITrainerPackage rhs)
	{
		return lhs.TrainerPackageId.IsSuccessorOf(rhs.TrainerPackageId);
	}

	public static bool IsBugFixUpdateOf(this ITrainerPackage lhs, ITrainerPackage rhs)
	{
		return lhs.TrainerPackageId.IsBugFixUpdateOf(rhs.TrainerPackageId);
	}

	public static bool MatchesKeyOf(this ITrainerPackage lhs, ITrainerPackage rhs)
	{
		return lhs.TrainerPackageId == rhs.TrainerPackageId;
	}

	public static int CompareIdTo(this ITrainerPackage lhs, ITrainerPackage rhs)
	{
		return TrainerPackageIdentifier.FamilyVersionSortComparer.Compare(lhs.TrainerPackageId, rhs.TrainerPackageId);
	}

	public static System.Collections.Generic.IEnumerable<AvionicsType> AvionicsCategories(this ITrainerPackage package)
	{
		return Enumerable.Distinct<AvionicsType>(Enumerable.Select<ITrainerModelInfo, AvionicsType>((System.Collections.Generic.IEnumerable<ITrainerModelInfo>)package.Models, (Func<ITrainerModelInfo, AvionicsType>)((ITrainerModelInfo m) => m.Category)));
	}

	public static bool IsACatalogPackage(this ITrainerPackage package)
	{
		return package.BaseInfo.PackageType == TrainerPackageType.Cataloged;
	}

	public static bool ContainsBreakingChangesSince(this ITrainerPackage package, Version binaryVersion)
	{
		return new PackageHistory(package.BaseInfo.Manifest).ContainsBreakingChangesBetween(package.TrainerPackageId.BinaryVersion, binaryVersion);
	}

	public static bool ContainsBreakingChangesSince(this ITrainerPackage package, TrainerPackageIdentifier crossReferencePackageInSameEmbeddedLine)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!package.TrainerPackageId.IsSameEmbeddedRelease(crossReferencePackageInSameEmbeddedLine))
		{
			throw new ArgumentException("Packages must be in the same embedded release line.");
		}
		return new PackageHistory(package.BaseInfo.Manifest).ContainsBreakingChangesBetween(package.TrainerPackageId.BinaryVersion, crossReferencePackageInSameEmbeddedLine.BinaryVersion);
	}

	public static bool IsOlderThan(this ITrainerPackage package, Version binaryVersion)
	{
		return package.TrainerPackageId.BinaryVersion < binaryVersion;
	}

	public static ITrainerUiAliasProvider GetUiAliases(this ITrainerPackage package)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ITrainerPackage package2 = package;
		Guid val = default(Guid);
		((Guid)(ref val))._002Ector("B892F539-2E77-4867-BAFE-F2D2BC30EB74");
		return package2.GetExtensionData<UiAliasesMap>(val, () => new UiAliasesMap(package2.BaseInfo.TrainerPackageId, package2.BaseInfo.Manifest));
	}

	public static ISettingDefinitionCollection GetSlotSettingDefinitions(this ITrainerPackage package)
	{
		return package.GetScopedSettingDefinitions()[ManifestSettingScope.Slot];
	}

	public static IReadOnlyDictionary<ManifestSettingScope, ISettingDefinitionCollection> GetScopedSettingDefinitions(this ITrainerPackage package)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ITrainerPackage package2 = package;
		Guid val = default(Guid);
		((Guid)(ref val))._002Ector("F321FE0A-9977-4AD5-9414-F5AF0BB701CF");
		return package2.GetExtensionData<IReadOnlyDictionary<ManifestSettingScope, ISettingDefinitionCollection>>(val, () => CreateScopedSupportedSettingsDefinitions(package2));
	}

	private static IReadOnlyDictionary<ManifestSettingScope, ISettingDefinitionCollection> CreateScopedSupportedSettingsDefinitions(ITrainerPackage package)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		ITrainerBaseInfo baseInfo = package.BaseInfo;
		Dictionary<string, Trainer_setting> val = baseInfo.Manifest.GetManifestData<Dictionary<string, Trainer_setting>>("SupportedSettings") ?? throw new FormatException("The 'SupportedSettings' root manifest property is required.");
		SettingIdentifier packageSupportedSettingsNamespace = SettingIdentifier.Concatenate(CreatePackageNamespace(baseInfo), SettingIdentifier.FromString("SupportedSettings"));
		Builder<ManifestSettingScope, ISettingDefinitionCollection> obj = ImmutableDictionary.CreateBuilder<ManifestSettingScope, ISettingDefinitionCollection>();
		obj.Add(ManifestSettingScope.Slot, (ISettingDefinitionCollection)new SettingDefinitionCollection(packageSupportedSettingsNamespace));
		obj.Add(ManifestSettingScope.Family, (ISettingDefinitionCollection)new SettingDefinitionCollection(packageSupportedSettingsNamespace));
		ImmutableDictionary<ManifestSettingScope, ISettingDefinitionCollection> obj2 = obj.ToImmutable();
		ImmutableDictionary<ManifestSettingScope, ISettingDefinitionCollection> items = ImmutableDictionary.ToImmutableDictionary<IGrouping<Trainer_settingScope, ValueTuple<KeyValuePair<string, Trainer_setting>, int>>, ManifestSettingScope, ISettingDefinitionCollection>(Enumerable.GroupBy<ValueTuple<KeyValuePair<string, Trainer_setting>, int>, Trainer_settingScope>(Enumerable.Select<KeyValuePair<string, Trainer_setting>, ValueTuple<KeyValuePair<string, Trainer_setting>, int>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, Trainer_setting>>)val, (Func<KeyValuePair<string, Trainer_setting>, int, ValueTuple<KeyValuePair<string, Trainer_setting>, int>>)((KeyValuePair<string, Trainer_setting> skv, int settingDocIndex) => new ValueTuple<KeyValuePair<string, Trainer_setting>, int>(skv, settingDocIndex))), (Func<ValueTuple<KeyValuePair<string, Trainer_setting>, int>, Trainer_settingScope>)((ValueTuple<KeyValuePair<string, Trainer_setting>, int> t) => t.Item1.Value.Scope)), (Func<IGrouping<Trainer_settingScope, ValueTuple<KeyValuePair<string, Trainer_setting>, int>>, ManifestSettingScope>)((IGrouping<Trainer_settingScope, ValueTuple<KeyValuePair<string, Trainer_setting>, int>> g) => ConvertToManifestSettingScope(g.Key)), (Func<IGrouping<Trainer_settingScope, ValueTuple<KeyValuePair<string, Trainer_setting>, int>>, ISettingDefinitionCollection>)delegate(IGrouping<Trainer_settingScope, ValueTuple<KeyValuePair<string, Trainer_setting>, int>> g)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			SettingDefinitionCollection settingDefinitionCollection = new SettingDefinitionCollection(packageSupportedSettingsNamespace);
			System.Collections.Generic.IEnumerator<ValueTuple<KeyValuePair<string, Trainer_setting>, int>> enumerator = ((System.Collections.Generic.IEnumerable<ValueTuple<KeyValuePair<string, Trainer_setting>, int>>)g).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					ValueTuple<KeyValuePair<string, Trainer_setting>, int> current = enumerator.Current;
					KeyValuePair<string, Trainer_setting> item = current.Item1;
					int item2 = current.Item2;
					if (item.Key != item.Value.Id)
					{
						throw new FormatException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Mismatched SupportedSetting Ids, {0} - {1}", (object)item.Key, (object)item.Value.Id));
					}
					Trainer_setting value = item.Value;
					SettingDefinition settingDefinition = new SettingDefinition(packageSupportedSettingsNamespace, SettingIdentifier.FromString(value.Id))
					{
						DisplayText = value.DisplayText,
						Description = value.Description,
						DocumentOrderIndex = item2,
						ControlType = TrainerPackageAdapters.ConvertSerializedEnumToActual(value.UIControl),
						DefaultValue = value.DefaultValue
					};
					settingDefinitionCollection.Add(settingDefinition);
					System.Collections.Generic.IEnumerator<ValueTuple<KeyValuePair<string, Trainer_setting_value>, int>> enumerator2 = Enumerable.Select<KeyValuePair<string, Trainer_setting_value>, ValueTuple<KeyValuePair<string, Trainer_setting_value>, int>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, Trainer_setting_value>>)value.SupportedValues, (Func<KeyValuePair<string, Trainer_setting_value>, int, ValueTuple<KeyValuePair<string, Trainer_setting_value>, int>>)((KeyValuePair<string, Trainer_setting_value> vkv, int valueDocIndex) => new ValueTuple<KeyValuePair<string, Trainer_setting_value>, int>(vkv, valueDocIndex))).GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator2).MoveNext())
						{
							ValueTuple<KeyValuePair<string, Trainer_setting_value>, int> current2 = enumerator2.Current;
							KeyValuePair<string, Trainer_setting_value> item3 = current2.Item1;
							int item4 = current2.Item2;
							if (item3.Key != item3.Value.Id)
							{
								throw new FormatException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Mismatched SupportedValue Ids for SupportedSetting {0}, {1} - {2}", (object)value.Id, (object)item.Key, (object)item.Value.Id));
							}
							Trainer_setting_value value2 = item3.Value;
							SettingValueDefinition settingValueDefinition = new SettingValueDefinition(settingDefinition.SettingId, SettingIdentifier.FromString(value2.Id))
							{
								DisplayText = value2.DisplayText,
								Description = value2.OptionDescription,
								DocumentOrderIndex = item4,
								SimulatorValue = value2.SwitchValue
							};
							settingDefinition.Add(settingValueDefinition);
							settingValueDefinition.Constraints = CreateConstraints(value2);
						}
					}
					finally
					{
						((System.IDisposable)enumerator2)?.Dispose();
					}
				}
				return settingDefinitionCollection;
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		});
		return (IReadOnlyDictionary<ManifestSettingScope, ISettingDefinitionCollection>)(object)obj2.SetItems((System.Collections.Generic.IEnumerable<KeyValuePair<ManifestSettingScope, ISettingDefinitionCollection>>)items);
	}

	private static ManifestSettingScope ConvertToManifestSettingScope(Trainer_settingScope key)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return key switch
		{
			Trainer_settingScope.Slot => ManifestSettingScope.Slot, 
			Trainer_settingScope.Family => ManifestSettingScope.Family, 
			_ => throw new NotSupportedException(((object)key).ToString()), 
		};
	}

	private static SettingIdentifier CreatePackageNamespace(ITrainerBaseInfo baseInfo)
	{
		return SettingIdentifier.Concatenate(SettingIdentifier.FromString("Packages"), SettingIdentifier.FromString(((object)baseInfo.TrainerPackageId).ToString()));
	}

	private static ImmutableArray<ValueConstraintDefinition> CreateConstraints(Trainer_setting_value value)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ImmutableArray.ToImmutableArray<ValueConstraintDefinition>(ValueConstraintDefinition.FromJsonConstraintArray((System.Collections.Generic.IEnumerable<Setting_constraint>)value.Constraints));
	}
}
