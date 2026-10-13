using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace LauncherCoreLib.Utility;

public sealed record OsWindow
{
	private System.Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(OsWindow);
		}
	}

	public System.IntPtr WindowHandle
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private init;
	}

	public OsWindowState State
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private init;
	}

	public Rectangle Rectangle
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public static readonly OsWindow DEFAULT = new OsWindow();

	public OsWindow()
	{
		WindowHandle = System.IntPtr.Zero;
		State = OsWindowState.None;
	}

	public OsWindow(System.IntPtr handle, OsWindowState state, Rectangle rectangle)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		WindowHandle = handle;
		State = state;
		Rectangle = rectangle;
	}

	public OsWindow ChangeState(OsWindowState state)
	{
		if (state != State)
		{
			return this with
			{
				State = state
			};
		}
		return this;
	}

	public OsWindow ChangeRect(Rectangle rectangle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (rectangle != Rectangle)
		{
			return this with
			{
				Rectangle = rectangle
			};
		}
		return this;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		StringBuilder val = new StringBuilder();
		val.Append("OsWindow");
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
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append("WindowHandle = ");
		builder.Append(((object)(nint)WindowHandle).ToString());
		builder.Append(", State = ");
		builder.Append(((object)State).ToString());
		builder.Append(", Rectangle = ");
		Rectangle rectangle = Rectangle;
		builder.Append(((object)(Rectangle)(ref rectangle)).ToString());
		return true;
	}

	[CompilerGenerated]
	public bool Equals(OsWindow? other)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<System.IntPtr>.Default.Equals(WindowHandle, other.WindowHandle) && EqualityComparer<OsWindowState>.Default.Equals(State, other.State))
			{
				return EqualityComparer<Rectangle>.Default.Equals(Rectangle, other.Rectangle);
			}
			return false;
		}
		return true;
	}

	[CompilerGenerated]
	private OsWindow(OsWindow original)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		WindowHandle = original.WindowHandle;
		State = original.State;
		Rectangle = original.Rectangle;
	}
}
