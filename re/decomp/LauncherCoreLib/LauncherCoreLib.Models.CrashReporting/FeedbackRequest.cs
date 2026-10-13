using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;

namespace LauncherCoreLib.Models.CrashReporting;

public sealed class FeedbackRequest
{
	[field: CompilerGenerated]
	public string FeedbackText
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string Subject
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<CrashReportStreamArgument> Attachments
	{
		[CompilerGenerated]
		get;
	}

	public FeedbackRequest(string subject, string feedbackText, System.Collections.Generic.IEnumerable<CrashReportStreamArgument>? attachments = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(feedbackText))
		{
			throw new ArgumentNullException("feedbackText");
		}
		if (string.IsNullOrWhiteSpace(feedbackText))
		{
			throw new FormatException("feedbackText cannot be only whitespace.");
		}
		FeedbackText = feedbackText.Replace(Environment.NewLine, "\n", (StringComparison)4);
		Subject = subject;
		Attachments = (System.Collections.Generic.IReadOnlyList<CrashReportStreamArgument>)(((attachments != null) ? Enumerable.ToList<CrashReportStreamArgument>(attachments) : null) ?? new List<CrashReportStreamArgument>()).AsReadOnly();
	}

	public FeedbackRequest AddAttachment(CrashReportStreamArgument attachment)
	{
		return new FeedbackRequest(Subject, FeedbackText, Enumerable.Concat<CrashReportStreamArgument>((System.Collections.Generic.IEnumerable<CrashReportStreamArgument>)Attachments, attachment.Yield()));
	}
}
