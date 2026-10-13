using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.StringHelpers.Paths;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.DeployedEnv;

public class PerProcessTempDirectoryHandling
{
	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fs;

	private readonly IOsProcessOperations _processes;

	public PerProcessTempDirectoryHandling(IDeploymentPathProvider deployment, IFileSystem fs, IOsProcessOperations processes)
	{
		_deployment = deployment;
		_fs = fs;
		_processes = processes;
	}

	public void PrepareOnStartup()
	{
		string text = _deployment.PerProcessTempDirectory();
		if (_fs.PathExists(text))
		{
			string message = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Reusing the per process temporary directory: {0} This should have been cleaned up on exit.", (object)PathHelpers.QuotePathIfNeeded(text));
			Logger.Log(LogLevel.Warning, message);
			_fs.DeleteIfExists(text);
		}
	}

	public void CleanupStaleDirectories()
	{
		//IL_00fb: Expected O, but got Unknown
		string text = LauncherPathHelpers.NonRootPathParentDirectory(_deployment.PerProcessTempDirectory());
		if (!_fs.PathExists(text))
		{
			return;
		}
		List<FileSystemEntry> val;
		try
		{
			val = Enumerable.ToList<FileSystemEntry>(_fs.DirectoriesIn(text));
		}
		catch (DirectoryNotFoundException)
		{
			val = new List<FileSystemEntry>();
		}
		IProcessSession thisProcess = _processes.ThisProcess;
		System.Collections.Generic.IList<ProcessSnapshot> runningProcesses = _processes.GetProcessSnapshotsByName(thisProcess.Name);
		val.RemoveAll((Predicate<FileSystemEntry>)delegate(FileSystemEntry fse)
		{
			int processId = int.Parse(fse.Name, (IFormatProvider)(object)CultureInfo.InvariantCulture);
			return Enumerable.Any<ProcessSnapshot>((System.Collections.Generic.IEnumerable<ProcessSnapshot>)runningProcesses, (Func<ProcessSnapshot, bool>)((ProcessSnapshot p) => p.ProcessId == processId));
		});
		System.Collections.Generic.IEnumerator<string> enumerator = Enumerable.Select<FileSystemEntry, string>((System.Collections.Generic.IEnumerable<FileSystemEntry>)val, (Func<FileSystemEntry, string>)((FileSystemEntry d) => d.FullPath)).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				string current = enumerator.Current;
				string message = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Removing stale temporary directory: {0} This should have been cleaned up on exit.", (object)PathHelpers.QuotePathIfNeeded(current));
				Logger.Log(LogLevel.Warning, message);
				try
				{
					_fs.DeleteIfExistsAndParentsThatAreEmptyStopIfLocked(current, _deployment.AppTempDirectoryPath);
				}
				catch (IOException val3)
				{
					Logger.LogFormattedException((System.Exception)val3, "Cleaning up previous per process temp directory.");
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public void CleanupOnExit()
	{
		string text = _deployment.PerProcessTempDirectory();
		if (_fs.PathExists(text))
		{
			Logger.Log(LogLevel.Info, "Deleting the per process temp directory.");
			_fs.DeleteIfExistsAndParentsThatAreEmptyStopIfLocked(text, _deployment.AppTempDirectoryPath);
		}
	}
}
