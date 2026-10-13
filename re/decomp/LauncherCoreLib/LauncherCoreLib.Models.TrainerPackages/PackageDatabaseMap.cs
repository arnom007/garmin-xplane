using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using LauncherCoreLib.Models.Winfs;

namespace LauncherCoreLib.Models.TrainerPackages;

[DefaultMember("Item")]
public sealed class PackageDatabaseMap
{
	public const string WINFS_ROOT_JSON_PROPERTY_NAME = "Winfs";

	public const string JSON_PROPERTY_NAME = "Databases";

	public const string JSON_PATH = "Winfs.Databases";

	private readonly Dictionary<string, string> _databaseMap;

	public System.Collections.Generic.IEnumerable<RelativePath> WinfsRelativePaths => Enumerable.Select<string, RelativePath>((System.Collections.Generic.IEnumerable<string>)_databaseMap.Values, (Func<string, RelativePath>)((string v) => new RelativePath(v)));

	public System.Collections.Generic.IEnumerable<KeyValuePair<string, RelativePath>> Entries => Enumerable.Select<KeyValuePair<string, string>, KeyValuePair<string, RelativePath>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)_databaseMap, (Func<KeyValuePair<string, string>, KeyValuePair<string, RelativePath>>)((KeyValuePair<string, string> kv) => new KeyValuePair<string, RelativePath>(kv.Key, new RelativePath(kv.Value))));

	public System.Collections.Generic.IEnumerable<KeyValuePair<SupportedWinfsDatabase, RelativePath>> SupportedEntries => Enumerable.Select<KeyValuePair<string, string>, KeyValuePair<SupportedWinfsDatabase, RelativePath>>(Enumerable.Where<KeyValuePair<string, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)_databaseMap, (Func<KeyValuePair<string, string>, bool>)((KeyValuePair<string, string> kv) => KeyIsValid(kv.Key))), (Func<KeyValuePair<string, string>, KeyValuePair<SupportedWinfsDatabase, RelativePath>>)((KeyValuePair<string, string> kv) => new KeyValuePair<SupportedWinfsDatabase, RelativePath>(ParseKey(kv.Key), new RelativePath(kv.Value))));

	public System.Collections.Generic.IEnumerable<string> DatabaseKeys => (System.Collections.Generic.IEnumerable<string>)_databaseMap.Keys;

	public System.Collections.Generic.IEnumerable<SupportedWinfsDatabase> SupportedDatabases => Enumerable.Where<SupportedWinfsDatabase>(Enumerable.Select<string, SupportedWinfsDatabase>(DatabaseKeys, (Func<string, SupportedWinfsDatabase>)ParseKey), (Func<SupportedWinfsDatabase, bool>)((SupportedWinfsDatabase db) => db != SupportedWinfsDatabase.Invalid));

	public System.Collections.Generic.IEnumerable<string> InvalidKeys => Enumerable.Where<string>(DatabaseKeys, (Func<string, bool>)((string key) => !KeyIsValid(key)));

	public RelativePath this[string databaseKey]
	{
		[return: MaybeNull]
		get
		{
			return GetWinfsPath(databaseKey);
		}
		set
		{
			SetWinfsPath(databaseKey, value);
		}
	}

	public RelativePath this[SupportedWinfsDatabase supportedWinfsDatabase]
	{
		[return: MaybeNull]
		get
		{
			return GetWinfsPath(supportedWinfsDatabase);
		}
		set
		{
			SetWinfsPath(ConvertEnumToKey(supportedWinfsDatabase), value);
		}
	}

	public PackageDatabaseMap()
	{
		_databaseMap = new Dictionary<string, string>();
	}

	public PackageDatabaseMap(Dictionary<string, string> existingMap)
	{
		_databaseMap = new Dictionary<string, string>((IDictionary<string, string>)(object)existingMap);
	}

	public RelativePath? GetWinfsPath(string databaseKey)
	{
		string path = default(string);
		_databaseMap.TryGetValue(databaseKey, ref path);
		return RelativePath.FromString(path);
	}

	public RelativePath? GetWinfsPath(SupportedWinfsDatabase supportedWinfsDatabase)
	{
		return GetWinfsPath(ConvertEnumToKey(supportedWinfsDatabase));
	}

	public void SetWinfsPath(string databaseKey, RelativePath winfsRelativePath)
	{
		if (winfsRelativePath == null)
		{
			_databaseMap.Remove(databaseKey);
		}
		else
		{
			_databaseMap[databaseKey] = ((object)winfsRelativePath).ToString();
		}
	}

	public void SetWinfsPath(SupportedWinfsDatabase supportedWinfsDatabase, RelativePath winfsRelativePath)
	{
		SetWinfsPath(ConvertEnumToKey(supportedWinfsDatabase), winfsRelativePath);
	}

	public Dictionary<string, string> ToJsonMap()
	{
		return new Dictionary<string, string>((IDictionary<string, string>)(object)_databaseMap);
	}

	private static bool KeyIsValid(string key)
	{
		return ParseKey(key) != SupportedWinfsDatabase.Invalid;
	}

	private static SupportedWinfsDatabase ParseKey(string key)
	{
		SupportedWinfsDatabase result = default(SupportedWinfsDatabase);
		if (!System.Enum.TryParse<SupportedWinfsDatabase>(key, ref result))
		{
			return SupportedWinfsDatabase.Invalid;
		}
		return result;
	}

	private static string ConvertEnumToKey(SupportedWinfsDatabase supportedWinfsDatabase)
	{
		return ((object)supportedWinfsDatabase).ToString();
	}
}
