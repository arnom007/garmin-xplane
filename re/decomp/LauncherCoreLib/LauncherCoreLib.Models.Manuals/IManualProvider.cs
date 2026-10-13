using System.Collections.Generic;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Manuals;

public interface IManualProvider
{
	bool IsPdfViewerInstalled { get; }

	System.Collections.Generic.IEnumerable<PdfDoc> InstalledManuals { get; }
}
