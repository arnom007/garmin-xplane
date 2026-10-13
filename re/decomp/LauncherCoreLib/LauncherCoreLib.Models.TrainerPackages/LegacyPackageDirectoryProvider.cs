using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LOLA.FileSystem;
using LOLA.Logging;
using LauncherCoreLib.Models.DeployedEnv;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class LegacyPackageDirectoryProvider : ITrainerPackageDirectoryProvider
{
	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fileSystem;

	public LegacyPackageDirectoryProvider(IDeploymentPathProvider deployment, IFileSystem fileSystem)
	{
		_deployment = deployment;
		_fileSystem = fileSystem;
	}

	public IObservable<PotentialPackageDirectory> SearchDirectories()
	{
		return Observable.Create<PotentialPackageDirectory>((Func<IObserver<PotentialPackageDirectory>, System.IDisposable>)([CompilerGenerated] (IObserver<PotentialPackageDirectory> o) =>
		{
			string text = _deployment.PackagesDirectoryPath();
			if (_fileSystem.PathExists(text))
			{
				Logger.Log(LogLevel.Info, "Loading legacy packages from {0}", text);
				return Enumerable.Select<FileSystemEntry, PotentialPackageDirectory>(Enumerable.Where<FileSystemEntry>(_fileSystem.EntriesIn(text), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => fse.EntryType == FileSystemEntryType.Directory)), (Func<FileSystemEntry, PotentialPackageDirectory>)((FileSystemEntry fse) => new PotentialPackageDirectory(TrainerPackageType.LegacyPermanentInstall, fse))).ToObservable().SubscribeSafe<PotentialPackageDirectory>(o);
			}
			Logger.Log(LogLevel.Debug, "[Warning] Cannot load trainer packages because an installation directory ('{0}') is missing.", text);
			o.OnCompleted();
			return Disposable.Empty;
		}));
	}
}
