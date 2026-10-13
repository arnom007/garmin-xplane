using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public interface IAvionicsRepo
{
	IObservable<AvionicsModel> AllModels();

	System.Threading.Tasks.Task<AvionicsModel> GetByModelKey(TrainerModelKey trainerModelKey);

	System.Threading.Tasks.Task<System.Collections.Generic.IList<AvionicsModel>> RemoveAllForPackage(TrainerPackageIdentifier trainerPackageId);
}
