using System;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class RoarCommandConnection : IRouterCommandConnection, System.IDisposable
{
	private readonly IRoarCommandConnection _connection;

	private readonly ConfigureRoarWrapper _configureWrapper;

	public IRouterConfigureCommands Configuration => _configureWrapper;

	public bool IsOpen => _connection.IsOpen;

	public RoarCommandConnection(IRoarCommandConnection connection)
	{
		_connection = connection;
		_configureWrapper = new ConfigureRoarWrapper(connection);
	}

	public void Dispose()
	{
		((System.IDisposable)_connection).Dispose();
	}

	public bool Open()
	{
		return _connection.Open();
	}

	public void Close()
	{
		_connection.Close();
	}
}
