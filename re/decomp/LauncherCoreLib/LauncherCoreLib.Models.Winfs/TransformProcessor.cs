using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Winfs;

internal class TransformProcessor : IWinfsTransformProcessor
{
	public static readonly TransformProcessor Instance = new TransformProcessor();

	private readonly IFileSystem _fileSystem;

	public TransformProcessor(IFileSystem fileSystem)
	{
		_fileSystem = fileSystem;
	}

	private TransformProcessor()
		: this(LolaFileSystem.Instance)
	{
	}

	public static WinfsTransformReport ProcessLinkRequest(WinfsTransformRequest linkRequest, IFileSystem fileSystem)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		if (linkRequest == null)
		{
			throw new ArgumentNullException("linkRequest");
		}
		ThrowIfWrongOperation(linkRequest, WinfsTranformOperation.Link);
		AssemblyLoadContext.Default.Resolving += ComAssemblyShim;
		try
		{
			string path = fileSystem.CreateShortcut(linkRequest.WinfsPath, linkRequest.InstalledPath);
			if (fileSystem.PathExists(path))
			{
				return linkRequest.Succeeded();
			}
			return linkRequest.Failed((System.Exception)new FileNotFoundException("Shortcut creation succeeded but the shortcut does not exist!"));
		}
		catch (System.Exception error)
		{
			return linkRequest.Failed(error);
		}
		finally
		{
			AssemblyLoadContext.Default.Resolving -= ComAssemblyShim;
		}
		[CompilerGenerated]
		static Assembly? ComAssemblyShim(AssemblyLoadContext c, AssemblyName n)
		{
			string name = n.Name;
			if (name == "Interop.IWshRuntimeLibrary")
			{
				return c.LoadFromAssemblyPath(Path.GetFullPath(name + ".dll"));
			}
			return null;
		}
	}

	public static WinfsTransformReport ProcessIndividualRequest(WinfsTransformRequest req, IFileSystem fileSystem)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		return req.Operation switch
		{
			WinfsTranformOperation.None => req.Skipped(), 
			WinfsTranformOperation.Copy => ProcessCopyRequest(req, fileSystem), 
			WinfsTranformOperation.Link => ProcessLinkRequest(req, fileSystem), 
			_ => throw new NotSupportedException(((object)req.Operation).ToString() + "is not supported."), 
		};
	}

	public static WinfsTransformReport ProcessCopyRequest(WinfsTransformRequest copyRequest, IFileSystem fileSystem)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (copyRequest == null)
		{
			throw new ArgumentNullException("copyRequest");
		}
		ThrowIfWrongOperation(copyRequest, WinfsTranformOperation.Copy);
		try
		{
			fileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(copyRequest.WinfsPath));
			fileSystem.CopyFile(copyRequest.InstalledPath, copyRequest.WinfsPath, overwrite: true);
			return copyRequest.Succeeded();
		}
		catch (System.Exception error)
		{
			return copyRequest.Failed(error);
		}
	}

	public System.Threading.Tasks.Task<WinfsTransformReport> ProcessRequest(WinfsTransformRequest request)
	{
		return System.Threading.Tasks.Task.FromResult<WinfsTransformReport>(ProcessIndividualRequest(request, _fileSystem));
	}

	private static void ThrowIfWrongOperation(WinfsTransformRequest copyRequest, WinfsTranformOperation operation)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (copyRequest.Operation != operation)
		{
			throw new InvalidOperationException("Invalid operation: " + ((object)copyRequest.Operation).ToString());
		}
	}
}
