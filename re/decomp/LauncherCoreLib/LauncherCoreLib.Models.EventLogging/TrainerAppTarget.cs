using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.EventLogging;

public sealed class TrainerAppTarget : TrackedTargetAppBase
{
	[field: CompilerGenerated]
	public ITrainerPackage TargetTrainer
	{
		[CompilerGenerated]
		get;
	}

	public static TrainerAppTarget Instance(ITrainerPackage targetTrainer)
	{
		return new TrainerAppTarget(targetTrainer);
	}

	private TrainerAppTarget(ITrainerPackage targetTrainer)
	{
		TargetTrainer = targetTrainer;
	}
}
