using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LOLA.Extensions;
using LOLA.Logging;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class SquawkBoard : ISquawkBoard
{
	private readonly ConcurrentHashSet<Squawk> _squawks = new ConcurrentHashSet<Squawk>();

	[CompilerGenerated]
	private EventHandler<SquawkChangeEventArgs>? m_OnBoardChange;

	public System.Collections.Generic.IReadOnlyCollection<Squawk> Squawks => _squawks;

	public event EventHandler<SquawkChangeEventArgs>? OnBoardChange
	{
		[CompilerGenerated]
		add
		{
			EventHandler<SquawkChangeEventArgs> val = this.m_OnBoardChange;
			EventHandler<SquawkChangeEventArgs> val2;
			do
			{
				val2 = val;
				EventHandler<SquawkChangeEventArgs> val3 = (EventHandler<SquawkChangeEventArgs>)(object)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler<SquawkChangeEventArgs>>(ref this.m_OnBoardChange, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<SquawkChangeEventArgs> val = this.m_OnBoardChange;
			EventHandler<SquawkChangeEventArgs> val2;
			do
			{
				val2 = val;
				EventHandler<SquawkChangeEventArgs> val3 = (EventHandler<SquawkChangeEventArgs>)(object)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler<SquawkChangeEventArgs>>(ref this.m_OnBoardChange, val3, val2);
			}
			while (val != val2);
		}
	}

	public void Add(Squawk squawk)
	{
		if (!_squawks.Contains(squawk))
		{
			Logger.Log(LogLevel.Info, "[Squawk] {0}", squawk);
			_squawks.Add(squawk);
			RaiseBoardChange(SquawkChangeEventArgs.CreateAdded(squawk.Yield()));
		}
	}

	public void Remove(Squawk squawk)
	{
		if (_squawks.Contains(squawk))
		{
			_squawks.Remove(squawk);
			RaiseBoardChange(SquawkChangeEventArgs.CreateRemoved(squawk.Yield()));
		}
	}

	public void Clear()
	{
		Squawk[] squawks = Enumerable.ToArray<Squawk>((System.Collections.Generic.IEnumerable<Squawk>)_squawks);
		_squawks.Clear();
		RaiseBoardChange(SquawkChangeEventArgs.CreateRemoved(squawks));
	}

	private void RaiseBoardChange(SquawkChangeEventArgs ca)
	{
		this.OnBoardChange?.Invoke((object)this, ca);
	}
}
