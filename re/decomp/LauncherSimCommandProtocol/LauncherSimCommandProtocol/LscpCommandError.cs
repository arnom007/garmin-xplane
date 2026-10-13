namespace LauncherSimCommandProtocol;

public enum LscpCommandError : ushort
{
	sim_LSCP_ERR_NONE,
	sim_LSCP_ERR_CMD_PENDING,
	sim_LSCP_ERR_PARSE_FAILED,
	sim_LSCP_ERR_COMMAND_PARSE_FAILED,
	sim_LSCP_ERR_CMD_FAILED,
	sim_LSCP_ERR_CMD_UNSUPPORTED
}
