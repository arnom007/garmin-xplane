using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingsRepo
{
	IGlobalSettingsProvider Globals { get; }

	System.Threading.Tasks.Task<ISettingsBinder<SimAircraftKey>> BindSimulator(SimAircraftKey simAircraftKey);

	System.Threading.Tasks.Task<ISettingsBinder<AircraftPackageFamily>> BindFamilySettings(AircraftPackageFamily aircraftPackageFamily);
}
