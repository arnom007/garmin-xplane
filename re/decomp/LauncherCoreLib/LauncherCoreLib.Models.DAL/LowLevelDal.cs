using System.Runtime.CompilerServices;
using LOLA.Compression;
using LOLA.FileSystem;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Persistence;

namespace LauncherCoreLib.Models.DAL;

public class LowLevelDal : ILowLevelDal
{
	[field: CompilerGenerated]
	public IDeploymentPathProvider Deployment
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IFileSystem FileSystem
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IZipCompression Compression
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IPersistArbitraryObjects ObjectPersistence
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}
}
