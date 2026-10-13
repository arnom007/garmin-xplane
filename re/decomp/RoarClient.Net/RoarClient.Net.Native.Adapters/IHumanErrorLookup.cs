namespace RoarClient.Net.Native.Adapters;

public interface IHumanErrorLookup
{
	string GetHumanReadableError(ROAR_err_type roarError);
}
