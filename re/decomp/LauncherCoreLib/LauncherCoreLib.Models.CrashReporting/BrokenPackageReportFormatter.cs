using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LOLA.Extensions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.CrashReporting;

public class BrokenPackageReportFormatter : IBrokenPackageReportFormatter
{
	[CompilerGenerated]
	private sealed class _003CAppendErrorInformation_003Ed__1 : System.Collections.Generic.IEnumerable<bool>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<bool>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private bool _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private StringBuilder sb;

		public StringBuilder _003C_003E3__sb;

		private IGrouping<string, TrainerPackageLoadException> errors;

		public IGrouping<string, TrainerPackageLoadException> _003C_003E3__errors;

		private ITrainerBaseInfo _003Cbi_003E5__2;

		private System.Collections.Generic.IEnumerator<global::_003C_003Ef__AnonymousType0<TrainerPackageLoadException, int>> _003C_003E7__wrap2;

		bool System.Collections.Generic.IEnumerator<bool>.Current
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
		public _003CAppendErrorInformation_003Ed__1(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 6)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
		}

		private bool MoveNext()
		{
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E2__current = AppendFormattedLine(sb, "[Package]");
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					_003C_003E2__current = AppendField(sb, "Location", errors.Key);
					_003C_003E1__state = 2;
					return true;
				case 2:
				{
					_003C_003E1__state = -1;
					TrainerPackageLoadException ex = Enumerable.FirstOrDefault<TrainerPackageLoadException>((System.Collections.Generic.IEnumerable<TrainerPackageLoadException>)errors, (Func<TrainerPackageLoadException, bool>)((TrainerPackageLoadException e) => e.PartialPackage != null && e.PartialPackage.BaseInfo != null));
					if (ex != null)
					{
						_003Cbi_003E5__2 = ex.PartialPackage?.BaseInfo;
						if (_003Cbi_003E5__2 != null)
						{
							_003C_003E2__current = AppendField(sb, "Family", _003Cbi_003E5__2.FamilyId);
							_003C_003E1__state = 3;
							return true;
						}
						goto IL_0197;
					}
					goto IL_019e;
				}
				case 3:
					_003C_003E1__state = -1;
					_003C_003E2__current = AppendField<Version>(sb, "Binary Version", _003Cbi_003E5__2.BinaryVersion);
					_003C_003E1__state = 4;
					return true;
				case 4:
					_003C_003E1__state = -1;
					_003C_003E2__current = AppendField<DateTimeOffset>(sb, "Published At", _003Cbi_003E5__2.PublishedAt);
					_003C_003E1__state = 5;
					return true;
				case 5:
					_003C_003E1__state = -1;
					goto IL_0197;
				case 6:
					_003C_003E1__state = -3;
					goto IL_023b;
				case 7:
					{
						_003C_003E1__state = -1;
						return false;
					}
					IL_0197:
					_003Cbi_003E5__2 = null;
					goto IL_019e;
					IL_019e:
					_003C_003E7__wrap2 = Enumerable.Select((System.Collections.Generic.IEnumerable<TrainerPackageLoadException>)errors, (TrainerPackageLoadException e, int i) => new
					{
						Error = e,
						Counter = i + 1
					}).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_023b;
					IL_023b:
					if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
					{
						var current = _003C_003E7__wrap2.Current;
						_003C_003E2__current = AppendField(sb, "Error_" + current.Counter, Environment.NewLine + current.Error.FormatExceptionTree());
						_003C_003E1__state = 6;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap2 = null;
					_003C_003E2__current = AppendLine(sb);
					_003C_003E1__state = 7;
					return true;
				}
			}
			catch
			{
				//try-fault
				((System.IDisposable)this).Dispose();
				throw;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2 != null)
			{
				((System.IDisposable)_003C_003E7__wrap2).Dispose();
			}
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<bool> System.Collections.Generic.IEnumerable<bool>.GetEnumerator()
		{
			_003CAppendErrorInformation_003Ed__1 _003CAppendErrorInformation_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CAppendErrorInformation_003Ed__ = this;
			}
			else
			{
				_003CAppendErrorInformation_003Ed__ = new _003CAppendErrorInformation_003Ed__1(0);
			}
			_003CAppendErrorInformation_003Ed__.sb = _003C_003E3__sb;
			_003CAppendErrorInformation_003Ed__.errors = _003C_003E3__errors;
			return _003CAppendErrorInformation_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<bool>)this).GetEnumerator();
		}
	}

	public string FormatFeedbackAttachment(System.Collections.Generic.IEnumerable<TrainerPackageLoadException> errors, int characterLimit)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		StringBuilder sb = new StringBuilder(characterLimit, characterLimit);
		sb.AppendLine("[Package Errors]");
		Enumerable.TakeWhile<bool>(Enumerable.SelectMany<IGrouping<string, TrainerPackageLoadException>, bool>(Enumerable.GroupBy<TrainerPackageLoadException, string>(errors, (Func<TrainerPackageLoadException, string>)((TrainerPackageLoadException error) => error.PackagePath)), (Func<IGrouping<string, TrainerPackageLoadException>, System.Collections.Generic.IEnumerable<bool>>)((IGrouping<string, TrainerPackageLoadException> errorsByPackagePath) => AppendErrorInformation(sb, errorsByPackagePath))), (Func<bool, bool>)((bool appendSucceeded) => appendSucceeded)).Execute();
		return ((object)sb).ToString();
	}

	[IteratorStateMachine(typeof(_003CAppendErrorInformation_003Ed__1))]
	private static System.Collections.Generic.IEnumerable<bool> AppendErrorInformation(StringBuilder sb, IGrouping<string, TrainerPackageLoadException> errors)
	{
		yield return AppendFormattedLine(sb, "[Package]");
		yield return AppendField(sb, "Location", errors.Key);
		TrainerPackageLoadException ex = Enumerable.FirstOrDefault<TrainerPackageLoadException>((System.Collections.Generic.IEnumerable<TrainerPackageLoadException>)errors, (Func<TrainerPackageLoadException, bool>)((TrainerPackageLoadException e) => e.PartialPackage != null && e.PartialPackage.BaseInfo != null));
		if (ex != null)
		{
			ITrainerBaseInfo bi = ex.PartialPackage?.BaseInfo;
			if (bi != null)
			{
				yield return AppendField(sb, "Family", bi.FamilyId);
				yield return AppendField<Version>(sb, "Binary Version", bi.BinaryVersion);
				yield return AppendField<DateTimeOffset>(sb, "Published At", bi.PublishedAt);
			}
		}
		var enumerator = Enumerable.Select((System.Collections.Generic.IEnumerable<TrainerPackageLoadException>)errors, (TrainerPackageLoadException e, int i) => new
		{
			Error = e,
			Counter = i + 1
		}).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				var current = enumerator.Current;
				yield return AppendField(sb, "Error_" + current.Counter, Environment.NewLine + current.Error.FormatExceptionTree());
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		yield return AppendLine(sb);
	}

	private static bool AppendLine(StringBuilder sb)
	{
		return AppendFormattedLine(sb, string.Empty);
	}

	private static bool AppendFormattedLine(StringBuilder sb, string format, params object[] formatArgs)
	{
		string text = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs);
		if (MessageFits(sb, text))
		{
			sb.AppendLine(text);
			return true;
		}
		if (MessageFits(sb, "--Report truncated by limit--"))
		{
			return AppendFormattedLine(sb, "--Report truncated by limit--");
		}
		return false;
	}

	private static bool MessageFits(StringBuilder sb, string message)
	{
		return sb.MaxCapacity - sb.Length > message.Length + 1;
	}

	private static bool AppendField<T>(StringBuilder sb, string fieldName, T fieldValue)
	{
		return AppendField(sb, fieldName, ((object)fieldValue)?.ToString());
	}

	private static bool AppendField(StringBuilder sb, string fieldName, string? fieldValue)
	{
		return AppendFormattedLine(sb, "{0}: {1}", fieldName, fieldValue ?? "<NULL>");
	}
}
