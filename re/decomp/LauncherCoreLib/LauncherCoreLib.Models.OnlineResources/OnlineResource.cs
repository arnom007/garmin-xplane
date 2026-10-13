using System;
using System.Runtime.CompilerServices;
using LOLA.Logging;
using LOLA.Processes;

namespace LauncherCoreLib.Models.OnlineResources;

public class OnlineResource
{
	[field: CompilerGenerated]
	public string DisplayText
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string Description
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public Uri URL
	{
		[CompilerGenerated]
		get;
	}

	public OnlineResource(string display, string description, Uri url)
	{
		DisplayText = display;
		Description = description;
		URL = url;
	}

	public void Open()
	{
		CreateProcessThroughExplorer(URL).Dispose();
	}

	private static ProcessSession CreateProcessThroughExplorer(Uri uri)
	{
		Logger.Log(LogLevel.Info, "Opening {0}", uri);
		return ProcessSession.ForkFromCurrentDirectory("explorer.exe", ((object)uri).ToString());
	}
}
