using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.OnlineResources;

internal class OnlineResourceProvider : IOnlineResourceProvider
{
	[CompilerGenerated]
	private sealed class _003CGetResources_003Ed__2 : System.Collections.Generic.IEnumerable<OnlineResource>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<OnlineResource>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private OnlineResource _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		OnlineResource System.Collections.Generic.IEnumerator<OnlineResource>.Current
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
		public _003CGetResources_003Ed__2(int _003C_003E1__state)
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
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = new OnlineResource("Garmin Aviation Training", "Access pilot training that includes self-study materials, eLearning courses, and instructor-led courses for GTN, G500/600, TXi, and more.", new Uri("https://garmin.com/aviation/training"));
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = new OnlineResource("Product Videos & Tutorials", "Garmin Aviation YouTube portal.", new Uri("https://www.youtube.com/user/garminaviation"));
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
		System.Collections.Generic.IEnumerator<OnlineResource> System.Collections.Generic.IEnumerable<OnlineResource>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CGetResources_003Ed__2(0);
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<OnlineResource>)this).GetEnumerator();
		}
	}

	public System.Collections.Generic.IEnumerable<OnlineResource> AvailableResources => GetResources();

	[IteratorStateMachine(typeof(_003CGetResources_003Ed__2))]
	private static System.Collections.Generic.IEnumerable<OnlineResource> GetResources()
	{
		yield return new OnlineResource("Garmin Aviation Training", "Access pilot training that includes self-study materials, eLearning courses, and instructor-led courses for GTN, G500/600, TXi, and more.", new Uri("https://garmin.com/aviation/training"));
		yield return new OnlineResource("Product Videos & Tutorials", "Garmin Aviation YouTube portal.", new Uri("https://www.youtube.com/user/garminaviation"));
	}
}
