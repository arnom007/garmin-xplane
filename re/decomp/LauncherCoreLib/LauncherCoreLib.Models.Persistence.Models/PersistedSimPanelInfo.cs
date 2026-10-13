using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Panel;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.Persistence.Models;

public class PersistedSimPanelInfo
{
	public const string DEFAULT_FILE_NAME = "panel.json";

	[JsonProperty("ActiveSimulatorKeys")]
	[field: CompilerGenerated]
	public System.Collections.Generic.IList<string> ActiveSimulatorKeyStrings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonIgnore]
	public System.Collections.Generic.IEnumerable<SimInstallKey> ActiveSimulatorKeys
	{
		get
		{
			return Enumerable.Select<string, SimInstallKey>(Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)ActiveSimulatorKeyStrings, (Func<string, bool>)((string s) => SimInstallKey.IsValid(s))), (Func<string, SimInstallKey>)((string s) => SimInstallKey.Parse(s)));
		}
		set
		{
			ActiveSimulatorKeyStrings = (System.Collections.Generic.IList<string>)Enumerable.ToList<string>(Enumerable.Select<SimInstallKey, string>((System.Collections.Generic.IEnumerable<SimInstallKey>)Enumerable.OrderBy<SimInstallKey, string>((System.Collections.Generic.IEnumerable<SimInstallKey>)(((object)value) ?? ((object)new List<SimInstallKey>())), (Func<SimInstallKey, string>)((SimInstallKey s) => s.SerializedKey)), (Func<SimInstallKey, string>)((SimInstallKey s) => s.SerializedKey)));
		}
	}

	public PersistedSimPanelInfo()
	{
		ActiveSimulatorKeyStrings = (System.Collections.Generic.IList<string>)new List<string>();
	}

	public static PersistedSimPanelInfo FromRuntime(SimPanel simulatorPanel)
	{
		return new PersistedSimPanelInfo
		{
			ActiveSimulatorKeys = (System.Collections.Generic.IEnumerable<SimInstallKey>)Enumerable.ToList<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)simulatorPanel.ActiveSimKeys)
		};
	}

	public PersistedSimPanelInfo FixTooManyActiveSimsInSlot()
	{
		ActiveSimulatorKeys = (System.Collections.Generic.IEnumerable<SimInstallKey>)Enumerable.ToList<SimInstallKey>(Enumerable.Select<IGrouping<InstallationSlot, SimInstallKey>, SimInstallKey>(Enumerable.GroupBy<SimInstallKey, InstallationSlot>(ActiveSimulatorKeys, (Func<SimInstallKey, InstallationSlot>)((SimInstallKey s) => s.Slot)), (Func<IGrouping<InstallationSlot, SimInstallKey>, SimInstallKey>)((IGrouping<InstallationSlot, SimInstallKey> groupedBySlot) => Enumerable.Last<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)groupedBySlot))));
		return this;
	}
}
