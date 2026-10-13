using System;
using LauncherCoreLib.Models.AbstractUi;

namespace LauncherCoreLib.Models.CurrentMainAction;

public class MainActionUserActionWrapper : MainActionBase
{
	private readonly IUserAction _action;

	public override bool CanDo => _action.CanDo;

	public MainActionUserActionWrapper(IUserAction userAction)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_action = userAction ?? throw new ArgumentNullException("userAction");
		base.ActionText = userAction.ActionText;
		base.ActionHint = userAction.ActionHint;
		base.Style = userAction.Style;
		base.ActionId = userAction.ActionId;
	}

	public override void Do()
	{
		_action.Do();
	}
}
