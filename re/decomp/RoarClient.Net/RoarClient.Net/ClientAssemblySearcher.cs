using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RoarClient.Net;

public static class ClientAssemblySearcher
{
	public static bool MatchesRoarClientRootDirectoryName(string directoryPath)
	{
		return RoarClientRootDir.MatchesDirectoryNamePattern(directoryPath);
	}

	public static IEnumerable<RoarClientRootDir> EnumerateClientRootDirectories(string mainAssemblyDir, Func<string, IEnumerable<string>>? enumerateImmediateDirectories = null)
	{
		Func<string, IEnumerable<string>> enumerateImmediateDirectories2 = enumerateImmediateDirectories;
		string mainAssemblyDir2 = mainAssemblyDir;
		if (enumerateImmediateDirectories2 == null)
		{
			enumerateImmediateDirectories2 = Directory.EnumerateDirectories;
		}
		return Defer(() => from d in enumerateImmediateDirectories2(mainAssemblyDir2)
			where RoarClientRootDir.MatchesDirectoryNamePattern(d)
			select new RoarClientRootDir(d)).SelectMany((Func<IEnumerable<RoarClientRootDir>> f) => f());
		static IEnumerable<Func<IEnumerable<RoarClientRootDir>>> Defer(Func<IEnumerable<RoarClientRootDir>> factory)
		{
			yield return factory;
		}
	}

	public static IEnumerable<SearchDir> SearchByArchitecture(this IEnumerable<RoarClientRootDir> rootDirs, SupportedArchitecture? architecture = null)
	{
		return rootDirs.Select((RoarClientRootDir rd) => rd.GetSearchDirForArchitecture(DefaultToCurrentProcessArch(architecture)));
	}

	public static IEnumerable<SearchDir> LegacySearchDirs(string mainAssemblyDir, SupportedArchitecture? architecture = null)
	{
		SupportedArchitecture supportedArchitecture = DefaultToCurrentProcessArch(architecture);
		yield return new SearchDir(Path.Combine(mainAssemblyDir, GetDirectoryName(supportedArchitecture)));
	}

	public static IEnumerable<DllFilePath> GatherDllsFromDirectories(this IEnumerable<SearchDir> searchDirs, Func<SearchDir, IEnumerable<DllFilePath>>? enumerateDlls = null)
	{
		Func<SearchDir, IEnumerable<DllFilePath>> enumerateDlls2 = enumerateDlls;
		if (enumerateDlls2 == null)
		{
			enumerateDlls2 = (SearchDir searchDir) => (!Directory.Exists(searchDir.FullPath)) ? Enumerable.Empty<DllFilePath>() : (from fp in Directory.EnumerateFiles(searchDir.FullPath, "*.dll")
				select new DllFilePath(fp));
		}
		return searchDirs.SelectMany((SearchDir searchDir) => enumerateDlls2(searchDir));
	}

	public static IEnumerable<DllFilePath> FilterByFileNameConvention(this IEnumerable<DllFilePath> dllPaths)
	{
		return dllPaths.Where((DllFilePath dp) => dp.MatchesRoarClientFileNamePattern());
	}

	public static IEnumerable<DllFilePath> FilterByArchitecture(this IEnumerable<DllFilePath> dllPaths, SupportedArchitecture? mustMatchArchitecture = null, Func<string, FileStream>? openReadonly = null)
	{
		Func<string, FileStream> openReadonly2 = openReadonly;
		if (openReadonly2 == null)
		{
			openReadonly2 = (string dllPath) => new FileStream(dllPath, FileMode.Open, FileAccess.Read);
		}
		SupportedArchitecture architecture2 = DefaultToCurrentProcessArch(mustMatchArchitecture);
		DllChecker.MachineType expectedMachineType = ConvertToMachineType(architecture2);
		return dllPaths.Where(delegate(DllFilePath dllPath)
		{
			DllChecker.MachineType dllMachineType;
			using (FileStream dllContents = openReadonly2(dllPath.FullPath))
			{
				dllMachineType = DllChecker.GetDllMachineType(dllContents);
			}
			return dllMachineType == expectedMachineType;
		});
		static DllChecker.MachineType ConvertToMachineType(SupportedArchitecture architecture)
		{
			if (architecture != 0)
			{
				throw new NotSupportedException($"{architecture} is not a supported architecture.");
			}
			return DllChecker.MachineType.IMAGE_FILE_MACHINE_I386;
		}
	}

	public static IEnumerable<(DllFilePath Path, Version NativeVersion)> IncludeVersions(this IEnumerable<DllFilePath> dllPaths, Func<DllFilePath, Version>? versionParser = null)
	{
		Func<DllFilePath, Version> versionParser2 = versionParser;
		if (versionParser2 == null)
		{
			versionParser2 = GetNativeVersion;
		}
		return dllPaths.Select((DllFilePath dllPath) => (dllPath: dllPath, versionParser2(dllPath) ?? throw new InvalidOperationException("Version Parser cannot return null for a Roar Client library.")));
	}

	public static SupportedArchitecture DefaultToCurrentProcessArch(SupportedArchitecture? architecture)
	{
		SupportedArchitecture? supportedArchitecture = architecture;
		if (!supportedArchitecture.HasValue)
		{
			if (IntPtr.Size != 4)
			{
				throw new NotSupportedException("This process architecture is not supported.");
			}
			return SupportedArchitecture.X86;
		}
		return supportedArchitecture.GetValueOrDefault();
	}

	public static string GetDirectoryName(SupportedArchitecture supportedArchitecture)
	{
		if (supportedArchitecture == SupportedArchitecture.X86)
		{
			return "x86";
		}
		throw new ArgumentException($"This architecture({supportedArchitecture}) is not currently known.");
	}

	private static Version GetNativeVersion(DllFilePath dllPath)
	{
		return Version.Parse(FileVersionInfo.GetVersionInfo(dllPath.FullPath).FileVersion ?? string.Empty);
	}
}
