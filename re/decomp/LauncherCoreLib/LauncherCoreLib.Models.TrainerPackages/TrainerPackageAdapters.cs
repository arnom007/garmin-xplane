using System;
using System.Collections.Generic;
using LauncherCoreLib.Models.Settings;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class TrainerPackageAdapters
{
	private const string WINFS_ROOT_PROPERTY = "Winfs";

	public static Winfs_root GetWinfsTransforms(this IManifestDataSource manifest)
	{
		return manifest.GetManifestData<Winfs_root>("Winfs");
	}

	public static FullModelIdentifier ToSettingIdentifier(this TrainerModelKey tmk)
	{
		return new FullModelIdentifier(tmk.TrainerPackageId, tmk.ModelId.IdentStringChecked);
	}

	public static SettingUIControlType ConvertSerializedEnumToActual(Trainer_settingUIControl serialEnum)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return serialEnum switch
		{
			Trainer_settingUIControl.NONE => SettingUIControlType.None, 
			Trainer_settingUIControl.PICKLIST => SettingUIControlType.Picklist, 
			_ => throw new ArgumentOutOfRangeException("serialEnum"), 
		};
	}

	public static PackageDatabaseMap? GetDatabaseMap(this IManifestDataSource manifest)
	{
		Dictionary<string, string> manifestData = manifest.GetManifestData<Dictionary<string, string>>("Winfs.Databases");
		if (manifestData == null)
		{
			return null;
		}
		return new PackageDatabaseMap(manifestData);
	}
}
