namespace LauncherCoreLib.Models.AbstractUi;

public interface ISignificantNounProvider
{
	SignificantNoun? GetNounByKeyOrNull(string key);
}
