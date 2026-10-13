using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class SettingDefinitionJsonWrapper
{
	[field: CompilerGenerated]
	public JsonObjectProvider Json
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int? LocalDocumentIndex
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string? SettingPropertyName
	{
		[CompilerGenerated]
		get;
	}

	public SettingDefinitionJsonWrapper(JsonObjectProvider json, string? settingPropertyName, int? localDocumentIndex)
	{
		Json = json;
		LocalDocumentIndex = localDocumentIndex;
		SettingPropertyName = settingPropertyName;
	}
}
