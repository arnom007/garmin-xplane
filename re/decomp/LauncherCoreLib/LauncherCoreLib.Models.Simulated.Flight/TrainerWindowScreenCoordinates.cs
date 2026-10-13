using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LauncherCoreLib.Models.Simulated.Flight;

public record TrainerWindowScreenCoordinates(int Left, int Top, int Width, int Height)
{
	protected virtual System.Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(TrainerWindowScreenCoordinates);
		}
	}

	[CompilerGenerated]
	public override string ToString()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		StringBuilder val = new StringBuilder();
		val.Append("TrainerWindowScreenCoordinates");
		val.Append(" { ");
		if (PrintMembers(val))
		{
			val.Append(' ');
		}
		val.Append('}');
		return ((object)val).ToString();
	}

	[CompilerGenerated]
	protected virtual bool PrintMembers(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append("Left = ");
		builder.Append(((object)Left).ToString());
		builder.Append(", Top = ");
		builder.Append(((object)Top).ToString());
		builder.Append(", Width = ");
		builder.Append(((object)Width).ToString());
		builder.Append(", Height = ");
		builder.Append(((object)Height).ToString());
		return true;
	}

	[CompilerGenerated]
	public virtual bool Equals(TrainerWindowScreenCoordinates? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(Left, other.Left) && EqualityComparer<int>.Default.Equals(Top, other.Top) && EqualityComparer<int>.Default.Equals(Width, other.Width))
			{
				return EqualityComparer<int>.Default.Equals(Height, other.Height);
			}
			return false;
		}
		return true;
	}
}
