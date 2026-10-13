using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Utility;

public static class LauncherTextHelpers
{
	[CompilerGenerated]
	private sealed class _003CSplit_003Ed__0 : System.Collections.Generic.IEnumerable<ReadOnlyMemory<char>>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<ReadOnlyMemory<char>>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private ReadOnlyMemory<char> _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Func<char, bool> controller;

		public Func<char, bool> _003C_003E3__controller;

		private ReadOnlyMemory<char> str;

		public ReadOnlyMemory<char> _003C_003E3__str;

		private int _003Cc_003E5__2;

		ReadOnlyMemory<char> System.Collections.Generic.IEnumerator<ReadOnlyMemory<char>>.Current
		{
			[DebuggerHidden]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CSplit_003Ed__0(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private unsafe bool MoveNext()
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			int num;
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				num = 0;
				_003Cc_003E5__2 = 0;
				goto IL_0097;
			case 1:
				_003C_003E1__state = -1;
				num = _003Cc_003E5__2 + 1;
				goto IL_0087;
			case 2:
				{
					_003C_003E1__state = -1;
					return false;
				}
				IL_0097:
				if (_003Cc_003E5__2 < str.Length)
				{
					if (controller.Invoke(*(char*)str.Span[_003Cc_003E5__2]))
					{
						_003C_003E2__current = str.Slice(num, _003Cc_003E5__2 - num);
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_0087;
				}
				_003C_003E2__current = str.Slice(num);
				_003C_003E1__state = 2;
				return true;
				IL_0087:
				_003Cc_003E5__2++;
				goto IL_0097;
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
		System.Collections.Generic.IEnumerator<ReadOnlyMemory<char>> System.Collections.Generic.IEnumerable<ReadOnlyMemory<char>>.GetEnumerator()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			_003CSplit_003Ed__0 _003CSplit_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CSplit_003Ed__ = this;
			}
			else
			{
				_003CSplit_003Ed__ = new _003CSplit_003Ed__0(0);
			}
			_003CSplit_003Ed__.str = _003C_003E3__str;
			_003CSplit_003Ed__.controller = _003C_003E3__controller;
			return _003CSplit_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<ReadOnlyMemory<char>>)this).GetEnumerator();
		}
	}

	[IteratorStateMachine(typeof(_003CSplit_003Ed__0))]
	public unsafe static System.Collections.Generic.IEnumerable<ReadOnlyMemory<char>> Split(ReadOnlyMemory<char> str, Func<char, bool> controller)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int c = 0; c < str.Length; c++)
		{
			if (controller.Invoke(*(char*)str.Span[c]))
			{
				yield return str.Slice(num, c - num);
				num = c + 1;
			}
		}
		yield return str.Slice(num);
	}

	public unsafe static System.ReadOnlySpan<char> TrimMatchingOuterQuotes(this System.ReadOnlySpan<char> input, char quote)
	{
		if (input.Length >= 2 && *(ushort*)input[0] == quote && *(ushort*)input[input.Length - 1] == quote)
		{
			return input.Slice(1, input.Length - 2);
		}
		return input;
	}

	public unsafe static ReadOnlyMemory<char> TrimMatchingQuotes(this ReadOnlyMemory<char> input, char quote)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		System.ReadOnlySpan<char> span = input.Span;
		if (span.Length >= 2 && *(ushort*)span[0] == quote && *(ushort*)span[span.Length - 1] == quote)
		{
			return input.Slice(1, span.Length - 2);
		}
		return input;
	}

	public static System.Collections.Generic.IEnumerable<ReadOnlyMemory<char>> SplitCommandLine(ReadOnlyMemory<char> commandLine)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bool inQuotes = false;
		return Enumerable.Where<ReadOnlyMemory<char>>(Enumerable.Select<ReadOnlyMemory<char>, ReadOnlyMemory<char>>(Split(commandLine, delegate(char c)
		{
			if (c == '"')
			{
				inQuotes = !inQuotes;
			}
			return !inQuotes && c == ' ';
		}), (Func<ReadOnlyMemory<char>, ReadOnlyMemory<char>>)((ReadOnlyMemory<char> arg) => MemoryExtensions.Trim(arg).TrimMatchingQuotes('"'))), (Func<ReadOnlyMemory<char>, bool>)((ReadOnlyMemory<char> arg) => arg.Length > 0));
	}
}
