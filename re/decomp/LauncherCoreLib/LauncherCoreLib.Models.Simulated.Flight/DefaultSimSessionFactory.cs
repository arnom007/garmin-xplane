using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DataRouterMatrix;
using LOLA.Logging;
using LOLA.Processes;
using LOLA.SoftConfig;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class DefaultSimSessionFactory : ISimSessionFactory
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass10_0
	{
		public DefaultSimSessionFactory _003C_003E4__this;

		public ISessionEnvironment environment;

		public TrainerSessionData tsd;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass6_0
	{
		public DefaultSimSessionFactory _003C_003E4__this;

		public ISessionEnvironment finalEnvironment;

		public List<SimProcessSession> launchedSimulators;

		internal IObservable<SimProcessSession> _003CCreateRouterAndSimulators_003Eb__2(IGrouping<AvionicsType, TrainerSessionData> catGroup)
		{
			return _003C_003E4__this.ForkTrainerGroup(finalEnvironment, catGroup);
		}

		internal void _003CCreateRouterAndSimulators_003Eb__3(SimProcessSession simproc)
		{
			launchedSimulators.Add(simproc);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		[StructLayout(3)]
		private struct _003C_003CForkTrainerGroup_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass9_0 _003C_003E4__this;

			public IObserver<SimProcessSession> o;

			private System.Collections.Generic.IEnumerator<TrainerSessionData> _003C_003E7__wrap1;

			private TrainerSessionData _003Ctrainer_003E5__3;

			private IProcessSession _003Cprocess_003E5__4;

			private ConfiguredTaskAwaiter<IProcessSession> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass9_0 _003C_003Ec__DisplayClass9_ = _003C_003E4__this;
				try
				{
					if (num != 0)
					{
						_003C_003E7__wrap1 = ((System.Collections.Generic.IEnumerable<TrainerSessionData>)Enumerable.OrderBy<TrainerSessionData, int>((System.Collections.Generic.IEnumerable<TrainerSessionData>)_003C_003Ec__DisplayClass9_.categoryGroup, (Func<TrainerSessionData, int>)((TrainerSessionData tsd) => tsd.InstallSlot.InstanceId))).GetEnumerator();
					}
					try
					{
						if (num == 0)
						{
							goto IL_0068;
						}
						goto IL_0146;
						IL_0146:
						if (((System.Collections.IEnumerator)_003C_003E7__wrap1).MoveNext())
						{
							_003Ctrainer_003E5__3 = _003C_003E7__wrap1.Current;
							_003Cprocess_003E5__4 = null;
							goto IL_0068;
						}
						goto end_IL_0047;
						IL_0068:
						try
						{
							ConfiguredTaskAwaiter<IProcessSession> awaiter;
							if (num != 0)
							{
								awaiter = _003C_003Ec__DisplayClass9_._003C_003E4__this.ForkTrainer(_003C_003Ec__DisplayClass9_.finalEnvironment, _003Ctrainer_003E5__3).DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IProcessSession>, _003C_003CForkTrainerGroup_003Eb__0_003Ed>(ref awaiter, ref this);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IProcessSession>);
								num = (_003C_003E1__state = -1);
							}
							IProcessSession result = awaiter.GetResult();
							_003Cprocess_003E5__4 = result;
						}
						catch (System.Exception ex)
						{
							if (_003Cprocess_003E5__4 != null)
							{
								_003Cprocess_003E5__4.ForceClose();
								((System.IDisposable)_003Cprocess_003E5__4).Dispose();
							}
							o.OnError(ex);
							goto end_IL_000e;
						}
						o.OnNext(new SimProcessSession(_003Ctrainer_003E5__3, _003Cprocess_003E5__4));
						_003Cprocess_003E5__4 = null;
						_003Ctrainer_003E5__3 = null;
						goto IL_0146;
						end_IL_0047:;
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap1 != null)
						{
							((System.IDisposable)_003C_003E7__wrap1).Dispose();
						}
					}
					_003C_003E7__wrap1 = null;
					o.OnCompleted();
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public IGrouping<AvionicsType, TrainerSessionData> categoryGroup;

		public DefaultSimSessionFactory _003C_003E4__this;

		public ISessionEnvironment finalEnvironment;

		[AsyncStateMachine(typeof(_003C_003CForkTrainerGroup_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CForkTrainerGroup_003Eb__0(IObserver<SimProcessSession> o)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CForkTrainerGroup_003Eb__0_003Ed _003C_003CForkTrainerGroup_003Eb__0_003Ed = default(_003C_003CForkTrainerGroup_003Eb__0_003Ed);
			_003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CForkTrainerGroup_003Eb__0_003Ed.o = o;
			_003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CForkTrainerGroup_003Eb__0_003Ed>(ref _003C_003CForkTrainerGroup_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CConfigureRouter_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IDataRouterSession> _003C_003Et__builder;

		public DefaultSimSessionFactory _003C_003E4__this;

		public ISessionEnvironment environment;

		private RoarUdpOnlyConfiguration _003CrouteTable_003E5__2;

		private ConfiguredTaskAwaiter<IDataRouterSession> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultSimSessionFactory defaultSimSessionFactory = _003C_003E4__this;
			IDataRouterSession result;
			try
			{
				ConfiguredTaskAwaiter<IDataRouterSession> awaiter;
				if (num != 0)
				{
					_003CrouteTable_003E5__2 = defaultSimSessionFactory.CreateRouteTable(environment);
					ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo> val = defaultSimSessionFactory._sessionRouterInstallAdapter.ResolveRouterAssembliesForEnvironment(environment);
					DataRouterAssemblyInfo item = val.Item1;
					DataRouterAssemblyInfo item2 = val.Item2;
					awaiter = defaultSimSessionFactory._routerSessionFactory.StartRouter(item, item2).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IDataRouterSession>, _003CConfigureRouter_003Ed__12>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IDataRouterSession>);
					num = (_003C_003E1__state = -1);
				}
				IDataRouterSession dataRouterSession = awaiter.GetResult();
				if (dataRouterSession == null)
				{
					throw new InvalidOperationException("Failed to start the data router. Please restart the launcher.");
				}
				try
				{
					IRouterCommandConnection routerCommandConnection = dataRouterSession.CreateCommandConnection();
					try
					{
						if (!routerCommandConnection.Open())
						{
							throw new InvalidOperationException("Failed to connect to the data router. Please restart the launcher.");
						}
						RoarRouteConfig[] array = routerCommandConnection.Configuration.LoadConfigurationAndReturnBrokenRoutes(_003CrouteTable_003E5__2);
						if (array.Length != 0 || !routerCommandConnection.Configuration.StartRouting())
						{
							if (array.Length != 0)
							{
								if (SoftConfigurations.IsDebug)
								{
									Logger.Log(LogLevel.Error, "Broken routes discovered:", string.Join("\t\n", Enumerable.Select<RoarRouteConfig, string>((System.Collections.Generic.IEnumerable<RoarRouteConfig>)array, (Func<RoarRouteConfig, string>)((RoarRouteConfig br) => ((object)br).ToString()))));
								}
								else
								{
									Logger.Log(LogLevel.Error, "Broken routes discovered");
								}
							}
							throw new InvalidOperationException("Failed to configure the data router for this session. Please restart the launcher. If this error persists please send feedback via the About page.");
						}
						List<RoarRouteConfig> routes = Enumerable.ToList<RoarRouteConfig>((System.Collections.Generic.IEnumerable<RoarRouteConfig>)_003CrouteTable_003E5__2.Routes.Values);
						dataRouterSession = new RoarRouterConfigSessionDecorator(dataRouterSession, new RoarRouteCollection((System.Collections.Generic.IReadOnlyCollection<RoarRouteConfig>)routes));
					}
					finally
					{
						if (num < 0)
						{
							((System.IDisposable)routerCommandConnection)?.Dispose();
						}
					}
				}
				catch (System.Exception)
				{
					((System.IDisposable)dataRouterSession).Dispose();
					throw;
				}
				result = dataRouterSession;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrouteTable_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrouteTable_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateRouterAndSimulators_003Ed__6 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimSession> _003C_003Et__builder;

		public DefaultSimSessionFactory _003C_003E4__this;

		public ISessionEnvironment finalEnvironment;

		private _003C_003Ec__DisplayClass6_0 _003C_003E8__1;

		private IDataRouterSession _003CrouterSession_003E5__2;

		private ConfiguredTaskAwaiter<IDataRouterSession> _003C_003Eu__1;

		private object _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultSimSessionFactory defaultSimSessionFactory = _003C_003E4__this;
			SimSession result2;
			try
			{
				ConfiguredTaskAwaiter<IDataRouterSession> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00d3;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass6_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.finalEnvironment = finalEnvironment;
					awaiter = defaultSimSessionFactory.ConfigureRouter(_003C_003E8__1.finalEnvironment).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IDataRouterSession>, _003CCreateRouterAndSimulators_003Ed__6>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IDataRouterSession>);
					num = (_003C_003E1__state = -1);
				}
				IDataRouterSession result = awaiter.GetResult();
				_003CrouterSession_003E5__2 = result;
				_003C_003E8__1.launchedSimulators = new List<SimProcessSession>();
				goto IL_00d3;
				IL_00d3:
				try
				{
					AsyncSubject<SimProcessSession> asyncSubject;
					if (num != 1)
					{
						asyncSubject = Enumerable.Select<IGrouping<AvionicsType, TrainerSessionData>, IObservable<SimProcessSession>>((System.Collections.Generic.IEnumerable<IGrouping<AvionicsType, TrainerSessionData>>)Enumerable.OrderBy<IGrouping<AvionicsType, TrainerSessionData>, AvionicsType>(Enumerable.GroupBy<TrainerSessionData, AvionicsType>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)_003C_003E8__1.finalEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, AvionicsType>)((TrainerSessionData tsd) => tsd.InstallSlot.Category)), (Func<IGrouping<AvionicsType, TrainerSessionData>, AvionicsType>)((IGrouping<AvionicsType, TrainerSessionData> catGroup) => catGroup.Key)), (Func<IGrouping<AvionicsType, TrainerSessionData>, IObservable<SimProcessSession>>)((IGrouping<AvionicsType, TrainerSessionData> catGroup) => _003C_003E8__1._003C_003E4__this.ForkTrainerGroup(_003C_003E8__1.finalEnvironment, catGroup))).Concat().Do<SimProcessSession>((Action<SimProcessSession>)delegate(SimProcessSession simproc)
						{
							_003C_003E8__1.launchedSimulators.Add(simproc);
						})
							.LastOrDefaultAsync<SimProcessSession>()
							.GetAwaiter<SimProcessSession>();
						if (!asyncSubject.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = asyncSubject;
							_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<SimProcessSession>, _003CCreateRouterAndSimulators_003Ed__6>(ref asyncSubject, ref this);
							return;
						}
					}
					else
					{
						asyncSubject = (AsyncSubject<SimProcessSession>)_003C_003Eu__2;
						_003C_003Eu__2 = null;
						num = (_003C_003E1__state = -1);
					}
					asyncSubject.GetResult();
					result2 = new SimSession(_003CrouterSession_003E5__2, (System.Collections.Generic.IReadOnlyList<SimProcessSession>)_003C_003E8__1.launchedSimulators, _003C_003E8__1.finalEnvironment.Inputs.SessionIndex);
				}
				catch (System.Exception)
				{
					if (_003CrouterSession_003E5__2 != null)
					{
						((System.IDisposable)_003CrouterSession_003E5__2).Dispose();
					}
					Enumerator<SimProcessSession> enumerator = _003C_003E8__1.launchedSimulators.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							((System.IDisposable)enumerator.Current.Process).Dispose();
						}
					}
					finally
					{
						if (num < 0)
						{
							((System.IDisposable)enumerator).Dispose();
						}
					}
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CrouterSession_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CrouterSession_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CForkTrainer_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IProcessSession> _003C_003Et__builder;

		public DefaultSimSessionFactory _003C_003E4__this;

		public ISessionEnvironment environment;

		public TrainerSessionData tsd;

		private _003C_003Ec__DisplayClass10_0 _003C_003E8__1;

		private IProcessSession _003Cprocess_003E5__2;

		private ISessionEnvironment _003C_003E7__wrap2;

		private TrainerSessionData _003C_003E7__wrap3;

		private ExeWorkingDirectoryPath _003C_003E7__wrap4;

		private ExeFilePath _003C_003E7__wrap5;

		private ExeCliArgs _003C_003E7__wrap6;

		private ConfiguredTaskAwaiter<TrainerWindowScreenCoordinates?> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<IProcessSession> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DefaultSimSessionFactory defaultSimSessionFactory = _003C_003E4__this;
			IProcessSession result3;
			try
			{
				ConfiguredTaskAwaiter<TrainerWindowScreenCoordinates> awaiter2;
				ConfiguredTaskAwaiter<IProcessSession> awaiter;
				IProcessSession result;
				TrainerWindowScreenCoordinates result2;
				TrainerForkParameters forkParameters;
				switch (num)
				{
				default:
				{
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.environment = environment;
					_003C_003E8__1.tsd = tsd;
					string text = BuildWinfsMountPath(_003C_003E8__1.tsd);
					if (text == null)
					{
						throw new InvalidOperationException("The Winfs mount cannot be null at this point. " + (string?)_003C_003E8__1.tsd.EndpointName + " has invalid WINFS mount path.");
					}
					string[] arguments = BuildSimArguments(_003C_003E8__1.environment, _003C_003E8__1.tsd);
					_003C_003E7__wrap2 = _003C_003E8__1.environment;
					_003C_003E7__wrap3 = _003C_003E8__1.tsd;
					_003C_003E7__wrap4 = new ExeWorkingDirectoryPath(text);
					_003C_003E7__wrap5 = new ExeFilePath(_003C_003E8__1.tsd.PackageInfo.BaseInfo.ExecutablePath);
					_003C_003E7__wrap6 = new ExeCliArgs((System.Collections.Generic.IEnumerable<string>)arguments);
					awaiter2 = defaultSimSessionFactory._003CForkTrainer_003Eg__GetWindowCoords_007C10_0(ref _003C_003E8__1).ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TrainerWindowScreenCoordinates>, _003CForkTrainer_003Ed__10>(ref awaiter2, ref this);
						return;
					}
					goto IL_0175;
				}
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TrainerWindowScreenCoordinates>);
					num = (_003C_003E1__state = -1);
					goto IL_0175;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<IProcessSession>);
					num = (_003C_003E1__state = -1);
					goto IL_023f;
				case 2:
					break;
					IL_023f:
					result = awaiter.GetResult();
					_003Cprocess_003E5__2 = result;
					_003Cprocess_003E5__2.CloseOnDispose = true;
					break;
					IL_0175:
					result2 = awaiter2.GetResult();
					forkParameters = new TrainerForkParameters(_003C_003E7__wrap2, _003C_003E7__wrap3, _003C_003E7__wrap4, _003C_003E7__wrap5, _003C_003E7__wrap6, result2);
					_003C_003E7__wrap2 = null;
					_003C_003E7__wrap3 = null;
					_003C_003E7__wrap4 = default(ExeWorkingDirectoryPath);
					_003C_003E7__wrap5 = default(ExeFilePath);
					_003C_003E7__wrap6 = default(ExeCliArgs);
					awaiter = defaultSimSessionFactory._forkTrainer(forkParameters).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IProcessSession>, _003CForkTrainer_003Ed__10>(ref awaiter, ref this);
						return;
					}
					goto IL_023f;
				}
				try
				{
					ConfiguredTaskAwaiter<bool> awaiter3;
					if (num != 2)
					{
						awaiter3 = _003Cprocess_003E5__2.WaitForMainWindow(TimeSpan.FromMilliseconds(500.0)).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CForkTrainer_003Ed__10>(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<bool>);
						num = (_003C_003E1__state = -1);
					}
					if (!awaiter3.GetResult())
					{
						Logger.Log(LogLevel.Debug, $"[Warning][Launch] The {_003C_003E8__1.tsd.EndpointName} trainer window did not load in time. It may appear out of order in the task bar.");
					}
				}
				catch (object obj) when (obj is InvalidOperationException && _003Cprocess_003E5__2.HasExited())
				{
					Logger.Log(LogLevel.Warning, $"[Launch] The {_003C_003E8__1.tsd.EndpointName} trainer process exited immediately.");
				}
				result3 = _003Cprocess_003E5__2;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = default(_003C_003Ec__DisplayClass10_0);
				_003Cprocess_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = default(_003C_003Ec__DisplayClass10_0);
			_003Cprocess_003E5__2 = null;
			_003C_003Et__builder.SetResult(result3);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ISessionRouteTableFactory _routeTableFactory;

	private readonly IDataRouterStarter _routerSessionFactory;

	private readonly ISimSessionRouterInstallationAdapter _sessionRouterInstallAdapter;

	private readonly GetDesiredTrainerWindow _getDesiredTrainerWindow;

	private readonly ForkTrainer _forkTrainer;

	public DefaultSimSessionFactory(ISessionRouteTableFactory routeTableFactory, IDataRouterStarter dataRouterFactory, ISimSessionRouterInstallationAdapter sessionRouterVersionResolver, ForkTrainer forkTrainer, GetDesiredTrainerWindow getDesiredTrainerWindow)
	{
		_routeTableFactory = routeTableFactory;
		_routerSessionFactory = dataRouterFactory;
		_sessionRouterInstallAdapter = sessionRouterVersionResolver;
		_forkTrainer = forkTrainer;
		_getDesiredTrainerWindow = getDesiredTrainerWindow;
	}

	[AsyncStateMachine(typeof(_003CCreateRouterAndSimulators_003Ed__6))]
	public async System.Threading.Tasks.Task<SimSession> CreateRouterAndSimulators(ISessionEnvironment finalEnvironment)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ISessionEnvironment finalEnvironment2 = finalEnvironment;
		IDataRouterSession routerSession = await ConfigureRouter(finalEnvironment2).DetachContext();
		List<SimProcessSession> launchedSimulators = new List<SimProcessSession>();
		try
		{
			await Enumerable.Select<IGrouping<AvionicsType, TrainerSessionData>, IObservable<SimProcessSession>>((System.Collections.Generic.IEnumerable<IGrouping<AvionicsType, TrainerSessionData>>)Enumerable.OrderBy<IGrouping<AvionicsType, TrainerSessionData>, AvionicsType>(Enumerable.GroupBy<TrainerSessionData, AvionicsType>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)finalEnvironment2.Data.TrainerData).Values, (Func<TrainerSessionData, AvionicsType>)((TrainerSessionData tsd) => tsd.InstallSlot.Category)), (Func<IGrouping<AvionicsType, TrainerSessionData>, AvionicsType>)((IGrouping<AvionicsType, TrainerSessionData> catGroup) => catGroup.Key)), (Func<IGrouping<AvionicsType, TrainerSessionData>, IObservable<SimProcessSession>>)((IGrouping<AvionicsType, TrainerSessionData> catGroup) => ForkTrainerGroup(finalEnvironment2, catGroup))).Concat().Do<SimProcessSession>((Action<SimProcessSession>)delegate(SimProcessSession simproc)
			{
				launchedSimulators.Add(simproc);
			})
				.LastOrDefaultAsync<SimProcessSession>();
			return new SimSession(routerSession, (System.Collections.Generic.IReadOnlyList<SimProcessSession>)launchedSimulators, finalEnvironment2.Inputs.SessionIndex);
		}
		catch (System.Exception)
		{
			((System.IDisposable)routerSession)?.Dispose();
			Enumerator<SimProcessSession> enumerator = launchedSimulators.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					((System.IDisposable)enumerator.Current.Process).Dispose();
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
			throw;
		}
	}

	private static string[] BuildSimArguments(ISessionEnvironment environment, TrainerSessionData s)
	{
		return new string[4]
		{
			"--configpath",
			"\"" + environment.Inputs.SessionJsonPath + "\"",
			"--endpt",
			s.EndpointName.IdentStringChecked
		};
	}

	private static string BuildWinfsMountPath(TrainerSessionData s)
	{
		return s.WinfsMountPath;
	}

	private IObservable<SimProcessSession> ForkTrainerGroup(ISessionEnvironment finalEnvironment, IGrouping<AvionicsType, TrainerSessionData> categoryGroup)
	{
		_003C_003Ec__DisplayClass9_0 _003C_003Ec__DisplayClass9_ = new _003C_003Ec__DisplayClass9_0();
		_003C_003Ec__DisplayClass9_.categoryGroup = categoryGroup;
		_003C_003Ec__DisplayClass9_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass9_.finalEnvironment = finalEnvironment;
		return Observable.Create<SimProcessSession>((Func<IObserver<SimProcessSession>, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_0._003C_003CForkTrainerGroup_003Eb__0_003Ed))] (IObserver<SimProcessSession> o) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass9_0._003C_003CForkTrainerGroup_003Eb__0_003Ed _003C_003CForkTrainerGroup_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass9_0._003C_003CForkTrainerGroup_003Eb__0_003Ed);
			_003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass9_;
			_003C_003CForkTrainerGroup_003Eb__0_003Ed.o = o;
			_003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass9_0._003C_003CForkTrainerGroup_003Eb__0_003Ed>(ref _003C_003CForkTrainerGroup_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CForkTrainerGroup_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}));
	}

	[AsyncStateMachine(typeof(_003CForkTrainer_003Ed__10))]
	private async System.Threading.Tasks.Task<IProcessSession> ForkTrainer(ISessionEnvironment environment, TrainerSessionData tsd)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ISessionEnvironment environment2 = environment;
		TrainerSessionData tsd2 = tsd;
		string text = BuildWinfsMountPath(tsd2);
		if (text == null)
		{
			throw new InvalidOperationException("The Winfs mount cannot be null at this point. " + (string?)tsd2.EndpointName + " has invalid WINFS mount path.");
		}
		string[] arguments = BuildSimArguments(environment2, tsd2);
		ISessionEnvironment sessionEnvironment = environment2;
		TrainerSessionData trainerSessionData = tsd2;
		ExeWorkingDirectoryPath workingDirectory = new ExeWorkingDirectoryPath(text);
		ExeFilePath exePath = new ExeFilePath(tsd2.PackageInfo.BaseInfo.ExecutablePath);
		ExeCliArgs arguments2 = new ExeCliArgs((System.Collections.Generic.IEnumerable<string>)arguments);
		TrainerForkParameters forkParameters = new TrainerForkParameters(sessionEnvironment, trainerSessionData, workingDirectory, exePath, arguments2, await GetWindowCoords().ConfigureAwait(false));
		IProcessSession process = await _forkTrainer(forkParameters).ConfigureAwait(false);
		process.CloseOnDispose = true;
		try
		{
			if (!(await process.WaitForMainWindow(TimeSpan.FromMilliseconds(500.0)).DetachContext()))
			{
				Logger.Log(LogLevel.Debug, $"[Warning][Launch] The {tsd2.EndpointName} trainer window did not load in time. It may appear out of order in the task bar.");
			}
		}
		catch (object obj) when (obj is InvalidOperationException && process.HasExited())
		{
			Logger.Log(LogLevel.Warning, $"[Launch] The {tsd2.EndpointName} trainer process exited immediately.");
		}
		return process;
		[CompilerGenerated]
		System.Threading.Tasks.Task<TrainerWindowScreenCoordinates?> GetWindowCoords()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return _getDesiredTrainerWindow(environment2, tsd2, CancellationToken.None);
		}
	}

	private RoarUdpOnlyConfiguration CreateRouteTable(ISessionEnvironment finalEnvironment)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return (_routeTableFactory.CreateRouteTable(finalEnvironment) ?? throw new InvalidOperationException("Route table is invalid.")).CopyAllData();
	}

	[AsyncStateMachine(typeof(_003CConfigureRouter_003Ed__12))]
	private async System.Threading.Tasks.Task<IDataRouterSession> ConfigureRouter(ISessionEnvironment environment)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RoarUdpOnlyConfiguration routeTable = CreateRouteTable(environment);
		ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo> val = _sessionRouterInstallAdapter.ResolveRouterAssembliesForEnvironment(environment);
		DataRouterAssemblyInfo item = val.Item1;
		DataRouterAssemblyInfo item2 = val.Item2;
		IDataRouterSession dataRouterSession = await _routerSessionFactory.StartRouter(item, item2).DetachContext();
		if (dataRouterSession == null)
		{
			throw new InvalidOperationException("Failed to start the data router. Please restart the launcher.");
		}
		try
		{
			using IRouterCommandConnection routerCommandConnection = dataRouterSession.CreateCommandConnection();
			if (!routerCommandConnection.Open())
			{
				throw new InvalidOperationException("Failed to connect to the data router. Please restart the launcher.");
			}
			RoarRouteConfig[] array = routerCommandConnection.Configuration.LoadConfigurationAndReturnBrokenRoutes(routeTable);
			if (array.Length != 0 || !routerCommandConnection.Configuration.StartRouting())
			{
				if (array.Length != 0)
				{
					if (SoftConfigurations.IsDebug)
					{
						Logger.Log(LogLevel.Error, "Broken routes discovered:", string.Join("\t\n", Enumerable.Select<RoarRouteConfig, string>((System.Collections.Generic.IEnumerable<RoarRouteConfig>)array, (Func<RoarRouteConfig, string>)((RoarRouteConfig br) => ((object)br).ToString()))));
					}
					else
					{
						Logger.Log(LogLevel.Error, "Broken routes discovered");
					}
				}
				throw new InvalidOperationException("Failed to configure the data router for this session. Please restart the launcher. If this error persists please send feedback via the About page.");
			}
			List<RoarRouteConfig> routes = Enumerable.ToList<RoarRouteConfig>((System.Collections.Generic.IEnumerable<RoarRouteConfig>)routeTable.Routes.Values);
			dataRouterSession = new RoarRouterConfigSessionDecorator(dataRouterSession, new RoarRouteCollection((System.Collections.Generic.IReadOnlyCollection<RoarRouteConfig>)routes));
			return dataRouterSession;
		}
		catch (System.Exception)
		{
			((System.IDisposable)dataRouterSession).Dispose();
			throw;
		}
	}

	[CompilerGenerated]
	private System.Threading.Tasks.Task<TrainerWindowScreenCoordinates?> _003CForkTrainer_003Eg__GetWindowCoords_007C10_0(ref _003C_003Ec__DisplayClass10_0 P_0)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return _getDesiredTrainerWindow(P_0.environment, P_0.tsd, CancellationToken.None);
	}
}
