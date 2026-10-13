using System;

namespace LauncherCoreLib.Models.Winfs;

internal interface IWinfsCreationContext
{
	string InstalledDirectoryPath { get; }

	string WinfsDirectoryPath { get; }

	IObservable<string> ListInstalledFilePaths(RelativePath installRelative);
}
