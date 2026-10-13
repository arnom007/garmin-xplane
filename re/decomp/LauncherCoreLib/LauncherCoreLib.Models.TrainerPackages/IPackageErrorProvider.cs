using System;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IPackageErrorProvider
{
	IObservable<TrainerPackageLoadException> GetAllCurrentErrors();
}
