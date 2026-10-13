using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LauncherCoreLib.Utility;

public static class LauncherPathHelpers
{
	public static string NonRootPathParentDirectory(string path)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return Path.GetDirectoryName(path) ?? throw new ArgumentException("Drive root cannot be used.");
	}

	public static System.Collections.Generic.IEnumerable<string> GenerateNumberedFileNamesWithSuffix(string targetFilePath, Func<int, string> suffixGenerator)
	{
		Func<int, string> suffixGenerator2 = suffixGenerator;
		string fileName = Path.GetFileNameWithoutExtension(targetFilePath) ?? string.Empty;
		if (fileName.Length == 0)
		{
			fileName = Path.GetFileName(targetFilePath) ?? string.Empty;
		}
		string extension = Path.GetExtension(targetFilePath) ?? string.Empty;
		if (extension == fileName)
		{
			extension = string.Empty;
		}
		string directory = NonRootPathParentDirectory(targetFilePath);
		directory = directory.TrimEnd(new char[2] { '/', '\\' });
		char slash = LastDirectoryDelimitter(string.op_Implicit(targetFilePath));
		return Enumerable.Select<int, string>(Enumerable.Range(0, 2147483647), (Func<int, string>)delegate(int i)
		{
			string text = suffixGenerator2.Invoke(i);
			return $"{directory}{slash}{fileName}{text}{extension}";
		});
	}

	public static System.Collections.Generic.IEnumerable<string> GenerateNumberedFileNamesWithSuffix(string targetFilePath, string suffixTemplate)
	{
		string suffixTemplate2 = suffixTemplate;
		return GenerateNumberedFileNamesWithSuffix(targetFilePath, (int i) => (i != 0) ? $"{suffixTemplate2}_{i + 1}" : suffixTemplate2);
	}

	public unsafe static char LastDirectoryDelimitter(System.ReadOnlySpan<char> path)
	{
		int num = MemoryExtensions.LastIndexOfAny<char>(path, '/', '\\');
		if (num != -1)
		{
			return *(char*)path[num];
		}
		return '\\';
	}
}
