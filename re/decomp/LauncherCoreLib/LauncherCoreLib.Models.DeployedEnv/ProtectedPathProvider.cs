using System;
using System.Collections.Generic;
using System.Linq;
using LOLA.StringHelpers.Paths;

namespace LauncherCoreLib.Models.DeployedEnv;

public class ProtectedPathProvider : IProtectedPathProvider
{
	private readonly System.Collections.Generic.IReadOnlyList<string> _protected_paths;

	public ProtectedPathProvider()
	{
		List<string> val = new List<string>();
		val.Add("C:\\Program Files");
		val.Add(Environment.GetFolderPath((SpecialFolder)38));
		val.Add(Environment.GetFolderPath((SpecialFolder)42));
		val.Add(Environment.GetFolderPath((SpecialFolder)35));
		val.Add(Environment.GetFolderPath((SpecialFolder)37));
		val.Add(Environment.GetFolderPath((SpecialFolder)41));
		val.Add(Environment.GetFolderPath((SpecialFolder)36));
		_protected_paths = (System.Collections.Generic.IReadOnlyList<string>)val;
	}

	public bool IsPathProtected(string path)
	{
		string path2 = path;
		path2 = PathHelpers.NormalizeToBackSlashes(path2);
		return Enumerable.Any<string>((System.Collections.Generic.IEnumerable<string>)_protected_paths, (Func<string, bool>)((string pp) => path2.StartsWith(pp, (StringComparison)5)));
	}
}
