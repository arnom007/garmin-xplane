using System.Runtime.CompilerServices;
using GTI.Net;

namespace LauncherCoreLib.Models.TrainerPackages;

public class GtiListingStatus : ITrainerInstallerListingInfo
{
	[field: CompilerGenerated]
	public GtiListingEntry ListingEntry
	{
		[CompilerGenerated]
		get;
	}

	public bool IsDiscontinued => ListingEntry.IsDiscontinued;

	public GtiListingStatus(GtiListingEntry entry)
	{
		ListingEntry = entry;
	}
}
