namespace LauncherCoreLib.Models.AbstractUi;

public interface IUiAliasRegistry : IUiAliasProvider
{
	UiAlias AddorUpdateAlias(UiAlias uiAlias);
}
