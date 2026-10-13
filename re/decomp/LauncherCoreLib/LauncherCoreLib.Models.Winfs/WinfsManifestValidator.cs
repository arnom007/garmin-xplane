using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using DotNet.Globbing;
using LOLA.FileSystem;
using LOLA.StringHelpers.Paths;
using LauncherCoreLib.Models.Settings;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Winfs;

public class WinfsManifestValidator : AppliesToEveryVersionManifestValidator
{
	[CompilerGenerated]
	private sealed class _003CCopyInstructionPaths_003Ed__8 : System.Collections.Generic.IEnumerable<string>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<string>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private string _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private WinfsCopyInstruction ci;

		public WinfsCopyInstruction _003C_003E3__ci;

		string System.Collections.Generic.IEnumerator<string>.Current
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
		public _003CCopyInstructionPaths_003Ed__8(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = ci.PackageBaseDirectory;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (ci.WinFsBaseDirectory != ci.PackageBaseDirectory)
				{
					_003C_003E2__current = ci.WinFsBaseDirectory;
					_003C_003E1__state = 2;
					return true;
				}
				break;
			case 2:
				_003C_003E1__state = -1;
				break;
			}
			return false;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator()
		{
			_003CCopyInstructionPaths_003Ed__8 _003CCopyInstructionPaths_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CCopyInstructionPaths_003Ed__ = this;
			}
			else
			{
				_003CCopyInstructionPaths_003Ed__ = new _003CCopyInstructionPaths_003Ed__8(0);
			}
			_003CCopyInstructionPaths_003Ed__.ci = _003C_003E3__ci;
			return _003CCopyInstructionPaths_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<string>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CFormatInvalidPathError_003Ed__16 : System.Collections.Generic.IEnumerable<string>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<string>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private string _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private string[] paths;

		public string[] _003C_003E3__paths;

		private SourcedBatchTransform transform;

		public SourcedBatchTransform _003C_003E3__transform;

		private string instructionType;

		public string _003C_003E3__instructionType;

		string System.Collections.Generic.IEnumerator<string>.Current
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
		public _003CFormatInvalidPathError_003Ed__16(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				System.Collections.Generic.IEnumerable<string> enumerable = Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)paths, (Func<string, bool>)ContainsInvalidCharacters);
				if (Enumerable.Any<string>(enumerable))
				{
					_003C_003E2__current = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} Winfs '{1}' transform specified invalid path characters: [{2}].", (object)transform.Source, (object)instructionType, (object)string.Join(", ", SurroundPathsWithSingleQuotes(enumerable)));
					_003C_003E1__state = 1;
					return true;
				}
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			return false;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator()
		{
			_003CFormatInvalidPathError_003Ed__16 _003CFormatInvalidPathError_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CFormatInvalidPathError_003Ed__ = this;
			}
			else
			{
				_003CFormatInvalidPathError_003Ed__ = new _003CFormatInvalidPathError_003Ed__16(0);
			}
			_003CFormatInvalidPathError_003Ed__.transform = _003C_003E3__transform;
			_003CFormatInvalidPathError_003Ed__.instructionType = _003C_003E3__instructionType;
			_003CFormatInvalidPathError_003Ed__.paths = _003C_003E3__paths;
			return _003CFormatInvalidPathError_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<string>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CFormatParentDirectoryPathError_003Ed__20 : System.Collections.Generic.IEnumerable<string>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<string>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private string _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private string[] noParents;

		public string[] _003C_003E3__noParents;

		private SourcedBatchTransform transform;

		public SourcedBatchTransform _003C_003E3__transform;

		private string instructionType;

		public string _003C_003E3__instructionType;

		string System.Collections.Generic.IEnumerator<string>.Current
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
		public _003CFormatParentDirectoryPathError_003Ed__20(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				System.Collections.Generic.IEnumerable<string> enumerable = Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)noParents, (Func<string, bool>)ContainsParentDirectoryDirectives);
				if (Enumerable.Any<string>(enumerable))
				{
					_003C_003E2__current = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} Winfs '{1}' transform used parent paths: [{2}] These are not allowed.", (object)transform.Source, (object)instructionType, (object)string.Join(", ", SurroundPathsWithSingleQuotes(enumerable)));
					_003C_003E1__state = 1;
					return true;
				}
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			return false;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator()
		{
			_003CFormatParentDirectoryPathError_003Ed__20 _003CFormatParentDirectoryPathError_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CFormatParentDirectoryPathError_003Ed__ = this;
			}
			else
			{
				_003CFormatParentDirectoryPathError_003Ed__ = new _003CFormatParentDirectoryPathError_003Ed__20(0);
			}
			_003CFormatParentDirectoryPathError_003Ed__.transform = _003C_003E3__transform;
			_003CFormatParentDirectoryPathError_003Ed__.instructionType = _003C_003E3__instructionType;
			_003CFormatParentDirectoryPathError_003Ed__.noParents = _003C_003E3__noParents;
			return _003CFormatParentDirectoryPathError_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<string>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CLinkInstructionPaths_003Ed__15 : System.Collections.Generic.IEnumerable<string>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<string>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private string _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private WinfsLinkInstruction li;

		public WinfsLinkInstruction _003C_003E3__li;

		string System.Collections.Generic.IEnumerator<string>.Current
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
		public _003CLinkInstructionPaths_003Ed__15(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = li.PackagePath;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (li.WinfsPath != li.PackagePath)
				{
					_003C_003E2__current = li.WinfsPath;
					_003C_003E1__state = 2;
					return true;
				}
				break;
			case 2:
				_003C_003E1__state = -1;
				break;
			}
			return false;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator()
		{
			_003CLinkInstructionPaths_003Ed__15 _003CLinkInstructionPaths_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CLinkInstructionPaths_003Ed__ = this;
			}
			else
			{
				_003CLinkInstructionPaths_003Ed__ = new _003CLinkInstructionPaths_003Ed__15(0);
			}
			_003CLinkInstructionPaths_003Ed__.li = _003C_003E3__li;
			return _003CLinkInstructionPaths_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<string>)this).GetEnumerator();
		}
	}

	private readonly IFileSystem _fs;

	private readonly ISourcedBatchTransformProvider _transformProvider;

	public override string ValidatorName => "Winfs";

	public WinfsManifestValidator(IFileSystem fileSystem, ISourcedBatchTransformProvider sourcedBatchTransformProvider)
	{
		_fs = fileSystem;
		_transformProvider = sourcedBatchTransformProvider;
	}

	protected override IObservable<string> LegacyManifestErrors(ITrainerPackage package, int manifestSchemaVersion, IManifestDataSource manifest)
	{
		ITrainerPackage package2 = package;
		IObservable<SourcedBatchTransform> val = _transformProvider.SourcedTransforms(package2.BaseInfo.Manifest, package2.BaseInfo.PackageRootPath).Publish<SourcedBatchTransform>().RefCount();
		return MissingTransforms(val).Merge<string>(val.SelectMany<SourcedBatchTransform, string>((Func<SourcedBatchTransform, IObservable<string>>)((SourcedBatchTransform transform) => ValidateTransform(package2, transform)))).Catch<string, FormatException>((Func<FormatException, IObservable<string>>)((FormatException fe) => Observable.Return(((System.Exception)(object)fe).Message)));
	}

	private static IObservable<string> MissingTransforms(IObservable<SourcedBatchTransform> transforms)
	{
		return transforms.Any<SourcedBatchTransform>().Where<bool>((Func<bool, bool>)((bool anyTransforms) => !anyTransforms)).Select<bool, string>((Func<bool, string>)((bool _) => "Must specify the 'Winfs' transformations as a root manifest property."));
	}

	private static string BuildFullInstallPath(ITrainerPackage package, RelativePath relativePath)
	{
		return PathHelpers.NormalizeToBackSlashes(relativePath.AppendTo(package.BaseInfo.PackageRootPath));
	}

	[IteratorStateMachine(typeof(_003CCopyInstructionPaths_003Ed__8))]
	private static System.Collections.Generic.IEnumerable<string> CopyInstructionPaths(WinfsCopyInstruction ci)
	{
		yield return ci.PackageBaseDirectory;
		if (ci.WinFsBaseDirectory != ci.PackageBaseDirectory)
		{
			yield return ci.WinFsBaseDirectory;
		}
	}

	private static System.Collections.Generic.IEnumerable<string> CopyInstructionParentDirectoryPaths(WinfsCopyInstruction ci)
	{
		return Enumerable.Concat<string>(CopyInstructionPaths(ci), Enumerable.Select<Glob, string>((System.Collections.Generic.IEnumerable<Glob>)ci.Globs, (Func<Glob, string>)((Glob g) => ((object)g).ToString())));
	}

	private static bool ContainsInvalidCharacters(string relpath)
	{
		return Enumerable.Any<char>(Enumerable.Intersect<char>((System.Collections.Generic.IEnumerable<char>)relpath, InvalidPathCharacters()));
	}

	private static System.Collections.Generic.IEnumerable<char> InvalidPathCharacters()
	{
		return Enumerable.Except<char>(Enumerable.Union<char>((System.Collections.Generic.IEnumerable<char>)Path.GetInvalidPathChars(), (System.Collections.Generic.IEnumerable<char>)Path.GetInvalidFileNameChars()), (System.Collections.Generic.IEnumerable<char>)new char[3]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar,
			'.'
		});
	}

	private static bool ContainsParentDirectoryDirectives(string relPath)
	{
		return Enumerable.Any<string>((System.Collections.Generic.IEnumerable<string>)relPath.Split(new char[2]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		}, (StringSplitOptions)1), (Func<string, bool>)((string dirName) => dirName == ".."));
	}

	private static string SurroundWithSingleQuotes(string s)
	{
		return "'" + s + "'";
	}

	private static System.Collections.Generic.IEnumerable<string> SurroundPathsWithSingleQuotes(System.Collections.Generic.IEnumerable<string> parentDirectories)
	{
		return Enumerable.Select<string, string>(parentDirectories, (Func<string, string>)SurroundWithSingleQuotes);
	}

	[IteratorStateMachine(typeof(_003CLinkInstructionPaths_003Ed__15))]
	private static System.Collections.Generic.IEnumerable<string> LinkInstructionPaths(WinfsLinkInstruction li)
	{
		yield return li.PackagePath;
		if (li.WinfsPath != li.PackagePath)
		{
			yield return li.WinfsPath;
		}
	}

	[IteratorStateMachine(typeof(_003CFormatInvalidPathError_003Ed__16))]
	private static System.Collections.Generic.IEnumerable<string> FormatInvalidPathError(SourcedBatchTransform transform, string instructionType, params string[] paths)
	{
		System.Collections.Generic.IEnumerable<string> enumerable = Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)paths, (Func<string, bool>)ContainsInvalidCharacters);
		if (Enumerable.Any<string>(enumerable))
		{
			yield return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} Winfs '{1}' transform specified invalid path characters: [{2}].", (object)transform.Source, (object)instructionType, (object)string.Join(", ", SurroundPathsWithSingleQuotes(enumerable)));
		}
	}

	private static bool ContainsInvalidSubstitutions(params string[] paths)
	{
		return Enumerable.Any<string>(Enumerable.SelectMany<string, string>((System.Collections.Generic.IEnumerable<string>)paths, (Func<string, System.Collections.Generic.IEnumerable<string>>)((string path) => SettingSubstitution.GetInvalidSubstitutions(path))));
	}

	private static string FormatMissingPathError(ITrainerPackage package, SourcedBatchTransform transform, string instructionType, string installedRelativePath)
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} Winfs '{1}' transform specified '{2}' which was not found at '{3}'.", new object[4]
		{
			transform.Source,
			instructionType,
			installedRelativePath,
			BuildFullInstallPath(package, installedRelativePath)
		});
	}

	private static string FormatInvalidSubError(SourcedBatchTransform transform, string instructionType, params string[] hasInvalidSubs)
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} Winfs '{1}' transform has invalid substitutions: [{2}].", (object)transform.Source, (object)instructionType, (object)string.Join(", ", SurroundPathsWithSingleQuotes(Enumerable.SelectMany<string, string>((System.Collections.Generic.IEnumerable<string>)hasInvalidSubs, (Func<string, System.Collections.Generic.IEnumerable<string>>)((string path) => SettingSubstitution.GetInvalidSubstitutions(path))))));
	}

	[IteratorStateMachine(typeof(_003CFormatParentDirectoryPathError_003Ed__20))]
	private static System.Collections.Generic.IEnumerable<string> FormatParentDirectoryPathError(SourcedBatchTransform transform, string instructionType, params string[] noParents)
	{
		System.Collections.Generic.IEnumerable<string> enumerable = Enumerable.Where<string>((System.Collections.Generic.IEnumerable<string>)noParents, (Func<string, bool>)ContainsParentDirectoryDirectives);
		if (Enumerable.Any<string>(enumerable))
		{
			yield return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The {0} Winfs '{1}' transform used parent paths: [{2}] These are not allowed.", (object)transform.Source, (object)instructionType, (object)string.Join(", ", SurroundPathsWithSingleQuotes(enumerable)));
		}
	}

	private static IObservable<string> ValidateInvalidPaths(SourcedBatchTransform transform)
	{
		SourcedBatchTransform transform2 = transform;
		return transform2.Transform.CopyInstructionStream.SelectMany<WinfsCopyInstruction, string>((Func<WinfsCopyInstruction, System.Collections.Generic.IEnumerable<string>>)((WinfsCopyInstruction ci) => Enumerable.Concat<string>(FormatInvalidPathError(transform2, "Copy", Enumerable.ToArray<string>(CopyInstructionPaths(ci))), FormatParentDirectoryPathError(transform2, "Copy", Enumerable.ToArray<string>(CopyInstructionParentDirectoryPaths(ci)))))).Concat<string>(transform2.Transform.LinkInstructionStream.SelectMany<WinfsLinkInstruction, string>((Func<WinfsLinkInstruction, System.Collections.Generic.IEnumerable<string>>)((WinfsLinkInstruction li) => Enumerable.Concat<string>(FormatInvalidPathError(transform2, "Link", Enumerable.ToArray<string>(LinkInstructionPaths(li))), FormatParentDirectoryPathError(transform2, "Link", Enumerable.ToArray<string>(LinkInstructionPaths(li)))))));
	}

	private IObservable<string> ValidateTransform(ITrainerPackage package, SourcedBatchTransform transform)
	{
		return Observable.Concat(new IObservable<string>[3]
		{
			ValidateInvalidPaths(transform),
			ValidateTransformPathAgainstFileSystem(package, transform),
			ValidateInvalidSubstitutions(transform)
		});
	}

	private IObservable<string> ValidateInvalidSubstitutions(SourcedBatchTransform transform)
	{
		SourcedBatchTransform transform2 = transform;
		return transform2.Transform.CopyInstructionStream.Where<WinfsCopyInstruction>((Func<WinfsCopyInstruction, bool>)PathFormatsAreValid).Where<WinfsCopyInstruction>((Func<WinfsCopyInstruction, bool>)((WinfsCopyInstruction ci) => ContainsInvalidSubstitutions(ci.PackageBaseDirectory, ci.WinFsBaseDirectory))).Select<WinfsCopyInstruction, string>((Func<WinfsCopyInstruction, string>)((WinfsCopyInstruction ci) => FormatInvalidSubError(transform2, "Copy", ci.PackageBaseDirectory, ci.WinFsBaseDirectory)))
			.Concat<string>(transform2.Transform.LinkInstructionStream.Where<WinfsLinkInstruction>((Func<WinfsLinkInstruction, bool>)PathFormatsAreValid).Where<WinfsLinkInstruction>((Func<WinfsLinkInstruction, bool>)((WinfsLinkInstruction li) => ContainsInvalidSubstitutions(li.PackagePath, li.WinfsPath))).Select<WinfsLinkInstruction, string>((Func<WinfsLinkInstruction, string>)((WinfsLinkInstruction li) => FormatInvalidSubError(transform2, "Link", li.PackagePath, li.WinfsPath))));
	}

	private IObservable<string> ValidateTransformPathAgainstFileSystem(ITrainerPackage package, SourcedBatchTransform transform)
	{
		ITrainerPackage package2 = package;
		SourcedBatchTransform transform2 = transform;
		if (package2.BaseInfo.PackageType.IsCompleteOnDisk())
		{
			return transform2.Transform.CopyInstructionStream.Where<WinfsCopyInstruction>((Func<WinfsCopyInstruction, bool>)PathFormatsAreValid).Where<WinfsCopyInstruction>((Func<WinfsCopyInstruction, bool>)((WinfsCopyInstruction ci) => IsPathInvalid(BuildFullInstallPath(package2, ci.PackageBaseDirectory)))).Select<WinfsCopyInstruction, string>((Func<WinfsCopyInstruction, string>)((WinfsCopyInstruction ci) => FormatMissingPathError(package2, transform2, "Copy", ci.PackageBaseDirectory)))
				.Concat<string>(transform2.Transform.LinkInstructionStream.Where<WinfsLinkInstruction>((Func<WinfsLinkInstruction, bool>)PathFormatsAreValid).Where<WinfsLinkInstruction>((Func<WinfsLinkInstruction, bool>)((WinfsLinkInstruction li) => IsPathInvalid(BuildFullInstallPath(package2, li.PackagePath)))).Select<WinfsLinkInstruction, string>((Func<WinfsLinkInstruction, string>)((WinfsLinkInstruction li) => FormatMissingPathError(package2, transform2, "Link", li.PackagePath))));
		}
		return Observable.Empty<string>();
	}

	private bool PathFormatsAreValid(WinfsLinkInstruction arg)
	{
		if (!ContainsInvalidCharacters(arg.PackagePath.RelPath) && !ContainsInvalidCharacters(arg.WinfsPath.RelPath) && !ContainsParentDirectoryDirectives(arg.PackagePath.RelPath))
		{
			return !ContainsParentDirectoryDirectives(arg.WinfsPath.RelPath);
		}
		return false;
	}

	private bool PathFormatsAreValid(WinfsCopyInstruction arg)
	{
		if (!ContainsInvalidCharacters(arg.PackageBaseDirectory.RelPath) && !ContainsInvalidCharacters(arg.WinFsBaseDirectory.RelPath) && !ContainsParentDirectoryDirectives(arg.PackageBaseDirectory.RelPath) && !ContainsParentDirectoryDirectives(arg.WinFsBaseDirectory.RelPath))
		{
			return !Enumerable.Any<Glob>((System.Collections.Generic.IEnumerable<Glob>)arg.Globs, (Func<Glob, bool>)((Glob glob) => ContainsParentDirectoryDirectives(((object)glob).ToString())));
		}
		return false;
	}

	private bool IsPathInvalid(string fullInstalledPath)
	{
		if (!SettingSubstitution.ContainsSubstitutions(fullInstalledPath))
		{
			return !_fs.PathExists(fullInstalledPath);
		}
		return false;
	}
}
