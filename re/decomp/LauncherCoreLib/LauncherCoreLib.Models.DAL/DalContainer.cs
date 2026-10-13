using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.DAL;

public class DalContainer
{
	[field: CompilerGenerated]
	public IUnitOfWork UnitOfWork
	{
		[CompilerGenerated]
		get;
	}

	public DalContainer(IUnitOfWork unitOfWork)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		UnitOfWork = unitOfWork ?? throw new ArgumentNullException("unitOfWork");
	}

	public static DalContainer<TData> Create<TData>(IUnitOfWork unitOfWork, TData data)
	{
		return new DalContainer<TData>(unitOfWork, data);
	}
}
public sealed class DalContainer<TData> : DalContainer
{
	public TData Data
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public DalContainer(IUnitOfWork unitOfWork, TData data)
		: base(unitOfWork)
	{
		Data = data;
	}

	public DalContainer<TData> WithNewData(TData data)
	{
		if (!EqualityComparer<TData>.Default.Equals(data, Data))
		{
			return new DalContainer<TData>(base.UnitOfWork, data);
		}
		return this;
	}
}
