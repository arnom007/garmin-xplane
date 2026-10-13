using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HttpLib;
using LauncherCoreLib.Models;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Airport;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace Aviation_Trainer_Launcher;

internal class TrainerTypes : IoCTypesBase
{
	[CompilerGenerated]
	private sealed class _003CSingletons_003Ed__0 : System.Collections.Generic.IEnumerable<TypeMapping>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<TypeMapping>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private TypeMapping _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		TypeMapping System.Collections.Generic.IEnumerator<TypeMapping>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CSingletons_003Ed__0(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IStartingAircraftFactory, PilotLauncherStartingAircraft>();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IHttpProvider, BasicHttpProvider>();
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				return false;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<TypeMapping> System.Collections.Generic.IEnumerable<TypeMapping>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CSingletons_003Ed__0(0);
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<TypeMapping>)this).GetEnumerator();
		}
	}

	[IteratorStateMachine(typeof(_003CSingletons_003Ed__0))]
	public override System.Collections.Generic.IEnumerable<TypeMapping> Singletons()
	{
		yield return TypeMapping.Create<IStartingAircraftFactory, PilotLauncherStartingAircraft>();
		yield return TypeMapping.Create<IHttpProvider, BasicHttpProvider>();
	}

	public override void Complex(IUnityContainer container)
	{
		container.RegisterInstance((IInstallationLogProvider)new WindowsInstallationLogFileProvider("Garmin_Aviation_Trainers"));
		container.RegisterFactory<IHttpRequester>((Func<IUnityContainer, object>)((IUnityContainer c) => c.Resolve<IHttpProvider>(System.Array.Empty<ResolverOverride>()).Http), (IFactoryLifetimeManager)null);
	}
}
