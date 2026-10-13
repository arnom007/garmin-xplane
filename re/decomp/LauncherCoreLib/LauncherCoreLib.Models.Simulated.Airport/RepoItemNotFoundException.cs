using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.Simulated.Airport;

public abstract class RepoItemNotFoundException : KeyNotFoundException
{
	protected RepoItemNotFoundException(string message)
		: base(message)
	{
	}

	protected RepoItemNotFoundException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}
}
