using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.DeployedEnv;

public class WindowsInstallationLogFileProvider : IInstallationLogProvider
{
	private readonly string _logFilePrefix;

	public IObservable<string> InstallationLogFilePaths => Observable.Defer<string>((Func<IObservable<string>>)([CompilerGenerated] () => Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)Directory.GetFiles(Path.GetTempPath(), _logFilePrefix + "*"), (Func<string, bool>)([CompilerGenerated] (string filePath) => Path.GetFileName(filePath).StartsWith(_logFilePrefix, (StringComparison)5))).ToObservable()));

	public WindowsInstallationLogFileProvider(string logFilePrefix)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_logFilePrefix = logFilePrefix ?? throw new ArgumentNullException("logFilePrefix");
	}
}
