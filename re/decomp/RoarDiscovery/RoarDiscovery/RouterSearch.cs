using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoarDiscovery;

public static class RouterSearch
{
	private static readonly Regex ROAR_ROUTER_PACKAGE_OUTPUT_FORMAT = new Regex("^roar\\.((\\d+\\.){3}\\d+)$");

	public static bool MatchesRoarRouterPackageDirectoryName(string directoryPath)
	{
		return ROAR_ROUTER_PACKAGE_OUTPUT_FORMAT.IsMatch(Path.GetFileName(directoryPath));
	}

	public static IEnumerable<RoarExePath> EnumerateRouterExes(string deploymentPath, Func<string, IEnumerable<string>>? enumerateImmediateDirectories = null, Func<string, bool>? fileExists = null)
	{
		Func<string, IEnumerable<string>> enumerateImmediateDirectories2 = enumerateImmediateDirectories;
		Func<string, bool> fileExists2 = fileExists;
		string deploymentPath2 = deploymentPath;
		return Defer(() => EnumerateExes()).SelectMany((Func<IEnumerable<RoarExePath>> f) => f());
		static IEnumerable<Func<IEnumerable<RoarExePath>>> Defer(Func<IEnumerable<RoarExePath>> factory)
		{
			yield return factory;
		}
		IEnumerable<RoarExePath> EnumerateExes()
		{
			if (enumerateImmediateDirectories2 == null)
			{
				enumerateImmediateDirectories2 = Directory.EnumerateDirectories;
			}
			if (fileExists2 == null)
			{
				fileExists2 = File.Exists;
			}
			return from dp in enumerateImmediateDirectories2(deploymentPath2).Where(MatchesRoarRouterPackageDirectoryName)
				select new RoarExePath(Path.Combine(dp, "roar.exe")) into exePath
				where fileExists2(exePath.ExePath)
				select exePath;
		}
	}

	public static Version GetNativeVersion(this RoarExePath roarExePath)
	{
		return Version.Parse(FileVersionInfo.GetVersionInfo(roarExePath.ThrowIfDefault("roarExePath")).FileVersion);
	}
}
