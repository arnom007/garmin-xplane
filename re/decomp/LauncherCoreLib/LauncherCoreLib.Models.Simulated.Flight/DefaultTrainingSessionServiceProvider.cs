using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using LOLA.Logging;
using LOLA.Tasks;
using Unity;
using Unity.Lifetime;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class DefaultTrainingSessionServiceProvider : ITrainingSessionServiceProvider, ITrainingSessionServiceRegistry
{
	private delegate object FactoryAdapter(ITrainingSession session);

	private readonly Dictionary<ITrainingSession, UnityContainer> _sessionContainers = new Dictionary<ITrainingSession, UnityContainer>();

	private readonly Dictionary<System.Type, FactoryAdapter> _factories = new Dictionary<System.Type, FactoryAdapter>();

	private readonly object _lock = new object();

	public object GetSessionService(System.Type requestedType, ITrainingSession trainingSession)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		ITrainingSession trainingSession2 = trainingSession;
		UnityContainer sessionContainer = default(UnityContainer);
		lock (_lock)
		{
			if (!_sessionContainers.TryGetValue(trainingSession2, ref sessionContainer))
			{
				sessionContainer = new UnityContainer();
				try
				{
					sessionContainer.RegisterInstance(trainingSession2, new ExternallyControlledLifetimeManager());
					Enumerator<System.Type, FactoryAdapter> enumerator = _factories.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<System.Type, FactoryAdapter> kv = enumerator.Current;
							((IUnityContainer)sessionContainer).RegisterFactory(kv.Key, (Func<IUnityContainer, object>)((IUnityContainer c) => kv.Value(trainingSession2)), (IFactoryLifetimeManager)new ContainerControlledLifetimeManager());
						}
					}
					finally
					{
						((System.IDisposable)enumerator).Dispose();
					}
					_sessionContainers[trainingSession2] = sessionContainer;
					TaskHelpers.Finally(trainingSession2.AllSimsExited, (Action)delegate
					{
						lock (_lock)
						{
							_sessionContainers.Remove(trainingSession2);
							sessionContainer.Dispose();
						}
					});
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Error creating training session service container");
					sessionContainer.Dispose();
					_sessionContainers.Remove(trainingSession2);
					throw;
				}
			}
		}
		return sessionContainer.Resolve(requestedType);
	}

	public System.IDisposable RegisterServiceFactory(System.Type targetType, ITrainingSessionServiceFactory serviceFactory)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		ITrainingSessionServiceFactory serviceFactory2 = serviceFactory;
		System.Type targetType2 = targetType;
		if (targetType2 == (System.Type)null)
		{
			throw new ArgumentNullException("targetType");
		}
		if (serviceFactory2 == null)
		{
			throw new ArgumentNullException("serviceFactory");
		}
		lock (_lock)
		{
			if (_factories.ContainsKey(targetType2))
			{
				throw new InvalidOperationException(targetType2.FullName + " is already registered.");
			}
			FactoryAdapter factoryAdapter = (ITrainingSession session) => serviceFactory2.CreateService(this, session);
			_factories[targetType2] = factoryAdapter;
			return Disposable.Create((Action)delegate
			{
				lock (_lock)
				{
					_factories.Remove(targetType2);
				}
			});
		}
	}
}
