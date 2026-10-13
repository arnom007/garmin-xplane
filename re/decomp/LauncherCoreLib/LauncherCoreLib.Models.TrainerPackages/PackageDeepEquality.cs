using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageDeepEquality : EqualityComparer<ITrainerPackage>
{
	[field: CompilerGenerated]
	public static PackageDeepEquality Instance
	{
		[CompilerGenerated]
		get;
	} = new PackageDeepEquality();


	public override bool Equals(ITrainerPackage? x, ITrainerPackage? y)
	{
		if (x != y)
		{
			if (x != null && y != null && EqualityComparer<ITrainerBaseInfo>.Default.Equals(x.BaseInfo, y.BaseInfo))
			{
				return ((System.Collections.Generic.IEnumerable<ITrainerModelInfo>)x.Models).SetEquals((System.Collections.Generic.IEnumerable<ITrainerModelInfo>)y.Models);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode(ITrainerPackage obj)
	{
		return ((object)obj.TrainerPackageId).GetHashCode();
	}
}
