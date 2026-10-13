namespace DataRouterMatrix;

public interface IDataRouterAssemblyTypes
{
	IDataRouterAssemblyProvider Routers { get; }

	IDataRouterAssemblyProvider Clients { get; }
}
