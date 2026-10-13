using System;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Utility;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftSnapshotMetaData : RecordBase<AircraftSnapshotMetaData>
{
	[field: CompilerGenerated]
	public string Description
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReadOnlyJsonDataSource ModuleData
	{
		[CompilerGenerated]
		get;
	}

	public AircraftSnapshotMetaData(string? description, IReadOnlyJsonDataSource? moduleData)
	{
		Description = description ?? string.Empty;
		ModuleData = moduleData ?? new ReadOnlyJsonDataSource();
	}

	public override void ThrowIfConstraintsAreViolated(AircraftSnapshotMetaData record)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (record.Description == null)
		{
			throw new ArgumentNullException("record", "Description");
		}
		if (record.ModuleData == null)
		{
			throw new ArgumentNullException("record", "ModuleData");
		}
	}
}
