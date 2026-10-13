using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

public class UpgradeDecisionData<T>
{
	public UpgradeUserDecision Selection
	{
		[CompilerGenerated]
		get;
	}

	public T Data
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public UpgradeDecisionData(UpgradeUserDecision selection, T data)
	{
		Selection = selection;
		Data = data;
	}
}
