using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

internal class MigrationSource : IMigrationSource, IRunMigrations, IRunCancelableMigrations
{
	private readonly ISubject<BatchMigration> _batches = new ReplaySubject<BatchMigration>(1);

	public IObservable<BatchMigration> MigrationsBeingApplied => ((IObservable<BatchMigration>)_batches).AsObservable<BatchMigration>();

	public BatchMigration ScheduleMigrations(params ILauncherMigration[] migrations)
	{
		BatchMigration batchMigration = new BatchMigration(Enumerable.Select<ILauncherMigration, IObservable<MigrationData<MigrationStatusUpdate>>>((System.Collections.Generic.IEnumerable<ILauncherMigration>)migrations, (Func<ILauncherMigration, IObservable<MigrationData<MigrationStatusUpdate>>>)((ILauncherMigration m) => CreateUpdateStream(m, (ILauncherMigration mi) => mi.MigrationSteps()))).Concat());
		((IObserver<BatchMigration>)_batches).OnNext(batchMigration);
		return batchMigration;
	}

	public BatchMigration ScheduleMigrations(CancellationToken token, params ICancelableMigration[] migrations)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		BatchMigration batchMigration = new BatchMigration(Enumerable.Select<ICancelableMigration, IObservable<MigrationData<MigrationStatusUpdate>>>((System.Collections.Generic.IEnumerable<ICancelableMigration>)migrations, (Func<ICancelableMigration, IObservable<MigrationData<MigrationStatusUpdate>>>)((ICancelableMigration m) => CreateUpdateStream(m, (ICancelableMigration mi) => mi.MigrationSteps(token)))).Concat());
		((IObserver<BatchMigration>)_batches).OnNext(batchMigration);
		return batchMigration;
	}

	private static IObservable<MigrationData<MigrationStatusUpdate>> CreateUpdateStream<T>(T migration, Func<T, IObservable<MigrationStatusUpdate>> factory) where T : ILauncherMigration
	{
		T migration2 = migration;
		Func<T, IObservable<MigrationStatusUpdate>> factory2 = factory;
		return Observable.Create<MigrationStatusUpdate>((Func<IObserver<MigrationStatusUpdate>, CancellationToken, System.Threading.Tasks.Task<System.IDisposable>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_0<>._003C_003CCreateUpdateStream_003Eb__0_003Ed))] async (IObserver<MigrationStatusUpdate> o, CancellationToken token) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			bool isMigrationRequired = false;
			try
			{
				isMigrationRequired = await migration2.IsMigrationRequired().DetachContext();
			}
			catch (System.Exception ex)
			{
				o.OnError(ex);
			}
			if (isMigrationRequired)
			{
				return factory2.Invoke(migration2).SubscribeSafe<MigrationStatusUpdate>(o);
			}
			o.OnCompleted();
			return Disposable.Empty;
		})).Do<MigrationStatusUpdate>((Action<MigrationStatusUpdate>)delegate(MigrationStatusUpdate statusUpdate)
		{
			if (statusUpdate.TotalSteps == 1)
			{
				Logger.Log(LogLevel.Info, "[Migration][{0}] {1}", migration2.MigrationName, statusUpdate.Message);
			}
			else
			{
				Logger.Log(LogLevel.Info, "[Migration][{0}] {2:P} {1} ", migration2.MigrationName, statusUpdate.Message, statusUpdate.PercentDone);
			}
		}).Select<MigrationStatusUpdate, MigrationData<MigrationStatusUpdate>>((Func<MigrationStatusUpdate, MigrationData<MigrationStatusUpdate>>)((MigrationStatusUpdate statusUpdate) => MigrationData.Create(migration2, statusUpdate)));
	}
}
