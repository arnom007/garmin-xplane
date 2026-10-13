using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Winfs;

internal static class BatchProcessor
{
	public static IObservable<WinfsTransformReport> ProcessBatch(BatchTransformation batch, IWinfsCreationContext creationContext)
	{
		return ProcessBatch(batch, creationContext, TransformProcessor.Instance);
	}

	public static IObservable<WinfsTransformReport> ProcessBatch(BatchTransformation batch, IWinfsCreationContext creationContext, IWinfsTransformProcessor processor)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (batch == null)
		{
			throw new ArgumentNullException("batch");
		}
		if (creationContext == null)
		{
			throw new ArgumentNullException("creationContext");
		}
		if (processor == null)
		{
			throw new ArgumentNullException("processor");
		}
		return RequestBuilder.CreateCopyRequests(batch.CopyInstructionStream, creationContext).Concat<WinfsTransformRequest>(RequestBuilder.CreateLinkRequests(batch.LinkInstructionStream, creationContext)).SelectMany<WinfsTransformRequest, WinfsTransformReport>((Func<WinfsTransformRequest, System.Threading.Tasks.Task<WinfsTransformReport>>)processor.ProcessRequest);
	}
}
