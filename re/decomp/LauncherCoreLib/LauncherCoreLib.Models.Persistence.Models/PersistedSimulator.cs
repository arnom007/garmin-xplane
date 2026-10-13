using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Persistence.Models;

internal sealed class PersistedSimulator
{
	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public string FullModelId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public string FamilyId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public string InstallationVersion
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty(Required = Required.Always)]
	[field: CompilerGenerated]
	public Dictionary<string, string> Options
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new Dictionary<string, string>();


	[field: CompilerGenerated]
	public JObject ModuleData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new JObject();


	public bool IsValid()
	{
		Version val = default(Version);
		if (FullModelIdentifier.IsValidFullModelIdentifier(FullModelId) && TrainerIdentifier.IsValidIdentifier(FamilyId))
		{
			return Version.TryParse(InstallationVersion, ref val);
		}
		return false;
	}
}
