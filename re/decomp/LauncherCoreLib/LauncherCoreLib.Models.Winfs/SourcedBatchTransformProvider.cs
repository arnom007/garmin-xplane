using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using DotNet.Globbing;
using LOLA.StringHelpers.Paths;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Winfs;

internal class SourcedBatchTransformProvider : ISourcedBatchTransformProvider
{
	[CompilerGenerated]
	private sealed class _003CGenerateTransforms_003Ed__14 : System.Collections.Generic.IEnumerable<SourcedBatchTransform>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<SourcedBatchTransform>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private SourcedBatchTransform _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private AvionicsModel model;

		public AvionicsModel _003C_003E3__model;

		private Winfs_root _003CwinfsTransforms_003E5__2;

		private System.Collections.Generic.IEnumerator<Constrained_transform_group> _003C_003E7__wrap2;

		SourcedBatchTransform System.Collections.Generic.IEnumerator<SourcedBatchTransform>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGenerateTransforms_003Ed__14(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
			switch (_003C_003E1__state)
			{
			case -3:
			case 1:
				try
				{
					break;
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			case -4:
			case 2:
				try
				{
					break;
				}
				finally
				{
					_003C_003Em__Finally2();
				}
			case -2:
			case -1:
			case 0:
				break;
			}
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					IManifestDataSource manifest = model.BaseInfo.Manifest;
					_003CwinfsTransforms_003E5__2 = manifest.GetWinfsTransforms();
					ThrowIfNull(_003CwinfsTransforms_003E5__2);
					_003C_003E7__wrap2 = ((Collection<Constrained_transform_group>)(object)_003CwinfsTransforms_003E5__2.Essentials).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_00a3;
				}
				case 1:
					_003C_003E1__state = -3;
					goto IL_00a3;
				case 2:
					{
						_003C_003E1__state = -4;
						break;
					}
					IL_00a3:
					if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
					{
						Constrained_transform_group current = _003C_003E7__wrap2.Current;
						_003C_003E2__current = CreateSourcedTransform(CreateBatchTransform(current), BatchTransformSource.Essentials);
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap2 = null;
					_003C_003E7__wrap2 = ((Collection<Constrained_transform_group>)(object)_003CwinfsTransforms_003E5__2.StartingUserData).GetEnumerator();
					_003C_003E1__state = -4;
					break;
				}
				if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
				{
					Constrained_transform_group current2 = _003C_003E7__wrap2.Current;
					_003C_003E2__current = CreateSourcedTransform(CreateBatchTransform(current2), BatchTransformSource.StartingUserData);
					_003C_003E1__state = 2;
					return true;
				}
				_003C_003Em__Finally2();
				_003C_003E7__wrap2 = null;
				return false;
			}
			catch
			{
				//try-fault
				((System.IDisposable)this).Dispose();
				throw;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2 != null)
			{
				((System.IDisposable)_003C_003E7__wrap2).Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2 != null)
			{
				((System.IDisposable)_003C_003E7__wrap2).Dispose();
			}
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<SourcedBatchTransform> System.Collections.Generic.IEnumerable<SourcedBatchTransform>.GetEnumerator()
		{
			_003CGenerateTransforms_003Ed__14 _003CGenerateTransforms_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CGenerateTransforms_003Ed__ = this;
			}
			else
			{
				_003CGenerateTransforms_003Ed__ = new _003CGenerateTransforms_003Ed__14(0);
			}
			_003CGenerateTransforms_003Ed__.model = _003C_003E3__model;
			return _003CGenerateTransforms_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<SourcedBatchTransform>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CGenerateTransforms_003Ed__15 : System.Collections.Generic.IEnumerable<SourcedBatchTransform>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<SourcedBatchTransform>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private SourcedBatchTransform _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private IManifestDataSource manifestSource;

		public IManifestDataSource _003C_003E3__manifestSource;

		private Winfs_root _003CwinfsTransforms_003E5__2;

		private System.Collections.Generic.IEnumerator<Constrained_transform_group> _003C_003E7__wrap2;

		SourcedBatchTransform System.Collections.Generic.IEnumerator<SourcedBatchTransform>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGenerateTransforms_003Ed__15(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
			switch (_003C_003E1__state)
			{
			case -3:
			case 1:
				try
				{
					break;
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			case -4:
			case 2:
				try
				{
					break;
				}
				finally
				{
					_003C_003Em__Finally2();
				}
			case -2:
			case -1:
			case 0:
				break;
			}
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CwinfsTransforms_003E5__2 = manifestSource.GetWinfsTransforms();
					ThrowIfNull(_003CwinfsTransforms_003E5__2);
					_003C_003E7__wrap2 = ((Collection<Constrained_transform_group>)(object)_003CwinfsTransforms_003E5__2.Essentials).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_0097;
				case 1:
					_003C_003E1__state = -3;
					goto IL_0097;
				case 2:
					{
						_003C_003E1__state = -4;
						break;
					}
					IL_0097:
					if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
					{
						Constrained_transform_group current = _003C_003E7__wrap2.Current;
						_003C_003E2__current = CreateSourcedTransform(CreateBatchTransform(current), BatchTransformSource.Essentials);
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap2 = null;
					_003C_003E7__wrap2 = ((Collection<Constrained_transform_group>)(object)_003CwinfsTransforms_003E5__2.StartingUserData).GetEnumerator();
					_003C_003E1__state = -4;
					break;
				}
				if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
				{
					Constrained_transform_group current2 = _003C_003E7__wrap2.Current;
					_003C_003E2__current = CreateSourcedTransform(CreateBatchTransform(current2), BatchTransformSource.StartingUserData);
					_003C_003E1__state = 2;
					return true;
				}
				_003C_003Em__Finally2();
				_003C_003E7__wrap2 = null;
				return false;
			}
			catch
			{
				//try-fault
				((System.IDisposable)this).Dispose();
				throw;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2 != null)
			{
				((System.IDisposable)_003C_003E7__wrap2).Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2 != null)
			{
				((System.IDisposable)_003C_003E7__wrap2).Dispose();
			}
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<SourcedBatchTransform> System.Collections.Generic.IEnumerable<SourcedBatchTransform>.GetEnumerator()
		{
			_003CGenerateTransforms_003Ed__15 _003CGenerateTransforms_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CGenerateTransforms_003Ed__ = this;
			}
			else
			{
				_003CGenerateTransforms_003Ed__ = new _003CGenerateTransforms_003Ed__15(0);
			}
			_003CGenerateTransforms_003Ed__.manifestSource = _003C_003E3__manifestSource;
			return _003CGenerateTransforms_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<SourcedBatchTransform>)this).GetEnumerator();
		}
	}

	private readonly IDatabaseDeploymentPathProvider _databasePathLookup;

	public SourcedBatchTransformProvider(IDatabaseDeploymentPathProvider dbLookup)
	{
		_databasePathLookup = dbLookup;
	}

	public IObservable<SourcedBatchTransform> SourcedTransforms(AvionicsModel trainerModel)
	{
		return Enumerable.Where<SourcedBatchTransform>(GenerateTransforms(trainerModel), (Func<SourcedBatchTransform, bool>)((SourcedBatchTransform t) => t != null)).ToObservable();
	}

	public IObservable<SourcedBatchTransform> SourcedTransforms(IManifestDataSource manifestDataSource, string fullpackageRootPath)
	{
		return Enumerable.Where<SourcedBatchTransform>(GenerateTransforms(manifestDataSource), (Func<SourcedBatchTransform, bool>)((SourcedBatchTransform t) => t != null)).ToObservable();
	}

	public IObservable<WinfsDatabaseLinkInstruction> DatabaseTransforms(IManifestDataSource manifestDataSource, string fullPackageRootPath)
	{
		return CreateDatabaseTransform(manifestDataSource, fullPackageRootPath).ToObservable();
	}

	private static Winfs_mount_setup GetWinfsTransformAtPath(IManifestDataSource manifestSource, string jsonpath)
	{
		return manifestSource.GetManifestData<Winfs_mount_setup>(jsonpath);
	}

	private static SourcedBatchTransform CreateSourcedTransform(BatchTransformation batchTransformation, BatchTransformSource source)
	{
		return new SourcedBatchTransform(batchTransformation, source);
	}

	private static BatchTransformation CreateBatchTransform(Constrained_transform_group jsonWinfs)
	{
		return new BatchTransformation(CreateCopyInstructions(jsonWinfs.Transform), CreateLinkInstructions(jsonWinfs.Transform), CreateConstraints(jsonWinfs.Constraints));
	}

	private static System.Collections.Generic.IEnumerable<ValueConstraintDefinition> CreateConstraints(ObservableCollection<Setting_constraint> constraints)
	{
		return ValueConstraintDefinition.FromJsonConstraintArray((System.Collections.Generic.IEnumerable<Setting_constraint>)constraints);
	}

	private static System.Collections.Generic.IEnumerable<WinfsLinkInstruction> CreateLinkInstructions(Winfs_mount_setup jsonWinfs)
	{
		return Enumerable.Select<Winfs_link_instruction, WinfsLinkInstruction>((System.Collections.Generic.IEnumerable<Winfs_link_instruction>)jsonWinfs.Links, (Func<Winfs_link_instruction, WinfsLinkInstruction>)((Winfs_link_instruction jsonlink) => new WinfsLinkInstruction(TrimPath(jsonlink.PackagePath), TrimPath(jsonlink.Winfs))));
	}

	private static System.Collections.Generic.IEnumerable<WinfsCopyInstruction> CreateCopyInstructions(Winfs_mount_setup jsonWinfs)
	{
		return Enumerable.SelectMany<Winfs_copy_instruction, WinfsCopyInstruction>((System.Collections.Generic.IEnumerable<Winfs_copy_instruction>)jsonWinfs.Copy, (Func<Winfs_copy_instruction, System.Collections.Generic.IEnumerable<WinfsCopyInstruction>>)((Winfs_copy_instruction jsoncopy) => Enumerable.Select<string, WinfsCopyInstruction>((System.Collections.Generic.IEnumerable<string>)jsoncopy.Globs, (Func<string, WinfsCopyInstruction>)((string glob) => new WinfsCopyInstruction(TrimPath(jsoncopy.PackagePath), TrimPath(jsoncopy.WinfsPath), Glob.Parse(glob))))));
	}

	private static string TrimPath(string relativePath)
	{
		return relativePath.Trim(new char[2] { '/', '\\' });
	}

	private static void ThrowIfNull(Winfs_root winfsTransforms)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (winfsTransforms == null)
		{
			throw new FormatException("'Winfs' is a required root property.");
		}
	}

	private static KeyValuePair<SupportedWinfsDatabase, RelativePath?> RotorNavigationFallsBackOn(PackageDatabaseMap databaseLookup, SupportedWinfsDatabase rotorNav, SupportedWinfsDatabase fallbackNav)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return KeyValuePair.Create<SupportedWinfsDatabase, RelativePath>(rotorNav, databaseLookup[fallbackNav]);
	}

	[IteratorStateMachine(typeof(_003CGenerateTransforms_003Ed__14))]
	private static System.Collections.Generic.IEnumerable<SourcedBatchTransform> GenerateTransforms(AvionicsModel model)
	{
		IManifestDataSource manifest = model.BaseInfo.Manifest;
		Winfs_root winfsTransforms = manifest.GetWinfsTransforms();
		ThrowIfNull(winfsTransforms);
		System.Collections.Generic.IEnumerator<Constrained_transform_group> enumerator = ((Collection<Constrained_transform_group>)(object)winfsTransforms.Essentials).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Constrained_transform_group current = enumerator.Current;
				yield return CreateSourcedTransform(CreateBatchTransform(current), BatchTransformSource.Essentials);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		enumerator = ((Collection<Constrained_transform_group>)(object)winfsTransforms.StartingUserData).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Constrained_transform_group current2 = enumerator.Current;
				yield return CreateSourcedTransform(CreateBatchTransform(current2), BatchTransformSource.StartingUserData);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	[IteratorStateMachine(typeof(_003CGenerateTransforms_003Ed__15))]
	private static System.Collections.Generic.IEnumerable<SourcedBatchTransform> GenerateTransforms(IManifestDataSource manifestSource)
	{
		Winfs_root winfsTransforms = manifestSource.GetWinfsTransforms();
		ThrowIfNull(winfsTransforms);
		System.Collections.Generic.IEnumerator<Constrained_transform_group> enumerator = ((Collection<Constrained_transform_group>)(object)winfsTransforms.Essentials).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Constrained_transform_group current = enumerator.Current;
				yield return CreateSourcedTransform(CreateBatchTransform(current), BatchTransformSource.Essentials);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		enumerator = ((Collection<Constrained_transform_group>)(object)winfsTransforms.StartingUserData).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Constrained_transform_group current2 = enumerator.Current;
				yield return CreateSourcedTransform(CreateBatchTransform(current2), BatchTransformSource.StartingUserData);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private System.Collections.Generic.IEnumerable<WinfsDatabaseLinkInstruction> CreateDatabaseTransform(IManifestDataSource manifestSource, string fullpackageRootPath)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		PackageDatabaseMap databaseMap = manifestSource.GetDatabaseMap();
		if (databaseMap == null)
		{
			throw new FormatException("'Databases' is a required root property.");
		}
		List<WinfsDatabaseLinkInstruction> val = CreateDbLinkInstructionsForPackage(databaseMap, fullpackageRootPath);
		UseJeppFixedWingNavDbPathIfRotorisNotSpecified(databaseMap, fullpackageRootPath, val);
		UseGarminFixedWingNavDbPathIfRotorisNotSpecified(databaseMap, fullpackageRootPath, val);
		return (System.Collections.Generic.IEnumerable<WinfsDatabaseLinkInstruction>)val;
	}

	private List<WinfsDatabaseLinkInstruction> CreateDbLinkInstructionsForPackage(PackageDatabaseMap databaseLookup, string packageRootPath)
	{
		string packageRootPath2 = packageRootPath;
		return Enumerable.ToList<WinfsDatabaseLinkInstruction>(Enumerable.Select<KeyValuePair<SupportedWinfsDatabase, RelativePath>, WinfsDatabaseLinkInstruction>(databaseLookup.SupportedEntries, (Func<KeyValuePair<SupportedWinfsDatabase, RelativePath>, WinfsDatabaseLinkInstruction>)((KeyValuePair<SupportedWinfsDatabase, RelativePath> kv) => CreateDbLinkInstruction(kv, packageRootPath2))));
	}

	private void UseJeppFixedWingNavDbPathIfRotorisNotSpecified(PackageDatabaseMap databaseLookup, string packageRootPath, List<WinfsDatabaseLinkInstruction> dbLinks)
	{
		DuplicateFixedPathForRotorIfNotAlreadyInMap(databaseLookup, packageRootPath, dbLinks, SupportedWinfsDatabase.JEPP_ROTOR_NAV, SupportedWinfsDatabase.JEPP_NAV);
	}

	private void UseGarminFixedWingNavDbPathIfRotorisNotSpecified(PackageDatabaseMap databaseLookup, string packageRootPath, List<WinfsDatabaseLinkInstruction> dbLinks)
	{
		DuplicateFixedPathForRotorIfNotAlreadyInMap(databaseLookup, packageRootPath, dbLinks, SupportedWinfsDatabase.GARMIN_ROTOR_NAV, SupportedWinfsDatabase.GARMIN_NAV);
	}

	private void DuplicateFixedPathForRotorIfNotAlreadyInMap(PackageDatabaseMap databaseLookup, string packageRootPath, List<WinfsDatabaseLinkInstruction> dbLinks, SupportedWinfsDatabase rotorNav, SupportedWinfsDatabase fixedNav)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if ((object)databaseLookup[rotorNav] == null && (object)databaseLookup[fixedNav] != null)
		{
			KeyValuePair<SupportedWinfsDatabase, RelativePath> kv = RotorNavigationFallsBackOn(databaseLookup, rotorNav, fixedNav);
			if ((object)kv.Value != null)
			{
				dbLinks.Add(CreateDbLinkInstruction(kv, packageRootPath));
			}
		}
	}

	private WinfsDatabaseLinkInstruction CreateDbLinkInstruction(KeyValuePair<SupportedWinfsDatabase, RelativePath> kv, string packageRootPath)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new WinfsDatabaseLinkInstruction(kv.Key, new WinfsLinkInstruction(BuildDatabasePathRelativeToPackage(kv, packageRootPath), kv.Value));
	}

	private string BuildDatabasePathRelativeToPackage(KeyValuePair<SupportedWinfsDatabase, RelativePath> kv, string relativeToPackage)
	{
		return PathHelpers.CreateRelativePath(PathHelpers.NormalizeToBackSlashes(relativeToPackage), PathHelpers.NormalizeToBackSlashes(_databasePathLookup.GetDatabasePath(((object)kv.Key).ToString())));
	}
}
