using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Flight;

namespace LauncherCoreLib.Models.Winfs;

internal class DefaultSourcedBatchFilterFactory : ISourcedBatchFilterFactory
{
	private sealed class ConstraintFilter : ISourcedBatchFilter
	{
		[StructLayout(3)]
		[CompilerGenerated]
		private struct _003CKeep_003Ed__3 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public ConstraintFilter _003C_003E4__this;

			public SourcedBatchTransform transform;

			private object _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				ConstraintFilter constraintFilter = _003C_003E4__this;
				bool result;
				try
				{
					AsyncSubject<bool> asyncSubject;
					if (num != 0)
					{
						asyncSubject = constraintFilter._watcher.AreConstraintsInBounds((System.Collections.Generic.IReadOnlyCollection<ValueConstraintDefinition>)(object)transform.Transform.Constraints).FirstOrDefaultAsync<bool>().GetAwaiter<bool>();
						if (!asyncSubject.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = asyncSubject;
							_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<bool>, _003CKeep_003Ed__3>(ref asyncSubject, ref this);
							return;
						}
					}
					else
					{
						asyncSubject = (AsyncSubject<bool>)_003C_003Eu__1;
						_003C_003Eu__1 = null;
						num = (_003C_003E1__state = -1);
					}
					result = asyncSubject.GetResult();
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

		private readonly ISettingSymbolTable _scopedTable;

		private readonly SettingTableConstraintWatcher _watcher;

		public ConstraintFilter(ISettingSymbolTable settingTable)
		{
			_scopedTable = settingTable;
			_watcher = new SettingTableConstraintWatcher(settingTable);
		}

		[AsyncStateMachine(typeof(_003CKeep_003Ed__3))]
		public async System.Threading.Tasks.Task<bool> Keep(SourcedBatchTransform transform)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _watcher.AreConstraintsInBounds((System.Collections.Generic.IReadOnlyCollection<ValueConstraintDefinition>)(object)transform.Transform.Constraints).FirstOrDefaultAsync<bool>();
		}

		public System.Threading.Tasks.Task<bool> ThrowAway(SourcedBatchTransform transform)
		{
			return System.Threading.Tasks.Task.FromResult<bool>(false);
		}
	}

	[CompilerGenerated]
	private sealed class _003CFilters_003Ed__1 : System.Collections.Generic.IEnumerable<ISourcedBatchFilter>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<ISourcedBatchFilter>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private ISourcedBatchFilter _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private ISettingSymbolTable settingsTable;

		public ISettingSymbolTable _003C_003E3__settingsTable;

		ISourcedBatchFilter System.Collections.Generic.IEnumerator<ISourcedBatchFilter>.Current
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
		public _003CFilters_003Ed__1(int _003C_003E1__state)
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
				_003C_003E2__current = new ConstraintFilter(settingsTable);
				_003C_003E1__state = 1;
				return true;
			case 1:
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
		System.Collections.Generic.IEnumerator<ISourcedBatchFilter> System.Collections.Generic.IEnumerable<ISourcedBatchFilter>.GetEnumerator()
		{
			_003CFilters_003Ed__1 _003CFilters_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CFilters_003Ed__ = this;
			}
			else
			{
				_003CFilters_003Ed__ = new _003CFilters_003Ed__1(0);
			}
			_003CFilters_003Ed__.settingsTable = _003C_003E3__settingsTable;
			return _003CFilters_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<ISourcedBatchFilter>)this).GetEnumerator();
		}
	}

	public ISourcedBatchFilter[] CreateFilters(ISessionInfo session, ISettingSymbolTable settingsTable)
	{
		return Enumerable.ToArray<ISourcedBatchFilter>(Filters(settingsTable));
	}

	[IteratorStateMachine(typeof(_003CFilters_003Ed__1))]
	private static System.Collections.Generic.IEnumerable<ISourcedBatchFilter> Filters(ISettingSymbolTable settingsTable)
	{
		yield return new ConstraintFilter(settingsTable);
	}
}
