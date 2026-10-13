using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.CrashReporting;

public class DisplayDeviceInfo
{
	[field: CompilerGenerated]
	public string AdapterName
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string Manufacturer
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string DriverVersionString
	{
		[CompilerGenerated]
		get;
	}

	public DisplayDeviceInfo(string deviceName, string manufacturer, string driverVersionString)
	{
		AdapterName = deviceName;
		Manufacturer = manufacturer;
		DriverVersionString = driverVersionString;
	}

	public virtual string ToString()
	{
		return AdapterName + " (v" + DriverVersionString + ")";
	}
}
