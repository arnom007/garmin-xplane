using LOLA.Compression;
using LOLA.FileSystem;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Persistence;

namespace LauncherCoreLib.Models.DAL;

public interface ILowLevelDal
{
	IDeploymentPathProvider Deployment { get; }

	IFileSystem FileSystem { get; }

	IZipCompression Compression { get; }

	IPersistArbitraryObjects ObjectPersistence { get; }
}
