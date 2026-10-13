using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Models.Persistence.Models;

public class PersistedSnapshotMetadata
{
	public const string DEFAULT_FILE_NAME = "metadata.json";

	[field: CompilerGenerated]
	public string Description
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public JObject ModuleData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new JObject();


	public static PersistedSnapshotMetadata FromRuntime(AircraftSnapshotMetaData metadata)
	{
		return new PersistedSnapshotMetadata
		{
			Description = metadata.Description,
			ModuleData = metadata.ModuleData.CopyAllDataAsObject()
		};
	}

	public AircraftSnapshotMetaData ToMetadata()
	{
		return new AircraftSnapshotMetaData(Description, new ReadOnlyJsonDataSource(ModuleData ?? new JObject()));
	}
}
