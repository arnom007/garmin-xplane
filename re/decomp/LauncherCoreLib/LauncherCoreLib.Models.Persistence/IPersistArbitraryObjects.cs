using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Models.Persistence;

public interface IPersistArbitraryObjects
{
	System.Threading.Tasks.Task Commit<TPersistenceModel>(TPersistenceModel persistenceModel, string path);

	System.Threading.Tasks.Task<TPersistenceModel> Load<TPersistenceModel>(string path);

	System.Threading.Tasks.Task<JToken> Load(string path);

	TPersistenceModel LoadSync<TPersistenceModel>(string path);
}
