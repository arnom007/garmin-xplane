using System;
using System.Threading;
using GTI.Net;
using LOLA.Tasks;

namespace LauncherCoreLib.Models;

public static class GtiProcessExtensions
{
	public static IGtiProcessExecution TryToCancelProcessWithToken(this IGtiProcessExecution gtiprocess, CancellationToken token)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		IGtiProcessExecution gtiprocess2 = gtiprocess;
		CancellationTokenRegistration registration = ((CancellationToken)(ref token)).Register((Action)delegate
		{
			gtiprocess2.SendCancelSignal();
		}, false);
		TaskHelpers.Finally<GtiProcessResult>(gtiprocess2.ProcessResult, (Action)delegate
		{
			((CancellationTokenRegistration)(ref registration)).Dispose();
		});
		return gtiprocess2;
	}
}
