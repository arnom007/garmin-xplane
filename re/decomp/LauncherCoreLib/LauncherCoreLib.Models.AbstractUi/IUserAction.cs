namespace LauncherCoreLib.Models.AbstractUi;

public interface IUserAction : IUserActionInfo
{
	bool CanDo { get; }

	void Do();
}
