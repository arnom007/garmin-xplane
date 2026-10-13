using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public interface ISynchronizedJsonDataSource
{
	IObservable<JsonDataUpdate> WhenValuesAreUpdated { get; }

	System.Threading.Tasks.Task<JToken?> GetValue(string jsonPath);

	JToken? GetValueSync(string jsonPath);

	System.Threading.Tasks.Task StoreValue(string jsonPath, JToken? value);

	void StoreValueSync(string jsonPath, JToken? value);
}
