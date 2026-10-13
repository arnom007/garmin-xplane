using System.Collections.Generic;

namespace LauncherCoreLib.Models.OnlineResources;

public interface IOnlineResourceProvider
{
	System.Collections.Generic.IEnumerable<OnlineResource> AvailableResources { get; }
}
