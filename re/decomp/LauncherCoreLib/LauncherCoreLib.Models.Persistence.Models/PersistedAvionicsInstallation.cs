using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Persistence.Models;

public class PersistedAvionicsInstallation
{
	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public string FullModelId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public string FamilyId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(Required = Required.Always)]
	[JsonConverter(typeof(StringEnumConverter))]
	[field: CompilerGenerated]
	public Avionics_category Category
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public int InstallId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public static PersistedAvionicsInstallation FromKeyAndPackage(SimInstallKey installKey, TrainerPackageIdentifier fullPackageId)
	{
		return new PersistedAvionicsInstallation
		{
			Category = PermanentCommonDataTypes.ConvertActualToSerial(installKey.Slot.Category),
			InstallId = installKey.Slot.InstanceId,
			FamilyId = installKey.ReleaseId.FamilyId.IdentStringChecked,
			FullModelId = ((object)new FullModelIdentifier(fullPackageId, installKey.ModelId.IdentStringChecked)).ToString()
		};
	}

	public bool IsValid()
	{
		if (InstallId > 0 && FullModelIdentifier.IsValidFullModelIdentifier(FullModelId) && Category != 0)
		{
			return TrainerIdentifier.IsValidIdentifier(FamilyId);
		}
		return false;
	}

	public SimInstallKey ToSimInstallKey()
	{
		FullModelIdentifier fullModelIdentifier = new FullModelIdentifier(FullModelId);
		AvionicsType category = PermanentCommonDataTypes.ConvertSerialToActual(Category);
		return new SimInstallKey(new InstallationSlot(InstallId, category), fullModelIdentifier.TrainerPackageId.EmbeddedReleaseCompositeId, fullModelIdentifier.ModelId);
	}
}
