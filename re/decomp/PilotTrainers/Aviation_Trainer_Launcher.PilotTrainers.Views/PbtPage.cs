using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Aviation_Trainer_Launcher.LauncherUILib.Views;

namespace Aviation_Trainer_Launcher.PilotTrainers.Views;

public class PbtPage : UserControl, IComponentConnector
{
	internal AircraftInstallsView AircraftInstalls;

	internal Border PanelEnablement;

	internal Border PanelVisWrapper;

	internal Expander AdvisoryExpander;

	internal AdvisoryListView AdvisoryList;

	private bool _contentLoaded;

	public PbtPage()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/PilotTrainers;component/views/pbtpage.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "6.0.7.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			AircraftInstalls = (AircraftInstallsView)target;
			break;
		case 2:
			PanelEnablement = (Border)target;
			break;
		case 3:
			PanelVisWrapper = (Border)target;
			break;
		case 4:
			AdvisoryExpander = (Expander)target;
			break;
		case 5:
			AdvisoryList = (AdvisoryListView)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
