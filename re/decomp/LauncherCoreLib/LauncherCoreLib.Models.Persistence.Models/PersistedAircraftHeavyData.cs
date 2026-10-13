using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Models.Persistence.Models;

public class PersistedAircraftHeavyData
{
	public const string DEFAULT_FILE_NAME = "heavy.json";

	[field: CompilerGenerated]
	public JObject AircraftModuleData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new JObject();


	public static PersistedAircraftHeavyData FromRuntime(AircraftHeavyData runtime)
	{
		return new PersistedAircraftHeavyData
		{
			AircraftModuleData = runtime.AircraftModuleData.CopyAllDataAsObject()
		};
	}

	public AircraftHeavyData ToRuntime(AircraftIdentifier aircraftIdentifier)
	{
		return new AircraftHeavyData(aircraftIdentifier, new ReadOnlyJsonDataSource(AircraftModuleData ?? new JObject()));
	}
}
