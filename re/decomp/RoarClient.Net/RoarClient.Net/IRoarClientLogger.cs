namespace RoarClient.Net;

public interface IRoarClientLogger
{
	void LogInfo(string message);

	void LogError(string errorMessage);
}
