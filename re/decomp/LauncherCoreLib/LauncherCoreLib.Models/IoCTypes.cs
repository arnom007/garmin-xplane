using System.Collections.Generic;
using Unity;

namespace LauncherCoreLib.Models;

public interface IoCTypes
{
	System.Collections.Generic.IEnumerable<TypeMapping> Transient();

	System.Collections.Generic.IEnumerable<TypeMapping> Singletons();

	void Complex(IUnityContainer container);
}
