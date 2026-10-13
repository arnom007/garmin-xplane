using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DataRouterMatrix;
using LOLA.Extensions;
using LOLA.FileSystem;
using LauncherCoreLib.Models.DeployedEnv;
using RoarClient.Net;
using RoarDiscovery;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class LauncherDataRouterAssemblyTypes : IDataRouterAssemblyTypes
{
	[field: CompilerGenerated]
	public IDataRouterAssemblyProvider Routers
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IDataRouterAssemblyProvider Clients
	{
		[CompilerGenerated]
		get;
	}

	public LauncherDataRouterAssemblyTypes(IExecutionPathProvider execution, IFileSystem fs)
	{
		Routers = new EnumerableAssemblyProvider(GetAllRouters(execution.ExeExecutionPath, fs));
		Clients = new EnumerableAssemblyProvider(GetAllClients(execution.ExeExecutionPath, fs));
	}

	private static System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo> GetAllRouters(string executionPath, IFileSystem fileSystem)
	{
		string executionPath2 = executionPath;
		IFileSystem fileSystem2 = fileSystem;
		return Defer<DataRouterAssemblyInfo>(() => Enumerable.Select<RoarExePath, DataRouterAssemblyInfo>(RouterSearch.EnumerateRouterExes(executionPath2, (string dir) => Enumerable.Select<FileSystemEntry, string>(fileSystem2.DirectoriesIn(dir), (Func<FileSystemEntry, string>)((FileSystemEntry fse) => fse.FullPath)), (string path) => fileSystem2.PathExists(path)), (Func<RoarExePath, DataRouterAssemblyInfo>)((RoarExePath exe) => new DataRouterAssemblyInfo(exe, fileSystem2.GetAssemblyFileVersion(exe.ExePath)))));
	}

	private static System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo> GetAllClients(string executionPath, IFileSystem fileSystem)
	{
		string executionPath2 = executionPath;
		IFileSystem fileSystem2 = fileSystem;
		return Defer<DataRouterAssemblyInfo>(() => Enumerable.Select<ValueTuple<DllFilePath, Version>, DataRouterAssemblyInfo>(ClientAssemblySearcher.EnumerateClientRootDirectories(executionPath2, (string dir) => Enumerable.Select<FileSystemEntry, string>(fileSystem2.DirectoriesIn(dir), (Func<FileSystemEntry, string>)((FileSystemEntry fse) => fse.FullPath))).SearchByArchitecture().GatherDllsFromDirectories((SearchDir dirPath) => Enumerable.Select<FileSystemEntry, DllFilePath>(Enumerable.Where<FileSystemEntry>(fileSystem2.FilesIn(dirPath.FullPath), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => fse.Name.EndsWith(".dll", (StringComparison)4))), (Func<FileSystemEntry, DllFilePath>)((FileSystemEntry fse) => new DllFilePath(fse.FullPath))))
			.FilterByArchitecture()
			.FilterByFileNameConvention()
			.IncludeVersions((DllFilePath dllPath) => fileSystem2.GetAssemblyFileVersion(dllPath.FullPath)), (Func<ValueTuple<DllFilePath, Version>, DataRouterAssemblyInfo>)((ValueTuple<DllFilePath, Version> t) => new DataRouterAssemblyInfo(t.Item1.FullPath, t.Item2))));
	}

	private static System.Collections.Generic.IEnumerable<TElement> Defer<TElement>(Func<System.Collections.Generic.IEnumerable<TElement>> factory)
	{
		return Enumerable.SelectMany<Func<System.Collections.Generic.IEnumerable<TElement>>, TElement>(factory.Yield<Func<System.Collections.Generic.IEnumerable<TElement>>>(), (Func<Func<System.Collections.Generic.IEnumerable<TElement>>, System.Collections.Generic.IEnumerable<TElement>>)((Func<System.Collections.Generic.IEnumerable<TElement>> e) => e.Invoke()));
	}
}
