using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LauncherCoreLib.Models.Settings;

public abstract class SettingsFilterBase : ISettingsFilter
{
	[CompilerGenerated]
	private EventHandler? m_FilterChanged;

	public event EventHandler FilterChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_FilterChanged;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_FilterChanged, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_FilterChanged;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_FilterChanged, val3, val2);
			}
			while (val != val2);
		}
	}

	public abstract bool FilterAppliesToProvider(ISettingsProvider provider);

	public abstract bool IsSettingValid(IActiveSetting activeSetting);

	protected void RaiseFilterChanged()
	{
		EventHandler? filterChanged = this.FilterChanged;
		if (filterChanged != null)
		{
			filterChanged.Invoke((object)this, EventArgs.Empty);
		}
	}
}
