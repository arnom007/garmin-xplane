namespace LauncherSimCommandProtocol;

public enum LscpCommandSupportResponseType : ushort
{
	sim_LSCP_SUPPORT_UNKNOWN,
	sim_LSCP_COMMAND_NOT_SUPPORTED,
	sim_LSCP_COMMAND_UNKNOWN,
	sim_LSCP_COMMAND_SUPPORTED
}
