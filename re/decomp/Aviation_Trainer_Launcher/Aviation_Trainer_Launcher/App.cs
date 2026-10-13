using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace Aviation_Trainer_Launcher;

public class App : TrainerLauncherApp
{
	private bool _contentLoaded;

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	public void InitializeComponent()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri val = new Uri("/Aviation_Trainer_Launcher;component/app.xaml", (UriKind)2);
			Application.LoadComponent((object)this, val);
		}
	}

	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	public static void Main()
	{
		App app = new App();
		app.InitializeComponent();
		((Application)app).Run();
	}
}
