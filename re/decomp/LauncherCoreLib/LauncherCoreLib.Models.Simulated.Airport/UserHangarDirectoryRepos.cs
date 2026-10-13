using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class UserHangarDirectoryRepos : DirectoryBasedHangar
{
	public UserHangarDirectoryRepos(ILowLevelDal lowLevelDal)
		: base(lowLevelDal, lowLevelDal.Deployment.HangarDirectoryPath())
	{
	}
}
