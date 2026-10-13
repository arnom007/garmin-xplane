using System;
using System.Runtime.CompilerServices;

namespace DataRouterMatrix;

public class DataRouterAssemblyInfo
{
	[field: CompilerGenerated]
	public string AssemblyFullPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public Version NativeVersion
	{
		[CompilerGenerated]
		get;
	}

	public DataRouterAssemblyInfo(string assemblyPath, Version nativeVersion)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		AssemblyFullPath = assemblyPath ?? throw new ArgumentNullException("assemblyPath");
		NativeVersion = nativeVersion ?? throw new ArgumentNullException("nativeVersion");
	}

	public virtual string ToString()
	{
		return ((object)NativeVersion).ToString();
	}
}
