namespace LauncherCoreLib.Models.AbstractUi;

public class RootUiAliasRegistry : UiAliasRegistry, IRootUiAliasRegistry, IUiAliasRegistry, IUiAliasProvider
{
	public RootUiAliasRegistry()
	{
		base.RegistryName = "Root";
	}
}
