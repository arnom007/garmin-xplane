using System;
using GTI.Net;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IGtiListingProvider
{
	IObservable<GtiListingEntry> GtiListEntries();
}
