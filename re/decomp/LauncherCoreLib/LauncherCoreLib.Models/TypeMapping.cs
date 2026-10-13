using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using LOLA.Extensions;

namespace LauncherCoreLib.Models;

public sealed class TypeMapping : IEquatable<TypeMapping>
{
	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyCollection<System.Type> Interfaces
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Type Concrete
	{
		[CompilerGenerated]
		get;
	}

	private TypeMapping(System.Collections.Generic.IReadOnlyCollection<System.Type> interfaces, System.Type concrete)
	{
		Interfaces = interfaces;
		Concrete = concrete;
	}

	public static TypeMapping Create<TInterface, TConcrete>() where TConcrete : TInterface
	{
		return new TypeMapping((System.Collections.Generic.IReadOnlyCollection<System.Type>)(object)new System.Type[1] { typeof(TInterface) }, typeof(TConcrete));
	}

	public static TypeMapping Create<TConcrete>(params System.Type[] interfaces)
	{
		return new TypeMapping((System.Collections.Generic.IReadOnlyCollection<System.Type>)(object)Exceptions.ThrowIfNull(interfaces, "interfaces"), typeof(TConcrete));
	}

	public bool Equals(TypeMapping? other)
	{
		if (this != other)
		{
			if (other != null && Concrete == other.Concrete)
			{
				return ((System.Collections.Generic.IEnumerable<System.Type>)Interfaces).SetEquals((System.Collections.Generic.IEnumerable<System.Type>)other.Interfaces);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TypeMapping other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)Concrete).GetHashCode();
	}

	public override string ToString()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} -> {1}", (object)string.Join(",", Enumerable.Select<System.Type, string>((System.Collections.Generic.IEnumerable<System.Type>)Interfaces, (Func<System.Type, string>)((System.Type i) => ((MemberInfo)i).Name))), (object)((MemberInfo)Concrete).Name);
	}
}
