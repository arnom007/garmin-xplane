using System;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public class PingSimReadyForLscp : IPingSimReadyForLscp
{
	private readonly ITrainingSessionServiceProvider _serviceProvider;

	public PingSimReadyForLscp(ITrainingSessionServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public System.Threading.Tasks.Task<LscpPingResult> PingSim(ITrainingSession trainingSession, SimInstallKey installKey, IScheduler scheduler, TimeSpan timeout, CancellationToken token)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		IScheduler scheduler2 = scheduler;
		if (trainingSession == null)
		{
			throw new ArgumentNullException("trainingSession");
		}
		if ((object)installKey == null)
		{
			throw new ArgumentNullException("installKey");
		}
		if (scheduler2 == null)
		{
			throw new ArgumentNullException("scheduler");
		}
		ITrainingSessionLauncherSimCommanderProvider sessionService = _serviceProvider.GetSessionService<ITrainingSessionLauncherSimCommanderProvider>(trainingSession);
		if (!sessionService.DoesSimSupportLscp(installKey))
		{
			return System.Threading.Tasks.Task.FromResult<LscpPingResult>(LscpPingResult.Unsupported);
		}
		ILauncherSimCommander lscpClient = sessionService.GetOrCreateSimCommander(installKey);
		if (lscpClient != null)
		{
			return Inner();
		}
		return System.Threading.Tasks.Task.FromResult<LscpPingResult>(LscpPingResult.Error);
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass2_0._003C_003CPingSim_003Eg__Inner_007C0_003Ed))]
		async System.Threading.Tasks.Task<LscpPingResult> Inner()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				LscpCommandSupportResponseType lscpCommandSupportResponseType = await lscpClient.DoesSimSupportMessage(SimCommandType.COMMAND_SUPPORTED_QUERY, scheduler2, timeout, token).DetachContext();
				LscpPingResult result = default(LscpPingResult);
				switch (lscpCommandSupportResponseType)
				{
				case LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN:
					result = LscpPingResult.Error;
					break;
				case LscpCommandSupportResponseType.sim_LSCP_COMMAND_NOT_SUPPORTED:
				case LscpCommandSupportResponseType.sim_LSCP_COMMAND_UNKNOWN:
				case LscpCommandSupportResponseType.sim_LSCP_COMMAND_SUPPORTED:
					result = LscpPingResult.Ready;
					break;
				default:
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(lscpCommandSupportResponseType);
					break;
				}
				return result;
			}
			catch (TimeoutException)
			{
				return LscpPingResult.Timeout;
			}
			catch (SimOfflineException)
			{
				return LscpPingResult.Offline;
			}
		}
	}
}
