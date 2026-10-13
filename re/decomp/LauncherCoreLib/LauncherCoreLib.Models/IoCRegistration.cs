using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace LauncherCoreLib.Models;

public static class IoCRegistration
{
	public static void RegisterTypes(IUnityContainer container, IoCTypes types)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (container == null)
		{
			throw new ArgumentNullException("container");
		}
		if (types == null)
		{
			throw new ArgumentNullException("types");
		}
		RegisterTypeMappings(container, types.Transient(), () => new TransientLifetimeManager());
		RegisterTypeMappings(container, types.Singletons(), () => new ContainerControlledLifetimeManager());
		types.Complex(container);
	}

	public static void RegisterTypeMappings(IUnityContainer container, System.Collections.Generic.IEnumerable<TypeMapping> mappings, Func<ITypeLifetimeManager> lifetimeFactory)
	{
		System.Collections.Generic.IEnumerator<TypeMapping> enumerator = mappings.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				TypeMapping current = enumerator.Current;
				ITypeLifetimeManager lifetime = lifetimeFactory.Invoke();
				RegisterTypeMapping(container, current, lifetime);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public static void RegisterTypeMapping(IUnityContainer container, TypeMapping mapping, ITypeLifetimeManager lifetime)
	{
		container.RegisterType(mapping.Concrete, (string)null, lifetime, System.Array.Empty<InjectionMember>());
		System.Collections.Generic.IEnumerator<System.Type> enumerator = ((System.Collections.Generic.IEnumerable<System.Type>)mapping.Interfaces).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				System.Type current = enumerator.Current;
				container.RegisterType(current, mapping.Concrete);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}
}
