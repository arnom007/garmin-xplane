using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Winfs;

[AttributeUsage(/*Could not decode attribute arguments.*/)]
public sealed class DatabaseDeployedRelativePathAttribute : System.Attribute
{
	[field: CompilerGenerated]
	public RelativePath DeployedRelativePath
	{
		[CompilerGenerated]
		get;
	}

	public DatabaseDeployedRelativePathAttribute(string deployedRelativePath)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(deployedRelativePath))
		{
			throw new ArgumentNullException("deployedRelativePath");
		}
		DeployedRelativePath = deployedRelativePath;
	}

	public static RelativePath? GetDeployedPath(SupportedWinfsDatabase supportedWinfsDatabase)
	{
		MemberInfo[] member = ((object)supportedWinfsDatabase).GetType().GetMember(((object)supportedWinfsDatabase).ToString());
		if (member != null && member.Length == 1)
		{
			object[] customAttributes = member[0].GetCustomAttributes(typeof(DatabaseDeployedRelativePathAttribute), false);
			if (customAttributes != null && customAttributes.Length == 1)
			{
				return ((DatabaseDeployedRelativePathAttribute)customAttributes[0]).DeployedRelativePath;
			}
		}
		return null;
	}
}
