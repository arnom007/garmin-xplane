using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public interface IReadOnlyJsonDataSource
{
	bool IsPathValid(string jsonPath);

	JToken CopyData(string jsonPath);

	bool IsTrackingSameSource(IReadOnlyJsonDataSource otherSource);
}
public interface IReadOnlyJsonDataSource<TExpectedType> : IReadOnlyJsonDataSource
{
}
