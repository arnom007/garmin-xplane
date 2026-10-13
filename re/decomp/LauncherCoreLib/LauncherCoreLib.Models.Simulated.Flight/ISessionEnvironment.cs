using System.Collections.Immutable;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISessionEnvironment
{
	ISessionInfo Inputs { get; }

	SessionData Data { get; }

	ImmutableDictionary<object, object> WorkflowAttachments { get; }
}
