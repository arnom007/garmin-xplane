using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using LauncherCoreLib.Models.Simulated.Panel;
using ReliableDirectMessageProtocol;
using RoarClient.Net;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class TrainingSessionRdmpProvider : ITrainingSessionRdmpProvider
{
	private readonly IRoarEndpoint _endpoint;

	private readonly Dictionary<SimInstallKey, RoarRouteConfig> _installKeyRouteLookup;

	private readonly Dictionary<SimInstallKey, ReliableDirectMessageCom> _createdRdmpClients = new Dictionary<SimInstallKey, ReliableDirectMessageCom>();

	private readonly object _createdRdmpClientsLock = new object();

	public TrainingSessionRdmpProvider(ITrainingSession session)
	{
		_endpoint = session.Router.GetOrOpenEndpoint("Launcher");
		Dictionary<TrainerIdentifier, TrainerSessionData> sims = Enumerable.ToDictionary<TrainerSessionData, TrainerIdentifier>(Enumerable.Select<SimProcessSession, TrainerSessionData>((System.Collections.Generic.IEnumerable<SimProcessSession>)session.Simulators, (Func<SimProcessSession, TrainerSessionData>)((SimProcessSession sim) => sim.Simulator)), (Func<TrainerSessionData, TrainerIdentifier>)((TrainerSessionData s) => s.EndpointName));
		_installKeyRouteLookup = Enumerable.ToDictionary(Enumerable.Select((System.Collections.Generic.IEnumerable<RoarRouteConfig>)session.Router.RouteCollection.GetRoutes((RoarRouteConfig config) => config.ActualFormat == ROAR_prtcl_fmt_type.ROAR_SIM_GENERIC_FMT && config.DestinationEndpoint != "Launcher"), delegate(RoarRouteConfig config)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			TrainerSessionData trainerSessionData = default(TrainerSessionData);
			if (!sims.TryGetValue((TrainerIdentifier)config.DestinationEndpoint, ref trainerSessionData))
			{
				throw new ArgumentException("No simulator has an endpoint named \"" + config.DestinationEndpoint + "\"");
			}
			return new
			{
				Key = trainerSessionData.InstallData.InstallationKey,
				Config = config
			};
		}), group => group.Key, group => group.Config);
	}

	public bool DoesSimSupportRdmp(SimInstallKey installKey)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		RoarRouteConfig roarRouteConfig = default(RoarRouteConfig);
		return _installKeyRouteLookup.TryGetValue(installKey ?? throw new ArgumentNullException("installKey"), ref roarRouteConfig);
	}

	public IReliableDirectMessageCom GetOrCreateRdmpClient(SimInstallKey installKey)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (installKey == null)
		{
			throw new ArgumentNullException("installKey");
		}
		ReliableDirectMessageCom reliableDirectMessageCom = default(ReliableDirectMessageCom);
		lock (_createdRdmpClientsLock)
		{
			if (!_createdRdmpClients.TryGetValue(installKey, ref reliableDirectMessageCom))
			{
				RoarRouteConfig roarRouteConfig = default(RoarRouteConfig);
				if (!_installKeyRouteLookup.TryGetValue(installKey, ref roarRouteConfig))
				{
					throw new KeyNotFoundException("installKey not found");
				}
				reliableDirectMessageCom = new ReliableDirectMessageCom(new BufferedRdmpTx(new RoarRdmpRxTx(_endpoint, roarRouteConfig.NativeSourceChannel), Scheduler.Default), new RdmpPacketNumberProvider(), Scheduler.Default);
				_createdRdmpClients.Add(installKey, reliableDirectMessageCom);
			}
		}
		return reliableDirectMessageCom;
	}
}
