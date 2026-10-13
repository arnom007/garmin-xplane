using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Utility;

public sealed class JsonDataUpdate
{
	[field: CompilerGenerated]
	public string JsonPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReadOnlyJsonDataSource UpdatedValue
	{
		[CompilerGenerated]
		get;
	}

	public JsonDataUpdate(string jsonpath, IReadOnlyJsonDataSource value)
	{
		JsonPath = jsonpath;
		UpdatedValue = value;
	}
}
