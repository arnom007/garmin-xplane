using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LauncherCoreLib.Models.Simulated.Flight;

public readonly record struct ExeCliArgs(IImmutableList<string> Arguments) : System.Collections.Generic.IEnumerable<string>, System.Collections.IEnumerable
{
	public ExeCliArgs(IImmutableList<string> Arguments)
	{
		this.Arguments = Arguments;
	}

	public ExeCliArgs(System.Collections.Generic.IEnumerable<string> arguments)
		: this((IImmutableList<string>)(object)ImmutableArray.ToImmutableArray<string>(arguments))
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public System.Collections.Generic.IEnumerator<string> GetEnumerator()
	{
		return ((System.Collections.Generic.IEnumerable<string>)Arguments).GetEnumerator();
	}

	public override int GetHashCode()
	{
		return Enumerable.Aggregate<string, int>((System.Collections.Generic.IEnumerable<string>)Arguments, 0, (Func<int, string, int>)((int p, string a) => a.GetHashCode((StringComparison)4) ^ p));
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return (System.Collections.IEnumerator)GetEnumerator();
	}

	[CompilerGenerated]
	public override string ToString()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		StringBuilder val = new StringBuilder();
		val.Append("ExeCliArgs");
		val.Append(" { ");
		if (PrintMembers(val))
		{
			val.Append(' ');
		}
		val.Append('}');
		return ((object)val).ToString();
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Arguments = ");
		builder.Append((object)Arguments);
		return true;
	}
}
