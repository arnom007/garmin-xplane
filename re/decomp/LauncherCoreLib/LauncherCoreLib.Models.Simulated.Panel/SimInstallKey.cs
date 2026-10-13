using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Winfs;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class SimInstallKey : IEquatable<SimInstallKey>
{
	private const string SERIAL_DELIMITTER = "-";

	private const string PATH_DELIMITTER = "\\";

	private static readonly string[] VALID_PARSE_DELIMITTERS = new string[3] { "-", "\\", "/" };

	[field: CompilerGenerated]
	public InstallationSlot Slot
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerEmbeddedReleaseId ReleaseId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerIdentifier ModelId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public string SerializedKey => ((object)this).ToString();

	public SimInstallKey(InstallationSlot installSlot, TrainerEmbeddedReleaseId releaseId, TrainerIdentifier modelId)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!modelId.IsValid)
		{
			throw new ArgumentException("Cannot use invalid identifiers.", "modelId");
		}
		Slot = installSlot ?? throw new ArgumentNullException("installSlot");
		ReleaseId = releaseId ?? throw new ArgumentNullException("releaseId");
		ModelId = modelId;
	}

	public static bool operator ==(SimInstallKey? lhs, SimInstallKey? rhs)
	{
		return EqualityComparer<SimInstallKey>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(SimInstallKey? lhs, SimInstallKey? rhs)
	{
		return !EqualityComparer<SimInstallKey>.Default.Equals(lhs, rhs);
	}

	public static bool TryParse(string input, out SimInstallKey key)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		key = null;
		string[] array = input.Split(VALID_PARSE_DELIMITTERS, (StringSplitOptions)0);
		if (array.Length != 3)
		{
			return false;
		}
		string serializedSlot = array[0];
		string embeddedReleaseId = array[1];
		string id = array[2];
		if (TrainerEmbeddedReleaseId.CanParse(embeddedReleaseId) && TrainerIdentifier.IsValidIdentifier(id) && InstallationSlot.TryParse(serializedSlot, out InstallationSlot slot))
		{
			TrainerEmbeddedReleaseId releaseId = new TrainerEmbeddedReleaseId(embeddedReleaseId);
			key = new SimInstallKey(modelId: new TrainerIdentifier(id), installSlot: slot, releaseId: releaseId);
			return true;
		}
		return false;
	}

	public static SimInstallKey Parse(string serializedKey)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!TryParse(serializedKey, out SimInstallKey key))
		{
			throw new FormatException("Invalid SimulatorInstallKey");
		}
		return key;
	}

	public static bool IsValid(string input)
	{
		SimInstallKey key;
		return TryParse(input, out key);
	}

	public RelativePath ToStandardRelativePath()
	{
		return new RelativePath(JoinWithDelimitter("\\"));
	}

	public SimInstallKey WithNewRelease(TrainerEmbeddedReleaseId releaseId)
	{
		if (releaseId != ReleaseId)
		{
			return new SimInstallKey(Slot, releaseId, ModelId);
		}
		return this;
	}

	public override int GetHashCode()
	{
		return ((object)Slot).GetHashCode() + ((object)ReleaseId).GetHashCode() + ((object)ModelId).GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		if (obj is SimInstallKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(SimInstallKey? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityComparer<InstallationSlot>.Default.Equals(Slot, other.Slot) && EqualityComparer<TrainerEmbeddedReleaseId>.Default.Equals(ReleaseId, other.ReleaseId))
			{
				return EqualityComparer<TrainerIdentifier>.Default.Equals(ModelId, other.ModelId);
			}
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return JoinWithDelimitter("-");
	}

	private string JoinWithDelimitter(string delimitter)
	{
		return string.Join(delimitter, new string[3] { Slot.SerializedSlot, ReleaseId, ModelId.IdentString });
	}
}
