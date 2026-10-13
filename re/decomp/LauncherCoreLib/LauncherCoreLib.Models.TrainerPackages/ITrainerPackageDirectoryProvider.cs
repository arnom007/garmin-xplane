using System;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ITrainerPackageDirectoryProvider
{
	IObservable<PotentialPackageDirectory> SearchDirectories();
}
