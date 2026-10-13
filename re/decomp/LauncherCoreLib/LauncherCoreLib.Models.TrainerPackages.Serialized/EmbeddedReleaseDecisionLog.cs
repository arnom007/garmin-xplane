using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.TrainerPackages.Serialized;

public class EmbeddedReleaseDecisionLog
{
	public sealed class Entry : IEquatable<Entry>
	{
		[JsonProperty(Required = Required.Always)]
		[field: CompilerGenerated]
		public Version LocalVersion
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = new Version();


		[JsonProperty(Required = Required.Always)]
		[field: CompilerGenerated]
		public Version UpgradeVersion
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = new Version();


		[JsonProperty(Required = Required.Always)]
		[field: CompilerGenerated]
		public UpgradeUserDecision Decision
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[JsonProperty(Required = Required.Always)]
		[field: CompilerGenerated]
		public DateTimeOffset DecidedAt
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[JsonIgnore]
		public VersionRange VersionRange => new VersionRange(LocalVersion, UpgradeVersion);

		public Entry Copy()
		{
			return (Entry)base.MemberwiseClone();
		}

		public bool IsSameVersionRange(Entry other)
		{
			return VersionRange == other.VersionRange;
		}

		public bool IsOverlapping(Entry other)
		{
			return VersionRange.Overlaps(other.VersionRange);
		}

		public bool Equals(Entry? other)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (this != other)
			{
				if (other != null && LocalVersion == other.LocalVersion && UpgradeVersion == other.UpgradeVersion && Decision == other.Decision)
				{
					return DecidedAt == other.DecidedAt;
				}
				return false;
			}
			return true;
		}

		public override bool Equals(object? obj)
		{
			if (obj is Entry other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			DateTimeOffset decidedAt = DecidedAt;
			return ((object)(DateTimeOffset)(ref decidedAt)).GetHashCode();
		}

		public override string ToString()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "[{0} -> {1}] {2} : {3}", new object[4] { LocalVersion, UpgradeVersion, Decision, DecidedAt });
		}
	}

	private sealed class EntryInsertionComparer : Comparer<Entry>
	{
		public override int Compare(Entry? x, Entry? y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			return CompareNonNull(x, y);
			[CompilerGenerated]
			static int CompareNonNull(Entry x, Entry y)
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				int num = x.LocalVersion.CompareTo(y.LocalVersion);
				if (num == 0)
				{
					int num2 = x.UpgradeVersion.CompareTo(y.UpgradeVersion);
					if (num2 == 0)
					{
						DateTimeOffset decidedAt = x.DecidedAt;
						return ((DateTimeOffset)(ref decidedAt)).CompareTo(y.DecidedAt) * -1;
					}
					return num2 * -1;
				}
				return num;
			}
		}
	}

	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<Entry> Entries
	{
		[CompilerGenerated]
		get;
	}

	public EmbeddedReleaseDecisionLog()
		: this((System.Collections.Generic.IList<Entry>)new List<Entry>())
	{
	}

	[JsonConstructor]
	public EmbeddedReleaseDecisionLog(System.Collections.Generic.IList<Entry> entries)
	{
		Entries = entries;
	}

	public static EmbeddedReleaseDecisionLog Build(System.Collections.Generic.IEnumerable<Entry> entries)
	{
		EmbeddedReleaseDecisionLog embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
		embeddedReleaseDecisionLog.AddEntries(entries);
		return embeddedReleaseDecisionLog;
	}

	public EmbeddedReleaseDecisionLog AddEntries(System.Collections.Generic.IEnumerable<Entry> newEntries)
	{
		EntryInsertionComparer comparer = new EntryInsertionComparer();
		System.Collections.Generic.IEnumerator<Entry> enumerator = newEntries.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Entry current = enumerator.Current;
				Entries.InsertInUniqueSortedPosition(current, (IComparer<Entry>?)(object)comparer);
			}
			return this;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public System.Collections.Generic.IEnumerable<Entry> CopyEntries()
	{
		return Enumerable.Select<Entry, Entry>((System.Collections.Generic.IEnumerable<Entry>)Entries, (Func<Entry, Entry>)((Entry e) => e.Copy()));
	}
}
