using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Persistence.Models;

[Obsolete("The aircraft info is saved in a different format now. See the migrations in the hangar class.")]
internal sealed class PersistedAircraft
{
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
	public string AircraftName
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

	[JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
	[DefaultValue("")]
	[field: CompilerGenerated]
	public string PrototypeIdentifier
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public bool CanBeDeleted
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public List<PersistedAvionicsInstallation> Installations
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public JObject ExtraMetadata
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new JObject();


	[field: CompilerGenerated]
	public JObject AircraftModuleData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new JObject();


	[JsonIgnore]
	public JObject AircraftModuleDataSafe => AircraftModuleData ?? new JObject();

	public PersistedAircraft()
	{
		Installations = new List<PersistedAvionicsInstallation>();
	}

	public AircraftListData ToListData()
	{
		return new AircraftListData(AircraftIdentifier, AircraftName, string.Empty, PermanentCommonDataTypes.ConvertSerialToActual(Airframe), isHidden: IsHidden, canBeDeleted: CanBeDeleted, listModuleData: new ReadOnlyJsonDataSource(ExtraMetadata ?? new JObject()));
	}

	public AircraftHeavyData ToHeavyData()
	{
		return new AircraftHeavyData(AircraftIdentifier, new ReadOnlyJsonDataSource(AircraftModuleDataSafe ?? new JObject()));
	}

	public void UpdatePersisted(AircraftListData updatedListData)
	{
		AircraftIdentifier = updatedListData.AircraftIdentifier;
		AircraftName = updatedListData.DisplayName;
		Airframe = PermanentCommonDataTypes.ConvertActualToSerial(updatedListData.AirframeCCT);
		CanBeDeleted = updatedListData.CanBeDeleted;
		IsHidden = updatedListData.IsHidden;
		ExtraMetadata = updatedListData.ListModuleData.CopyAllDataAsObject();
	}

	public void UpdatePersisted(AircraftHeavyData heavyData)
	{
		AircraftModuleData = heavyData.AircraftModuleData.CopyAllDataAsObject();
	}

	public PersistedAircraft FixTooManyActiveSimsInSlot()
	{
		Installations = Enumerable.ToList<PersistedAvionicsInstallation>(Enumerable.Select<IGrouping<InstallationSlot, PersistedAvionicsInstallation>, PersistedAvionicsInstallation>(Enumerable.GroupBy<PersistedAvionicsInstallation, InstallationSlot>((System.Collections.Generic.IEnumerable<PersistedAvionicsInstallation>)Installations, (Func<PersistedAvionicsInstallation, InstallationSlot>)((PersistedAvionicsInstallation s) => s.ToSimInstallKey().Slot)), (Func<IGrouping<InstallationSlot, PersistedAvionicsInstallation>, PersistedAvionicsInstallation>)((IGrouping<InstallationSlot, PersistedAvionicsInstallation> groupedBySlot) => Enumerable.Last<PersistedAvionicsInstallation>((System.Collections.Generic.IEnumerable<PersistedAvionicsInstallation>)groupedBySlot))));
		return this;
	}

	public PersistedAircraftListData ToSeparatedListDataFile()
	{
		return PersistedAircraftListData.FromRuntime(ToListData());
	}

	public PersistedAircraftHeavyData ToSeparatedHeavyDataFile()
	{
		return PersistedAircraftHeavyData.FromRuntime(ToHeavyData());
	}

	public PersistedSimPanelInfo ToSeparatedSimPanelFile()
	{
		return new PersistedSimPanelInfo
		{
			ActiveSimulatorKeys = (System.Collections.Generic.IEnumerable<SimInstallKey>)Enumerable.ToList<SimInstallKey>(Enumerable.Select<PersistedAvionicsInstallation, SimInstallKey>((System.Collections.Generic.IEnumerable<PersistedAvionicsInstallation>)Installations, (Func<PersistedAvionicsInstallation, SimInstallKey>)((PersistedAvionicsInstallation i) => i.ToSimInstallKey())))
		};
	}
}
