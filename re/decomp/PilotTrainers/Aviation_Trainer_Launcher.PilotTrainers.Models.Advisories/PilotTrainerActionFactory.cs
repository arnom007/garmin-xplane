using System.Collections.Generic;
using Aviation_Trainer_Launcher.LauncherAdvisories;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.Advisories;

public class PilotTrainerActionFactory : IBasicAdvisoryActionFactory
{
	public IList<IAdvisoryAction> CreateActions(BasicAdvisoryInstance instance)
	{
		return new List<IAdvisoryAction>();
	}
}
