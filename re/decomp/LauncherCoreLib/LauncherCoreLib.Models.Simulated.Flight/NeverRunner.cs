using System;
using System.Runtime.CompilerServices;
using LOLA.Reactive;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class NeverRunner : ISimRunnerEvolved
{
	private static readonly IReactiveEntity<PowerStage> POWERED_OFF_STAGE = ReactiveEntity.Constant(PowerStage.PoweredDown);

	private static readonly IReactiveEntity<bool> CANNOT_POWER_ON = ReactiveEntity.Constant(constant: false);

	private static readonly IReactiveEntity<ISimSessionEvolved?> NO_SESSIONS = ReactiveEntity.Constant<ISimSessionEvolved>(null);

	[field: CompilerGenerated]
	public ISimSessionCoordinator Coordinator
	{
		[CompilerGenerated]
		get;
	}

	public IReactiveEntity<PowerStage> Stage => POWERED_OFF_STAGE;

	public IReactiveEntity<bool> CanPowerOn => CANNOT_POWER_ON;

	public IReactiveEntity<ISimSessionEvolved?> Sessions => NO_SESSIONS;

	public NeverRunner(ISimSessionCoordinator coordinator)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Coordinator = coordinator ?? throw new ArgumentNullException("coordinator");
	}

	public ISimSessionEvolved StartSession()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		throw new InvalidOperationException("This runner can never be run.");
	}
}
