using System.ComponentModel;
using LauncherCoreLib.Models.AbstractUi;

namespace LauncherCoreLib.Models.CurrentMainAction;

public interface IMainAction : IUserAction, IUserActionInfo, INotifyPropertyChanged
{
}
