using System;
using System.IO;
using System.Runtime.CompilerServices;
using LOLA.FileSystem;
using LOLA.StringHelpers.Paths;

namespace LauncherCoreLib.Utility;

public static class LolaFsExtensions
{
	public delegate string TempPathFactory(string targetPath);

	private const int DOT_NET_HRESULT_DIR_ALREADY_EXISTS = -2146232800;

	private const int DOT_NET_HRESULT_FILE_ALREADY_EXISTS = -2147024713;

	[field: CompilerGenerated]
	public static TempPathFactory DefaultTempFactory
	{
		[CompilerGenerated]
		get;
	} = (string t) => Path.Combine(Path.GetDirectoryName(t) ?? throw new ArgumentException("Cannot be used for roots of drives."), ".tmp_" + Path.GetRandomFileName());


	public static void MoveOrCopyAndDeleteToDifferentDrive(this IFileSystem fs, TempPathFactory tempFactory, string sourcePath, string destinationPath)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		IFileSystem fs2 = fs;
		string destinationPath2 = destinationPath;
		if (PathHelpers.PathsAreOnTheSameDrive(sourcePath, destinationPath2))
		{
			fs2.MoveAndCreatePath(sourcePath, destinationPath2);
			return;
		}
		ThrowIfDestinationExists();
		string directoryPath = Path.GetDirectoryName(destinationPath2) ?? throw new ArgumentException("destination path (" + destinationPath2 + ") has no parent directory.");
		string text = RunTempFactoryAndThrowIfInvalid(tempFactory, destinationPath2);
		fs2.CreateDirectory(directoryPath);
		FileSystemEntryType entryType = fs2.StatIfExists(sourcePath).Entry.EntryType;
		try
		{
			switch (entryType)
			{
			case FileSystemEntryType.Directory:
				fs2.CopyDirectory(sourcePath, text);
				break;
			case FileSystemEntryType.File:
				fs2.CopyFile(sourcePath, text, overwrite: false);
				break;
			default:
				throw new NotSupportedException(((object)entryType).ToString());
			}
			ThrowIfDestinationExists();
			fs2.MoveAndCreatePath(text, destinationPath2);
		}
		catch
		{
			if (fs2.PathExists(text))
			{
				fs2.DeleteAnythingNotLocked(text);
			}
			throw;
		}
		fs2.DeleteAnythingNotLocked(sourcePath);
		[CompilerGenerated]
		void ThrowIfDestinationExists()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			switch (fs2.StatIfExists(destinationPath2).Entry.EntryType)
			{
			case FileSystemEntryType.Directory:
				throw new IOException("The destination path already exists.", -2146232800);
			case FileSystemEntryType.File:
				throw new IOException("Cannot create a file when that file already exists.", -2147024713);
			default:
				throw new NotSupportedException("Unknown file system type.");
			case FileSystemEntryType.None:
				break;
			}
		}
	}

	public static void MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(this IFileSystem fs, TempPathFactory tempPathFactory, string sourcePath, string destinationPath)
	{
		if (PathHelpers.PathsAreOnTheSameDrive(sourcePath, destinationPath))
		{
			fs.MoveAndOverwrite(sourcePath, destinationPath);
			return;
		}
		string text = RunTempFactoryAndThrowIfInvalid(tempPathFactory, destinationPath);
		try
		{
			fs.MoveOrCopyAndDeleteToDifferentDrive(tempPathFactory, sourcePath, text);
			fs.MoveAndOverwrite(text, destinationPath);
		}
		catch
		{
			if (!fs.PathExists(sourcePath))
			{
				fs.MoveOrCopyAndDeleteToDifferentDrive(tempPathFactory, text, sourcePath);
			}
			throw;
		}
		finally
		{
			fs.DeleteAnythingNotLocked(text);
		}
		fs.DeleteAnythingNotLocked(sourcePath);
	}

	private static string RunTempFactoryAndThrowIfInvalid(TempPathFactory tempFactory, string destinationPath)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		string text = tempFactory(destinationPath);
		if (string.Equals(text, destinationPath, (StringComparison)4))
		{
			throw new ArgumentException("Temporary file path cannot be the same as destinationPath.", "tempFactory");
		}
		return text;
	}
}
