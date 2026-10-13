using System;
using System.Threading.Tasks;
using RoarClient.Net;
using RoarClient.Net.Native;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRoarEndpoint
{
	System.Threading.Tasks.Task Send(ROAR_prtcl_fmt_type format, ROAR_rte_chnl_type channel, byte[] payload);

	IObservable<RoarMessageEventArgs> Messages();
}
