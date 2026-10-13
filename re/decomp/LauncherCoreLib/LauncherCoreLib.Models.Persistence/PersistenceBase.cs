using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using LOLA.FileSystem;
using LOLA.Logging;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Persistence;

public abstract class PersistenceBase<T> : IPersistence where T : class
{
	private const string FILE_EXTENSION = ".json";

	private readonly IPersistencePathProvider _pathProvider;

	private readonly IFileSystem _fileSystem;

	private readonly string _filename;

	private T? _persistedContainer;

	protected T? Container => _persistedContainer;

	protected object FileMutex
	{
		[CompilerGenerated]
		get;
	} = new object();


	private string PersistedFilePath => Path.Combine(_pathProvider.PersistenceDirectoryPath, _filename);

	protected PersistenceBase(IPersistencePathProvider pathProvider, IFileSystem fileSystem, string filename)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(filename))
		{
			throw new ArgumentException("'filename' cannot be null or empty.", "filename");
		}
		EnsureTemplateIsSerializeable();
		_pathProvider = pathProvider ?? throw new ArgumentNullException("pathProvider");
		_fileSystem = fileSystem ?? throw new ArgumentNullException("fileSystem");
		_filename = filename + ".json";
		Refresh();
	}

	public void Commit()
	{
		_fileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(PersistedFilePath));
		lock (FileMutex)
		{
			LogIfPersistedPathIsADirectory();
			SerializationUtil.SaveToJson(PersistedFilePath, _persistedContainer, _fileSystem);
		}
	}

	public void Refresh()
	{
		lock (FileMutex)
		{
			if (!_fileSystem.PathExists(PersistedFilePath))
			{
				_persistedContainer = CreateDefaultContainer();
				return;
			}
			LogIfPersistedPathIsADirectory();
			T val = SerializationUtil.LoadFromJson<T>(PersistedFilePath, _fileSystem);
			if (val == null)
			{
				val = CreateDefaultContainer();
			}
			_persistedContainer = val;
		}
	}

	protected abstract T CreateDefaultContainer();

	private static void EnsureTemplateIsSerializeable()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		System.Type typeFromHandle = typeof(T);
		if (System.Attribute.GetCustomAttribute((MemberInfo)(object)typeFromHandle, typeof(SerializableAttribute)) == null)
		{
			throw new InvalidOperationException(((MemberInfo)typeFromHandle).Name + " is not marked [Serializeable].");
		}
	}

	private void LogIfPersistedPathIsADirectory()
	{
		if (_fileSystem.StatIfExists(PersistedFilePath).Entry.EntryType == FileSystemEntryType.Directory)
		{
			Logger.Log(LogLevel.Error, PersistedFilePath + " is a directory.");
		}
	}
}
