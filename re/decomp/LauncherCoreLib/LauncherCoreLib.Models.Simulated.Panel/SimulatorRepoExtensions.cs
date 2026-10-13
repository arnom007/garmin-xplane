using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Panel;

public static class SimulatorRepoExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetSimulatorOrDefault_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimInstallData> _003C_003Et__builder;

		public ISimulatorRepository repo;

		public AircraftIdentifier aircraftIdentifier;

		public SimInstallKey simKey;

		private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SimInstallData result;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<SimInstallData> awaiter;
					if (num != 0)
					{
						awaiter = repo.GetSimulator(aircraftIdentifier, simKey).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CGetSimulatorOrDefault_003Ed__0>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimInstallData>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (SimulatorNotFoundException)
				{
					result = new SimInstallData(new SimAircraftKey(aircraftIdentifier, simKey));
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CGetSimulatorOrDefault_003Ed__0))]
	public static async System.Threading.Tasks.Task<SimInstallData> GetSimulatorOrDefault(this ISimulatorRepository repo, AircraftIdentifier aircraftIdentifier, SimInstallKey simKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return await repo.GetSimulator(aircraftIdentifier, simKey).DetachContext();
		}
		catch (SimulatorNotFoundException)
		{
			return new SimInstallData(new SimAircraftKey(aircraftIdentifier, simKey));
		}
	}
}
