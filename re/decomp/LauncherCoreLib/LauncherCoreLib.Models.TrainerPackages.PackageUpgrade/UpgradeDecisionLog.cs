using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.TrainerPackages.Serialized;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

public class UpgradeDecisionLog
{
	private readonly EmbeddedReleaseDecisionLog _serialized;

	[field: CompilerGenerated]
	public TrainerEmbeddedReleaseId EmbeddedReleaseId
	{
		[CompilerGenerated]
		get;
	}

	public System.Collections.Generic.IEnumerable<EmbeddedReleaseDecisionLog.Entry> Entries => _serialized.CopyEntries();

	public UpgradeDecisionLog(TrainerEmbeddedReleaseId embeddedReleaseId, EmbeddedReleaseDecisionLog serializedLog)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EmbeddedReleaseId = embeddedReleaseId ?? throw new ArgumentNullException("embeddedReleaseId");
		_serialized = serializedLog ?? throw new ArgumentNullException("serializedLog");
	}

	public UpgradeUserDecision GetDecision(PackageUpgradeVersions upgradeVersions)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		PackageUpgradeVersions upgradeVersions2 = upgradeVersions;
		if (upgradeVersions2 == null)
		{
			throw new ArgumentNullException("upgradeVersions");
		}
		if (upgradeVersions2.EmbeddedReleaseId != EmbeddedReleaseId)
		{
			throw new ArgumentException("Can only compare against the same embedded release.");
		}
		List<EmbeddedReleaseDecisionLog.Entry> inSearchOrder = Enumerable.ToList<EmbeddedReleaseDecisionLog.Entry>(Enumerable.Where<EmbeddedReleaseDecisionLog.Entry>((System.Collections.Generic.IEnumerable<EmbeddedReleaseDecisionLog.Entry>)_serialized.Entries, (Func<EmbeddedReleaseDecisionLog.Entry, bool>)((EmbeddedReleaseDecisionLog.Entry e) => e.LocalVersion <= upgradeVersions2.LocalVersion || e.UpgradeVersion >= upgradeVersions2.UpgradeVersion)));
		List<EmbeddedReleaseDecisionLog.Entry> gapLessEntries = GetGapLessEntries(upgradeVersions2, (System.Collections.Generic.IReadOnlyList<EmbeddedReleaseDecisionLog.Entry>)inSearchOrder, 0);
		if (GaplessEntriesReachUpgradeVersion(upgradeVersions2, gapLessEntries))
		{
			EmbeddedReleaseDecisionLog.Entry entry = gapLessEntries[gapLessEntries.Count - 1];
			if (entry.UpgradeVersion >= upgradeVersions2.UpgradeVersion)
			{
				return entry.Decision;
			}
		}
		return UpgradeUserDecision.Undecided;
	}

	private static bool GaplessEntriesReachUpgradeVersion(PackageUpgradeVersions upgradeVersions, List<EmbeddedReleaseDecisionLog.Entry> gapLessApprovedUpgrades)
	{
		if (gapLessApprovedUpgrades.Count > 0)
		{
			return gapLessApprovedUpgrades[gapLessApprovedUpgrades.Count - 1].UpgradeVersion >= upgradeVersions.UpgradeVersion;
		}
		return false;
	}

	private static List<EmbeddedReleaseDecisionLog.Entry> GetGapLessEntries(PackageUpgradeVersions upgradeVersions, System.Collections.Generic.IReadOnlyList<EmbeddedReleaseDecisionLog.Entry> inSearchOrder, int startingIndex)
	{
		List<EmbeddedReleaseDecisionLog.Entry> val = new List<EmbeddedReleaseDecisionLog.Entry>();
		EmbeddedReleaseDecisionLog.Entry entry = null;
		for (int i = startingIndex; i < ((System.Collections.Generic.IReadOnlyCollection<EmbeddedReleaseDecisionLog.Entry>)inSearchOrder).Count; i++)
		{
			EmbeddedReleaseDecisionLog.Entry entry2 = inSearchOrder[i];
			if (entry == null)
			{
				if (entry2.LocalVersion > upgradeVersions.LocalVersion)
				{
					break;
				}
				val.Add(entry2);
				entry = entry2;
				continue;
			}
			if (entry.UpgradeVersion >= upgradeVersions.UpgradeVersion)
			{
				break;
			}
			if (!entry2.IsSameVersionRange(entry))
			{
				if (!entry2.IsOverlapping(entry))
				{
					break;
				}
				if (entry2.UpgradeVersion > entry.UpgradeVersion)
				{
					entry = entry2;
					val.Add(entry2);
				}
			}
		}
		return val;
	}
}
