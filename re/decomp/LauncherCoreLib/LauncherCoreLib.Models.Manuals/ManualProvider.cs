using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Manuals;

internal class ManualProvider : IManualProvider
{
	private const string MANUAL_DIRECTORY_NAME = "Manuals";

	private readonly IDeploymentPathProvider _installedPath;

	public System.Collections.Generic.IEnumerable<PdfDoc> InstalledManuals => GetManuals();

	public bool IsPdfViewerInstalled
	{
		get
		{
			if (Enumerable.Any<PdfDoc>(GetManuals()))
			{
				return Enumerable.Any<PdfDoc>(GetManuals(), (Func<PdfDoc, bool>)((PdfDoc p) => p.IsDefaultAssociated));
			}
			return true;
		}
	}

	public ManualProvider(IDeploymentPathProvider deploymentPath)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_installedPath = deploymentPath ?? throw new ArgumentNullException("deploymentPath");
	}

	private System.Collections.Generic.IEnumerable<PdfDoc> GetManuals()
	{
		string text = Path.Combine(_installedPath.DeploymentPath, "Manuals");
		if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
		{
			return Enumerable.Select<string, PdfDoc>((System.Collections.Generic.IEnumerable<string>)Directory.GetFiles(text, "*.pdf", (SearchOption)0), (Func<string, PdfDoc>)((string manual_file) => new PdfDoc(Path.GetFileNameWithoutExtension(manual_file), manual_file)));
		}
		return Enumerable.Empty<PdfDoc>();
	}
}
