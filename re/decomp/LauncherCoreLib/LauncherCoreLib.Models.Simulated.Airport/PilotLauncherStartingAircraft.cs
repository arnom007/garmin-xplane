using System.Collections.Generic;
using System.Linq;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class PilotLauncherStartingAircraft : IStartingAircraftFactory
{
	private const string GARMIN_PROVIDED_PREFIX = "NG";

	public System.Collections.Generic.IList<AircraftListData> CreateStartingAircraftInfos()
	{
		List<AircraftListData> obj = new List<AircraftListData>();
		obj.Add(CreateBuiltInInfo(AirframeCategoryClassType.SingleEngine));
		obj.Add(CreateBuiltInInfo(AirframeCategoryClassType.MultiEngine));
		obj.Add(CreateBuiltInInfo(AirframeCategoryClassType.SingleTurboProp));
		obj.Add(CreateBuiltInInfo(AirframeCategoryClassType.MultiTurboProp));
		obj.Add(CreateBuiltInInfo(AirframeCategoryClassType.MultiTurboFan));
		obj.Add(CreateBuiltInInfo(AirframeCategoryClassType.Rotorcraft));
		return (System.Collections.Generic.IList<AircraftListData>)obj;
	}

	private static AircraftListData CreateBuiltInInfo(AirframeCategoryClassType airframeCategoryClassType)
	{
		return new AircraftListData(Enumerable.First<AircraftIdentifier>(AircraftIdentifier.GenerateIdentifiers(airframeCategoryClassType, "NG")), airframeCategoryClassType.ToUIString(), "", airframeCategoryClassType);
	}
}
