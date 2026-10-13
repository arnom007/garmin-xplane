using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AbstractUi;

public sealed class UserAction : IUserAction, IUserActionInfo
{
	private readonly Action? _action;

	private bool _canInvokeOverride;

	[field: CompilerGenerated]
	public string ActionId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string ActionText
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string ActionHint
	{
		[CompilerGenerated]
		get;
	}

	public bool CanDo
	{
		get
		{
			if (_action != null)
			{
				return _canInvokeOverride;
			}
			return false;
		}
	}

	[field: CompilerGenerated]
	public ActionStyling Style
	{
		[CompilerGenerated]
		get;
	}

	public UserAction(string text, string hint, Action? action = null, ActionStyling style = ActionStyling.Default, string? actionId = null)
	{
		ActionText = text;
		ActionHint = hint;
		_action = action;
		Style = style;
		_canInvokeOverride = action != null;
		ActionId = actionId ?? "<NO_ID>";
	}

	public static UserAction DecorateAction(IUserAction action, Action<Action> decorateAction)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		Action<Action> decorateAction2 = decorateAction;
		IUserAction action2 = action;
		return new UserAction(action2.ActionText, action2.ActionHint, (Action)delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			Action<Action> obj = decorateAction2;
			IUserAction userAction = action2;
			obj.Invoke(new Action(userAction.Do));
		}, action2.Style, action2.ActionId);
	}

	public UserAction ToCanDoOverride(bool canDo)
	{
		if (canDo == CanDo)
		{
			return this;
		}
		return new UserAction(ActionText, ActionHint, _action, Style, ActionId)
		{
			_canInvokeOverride = canDo
		};
	}

	public void Do()
	{
		if (CanDo)
		{
			_action.Invoke();
		}
	}

	public string ToString()
	{
		return ActionId + " - " + ActionText;
	}
}
