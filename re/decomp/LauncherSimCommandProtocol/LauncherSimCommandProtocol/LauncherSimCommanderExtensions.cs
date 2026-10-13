using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherSimCommandProtocol;

public static class LauncherSimCommanderExtensions
{
	public static System.Threading.Tasks.Task<LscpCommandSupportResponseType> DoesSimSupportMessage(this ILauncherSimCommander commander, SimCommandType commandType, IScheduler scheduler, TimeSpan timeout, CancellationToken token)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ILauncherSimCommander commander2 = commander;
		IScheduler scheduler2 = scheduler;
		if (commander2 == null)
		{
			throw new ArgumentNullException("commander");
		}
		if (scheduler2 == null)
		{
			throw new ArgumentNullException("scheduler");
		}
		return Inner();
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CDoesSimSupportMessage_003Eg__Inner_007C0_003Ed))]
		async System.Threading.Tasks.Task<LscpCommandSupportResponseType> Inner()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			LscpCommandResponse response;
			LscpCommandSupportQueryResponse commandQuerryResponse;
			System.Threading.Tasks.Task<ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>> supportTask = commander2.Messages().Where<LscpPacketReceipt>((Func<LscpPacketReceipt, bool>)((LscpPacketReceipt m) => m.CommandPacket.CommandType == SimCommandType.COMMAND_RESPONSE)).Select<LscpPacketReceipt, ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>((Func<LscpPacketReceipt, ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>)((LscpPacketReceipt m) => (LscpCommandResponse.TryParseForCommand(m.CommandPacket.OpenPayload(), SimCommandType.COMMAND_SUPPORTED_QUERY, out response) && LscpCommandSupportQueryResponse.TryParse(response.OpenPayload(), out commandQuerryResponse)) ? ParsedLscpCommandResponse.Create(response, commandQuerryResponse) : null))
				.Where<ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>((Func<ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>, bool>)((ParsedLscpCommandResponse<LscpCommandSupportQueryResponse> r) => r != null))
				.FirstOrDefaultAsync<ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>()
				.Timeout<ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>(timeout, scheduler2)
				.ToTask<ParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>(token);
			LauncherSimCommandPacket launcherSimCommandPacket = LauncherSimCommandPacket.CreatePacket(SimCommandType.COMMAND_SUPPORTED_QUERY, new LscpCommandSupportQuery(commandType).ToBinary());
			await commander2.SendToClient(launcherSimCommandPacket.ToBinary(), token).DetachContext();
			ParsedLscpCommandResponse<LscpCommandSupportQueryResponse> parsedLscpCommandResponse = await supportTask.DetachContext();
			if (parsedLscpCommandResponse != null && parsedLscpCommandResponse.GenericResponse.IsSuccess && parsedLscpCommandResponse.Payload != null)
			{
				return parsedLscpCommandResponse.Payload.SupportedResponse;
			}
			return LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN;
		}
	}
}
