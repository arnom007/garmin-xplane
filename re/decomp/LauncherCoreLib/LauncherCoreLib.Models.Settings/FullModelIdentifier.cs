using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Settings;

public sealed class FullModelIdentifier : IHaveModelIdentity, IHaveTrainerPackageIdentity, IEquatable<FullModelIdentifier>, IEquatable<SettingIdentifier>
{
	private const string PACKAGE_NAMESPACE = "Packages";

	private const string MODEL_SUB_NAMESPACE = "Models";

	private readonly SettingIdentifier _ident;

	private readonly TrainerIdentifier _modelShortId;

	[field: CompilerGenerated]
	public TrainerPackageIdentifier TrainerPackageId
	{
		[CompilerGenerated]
		get;
	}

	public TrainerIdentifier ModelId => _modelShortId;

	public FullModelIdentifier(SettingIdentifier settingIdentifier)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (settingIdentifier == null)
		{
			throw new ArgumentNullException("settingIdentifier");
		}
		SettingIdentifier settingIdentifier2 = settingIdentifier.RelativeTo(SettingIdentifier.FromString("Packages"));
		if ((object)settingIdentifier2 == null)
		{
			throw new FormatException("Setting identifier must contain a Packages sub identifier.");
		}
		settingIdentifier2 = settingIdentifier2.Root;
		SettingIdentifier settingIdentifier3 = settingIdentifier.RelativeTo(SettingIdentifier.CombineSubIdentifiers("Packages", settingIdentifier2, "Models"));
		if (settingIdentifier3 == null)
		{
			throw new FormatException("Setting identifier must contain a Models sub identifier.");
		}
		settingIdentifier3 = settingIdentifier3.Root;
		_ident = SettingIdentifier.CombineSubIdentifiers("Packages", settingIdentifier2, "Models", settingIdentifier3);
		TrainerPackageId = TrainerPackageIdentifier.Parse(settingIdentifier2);
		_modelShortId = (string)settingIdentifier3;
	}

	public FullModelIdentifier(string shortPackage, string shortModel)
		: this(TrainerPackageIdentifier.Parse(shortPackage), shortModel)
	{
	}

	public FullModelIdentifier(TrainerPackageIdentifier packageId, string shortModel)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!SettingIdentifier.IsValidSubIdentifier(packageId.Ident))
		{
			throw new FormatException("packageId is not a valid sub identifier.");
		}
		if (!SettingIdentifier.IsValidSubIdentifier(shortModel))
		{
			throw new FormatException("shortModel is not a valid sub identifier.");
		}
		_ident = SettingIdentifier.CombineSubIdentifiers("Packages", packageId.Ident.IdentStringChecked, "Models", shortModel);
		TrainerPackageId = packageId;
		_modelShortId = shortModel;
	}

	public static bool IsValidFullModelIdentifier(string identifier)
	{
		if (!SettingIdentifier.IsValidFullId(identifier))
		{
			return false;
		}
		SettingIdentifier settingIdentifier = SettingIdentifier.FromString(identifier);
		SettingIdentifier settingIdentifier2 = settingIdentifier.RelativeTo(SettingIdentifier.FromString("Packages"));
		if (settingIdentifier2 == null || settingIdentifier.RelativeTo(SettingIdentifier.CombineSubIdentifiers("Packages", settingIdentifier2.Root, "Models")) == null)
		{
			return false;
		}
		return true;
	}

	public TrainerModelKey ToCompositeKey()
	{
		return new TrainerModelKey(TrainerPackageId, _modelShortId.IdentStringChecked);
	}

	public bool Equals(FullModelIdentifier? other)
	{
		if (this != other)
		{
			if (other != null)
			{
				return EqualityComparer<SettingIdentifier>.Default.Equals(_ident, other._ident);
			}
			return false;
		}
		return true;
	}

	public bool Equals(SettingIdentifier? other)
	{
		return EqualityComparer<SettingIdentifier>.Default.Equals(_ident, other);
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is FullModelIdentifier other) || !Equals(other))
		{
			if (obj is SettingIdentifier other2)
			{
				return Equals(other2);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return ((object)_ident).GetHashCode();
	}

	public override string ToString()
	{
		return ((object)_ident).ToString();
	}
}
