using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal sealed class DefaultSessionEnvironment : ISessionEnvironment
{
	[field: CompilerGenerated]
	public ISessionInfo Inputs
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public SessionData Data
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ImmutableDictionary<object, object> WorkflowAttachments
	{
		[CompilerGenerated]
		get;
	}

	public DefaultSessionEnvironment(ISessionInfo info)
		: this(info, new SessionData(), ImmutableDictionary<object, object>.Empty)
	{
	}

	public DefaultSessionEnvironment(ISessionInfo info, SessionData startingData, ImmutableDictionary<object, object> attachment)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Inputs = info ?? throw new ArgumentNullException("info");
		Data = startingData ?? new SessionData();
		WorkflowAttachments = attachment ?? ImmutableDictionary<object, object>.Empty;
	}
}
