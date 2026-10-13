using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DotNet.Globbing;
using LOLA.Extensions;

namespace LauncherCoreLib.Models.Winfs;

public sealed class WinfsCopyInstruction
{
	[field: CompilerGenerated]
	public RelativePath PackageBaseDirectory
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public RelativePath WinFsBaseDirectory
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<Glob> Globs
	{
		[CompilerGenerated]
		get;
	}

	public System.Collections.Generic.IEnumerable<WinfsCopyInstruction> Flatten => Enumerable.Select<Glob, WinfsCopyInstruction>((System.Collections.Generic.IEnumerable<Glob>)Globs, (Func<Glob, WinfsCopyInstruction>)([CompilerGenerated] (Glob g) => ToSingleGlob(g)));

	public WinfsCopyInstruction(RelativePath package, RelativePath winfs, System.Collections.Generic.IEnumerable<Glob> globs)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (globs == null)
		{
			throw new ArgumentNullException("globs");
		}
		PackageBaseDirectory = package ?? throw new ArgumentNullException("package");
		WinFsBaseDirectory = winfs ?? throw new ArgumentNullException("winfs");
		Globs = (System.Collections.Generic.IReadOnlyList<Glob>)Enumerable.ToList<Glob>(globs);
	}

	public WinfsCopyInstruction(RelativePath package, RelativePath winfs, Glob glob)
		: this(package, winfs, glob.Yield())
	{
	}

	public WinfsCopyInstruction ToSingleGlob(Glob glob)
	{
		return new WinfsCopyInstruction(PackageBaseDirectory, WinFsBaseDirectory, glob);
	}
}
