using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TrainerPackageLib;
using Unity;
using Unity.Injection;

namespace LauncherCoreLib.Models.Winfs;

public class WinFsTypes : IoCTypesBase
{
	[CompilerGenerated]
	private sealed class _003CSingletons_003Ed__1 : System.Collections.Generic.IEnumerable<TypeMapping>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<TypeMapping>, System.Collections.IEnumerator, System.IDisposable
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
		public _003CSingletons_003Ed__1(int _003C_003E1__state)
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
				_003C_003E2__current = TypeMapping.Create<ICreationContextFactory, CreationContextFactory>();
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISourcedBatchTransformProvider, SourcedBatchTransformProvider>();
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<ISourcedBatchFilterFactory, DefaultSourcedBatchFilterFactory>();
				_003C_003E1__state = 3;
				return true;
			case 3:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IWinfsTransformProcessor, TransformProcessor>();
				_003C_003E1__state = 4;
				return true;
			case 4:
				_003C_003E1__state = -1;
				_003C_003E2__current = TypeMapping.Create<IDatabaseDeploymentPathProvider, DatabaseDeploymentPathProvider>();
				_003C_003E1__state = 5;
				return true;
			case 5:
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
			return new _003CSingletons_003Ed__1(0);
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<TypeMapping>)this).GetEnumerator();
		}
	}

	public override void Complex(IUnityContainer container)
	{
		container.RegisterType<IManifestValidator, WinfsManifestValidator>("Winfs", System.Array.Empty<InjectionMember>());
	}

	[IteratorStateMachine(typeof(_003CSingletons_003Ed__1))]
	public override System.Collections.Generic.IEnumerable<TypeMapping> Singletons()
	{
		yield return TypeMapping.Create<ICreationContextFactory, CreationContextFactory>();
		yield return TypeMapping.Create<ISourcedBatchTransformProvider, SourcedBatchTransformProvider>();
		yield return TypeMapping.Create<ISourcedBatchFilterFactory, DefaultSourcedBatchFilterFactory>();
		yield return TypeMapping.Create<IWinfsTransformProcessor, TransformProcessor>();
		yield return TypeMapping.Create<IDatabaseDeploymentPathProvider, DatabaseDeploymentPathProvider>();
	}
}
