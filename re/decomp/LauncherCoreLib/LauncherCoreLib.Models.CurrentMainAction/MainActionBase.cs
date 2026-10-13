using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using LOLA.Reactive;
using LauncherCoreLib.Models.AbstractUi;

namespace LauncherCoreLib.Models.CurrentMainAction;

public abstract class MainActionBase : IMainAction, IUserAction, IUserActionInfo, INotifyPropertyChanged
{
	private string _actionText = string.Empty;

	private string _actionHint = string.Empty;

	private ActionStyling _style;

	[CompilerGenerated]
	private PropertyChangedEventHandler? m_PropertyChanged;

	public string ActionText
	{
		get
		{
			return _actionText;
		}
		protected set
		{
			_actionText = value;
			RaisePropertyChanged("ActionText");
		}
	}

	public string ActionHint
	{
		get
		{
			return _actionHint;
		}
		protected set
		{
			_actionHint = value;
			RaisePropertyChanged("ActionHint");
		}
	}

	public abstract bool CanDo { get; }

	public ActionStyling Style
	{
		get
		{
			return _style;
		}
		protected set
		{
			_style = value;
			RaisePropertyChanged("Style");
		}
	}

	[field: CompilerGenerated]
	public string ActionId
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		protected set;
	} = string.Empty;


	public event PropertyChangedEventHandler PropertyChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			PropertyChangedEventHandler val = this.m_PropertyChanged;
			PropertyChangedEventHandler val2;
			do
			{
				val2 = val;
				PropertyChangedEventHandler val3 = (PropertyChangedEventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.m_PropertyChanged, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			PropertyChangedEventHandler val = this.m_PropertyChanged;
			PropertyChangedEventHandler val2;
			do
			{
				val2 = val;
				PropertyChangedEventHandler val3 = (PropertyChangedEventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.m_PropertyChanged, val3, val2);
			}
			while (val != val2);
		}
	}

	public abstract void Do();

	public virtual IObservable<bool> WhenCanDo()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		return this.WhenPropertyChanges<bool, MainActionBase>(Expression.Lambda<Func<bool>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(MainActionBase)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), System.Array.Empty<ParameterExpression>()));
	}

	protected void RaiseCanDoChange()
	{
		RaisePropertyChanged("CanDo");
	}

	protected void RaisePropertyChanged([CallerMemberName] string? property = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		PropertyChangedEventHandler? propertyChanged = this.PropertyChanged;
		if (propertyChanged != null)
		{
			propertyChanged.Invoke((object)this, new PropertyChangedEventArgs(property));
		}
	}
}
