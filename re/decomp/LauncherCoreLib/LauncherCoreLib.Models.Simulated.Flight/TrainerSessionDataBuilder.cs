using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Panel;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class TrainerSessionDataBuilder
{
	[field: CompilerGenerated]
	public SimInstallData? InstallData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public TrainerIdentifier EndpointName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = TrainerIdentifier.Invalid;


	[field: CompilerGenerated]
	public AvionicsModel? PackageInfo
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IImmutableDictionary<string, string> Settings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = (IImmutableDictionary<string, string>)(object)ImmutableDictionary<string, string>.Empty;


	[field: CompilerGenerated]
	public string WinfsMountPath
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	public TrainerSessionDataBuilder()
	{
	}

	public TrainerSessionDataBuilder(TrainerSessionData copyThis)
	{
		InstallData = copyThis.InstallData;
		EndpointName = copyThis.EndpointName;
		PackageInfo = copyThis.PackageInfo;
		Settings = copyThis.Settings;
		WinfsMountPath = copyThis.WinfsMountPath;
	}

	public TrainerSessionData ToImmutable()
	{
		return new TrainerSessionData(this);
	}
}
