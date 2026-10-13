using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LOLA.Extensions;
using LauncherCoreLib.Models.Settings;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public readonly struct AircraftIdentifier : IEquatable<AircraftIdentifier>, IEquatable<TrainerIdentifier>, IComparable<AircraftIdentifier>
{
	public static readonly string TAIL_NUMBER_CHARACTERS = new string(Enumerable.ToArray<char>(Enumerable.Select<int, char>(Enumerable.Concat<int>(Enumerable.Range(49, 9), Enumerable.Range(65, 26)), (Func<int, char>)((int i) => (char)i))));

	private const int MAX_TAIL_NUMBER_LENGTH = 6;

	private readonly TrainerIdentifier _identifier;

	[field: CompilerGenerated]
	public static AircraftIdentifier Default
	{
		[CompilerGenerated]
		get;
	}

	public bool IsValid => this != Default;

	public string? IdentString => _identifier.IdentString;

	public string IdentStringChecked => _identifier.IdentStringChecked;

	public AircraftIdentifier(TrainerIdentifier identifier)
	{
		_identifier = identifier;
	}

	public static implicit operator string?(AircraftIdentifier ai)
	{
		return ai.IdentString;
	}

	public static implicit operator TrainerIdentifier(AircraftIdentifier ai)
	{
		return ai.ToTrainerIdentifier();
	}

	public static implicit operator SettingIdentifier(AircraftIdentifier ai)
	{
		return ai.ToSettingIdentifier();
	}

	[Obsolete("Dangerous please review usage of this conversion.")]
	public static explicit operator AircraftIdentifier(TrainerIdentifier ti)
	{
		return new AircraftIdentifier(ti);
	}

	public static explicit operator AircraftIdentifier(string identString)
	{
		return new AircraftIdentifier(identString);
	}

	public static bool operator ==(AircraftIdentifier lhs, AircraftIdentifier rhs)
	{
		return EqualityComparer<AircraftIdentifier>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(AircraftIdentifier lhs, AircraftIdentifier rhs)
	{
		return !EqualityComparer<AircraftIdentifier>.Default.Equals(lhs, rhs);
	}

	public static bool IsValidIdentifier(string id)
	{
		return TrainerIdentifier.IsValidIdentifier(id);
	}

	public static AircraftIdentifier FromTrainerIdentifier(TrainerIdentifier indentifier)
	{
		return new AircraftIdentifier(indentifier);
	}

	public static AircraftIdentifier FromString(string identString)
	{
		return FromTrainerIdentifier(new TrainerIdentifier(identString));
	}

	public static System.Collections.Generic.IEnumerable<AircraftIdentifier> GenerateIdentifiers(AirframeCategoryClassType airframeCategoryClassType, string prefix)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (prefix == null)
		{
			throw new ArgumentNullException("prefix");
		}
		if (prefix.Length < 1 || prefix.Length > 3)
		{
			throw new ArgumentException("prefix must be between 1 and 3 characters.");
		}
		StringBuilder sb = new StringBuilder(6, 6);
		sb.Append(prefix);
		sb.Append(AirframeLetters(airframeCategoryClassType));
		int gapLength = sb.Capacity - sb.Length;
		int startIndex = sb.Length;
		for (int i = 0; i < gapLength; i++)
		{
			sb.Append('-');
		}
		return Generator();
		[IteratorStateMachine(typeof(_003C_003Ec__DisplayClass23_0._003C_003CGenerateIdentifiers_003Eg__Generator_007C0_003Ed))]
		System.Collections.Generic.IEnumerable<AircraftIdentifier> Generator()
		{
			System.Collections.Generic.IEnumerator<string> enumerator = AvailableSuffixNumbers(gapLength).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					string current = enumerator.Current;
					sb.Remove(startIndex, gapLength);
					sb.Append(current);
					yield return new AircraftIdentifier(((object)sb).ToString());
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
	}

	public static bool operator <(AircraftIdentifier left, AircraftIdentifier right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator <=(AircraftIdentifier left, AircraftIdentifier right)
	{
		return left.CompareTo(right) <= 0;
	}

	public static bool operator >(AircraftIdentifier left, AircraftIdentifier right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator >=(AircraftIdentifier left, AircraftIdentifier right)
	{
		return left.CompareTo(right) >= 0;
	}

	public TrainerIdentifier ToTrainerIdentifier()
	{
		return _identifier;
	}

	public SettingIdentifier ToSettingIdentifier()
	{
		return SettingIdentifier.FromString(IdentStringChecked);
	}

	public bool Equals(AircraftIdentifier other)
	{
		return _identifier == other._identifier;
	}

	public bool Equals(TrainerIdentifier other)
	{
		return _identifier == other;
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is AircraftIdentifier other) || !Equals(other))
		{
			if (obj is TrainerIdentifier other2)
			{
				return Equals(other2);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return ((object)_identifier).GetHashCode();
	}

	public int CompareTo(AircraftIdentifier other)
	{
		return _identifier.CompareTo(other._identifier);
	}

	public override string ToString()
	{
		return IdentString ?? "<INVALID>";
	}

	public AircraftIdentifier ThrowIfDefault(System.Exception? error)
	{
		System.Exception error2 = error;
		return ThrowIfDefault((error2 != null) ? ((Func<System.Exception>)(() => error2)) : null);
	}

	public AircraftIdentifier ThrowIfDefault(Func<System.Exception>? errorFactory = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (!IsValid)
		{
			System.Exception ex = (System.Exception)new InvalidOperationException("Default Aircraft is not valid here.");
			if (errorFactory != null)
			{
				System.Exception ex2 = errorFactory.Invoke();
				if (ex2 != null)
				{
					ex = ex2;
				}
			}
			throw ex;
		}
		return this;
	}

	private static string AirframeLetters(AirframeCategoryClassType airframeCCT)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		return airframeCCT switch
		{
			AirframeCategoryClassType.Unconfigured => "UNC", 
			AirframeCategoryClassType.SingleEngine => "SEP", 
			AirframeCategoryClassType.MultiEngine => "MEP", 
			AirframeCategoryClassType.Rotorcraft => "ROT", 
			AirframeCategoryClassType.SingleTurboProp => "STP", 
			AirframeCategoryClassType.MultiTurboProp => "MTP", 
			AirframeCategoryClassType.MultiTurboFan => "MTF", 
			_ => throw new NotSupportedException(), 
		};
	}

	private static System.Collections.Generic.IEnumerable<string> AvailableSuffixNumbers(int length)
	{
		return GenerateStringsOfLength(length);
	}

	private static System.Collections.Generic.IEnumerable<string> GenerateStringsOfLength(int length, System.Collections.Generic.IEnumerable<string>? builtUpStrings = null)
	{
		builtUpStrings = builtUpStrings ?? string.Empty.Yield();
		if (length <= 0)
		{
			return builtUpStrings;
		}
		return GenerateStringsOfLength(length - 1, AppendAllPossibleCharactersToEveryPossibleString(builtUpStrings));
	}

	private static System.Collections.Generic.IEnumerable<string> AppendAllPossibleCharactersToEveryPossibleString(System.Collections.Generic.IEnumerable<string> builtUpStrings)
	{
		return Enumerable.SelectMany<string, string>(builtUpStrings, (Func<string, System.Collections.Generic.IEnumerable<string>>)((string s) => Enumerable.Select<char, string>((System.Collections.Generic.IEnumerable<char>)TAIL_NUMBER_CHARACTERS, (Func<char, string>)((char c) => s + c))));
	}
}
