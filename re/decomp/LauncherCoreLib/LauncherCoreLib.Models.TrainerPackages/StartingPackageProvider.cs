using System;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class StartingPackageProvider : IStartingPackageProvider
{
	[field: CompilerGenerated]
	public IObservable<ITrainerPackage> StartingPackages
	{
		[CompilerGenerated]
		get;
	}

	public StartingPackageProvider(ITrainerPackageDirectoryProvider directories, ITrainerPackageParser packageParser, IPackageScanner scanner)
	{
		StartingPackages = CreateStartingPackageStream(directories, packageParser, scanner);
	}

	private static IObservable<ITrainerPackage> CreateStartingPackageStream(ITrainerPackageDirectoryProvider directories, ITrainerPackageParser packageParser, IPackageScanner scanner)
	{
		ITrainerPackageDirectoryProvider directories2 = directories;
		ITrainerPackageParser packageParser2 = packageParser;
		IPackageScanner scanner2 = scanner;
		return Observable.Defer<ITrainerPackage>((Func<IObservable<ITrainerPackage>>)(() => directories2.SearchDirectories().Where<PotentialPackageDirectory>((Func<PotentialPackageDirectory, bool>)PotentialPackageDirectory.IsValid).SelectMany<PotentialPackageDirectory, ITrainerPackage>((Func<PotentialPackageDirectory, System.Threading.Tasks.Task<ITrainerPackage>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass4_0._003C_003CCreateStartingPackageStream_003Eb__1_003Ed))] async (PotentialPackageDirectory packageDir) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_ = 1;
			try
			{
				ITrainerPackage package = await packageParser2.ParsePackage(packageDir).DetachContext();
				if (await scanner2.CanUsePackage(package).DetachContext())
				{
					return package;
				}
			}
			catch (TrainerPackageLoadException)
			{
			}
			return (ITrainerPackage)null;
		}))
			.WhereNotNull<ITrainerPackage>()));
	}
}
