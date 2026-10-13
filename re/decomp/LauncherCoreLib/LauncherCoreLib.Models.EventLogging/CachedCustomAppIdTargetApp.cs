using System;
using System.Runtime.CompilerServices;
using AppCenter.EventLogging;

namespace LauncherCoreLib.Models.EventLogging;

public class CachedCustomAppIdTargetApp : TrackedTargetAppBase
{
	[field: CompilerGenerated]
	public AppCenterAppId AppId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public Version AppVersion
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string AppBuild
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string AppNamespace
	{
		[CompilerGenerated]
		get;
	}

	public CachedCustomAppIdTargetApp(AppCenterAppId appId, Version appVersion, string appBuild, string appNamespace)
	{
		AppId = appId;
		AppVersion = appVersion;
		AppBuild = appBuild;
		AppNamespace = appNamespace;
	}
}
