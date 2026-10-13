using System;
using LauncherCoreLib.Models.Simulated.Panel;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Winfs;

public interface ISourcedBatchTransformProvider
{
	IObservable<SourcedBatchTransform> SourcedTransforms(AvionicsModel trainerModel);

	IObservable<SourcedBatchTransform> SourcedTransforms(IManifestDataSource manifestDataSource, string fullpackageRootPath);

	IObservable<WinfsDatabaseLinkInstruction> DatabaseTransforms(IManifestDataSource manifestDataSource, string fullPackageRootPath);
}
