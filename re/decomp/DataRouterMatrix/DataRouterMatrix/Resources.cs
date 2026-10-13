using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DataRouterMatrix;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("DataRouterMatrix.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string DataRouterClientResolutionInternalError => ResourceManager.GetString("DataRouterClientResolutionInternalError", resourceCulture);

	internal static string NoDataRouterClientsAreInstalled => ResourceManager.GetString("NoDataRouterClientsAreInstalled", resourceCulture);

	internal static string NoDataRoutersAreInstalled => ResourceManager.GetString("NoDataRoutersAreInstalled", resourceCulture);

	internal static string NoSolutionExists => ResourceManager.GetString("NoSolutionExists", resourceCulture);

	internal static string NoTrainerClientVersionsToCheck => ResourceManager.GetString("NoTrainerClientVersionsToCheck", resourceCulture);

	internal Resources()
	{
	}
}
