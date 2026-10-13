namespace RoarClient.Net.Native.Adapters;

public class NoNativeErrors : IHumanErrorLookup
{
	public string GetHumanReadableError(ROAR_err_type roarError)
	{
		return roarError.ToString();
	}
}
