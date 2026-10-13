using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LOLA.Processes;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using ReliableDirectMessageProtocol;

namespace LauncherSimCommandProtocol;

public class TrainingSessionLauncherSimCommanderProvider : ITrainingSessionLauncherSimCommanderProvider
{
	private readonly Dictionary<SimInstallKey, ILauncherSimCommander> _commanders;

	private readonly IReadOnlyDictionary<SimInstallKey, System.Threading.Tasks.Task<ProcessResult>> _exitTasks;

	private readonly ITrainingSessionRdmpProvider _rdmpProvider;

	public TrainingSessionLauncherSimCommanderProvider(ITrainingSessionRdmpProvider rdmpProvider, IReadOnlyDictionary<SimInstallKey, System.Threading.Tasks.Task<ProcessResult>> exitTasks)
	{
		_rdmpProvider = rdmpProvider;
		_exitTasks = exitTasks;
		_commanders = new Dictionary<SimInstallKey, ILauncherSimCommander>();
	}

	public bool DoesSimSupportLscp(SimInstallKey installKey)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return _rdmpProvider.DoesSimSupportRdmp(installKey ?? throw new ArgumentNullException("installKey"));
	}

	public ILauncherSimCommander GetOrCreateSimCommander(SimInstallKey installKey)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		lock (_commanders)
		{
			ILauncherSimCommander launcherSimCommander = default(ILauncherSimCommander);
			if (!_commanders.TryGetValue(installKey, ref launcherSimCommander))
			{
				System.Threading.Tasks.Task<ProcessResult> exitTask = default(System.Threading.Tasks.Task<ProcessResult>);
				if (!_exitTasks.TryGetValue(installKey, ref exitTask))
				{
					throw new KeyNotFoundException("Invalid InstallKey. Key: " + installKey.SerializedKey);
				}
				IReliableDirectMessageCom orCreateRdmpClient = _rdmpProvider.GetOrCreateRdmpClient(installKey);
				launcherSimCommander = new RdmpLauncherSimCommander(installKey, exitTask, orCreateRdmpClient);
				_commanders.Add(installKey, launcherSimCommander);
			}
			return launcherSimCommander;
		}
	}
}
