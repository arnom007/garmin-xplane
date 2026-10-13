namespace LauncherSimCommandProtocol;

public enum SimCommandType : ushort
{
	COMMAND_RESPONSE,
	COMMAND_SUPPORTED_QUERY,
	CAPTURE_SCREENSHOT,
	CONFIG_SD_SAVE
}
