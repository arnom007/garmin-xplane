using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LOLA.FileSystem;

namespace LauncherCoreLib.Models.Winfs;

internal sealed class TrainerWinfsCreationContext : IWinfsCreationContext
{
	private readonly IFileSystem _fileLister;

	[field: CompilerGenerated]
	public string InstalledDirectoryPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string WinfsDirectoryPath
	{
		[CompilerGenerated]
		get;
	}

	public TrainerWinfsCreationContext(string installed, string winfs, IFileSystem fileLister)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (installed == null)
		{
			throw new ArgumentNullException("installed");
		}
		if (winfs == null)
		{
			throw new ArgumentNullException("winfs");
		}
		if (installed == winfs)
		{
			throw new InvalidOperationException("Installation and Winfs path cannot be the same.");
		}
		_fileLister = fileLister;
		InstalledDirectoryPath = installed;
		WinfsDirectoryPath = winfs;
	}

	public IObservable<string> ListInstalledFilePaths(RelativePath installRelative)
	{
		return Enumerable.Select<FileSystemEntry, string>(_fileLister.RecursiveFilesIn(installRelative.ToAbsolutePath(InstalledDirectoryPath)), (Func<FileSystemEntry, string>)((FileSystemEntry fse) => fse.FullPath)).ToObservable();
	}
}
