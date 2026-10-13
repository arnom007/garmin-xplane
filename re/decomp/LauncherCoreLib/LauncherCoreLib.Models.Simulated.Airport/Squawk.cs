using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class Squawk
{
	public static readonly Squawk Empty = new Squawk
	{
		Message = string.Empty,
		Status = SquawkStatus.Info
	};

	[field: CompilerGenerated]
	public string Message
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public SquawkStatus Status
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public IObservable<double> Progress
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	private Squawk()
	{
		Message = string.Empty;
		Progress = Observable.Empty<double>();
	}

	public static Squawk Create(string message, SquawkStatus status = SquawkStatus.Info)
	{
		return new Squawk
		{
			Message = message,
			Status = status
		};
	}

	public static Squawk CreateInfo(string format, params object[] formatArgs)
	{
		return Create(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs));
	}

	public static Squawk CreateProgress(IObservable<double> progress, string format, params object[] formatArgs)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (progress == null)
		{
			throw new ArgumentNullException("progress");
		}
		return new Squawk
		{
			Message = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs),
			Status = SquawkStatus.Info,
			Progress = progress.Replay<double>(1).RefCount()
		};
	}

	public static Squawk CreateIndeterminateProgress(string format, params object[] formatArgs)
	{
		return CreateProgress(Observable.Return(0.6).Concat<double>(Observable.Never<double>()), format, formatArgs);
	}

	public static Squawk CreateError(string format, params object[] formatArgs)
	{
		return Create(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs), SquawkStatus.Error);
	}

	public static Squawk CreateWarning(string format, params object[] formatArgs)
	{
		return Create(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs), SquawkStatus.Warning);
	}

	public virtual string ToString()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} - {1}", (object)Status, (object)Message);
	}
}
