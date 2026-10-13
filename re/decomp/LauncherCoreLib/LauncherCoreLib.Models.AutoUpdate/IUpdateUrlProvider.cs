using System;

namespace LauncherCoreLib.Models.AutoUpdate;

public interface IUpdateUrlProvider
{
	Uri? UpdateFeedUrl { get; }
}
