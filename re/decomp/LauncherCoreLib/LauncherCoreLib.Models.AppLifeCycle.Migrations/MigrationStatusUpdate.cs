using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public sealed class MigrationStatusUpdate : IEquatable<MigrationStatusUpdate>
{
	[field: CompilerGenerated]
	public string Message
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int StepsDone
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public int TotalSteps
	{
		[CompilerGenerated]
		get;
	}

	public double PercentDone => (double)StepsDone / (double)TotalSteps;

	public MigrationStatusUpdate(string? message = null, int stepsDone = 0, int totalSteps = 1)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (stepsDone < 0)
		{
			throw new ArgumentOutOfRangeException("stepsDone", "stepsDone must be greater or equal to 0.");
		}
		if (totalSteps < 1)
		{
			throw new ArgumentOutOfRangeException("totalSteps", "totalSteps must be greater than 0.");
		}
		Message = message ?? string.Empty;
		StepsDone = stepsDone;
		TotalSteps = totalSteps;
	}

	public MigrationStatusUpdate IncrementStep()
	{
		return new MigrationStatusUpdate(Message, StepsDone + 1, TotalSteps);
	}

	public MigrationStatusUpdate ChangeMessage(string message)
	{
		if (message != Message)
		{
			return new MigrationStatusUpdate(message, StepsDone, TotalSteps);
		}
		return this;
	}

	public MigrationStatusUpdate ChangeMessage(string format, params object[] formatArgs)
	{
		return ChangeMessage(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs));
	}

	public MigrationStatusUpdate SetTotal(int totalSteps)
	{
		if (totalSteps != TotalSteps)
		{
			return new MigrationStatusUpdate(Message, StepsDone, totalSteps);
		}
		return this;
	}

	public bool Equals(MigrationStatusUpdate? other)
	{
		if (this != other)
		{
			if (other != null && Message == other.Message && StepsDone == other.StepsDone)
			{
				return TotalSteps == other.TotalSteps;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is MigrationStatusUpdate other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<int, int, string>(StepsDone, TotalSteps, Message);
	}
}
