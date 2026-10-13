using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.Winfs;

internal interface ISourcedBatchFilterFactory
{
	ISourcedBatchFilter[] CreateFilters(ISessionInfo session, ISettingSymbolTable settingsTable);
}
