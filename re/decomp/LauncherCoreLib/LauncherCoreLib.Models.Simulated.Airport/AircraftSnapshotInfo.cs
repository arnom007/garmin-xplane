using System;
using System.Runtime.CompilerServices;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftSnapshotInfo : RecordBase<AircraftSnapshotInfo>, IHaveAircraftId
{
	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public SnapshotIdentifier SnapshotIdentifier
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public DateTimeOffset TakenAt
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public AircraftListData? PrototypeAircraftListData
	{
		[CompilerGenerated]
		get;
	}

	public bool IsFromPrototypeAircraft => PrototypeAircraftListData != null;

	[field: CompilerGenerated]
	public AircraftSnapshotMetaData MetaData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public long StorageSize
	{
		[CompilerGenerated]
		get;
	}

	public AircraftIdentifier AircraftIdentifierInSnapShot
	{
		get
		{
			if (!IsFromPrototypeAircraft)
			{
				return AircraftIdentifier;
			}
			return PrototypeAircraftListData.AircraftIdentifier;
		}
	}

	public AircraftSnapshotInfo(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier, DateTimeOffset takenAt, AircraftListData? prototypeListData, AircraftSnapshotMetaData metadata, long storageSize)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		AircraftIdentifier = aircraftIdentifier.ThrowIfDefault();
		SnapshotIdentifier = snapshotIdentifier;
		TakenAt = takenAt;
		PrototypeAircraftListData = prototypeListData;
		MetaData = metadata;
		ThisClassesValidation(this);
		StorageSize = storageSize;
	}

	public override void ThrowIfConstraintsAreViolated(AircraftSnapshotInfo record)
	{
		ThisClassesValidation(record);
	}

	private static void ThisClassesValidation(AircraftSnapshotInfo copy)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		copy.AircraftIdentifier.ThrowIfDefault();
		if (copy.SnapshotIdentifier == null)
		{
			throw new ArgumentNullException("copy", "SnapshotIdentifier");
		}
		if (copy.MetaData == null)
		{
			throw new ArgumentNullException("copy", "MetaData");
		}
	}
}
