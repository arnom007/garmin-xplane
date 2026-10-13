using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
				resourceMan = new ResourceManager("LauncherCoreLib.Properties.Resources", typeof(Resources).Assembly);
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

	internal static string ARG_CLOSEONERROR => ResourceManager.GetString("ARG_CLOSEONERROR", resourceCulture);

	internal static string ARG_EXITWHENDONE => ResourceManager.GetString("ARG_EXITWHENDONE", resourceCulture);

	internal static string ARG_PROC => ResourceManager.GetString("ARG_PROC", resourceCulture);

	internal static string ARG_RUNALL => ResourceManager.GetString("ARG_RUNALL", resourceCulture);

	internal static string ARG_VAR_DELIM => ResourceManager.GetString("ARG_VAR_DELIM", resourceCulture);

	internal static string ARG_VARS => ResourceManager.GetString("ARG_VARS", resourceCulture);

	internal static string COMPANY_NAME => ResourceManager.GetString("COMPANY_NAME", resourceCulture);

	internal static string ERR_VAR_INCTOOL_CAPTION => ResourceManager.GetString("ERR_VAR_INCTOOL_CAPTION", resourceCulture);

	internal static string ERR_VAR_INCTOOL_MOREINFO => ResourceManager.GetString("ERR_VAR_INCTOOL_MOREINFO", resourceCulture);

	internal static string ERR_VAR_INVALID_SWITCH => ResourceManager.GetString("ERR_VAR_INVALID_SWITCH", resourceCulture);

	internal static string ERR_VAR_MULTIPLE => ResourceManager.GetString("ERR_VAR_MULTIPLE", resourceCulture);

	internal static string ERR_VAR_NULL => ResourceManager.GetString("ERR_VAR_NULL", resourceCulture);

	internal static string ERR_VAR_UNIQUE_SUPPORT_VAR => ResourceManager.GetString("ERR_VAR_UNIQUE_SUPPORT_VAR", resourceCulture);

	internal static string ONE_OR_MORE_CRASH_REPORT_DIRECTORY_PATHS_TOO_LONG => ResourceManager.GetString("ONE_OR_MORE_CRASH_REPORT_DIRECTORY_PATHS_TOO_LONG", resourceCulture);

	internal static string PACKAGES_DIR_NAME => ResourceManager.GetString("PACKAGES_DIR_NAME", resourceCulture);

	internal static string POWER_OFF_BUTTON_TEXT => ResourceManager.GetString("POWER_OFF_BUTTON_TEXT", resourceCulture);

	internal static string POWER_ON_BUTTON_TEXT => ResourceManager.GetString("POWER_ON_BUTTON_TEXT", resourceCulture);

	internal static string REGEX_VAR => ResourceManager.GetString("REGEX_VAR", resourceCulture);

	internal static string RegTrainerKeyRoot => ResourceManager.GetString("RegTrainerKeyRoot", resourceCulture);

	internal static string SimAppInfoSerializationFormat => ResourceManager.GetString("SimAppInfoSerializationFormat", resourceCulture);

	internal static string URI_DATETIME_FORMAT => ResourceManager.GetString("URI_DATETIME_FORMAT", resourceCulture);

	internal static string VAR_HIDE_PATTERN => ResourceManager.GetString("VAR_HIDE_PATTERN", resourceCulture);

	internal Resources()
	{
	}
}
