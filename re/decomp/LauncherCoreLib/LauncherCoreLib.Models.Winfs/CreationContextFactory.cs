using System;
using LOLA.FileSystem;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.Winfs;

internal class CreationContextFactory : ICreationContextFactory
{
	private readonly IFileSystem _files;

	public CreationContextFactory(IFileSystem fileLister)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_files = fileLister ?? throw new ArgumentNullException("fileLister");
	}

	public IWinfsCreationContext CreateContext(ISessionInfo session, TrainerSessionData trainerSessionData)
	{
		return new TrainerWinfsCreationContext(trainerSessionData.PackageInfo.BaseInfo.PackageRootPath, trainerSessionData.WinfsMountPath, _files);
	}
}
