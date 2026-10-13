using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class AvionicsModel : IEquatable<AvionicsModel>
{
	private readonly ITrainerModelInfo _modelInfo;

	[field: CompilerGenerated]
	public ITrainerPackage Package
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerModelKey Key
	{
		[CompilerGenerated]
		get;
	}

	public ITrainerModelInfo ModelInfo => _modelInfo;

	public string SimulatorDisplayName => BaseInfo.DisplayName;

	public AvionicsType AvionicsType => _modelInfo.Category;

	public TrainerIdentifier FamilyId => BaseInfo.FamilyId;

	public ITrainerBaseInfo BaseInfo => Package.BaseInfo;

	public int MaxInstances => BaseInfo.MaxInstances;

	public string DisplayName => _modelInfo.DisplayName;

	public int DisplayOrderIndex => _modelInfo.DisplayOrderIndex;

	public string IconImagePath => _modelInfo.IconImagePath;

	public string VersionString => BaseInfo.VersionString;

	public AvionicsModel(ITrainerPackage package, ITrainerModelInfo modelInfo)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Package = package ?? throw new ArgumentNullException("package");
		_modelInfo = modelInfo ?? throw new ArgumentNullException("modelInfo");
		Key = new TrainerModelKey(package.TrainerPackageId, modelInfo.ModelId.IdentStringChecked);
	}

	public static System.Collections.Generic.IEnumerable<AvionicsModel> CreateFromPackage(ITrainerPackage package)
	{
		ITrainerPackage package2 = package;
		return Enumerable.Select<ITrainerModelInfo, AvionicsModel>((System.Collections.Generic.IEnumerable<ITrainerModelInfo>)package2.Models, (Func<ITrainerModelInfo, AvionicsModel>)((ITrainerModelInfo modelInfo) => new AvionicsModel(package2, modelInfo)));
	}

	public static bool operator ==(AvionicsModel? lhs, AvionicsModel? rhs)
	{
		return EqualityComparer<AvionicsModel>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(AvionicsModel? lhs, AvionicsModel? rhs)
	{
		return !EqualityComparer<AvionicsModel>.Default.Equals(lhs, rhs);
	}

	public bool CanBeInstalledIn(AirframeCategoryClassType airframe)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return airframe switch
		{
			AirframeCategoryClassType.None => throw new ArgumentException(((object)airframe).ToString() + " is not a valid airframe"), 
			AirframeCategoryClassType.Unconfigured => true, 
			_ => _modelInfo.SupportedAirframes.Contains(airframe), 
		};
	}

	public bool Equals(AvionicsModel? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && Key == other.Key && EqualityComparer<ITrainerBaseInfo>.Default.Equals(BaseInfo, other.BaseInfo))
			{
				return EqualityComparer<ITrainerModelInfo>.Default.Equals(_modelInfo, other._modelInfo);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is AvionicsModel other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)Key).GetHashCode();
	}

	public override string ToString()
	{
		return DisplayName;
	}
}
