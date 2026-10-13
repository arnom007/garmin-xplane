using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Aviation_Trainer_Launcher.LauncherUILib.Views;

namespace Aviation_Trainer_Launcher.Views;

public class MainWindow : Window, IComponentConnector
{
	internal ModalMigrationOverlayView MigrationOverlay;

	internal WizardDialogHostView WizardOverlay;

	internal WindowClosingBinderHost WindowClosingHost;

	private bool _contentLoaded;

	public MainWindow()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	public void InitializeComponent()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri val = new Uri("/Aviation_Trainer_Launcher;component/views/mainwindow.xaml", (UriKind)2);
			Application.LoadComponent((object)this, val);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			MigrationOverlay = (ModalMigrationOverlayView)target;
			break;
		case 2:
			WizardOverlay = (WizardDialogHostView)target;
			break;
		case 3:
			WindowClosingHost = (WindowClosingBinderHost)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
