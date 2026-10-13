using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AutoUpdate;

public sealed class ImmutableUrlFeedProvider : IUpdateUrlProvider
{
	[field: CompilerGenerated]
	public Uri? UpdateFeedUrl
	{
		[CompilerGenerated]
		get;
	}

	public ImmutableUrlFeedProvider(Uri? updateFeedurl)
	{
		UpdateFeedUrl = updateFeedurl;
	}

	public ImmutableUrlFeedProvider(string? updateFeedurl)
		: this((updateFeedurl == null) ? ((Uri)null) : new Uri(updateFeedurl))
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)

}
