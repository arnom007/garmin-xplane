using System.Collections.Generic;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface IPackageInstallRequest
{
	System.Collections.Generic.IReadOnlyCollection<ITrainerPackage> PackagesToDownload { get; }

	System.Collections.Generic.IEnumerable<KeyValuePair<object, object>> Attachments { get; }

	object? GetAttachment(object attachmentKey);
}
