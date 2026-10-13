using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using LOLA.Logging;
using LOLA.Processes;
using Microsoft.Win32;

namespace LauncherCoreLib.Utility;

public class PdfDoc
{
	private enum FindExecutableErrorCodes
	{
		SE_ERR_FNF = 2,
		SE_ERR_NOASSOC = 31,
		SE_ERR_OOM = 8
	}

	private const string IE_PROG_ID = "IE.HTTP";

	public static bool BrowserCanViewPdf
	{
		get
		{
			string browser;
			return BrowserLikelySupportsPdf(out browser);
		}
	}

	[field: CompilerGenerated]
	public string DisplayName
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string FilePath
	{
		[CompilerGenerated]
		get;
	}

	public bool CanOpen
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			FileInfo val = new FileInfo(FilePath);
			if (((FileSystemInfo)val).Exists)
			{
				return val.Length > 0;
			}
			return false;
		}
	}

	[field: CompilerGenerated]
	public bool IsDefaultAssociated
	{
		[CompilerGenerated]
		get;
	}

	public PdfDoc(string displayName, string filepath)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		DisplayName = displayName;
		FilePath = filepath;
		IsDefaultAssociated = FindExecutableW(filepath, null, new StringBuilder(261)) > 32;
	}

	public void Open()
	{
		if (CanOpen)
		{
			CreateDocProcess();
		}
	}

	private static string? ClassRootValue(string programId)
	{
		//IL_0059: Expected O, but got Unknown
		if (programId == null)
		{
			return null;
		}
		string text = programId + "\\shell\\open\\command";
		try
		{
			RegistryKey val = Registry.ClassesRoot.OpenSubKey(text);
			try
			{
				if (val == null)
				{
					return null;
				}
				return val.GetValue((string)null)?.ToString();
			}
			finally
			{
				((System.IDisposable)val)?.Dispose();
			}
		}
		catch (SecurityException val2)
		{
			Logger.LogFormattedException((System.Exception)val2, "Cannot access Registry: " + text);
		}
		return null;
	}

	private static string GetBrowserProgramid()
	{
		//IL_0055: Expected O, but got Unknown
		string result = "IE.HTTP";
		try
		{
			RegistryKey val = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice");
			try
			{
				if (val != null)
				{
					object value = val.GetValue("Progid");
					if (value != null)
					{
						result = value.ToString() ?? "IE.HTTP";
					}
				}
			}
			finally
			{
				((System.IDisposable)val)?.Dispose();
			}
		}
		catch (SecurityException val2)
		{
			Logger.LogFormattedException((System.Exception)val2, "Cannot access the default browser registry. Using IE instead.");
		}
		return result;
	}

	[DllImport("shell32.dll", CharSet = 3)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	private static extern int FindExecutableW(string lpFile, string? lpDirectory, [Out] StringBuilder lpResult);

	private static bool BrowserLikelySupportsPdf(out string browser)
	{
		browser = GetBrowserProgramid();
		if (!string.IsNullOrEmpty(browser) && browser != "IE.HTTP")
		{
			return ClassRootValue(browser) != null;
		}
		return false;
	}

	private ProcessSession CreateDocProcess()
	{
		if (!IsDefaultAssociated && BrowserLikelySupportsPdf(out string browser))
		{
			return CreateProcessForDefaultBrowser(browser) ?? CreateProcessThroughExplorer();
		}
		return CreateProcessThroughExplorer();
	}

	private ProcessSession CreateProcessThroughExplorer()
	{
		Logger.Log(LogLevel.Info, "Opening {0} with explorer.", DisplayName);
		return ProcessSession.ForkFromCurrentDirectory("explorer.exe", CreateDocArg());
	}

	private ProcessSession? CreateProcessForDefaultBrowser(string browser)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		string text = ClassRootValue(browser);
		if (text != null)
		{
			Logger.Log(LogLevel.Info, "Opening {0} with {1}", DisplayName, browser);
			string text2 = text.ToUpper(CultureInfo.InvariantCulture).Replace("\"", string.Empty, (StringComparison)4);
			if (!text2.EndsWith(".EXE", (StringComparison)4))
			{
				text2 = text2.Substring(0, text2.LastIndexOf(".EXE", (StringComparison)4) + ".EXE".Length);
				return ProcessSession.ForkFromCurrentDirectory(((FileSystemInfo)new FileInfo(text2)).FullName, CreateDocArg());
			}
		}
		return null;
	}

	private string CreateDocArg()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "\"{0}\"", (object)FilePath);
	}
}
