using System;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IGtiDirectorySuppressor
{
	System.IDisposable SuppressGtiPackageAtDirectory(string directoryPath);
}
