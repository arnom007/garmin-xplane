namespace LauncherCoreLib.Models.AbstractUi;

public interface IUserActionInfo
{
	string ActionId { get; }

	string ActionText { get; }

	string ActionHint { get; }

	ActionStyling Style { get; }
}
