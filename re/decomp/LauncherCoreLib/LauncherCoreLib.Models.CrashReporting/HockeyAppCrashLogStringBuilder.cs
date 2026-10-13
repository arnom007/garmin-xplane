using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace LauncherCoreLib.Models.CrashReporting;

internal class HockeyAppCrashLogStringBuilder
{
	public const char HOCKEY_CRASH_LOG_LINE_ENDING = '\n';

	[field: CompilerGenerated]
	protected StringBuilder Builder
	{
		[CompilerGenerated]
		get;
	} = new StringBuilder();


	public HockeyAppCrashLogStringBuilder AppendLine()
	{
		Builder.Append('\n');
		return this;
	}

	public HockeyAppCrashLogStringBuilder AppendFormattedLine(string formatString, params object[] args)
	{
		Builder.Append(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, formatString, args));
		AppendLine();
		return this;
	}

	public virtual string ToString()
	{
		return ((object)Builder).ToString();
	}
}
