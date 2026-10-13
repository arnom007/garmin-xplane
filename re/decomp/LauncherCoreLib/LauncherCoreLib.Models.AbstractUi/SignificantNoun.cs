using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AbstractUi;

public class SignificantNoun
{
	[field: CompilerGenerated]
	public string Singular
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string Plural
	{
		[CompilerGenerated]
		get;
	}

	public SignificantNoun(string singular, string plural)
	{
		Singular = singular;
		Plural = plural;
	}

	public string ForCount(int count)
	{
		if (count != 1)
		{
			return Plural;
		}
		return Singular;
	}

	public virtual string ToString()
	{
		return Singular;
	}
}
