using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class SimPackModel : IHaveAircraftId
{
	[field: CompilerGenerated]
	public SimInstallData Simulator
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public AvionicsModel PackageAndModel
	{
		[CompilerGenerated]
		get;
	}

	public TrainerModelKey ModelKey => PackageAndModel.Key;

	public InstallationSlot InstallSlot => Simulator.InstallationKey.Slot;

	public ITrainerBaseInfo PackageInfo => PackageAndModel.BaseInfo;

	public string SimulatorName => ModelInfo.DisplayName;

	public string InstallationName => FormatInstallationString();

	public AircraftIdentifier AircraftIdentifier => AircraftSimKey.AircraftIdentifier;

	public SimInstallKey InstallationKey => AircraftSimKey.InstallationKey;

	public SimAircraftKey AircraftSimKey => Simulator.AircraftSimKey;

	public ITrainerModelInfo ModelInfo => PackageAndModel.ModelInfo;

	public SimPackModel(SimInstallData sim, AvionicsModel packageAndModel)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Simulator = sim ?? throw new ArgumentNullException("sim");
		PackageAndModel = packageAndModel ?? throw new ArgumentNullException("packageAndModel");
	}

	private string FormatInstallationString()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (InstallSlot.InstanceId > PackageInfo.MaxInstances)
		{
			throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The sim description has changed. InstanceId({0}) is larger than the 'MaxInstances' ({1}) specified for {2}", (object)InstallSlot.InstanceId, (object)PackageInfo.MaxInstances, (object)SimulatorName));
		}
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} {1}", (object)SimulatorName, (object)InstallSlot.InstanceId);
	}
}
