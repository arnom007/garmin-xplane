using System;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IStartingPackageProvider
{
	IObservable<ITrainerPackage> StartingPackages { get; }
}
