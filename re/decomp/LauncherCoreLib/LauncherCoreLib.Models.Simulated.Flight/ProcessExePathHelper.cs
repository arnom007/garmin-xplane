using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class ProcessExePathHelper
{
	public static GetProcessExePath FallBackOn(this GetProcessExePath primary, GetProcessExePath secondary)
	{
		GetProcessExePath primary2 = primary;
		GetProcessExePath secondary2 = secondary;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CFallBackOn_003Eb__0_003Ed))] async (int id, CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			string text = await primary2(id, t).ConfigureAwait(false);
			return (text == null) ? (await secondary2(id, t).ConfigureAwait(false)) : text;
		};
	}

	public static GetProcessExePath FromProcessSession(IOsProcessOperations processes)
	{
		IOsProcessOperations processes2 = processes;
		return delegate(int processId, CancellationToken cancel)
		{
			//IL_0052: Expected O, but got Unknown
			int num = System.IntPtr.Size * 8;
			int processArchitecture = processes2.GetProcessArchitecture(processId);
			if (num != processArchitecture)
			{
				return System.Threading.Tasks.Task.FromResult<string>((string)null);
			}
			try
			{
				return System.Threading.Tasks.Task.FromResult<string>(processes2.ProcessExecutablePath(processId));
			}
			catch (InvalidOperationException val)
			{
				Logger.LogFormattedException((System.Exception)val, "Determining a process exe path. Our process is {0}bits", System.IntPtr.Size * 8);
				return System.Threading.Tasks.Task.FromResult<string>((string)null);
			}
		};
	}

	public static GetProcessExePath WmiQuery()
	{
		return [AsyncStateMachine(typeof(_003C_003Ec._003C_003CWmiQuery_003Eb__2_0_003Ed))] async (int processId, CancellationToken cancel) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			string queryString = $"SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = {processId}";
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(queryString);
			try
			{
				ManagementOperationObserver observer = new ManagementOperationObserver();
				List<string> records = new List<string>();
				TaskCompletionSource<object?> queryComplete = new TaskCompletionSource<object>();
				observer.ObjectReady += delegate(object s, ObjectReadyEventArgs e)
				{
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					records.Add(e.NewObject["ExecutablePath"]?.ToString() ?? throw new KeyNotFoundException("Missing ExecutablePath from WMI query."));
				};
				observer.Completed += delegate(object s, CompletedEventArgs e)
				{
					if (e.Status == ManagementStatus.NoError)
					{
						queryComplete.SetResult((object)null);
					}
					else if (((CancellationToken)(ref cancel)).CanBeCanceled && ((CancellationToken)(ref cancel)).IsCancellationRequested)
					{
						queryComplete.SetCanceled();
					}
					else
					{
						queryComplete.SetException((System.Exception)(object)new ManagementException(((object)e.Status).ToString()));
					}
				};
				searcher.Get(observer);
				CancellationTokenRegistration val = ((CancellationToken)(ref cancel)).Register((Action)delegate
				{
					observer.Cancel();
				}, false);
				try
				{
					await queryComplete.Task.ConfigureAwait(false);
					return Enumerable.SingleOrDefault<string>((System.Collections.Generic.IEnumerable<string>)records);
				}
				finally
				{
					((System.IDisposable)(CancellationTokenRegistration)(ref val)).Dispose();
				}
			}
			finally
			{
				((System.IDisposable)searcher)?.Dispose();
			}
		};
	}
}
