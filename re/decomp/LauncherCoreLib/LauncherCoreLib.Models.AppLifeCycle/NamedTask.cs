using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public class NamedTask
{
	private static readonly Lazy<FieldInfo?> DELEGATE_FIELD_INFO = new Lazy<FieldInfo>((Func<FieldInfo>)(() => typeof(System.Threading.Tasks.Task).GetField("m_action", (BindingFlags)36)));

	[field: CompilerGenerated]
	public string Name
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Threading.Tasks.Task Task
	{
		[CompilerGenerated]
		get;
	}

	public NamedTask(System.Threading.Tasks.Task task, string? name = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Task = task ?? throw new ArgumentNullException("task");
		Name = name ?? GetTaskMethodName(task).Item2;
	}

	public static ValueTuple<bool, string> GetTaskMethodName(System.Threading.Tasks.Task task)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		FieldInfo value = DELEGATE_FIELD_INFO.Value;
		if (value == (FieldInfo)null)
		{
			return new ValueTuple<bool, string>(false, "Unknown - Task does not have m_action field.");
		}
		if (!(value.GetValue((object)task) is System.Delegate @delegate))
		{
			if (task.IsCompleted)
			{
				return new ValueTuple<bool, string>(false, "Unknown - Task method completed.");
			}
			return new ValueTuple<bool, string>(false, "Unknown - Task delegate was null.");
		}
		string text = ((MemberInfo)@delegate.Method).DeclaringType?.FullName;
		if (text != null)
		{
			text += ".";
		}
		if (text == null)
		{
			text = string.Empty;
		}
		return new ValueTuple<bool, string>(true, text + ((MemberInfo)@delegate.Method).Name);
	}

	public static ValueTuple<bool, string> GetTaskMethodName(Func<System.Threading.Tasks.Task> startTask)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		string text = ((MemberInfo)((System.Delegate)(object)startTask).Method).Name ?? ((object)startTask).ToString();
		return new ValueTuple<bool, string>(!string.IsNullOrEmpty(text), text ?? "Task method name could not be determined");
	}

	public virtual string ToString()
	{
		return Name;
	}
}
