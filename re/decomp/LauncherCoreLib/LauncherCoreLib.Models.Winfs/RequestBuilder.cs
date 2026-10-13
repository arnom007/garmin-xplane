using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using DotNet.Globbing;

namespace LauncherCoreLib.Models.Winfs;

internal static class RequestBuilder
{
	public static IObservable<WinfsTransformRequest> CreateCopyRequests(IObservable<WinfsCopyInstruction> copyInstructions, IWinfsCreationContext creationContext)
	{
		IObservable<WinfsCopyInstruction> copyInstructions2 = copyInstructions;
		IWinfsCreationContext creationContext2 = creationContext;
		return (from request in Observable.SelectMany(Observable.Select(AllFilepathsForAllInstructions(copyInstructions2, creationContext2), (string fp) => new
			{
				InstalledFilepath = fp,
				MatchedInstructions = GetMatchingInstructions(copyInstructions2, creationContext2, fp)
			}), combined => combined.MatchedInstructions.Select<WinfsCopyInstruction, WinfsTransformRequest>((Func<WinfsCopyInstruction, WinfsTransformRequest>)((WinfsCopyInstruction mi) => new WinfsTransformRequest(combined.InstalledFilepath, CreateWinFsPath(creationContext2, combined.InstalledFilepath, mi), WinfsTranformOperation.Copy))))
			group request by request.WinfsPath).SelectMany<IGroupedObservable<string, WinfsTransformRequest>, WinfsTransformRequest>((Func<IGroupedObservable<string, WinfsTransformRequest>, IObservable<WinfsTransformRequest>>)((IGroupedObservable<string, WinfsTransformRequest> winfsPath) => ((IObservable<WinfsTransformRequest>)winfsPath).LastAsync<WinfsTransformRequest>()));
	}

	public static IObservable<WinfsTransformRequest> CreateLinkRequests(IObservable<WinfsLinkInstruction> linkInstructions, IWinfsCreationContext creationContext)
	{
		IWinfsCreationContext creationContext2 = creationContext;
		return linkInstructions.Select<WinfsLinkInstruction, WinfsTransformRequest>((Func<WinfsLinkInstruction, WinfsTransformRequest>)((WinfsLinkInstruction li) => new WinfsTransformRequest(li.PackagePath.ToAbsolutePath(creationContext2.InstalledDirectoryPath), li.WinfsPath.ToAbsolutePath(creationContext2.WinfsDirectoryPath), WinfsTranformOperation.Link)));
	}

	private static IObservable<string> AllFilepathsForAllInstructions(IObservable<WinfsCopyInstruction> copyInstructions, IWinfsCreationContext creationContext)
	{
		IWinfsCreationContext creationContext2 = creationContext;
		return (from ci in copyInstructions
			group ci by ci.PackageBaseDirectory).SelectMany<IGroupedObservable<RelativePath, WinfsCopyInstruction>, string>((Func<IGroupedObservable<RelativePath, WinfsCopyInstruction>, IObservable<string>>)((IGroupedObservable<RelativePath, WinfsCopyInstruction> cig) => ((IObservable<WinfsCopyInstruction>)cig).FirstAsync<WinfsCopyInstruction>().SelectMany<WinfsCopyInstruction, string>((Func<WinfsCopyInstruction, IObservable<string>>)((WinfsCopyInstruction firstCi) => creationContext2.ListInstalledFilePaths(firstCi.PackageBaseDirectory)))));
	}

	private static string CreateWinFsPath(IWinfsCreationContext creationContext, string installedFullPath, WinfsCopyInstruction ci)
	{
		string text = FullInstallDirPath(creationContext, ci);
		string text2 = installedFullPath.Substring(text.Length);
		if (text2.StartsWith("\\", (StringComparison)4))
		{
			text2 = text2.Substring(1);
		}
		return Path.Combine(creationContext.WinfsDirectoryPath, (string)ci.WinFsBaseDirectory, text2);
	}

	private static IObservable<WinfsCopyInstruction> GetMatchingInstructions(IObservable<WinfsCopyInstruction> copyInstructions, IWinfsCreationContext creationContext, string installedFilePath)
	{
		IWinfsCreationContext creationContext2 = creationContext;
		string installedFilePath2 = installedFilePath;
		return copyInstructions.SelectMany<WinfsCopyInstruction, WinfsCopyInstruction>((Func<WinfsCopyInstruction, System.Collections.Generic.IEnumerable<WinfsCopyInstruction>>)((WinfsCopyInstruction ci) => ci.Flatten)).Where<WinfsCopyInstruction>((Func<WinfsCopyInstruction, bool>)((WinfsCopyInstruction ci) => Enumerable.Any<Glob>((System.Collections.Generic.IEnumerable<Glob>)ci.Globs, (Func<Glob, bool>)((Glob globPattern) => IsGlobMatch(creationContext2, installedFilePath2, ci, globPattern)))));
	}

	private static bool IsGlobMatch(IWinfsCreationContext creationContext, string installedFilePath, WinfsCopyInstruction ci, Glob globPattern)
	{
		string text = FullInstallDirPath(creationContext, ci) + "\\";
		if (installedFilePath.StartsWith(text, (StringComparison)4))
		{
			return Glob.Parse(((object)globPattern).ToString().Replace("/", "\\", (StringComparison)4)).IsMatch(MemoryExtensions.AsSpan(installedFilePath, text.Length));
		}
		return false;
	}

	private static string FullInstallDirPath(IWinfsCreationContext creationContext, WinfsCopyInstruction ci)
	{
		return ci.PackageBaseDirectory.AppendTo(creationContext.InstalledDirectoryPath);
	}
}
