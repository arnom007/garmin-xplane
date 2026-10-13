using System.Drawing;
using System.Windows;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public static class ScreenRectangleProvider
{
	public static GetScreenRectangle Instance => () => new Rectangle(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
}
