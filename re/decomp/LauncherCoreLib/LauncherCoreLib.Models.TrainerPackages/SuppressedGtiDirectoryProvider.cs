using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class SuppressedGtiDirectoryProvider : ITrainerPackageDirectoryProvider, IGtiDirectorySuppressor
{
	private sealed class PathSuppression
	{
		[field: CompilerGenerated]
		public string Path
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = string.Empty;

	}

	private readonly object _suppressionLock = new object();

	private readonly GtiPackageDirectoryProvider _gtiProvider;

	private readonly List<PathSuppression> _suppressedPaths = new List<PathSuppression>();

	public SuppressedGtiDirectoryProvider(GtiPackageDirectoryProvider gtiPackageDirectoryProvider)
	{
		_gtiProvider = gtiPackageDirectoryProvider;
	}

	public IObservable<PotentialPackageDirectory> SearchDirectories()
	{
		return _gtiProvider.SearchDirectories().Where<PotentialPackageDirectory>((Func<PotentialPackageDirectory, bool>)([CompilerGenerated] (PotentialPackageDirectory directory) =>
		{
			if (_suppressedPaths.Count > 0)
			{
				lock (_suppressionLock)
				{
					return !Enumerable.Any<PathSuppression>((System.Collections.Generic.IEnumerable<PathSuppression>)_suppressedPaths, (Func<PathSuppression, bool>)((PathSuppression supressed) => supressed.Path == directory.PackageDirectory.FullPath));
				}
			}
			return true;
		}));
	}

	public System.IDisposable SuppressGtiPackageAtDirectory(string directoryPath)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		lock (_suppressionLock)
		{
			PathSuppression suppressed = new PathSuppression
			{
				Path = directoryPath
			};
			_suppressedPaths.Add(suppressed);
			return Disposable.Create((Action)delegate
			{
				lock (_suppressionLock)
				{
					_suppressedPaths.Remove(suppressed);
				}
			});
		}
	}
}
