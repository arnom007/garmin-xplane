using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.AppLifeCycle.Migrations;

public class MigrationData
{
	[field: CompilerGenerated]
	public ILauncherMigration Migration
	{
		[CompilerGenerated]
		get;
	}

	public MigrationData(ILauncherMigration migration)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Migration = migration ?? throw new ArgumentNullException("migration");
	}

	public static MigrationData<TData> Create<TData>(ILauncherMigration migration, TData data)
	{
		return new MigrationData<TData>(migration, data);
	}
}
public sealed class MigrationData<TData> : MigrationData
{
	public TData Data
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public MigrationData(ILauncherMigration migration, TData data)
		: base(migration)
	{
		Data = data;
	}
}
