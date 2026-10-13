using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Models.AppLifeCycle;

[Serializable]
public class AppLifeCycleActionFailedException : System.Exception
{
	[field: CompilerGenerated]
	public AppLifeCycleActionRecord ActionRecord
	{
		[CompilerGenerated]
		get;
	}

	public AppLifeCycleActionFailedException(AppLifeCycleActionRecord record)
		: base(FormatErrorMessage(record))
	{
		ActionRecord = VerifyIsFailed(record);
	}

	public AppLifeCycleActionFailedException(AppLifeCycleActionRecord record, System.Exception innerException)
		: base(FormatErrorMessage(record, innerException), innerException)
	{
		ActionRecord = VerifyIsFailed(record);
	}

	protected AppLifeCycleActionFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ActionRecord = (AppLifeCycleActionRecord)serializationInfo.GetValue("ActionRecord", typeof(AppLifeCycleActionRecord));
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base.GetObjectData(info, context);
		info.AddValue("ActionRecord", (object)ActionRecord);
	}

	private static string FormatErrorMessage(AppLifeCycleActionRecord record)
	{
		return "The " + record.Action.ActionName + " action failed.";
	}

	private static string FormatErrorMessage(AppLifeCycleActionRecord record, System.Exception innerException)
	{
		return "The " + record.Action.ActionName + " action failed because: " + innerException.Message;
	}

	private static AppLifeCycleActionRecord VerifyIsFailed(AppLifeCycleActionRecord record)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!record.Completion.IsFaulted)
		{
			throw new ArgumentException("Must be a faulted record.", "record");
		}
		return record;
	}
}
