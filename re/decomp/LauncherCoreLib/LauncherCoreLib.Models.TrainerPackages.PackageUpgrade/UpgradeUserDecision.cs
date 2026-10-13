using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

[JsonConverter(typeof(StringEnumConverter))]
public enum UpgradeUserDecision
{
	Undecided,
	SkippedUpgrade,
	ApprovedUpgrade
}
