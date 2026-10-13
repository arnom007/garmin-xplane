using System;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ICachedGtiDataProvider
{
	void NotifyOfCatalogUpdate(Version catalogVersion);
}
