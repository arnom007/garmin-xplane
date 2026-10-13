using System.Drawing;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.AppLifeCycle;

public class DefaultWindowSizeProvider : IDefaultWindowSizeProvider
{
	[field: CompilerGenerated]
	public WindowSizeAndLocation DefaultSize
	{
		[CompilerGenerated]
		get;
	}

	public DefaultWindowSizeProvider(int width, int height)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		DefaultSize = new WindowSizeAndLocation(OsWindowState.None, new Rectangle(0, 0, width, height));
	}
}
