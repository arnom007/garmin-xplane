using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingIdentifier : IEquatable<SettingIdentifier>, IComparable<SettingIdentifier>
{
	[CompilerGenerated]
	private sealed class _003CScopes_003Ed__52 : System.Collections.Generic.IEnumerable<SettingIdentifier>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<SettingIdentifier>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private SettingIdentifier _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		public SettingIdentifier _003C_003E4__this;

		private int _003Ci_003E5__2;

		SettingIdentifier System.Collections.Generic.IEnumerator<SettingIdentifier>.Current
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
		public _003CScopes_003Ed__52(int _003C_003E1__state)
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
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SettingIdentifier settingIdentifier = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Ci_003E5__2 = settingIdentifier._identParts.Length - 1;
				break;
			case 1:
				_003C_003E1__state = -1;
				_003Ci_003E5__2--;
				break;
			}
			if (_003Ci_003E5__2 > 0)
			{
				_003C_003E2__current = new SettingIdentifier(settingIdentifier._identParts.RemoveRange(_003Ci_003E5__2, settingIdentifier._identParts.Length - _003Ci_003E5__2));
				_003C_003E1__state = 1;
				return true;
			}
			return false;
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
		System.Collections.Generic.IEnumerator<SettingIdentifier> System.Collections.Generic.IEnumerable<SettingIdentifier>.GetEnumerator()
		{
			_003CScopes_003Ed__52 result;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				result = this;
			}
			else
			{
				result = new _003CScopes_003Ed__52(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			return result;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<SettingIdentifier>)this).GetEnumerator();
		}
	}

	public const string TRAINER_PACKAGES_NAMESPACE = "Packages";

	public static readonly SettingIdentifier Empty;

	public static readonly SettingIdentifier Parent;

	private const string FULL_ID_REGEX = "^(((?<namespace>\\w+\\.)*(?<localId>\\w+)?)|(\\.\\.))$";

	private const string IDENTIFIER_DELIMITTER = ".";

	private const string SUB_ID_FORMAT = "\\w+";

	private const char IDENTIFIER_DELIMITTER_CHAR = '.';

	private const string CURRENT_IDENTIFIER = "";

	private static readonly Regex SUB_ID_REGEX;

	private static readonly Regex FULL_ID_PARSER;

	private static readonly char[] SPLIT_ON_PERIOD;

	private readonly ImmutableArray<string> _identParts;

	[field: CompilerGenerated]
	public string FullId
	{
		[CompilerGenerated]
		get;
	}

	public SettingIdentifier Namespace => ParseNamespace(FullId);

	public SettingIdentifier LocalId => ParseLocalId(FullId);

	public SettingIdentifier Root => ParseRoot(FullId);

	public int Depth => _identParts.Length;

	static SettingIdentifier()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		FULL_ID_PARSER = new Regex("^(((?<namespace>\\w+\\.)*(?<localId>\\w+)?)|(\\.\\.))$");
		SUB_ID_REGEX = new Regex("^\\w+$");
		SPLIT_ON_PERIOD = new char[1] { '.' };
		Empty = new SettingIdentifier();
		Parent = new SettingIdentifier("..");
	}

	public SettingIdentifier()
		: this("")
	{
	}

	public SettingIdentifier(string fullIdentifier)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!IsValidFullId(fullIdentifier))
		{
			throw new FormatException("Invalid identifier");
		}
		FullId = fullIdentifier;
		_identParts = ImmutableArray.ToImmutableArray<string>((System.Collections.Generic.IEnumerable<string>)FullId.Split(SPLIT_ON_PERIOD, (StringSplitOptions)1));
	}

	private SettingIdentifier(string[] identParts)
		: this(ImmutableArray.ToImmutableArray<string>((System.Collections.Generic.IEnumerable<string>)identParts))
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	private SettingIdentifier(ImmutableArray<string> identParts)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_identParts = identParts;
		FullId = SkipAndCombineTheRest(0);
		if (!IsValidFullId(FullId))
		{
			throw new FormatException("Invalid identifier");
		}
	}

	public static implicit operator SettingIdentifier(string identifier)
	{
		return FromNullString(identifier);
	}

	public static implicit operator string(SettingIdentifier identifier)
	{
		return identifier?.FullId;
	}

	public static implicit operator SettingIdentifier?(TrainerIdentifier identifier)
	{
		return FromMaybeInvalidTrainerIdentifier(identifier);
	}

	public static bool IsValidFullId(string fullIdentifier)
	{
		if (fullIdentifier != null)
		{
			return FULL_ID_PARSER.IsMatch(fullIdentifier);
		}
		return false;
	}

	public static bool IsValidSimulatorDefinedId(string fullIdentifier)
	{
		if (IsValidFullId(fullIdentifier))
		{
			return fullIdentifier.StartsWith("Packages.", (StringComparison)4);
		}
		return false;
	}

	public static SettingIdentifier ParseNamespace(string fullIdentifier)
	{
		int lastPeriod = GetLastPeriod(fullIdentifier);
		if (lastPeriod < 0)
		{
			return Empty;
		}
		return FromString(fullIdentifier.Substring(0, lastPeriod));
	}

	public static SettingIdentifier ParseLocalId(string fullIdentifier)
	{
		int lastPeriod = GetLastPeriod(fullIdentifier);
		return FromString((lastPeriod >= 0) ? fullIdentifier.Substring(lastPeriod + 1) : fullIdentifier);
	}

	public static SettingIdentifier ParseRoot(string fullIdentifier)
	{
		int firstPeriod = GetFirstPeriod(fullIdentifier);
		return FromString((firstPeriod >= 0) ? fullIdentifier.Substring(0, firstPeriod) : fullIdentifier);
	}

	public static SettingIdentifier CombineSubIdentifiers(params string[] subIdentifiers)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IEnumerable<string> enumerable = Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)subIdentifiers, (Func<string, bool>)((string subId) => !IsValidSubIdentifier(subId)));
		if (Enumerable.Any<string>(enumerable))
		{
			throw new FormatException(string.Join(", ", enumerable) + " are invalid sub identifiers.");
		}
		return string.Join(".", subIdentifiers);
	}

	public static SettingIdentifier? FromNullString(string? identifier)
	{
		if (identifier == null)
		{
			return null;
		}
		return new SettingIdentifier(identifier);
	}

	public static SettingIdentifier FromString(string identifier)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return FromNullString(identifier) ?? throw new ArgumentNullException("identifier");
	}

	public static SettingIdentifier Concatenate(SettingIdentifier lhs, SettingIdentifier rhs)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (lhs == Parent || rhs == Parent)
		{
			throw new NotSupportedException("For now Parent concatenation is not allowed.");
		}
		if (rhs == Empty)
		{
			return lhs;
		}
		if (lhs == Empty)
		{
			return rhs;
		}
		return new SettingIdentifier(lhs._identParts.AddRange(rhs._identParts));
	}

	public static bool operator ==(SettingIdentifier? lhs, SettingIdentifier? rhs)
	{
		return EqualityComparer<SettingIdentifier>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(SettingIdentifier? lhs, SettingIdentifier? rhs)
	{
		return !EqualityComparer<SettingIdentifier>.Default.Equals(lhs, rhs);
	}

	public static SettingIdentifier operator +(SettingIdentifier lhs, SettingIdentifier rhs)
	{
		return Concatenate(lhs, rhs);
	}

	public static string operator +(SettingIdentifier lhs, string rhs)
	{
		return lhs.FullId + rhs;
	}

	public static string operator +(string lhs, SettingIdentifier rhs)
	{
		return lhs + rhs.FullId;
	}

	public static bool IsValidSubIdentifier(string? subIdentifier)
	{
		if (subIdentifier != null)
		{
			return SUB_ID_REGEX.IsMatch(subIdentifier);
		}
		return false;
	}

	public static SettingIdentifier FromTrainerIdentifier(TrainerIdentifier identifier)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!identifier.IsValid)
		{
			throw new ArgumentException("identifier is invalid.", "identifier");
		}
		return new SettingIdentifier(identifier.IdentStringChecked);
	}

	public static SettingIdentifier? FromMaybeInvalidTrainerIdentifier(TrainerIdentifier identifier)
	{
		if (!identifier.IsValid)
		{
			return null;
		}
		return new SettingIdentifier(identifier.IdentStringChecked);
	}

	public static bool operator <(SettingIdentifier left, SettingIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) < 0;
		}
		return (object)right != null;
	}

	public static bool operator <=(SettingIdentifier left, SettingIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) <= 0;
		}
		return true;
	}

	public static bool operator >(SettingIdentifier left, SettingIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) > 0;
		}
		return false;
	}

	public static bool operator >=(SettingIdentifier left, SettingIdentifier right)
	{
		if ((object)left != null)
		{
			return left.CompareTo(right) >= 0;
		}
		return (object)right == null;
	}

	[IteratorStateMachine(typeof(_003CScopes_003Ed__52))]
	public System.Collections.Generic.IEnumerable<SettingIdentifier> Scopes()
	{
		for (int i = _identParts.Length - 1; i > 0; i--)
		{
			yield return new SettingIdentifier(_identParts.RemoveRange(i, _identParts.Length - i));
		}
	}

	public System.Collections.Generic.IEnumerable<SettingIdentifier> EnumerateParts()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ImmutableArrayExtensions.Select<string, SettingIdentifier>(_identParts, (Func<string, SettingIdentifier>)((string p) => p));
	}

	public SettingIdentifier? RelativeTo(SettingIdentifier identifier)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (identifier == null)
		{
			throw new ArgumentNullException("identifier");
		}
		if (Equals(identifier))
		{
			return Empty;
		}
		if (ImmutableArrayExtensions.SequenceEqual<string, string>(identifier._identParts, Enumerable.Take<string>((System.Collections.Generic.IEnumerable<string>)(object)_identParts, identifier._identParts.Length), (IEqualityComparer<string>)null))
		{
			return new SettingIdentifier(_identParts.RemoveRange(0, identifier._identParts.Length));
		}
		return null;
	}

	public SettingIdentifier Concatenate(params SettingIdentifier[] settingIdentifiers)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		return new SettingIdentifier(_identParts.AddRange(Enumerable.SelectMany<SettingIdentifier, string>((System.Collections.Generic.IEnumerable<SettingIdentifier>)settingIdentifiers, (Func<SettingIdentifier, System.Collections.Generic.IEnumerable<string>>)((SettingIdentifier si) => (System.Collections.Generic.IEnumerable<string>)(object)si._identParts))));
	}

	public bool Equals(SettingIdentifier? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null)
			{
				return FullId == other.FullId;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is SettingIdentifier other) || !Equals(other))
		{
			if (obj is string text)
			{
				return Equals(text);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return FullId.GetHashCode((StringComparison)4);
	}

	public override string ToString()
	{
		return FullId;
	}

	public int CompareTo(SettingIdentifier? other)
	{
		return string.CompareOrdinal(FullId, other?.FullId);
	}

	private static int GetLastPeriod(string fullIdentifier)
	{
		return fullIdentifier.LastIndexOf(".", (StringComparison)4);
	}

	private static int GetFirstPeriod(string fullIdentifier)
	{
		return fullIdentifier.IndexOf(".", (StringComparison)4);
	}

	private string SkipAndCombineTheRest(int i)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return string.Join(".", Enumerable.Skip<string>((System.Collections.Generic.IEnumerable<string>)(object)_identParts, i));
	}
}
