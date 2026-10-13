using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Utility;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.AppLifeCycle;

public sealed class WindowSizeAndLocation : IEquatable<WindowSizeAndLocation>
{
	[JsonProperty]
	[field: CompilerGenerated]
	public OsWindowState State
	{
		[CompilerGenerated]
		get;
	}

	[JsonProperty]
	[field: CompilerGenerated]
	public int X
	{
		[CompilerGenerated]
		get;
	}

	[JsonProperty]
	[field: CompilerGenerated]
	public int Y
	{
		[CompilerGenerated]
		get;
	}

	[JsonProperty]
	[field: CompilerGenerated]
	public int Width
	{
		[CompilerGenerated]
		get;
	}

	[JsonProperty]
	[field: CompilerGenerated]
	public int Height
	{
		[CompilerGenerated]
		get;
	}

	public WindowSizeAndLocation()
		: this(OsWindowState.None, new Rectangle(0, 0, 0, 0))
	{
	}//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public WindowSizeAndLocation(OsWindow window)
		: this(window.State, window.Rectangle)
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)


	[JsonConstructor]
	public WindowSizeAndLocation(OsWindowState state, int x, int y, int width, int height)
		: this(state, new Rectangle(x, y, width, height))
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)


	public WindowSizeAndLocation(OsWindowState state, Rectangle windowRectangle)
	{
		State = state;
		X = ((Rectangle)(ref windowRectangle)).X;
		Y = ((Rectangle)(ref windowRectangle)).Y;
		Width = ((Rectangle)(ref windowRectangle)).Width;
		Height = ((Rectangle)(ref windowRectangle)).Height;
	}

	public Rectangle ToRectangle()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Rectangle(X, Y, Width, Height);
	}

	public WindowSizeAndLocation DefaultToDimensions(int width, int height)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		WindowSizeAndLocation windowSizeAndLocation = this;
		if (windowSizeAndLocation.Width < 0)
		{
			windowSizeAndLocation = new WindowSizeAndLocation(windowSizeAndLocation.State, new Rectangle(X, Y, width, windowSizeAndLocation.Height));
		}
		if (windowSizeAndLocation.Height < 0)
		{
			windowSizeAndLocation = new WindowSizeAndLocation(windowSizeAndLocation.State, new Rectangle(X, Y, windowSizeAndLocation.Width, height));
		}
		return windowSizeAndLocation;
	}

	public WindowSizeAndLocation With(OsWindowState? state = null, int? x = null, int? y = null, int? width = null, int? height = null)
	{
		return new WindowSizeAndLocation(state ?? State, x ?? X, y ?? Y, width ?? Width, height ?? Height);
	}

	public WindowSizeAndLocation ToMaximized()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new WindowSizeAndLocation(OsWindowState.Maximized, ToRectangle());
	}

	public bool Equals(WindowSizeAndLocation? other)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (this != other)
		{
			if (other != null && State == other.State)
			{
				return ToRectangle() == other.ToRectangle();
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is WindowSizeAndLocation other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		int hashCode = ((object)State).GetHashCode();
		Rectangle val = ToRectangle();
		return hashCode ^ ((object)(Rectangle)(ref val)).GetHashCode();
	}

	public override string ToString()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} - {1}", (object)State, (object)ToRectangle());
	}
}
