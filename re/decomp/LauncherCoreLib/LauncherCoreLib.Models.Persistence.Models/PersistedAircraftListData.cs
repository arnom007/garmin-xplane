using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Persistence.Models;

public class PersistedAircraftListData
{
	public const string DEFAULT_FILE_NAME = "list.json";

	[JsonProperty("AircraftIdentifier", Required = Required.Always)]
	[field: CompilerGenerated]
	public string AircraftIdentifierString
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonIgnore]
	public AircraftIdentifier AircraftIdentifier
	{
		get
		{
			return new AircraftIdentifier(AircraftIdentifierString);
		}
		set
		{
			AircraftIdentifierString = value.IdentStringChecked;
		}
	}

	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public string DisplayName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(Required = Required.Always)]
	[JsonConverter(typeof(StringEnumConverter))]
	[field: CompilerGenerated]
	public Airframe_type Airframe
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public bool IsHidden
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public bool CanBeDeleted
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public JObject ListModuleData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new JObject();


	public static PersistedAircraftListData FromRuntime(AircraftListData runtime)
	{
		return new PersistedAircraftListData
		{
			AircraftIdentifier = runtime.AircraftIdentifier,
			DisplayName = runtime.DisplayName,
			Airframe = PermanentCommonDataTypes.ConvertActualToSerial(runtime.AirframeCCT),
			CanBeDeleted = runtime.CanBeDeleted,
			IsHidden = runtime.IsHidden,
			ListModuleData = runtime.ListModuleData.CopyAllDataAsObject()
		};
	}

	public AircraftListData ToListData()
	{
		return new AircraftListData(AircraftIdentifier, DisplayName, string.Empty, PermanentCommonDataTypes.ConvertSerialToActual(Airframe), isHidden: IsHidden, canBeDeleted: CanBeDeleted, listModuleData: new ReadOnlyJsonDataSource(ListModuleData ?? new JObject()));
	}
}
