using System.Collections.Generic;
using System.Linq;
using Unity;

namespace LauncherCoreLib.Models;

public class IoCTypesBase : IoCTypes
{
	public virtual System.Collections.Generic.IEnumerable<TypeMapping> Transient()
	{
		return Enumerable.Empty<TypeMapping>();
	}

	public virtual System.Collections.Generic.IEnumerable<TypeMapping> Singletons()
	{
		return Enumerable.Empty<TypeMapping>();
	}

	public virtual void Complex(IUnityContainer container)
	{
	}
}
