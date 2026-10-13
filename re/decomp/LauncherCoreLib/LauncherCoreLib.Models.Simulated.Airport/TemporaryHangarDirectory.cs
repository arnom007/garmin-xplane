using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.StringHelpers.Paths;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.Simulated.Airport;

public sealed class TemporaryHangarDirectory : DirectoryBasedHangar
{
	private readonly ILowLevelDal _lowLevel;

	private readonly IFileSystem _fs;

	private bool _disposed;

	public TemporaryHangarDirectory(ILowLevelDal lowLevelDal, string rootDirectory)
		: base(lowLevelDal, rootDirectory)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		_lowLevel = lowLevelDal;
		_fs = lowLevelDal.FileSystem;
		if (!Enumerable.Contains<string>(PathHelpers.ParentsOf(rootDirectory), _lowLevel.Deployment.AppTempDirectoryPath, (IEqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash))
		{
			throw new ArgumentException("Must be placed under the temporary directory.");
		}
	}

	public static TemporaryHangarDirectory CreateNewUniqueHangar(ILowLevelDal lowLevelDal)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		string text = lowLevelDal.Deployment.PerProcessTempDirectory();
		Guid val = Guid.NewGuid();
		string text2 = Path.Combine(text, "TempHangar", ((object)(Guid)(ref val)).ToString());
		if (lowLevelDal.FileSystem.PathExists(text2))
		{
			Logger.Log(LogLevel.Error, "Duplicate temporary hangar mount path encountered (" + PathHelpers.QuotePathIfNeeded(text2) + "). This shouldn't be possible.");
			throw new InvalidOperationException("Guid directory already existed!");
		}
		return new TemporaryHangarDirectory(lowLevelDal, text2);
	}

	protected override void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			_disposed = true;
			try
			{
				_fs.DeleteIfExistsAndParentsThatAreEmptyStopIfLocked(base.RootDirectory, _lowLevel.Deployment.AppTempDirectoryPath);
			}
			catch (System.Exception ex) when (ex.IsAccessDenied())
			{
			}
		}
		base.Dispose(disposing);
	}
}
