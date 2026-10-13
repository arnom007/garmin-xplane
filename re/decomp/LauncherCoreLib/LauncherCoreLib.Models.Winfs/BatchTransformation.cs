using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Settings;

namespace LauncherCoreLib.Models.Winfs;

public sealed class BatchTransformation
{
	public IObservable<WinfsCopyInstruction> CopyInstructionStream => ((System.Collections.Generic.IEnumerable<WinfsCopyInstruction>)(object)CopyInstructions).ToObservable();

	public IObservable<WinfsLinkInstruction> LinkInstructionStream => ((System.Collections.Generic.IEnumerable<WinfsLinkInstruction>)(object)LinkInstructions).ToObservable();

	[field: CompilerGenerated]
	public ImmutableArray<WinfsCopyInstruction> CopyInstructions
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public ImmutableArray<WinfsLinkInstruction> LinkInstructions
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public ImmutableArray<ValueConstraintDefinition> Constraints
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public BatchTransformation(System.Collections.Generic.IEnumerable<WinfsCopyInstruction> copies, System.Collections.Generic.IEnumerable<WinfsLinkInstruction> links, System.Collections.Generic.IEnumerable<ValueConstraintDefinition> constraints)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		CopyInstructions = ImmutableArray.ToImmutableArray<WinfsCopyInstruction>(copies);
		LinkInstructions = ImmutableArray.ToImmutableArray<WinfsLinkInstruction>(links);
		Constraints = CreateImmutableArray(constraints);
	}

	public static BatchTransformation CopiesOnly(System.Collections.Generic.IEnumerable<WinfsCopyInstruction> copies)
	{
		return new BatchTransformation(copies, Enumerable.Empty<WinfsLinkInstruction>(), Enumerable.Empty<ValueConstraintDefinition>());
	}

	public static BatchTransformation LinksOnly(System.Collections.Generic.IEnumerable<WinfsLinkInstruction> links)
	{
		return new BatchTransformation(Enumerable.Empty<WinfsCopyInstruction>(), links, Enumerable.Empty<ValueConstraintDefinition>());
	}

	private static ImmutableArray<ValueConstraintDefinition> CreateImmutableArray(System.Collections.Generic.IEnumerable<ValueConstraintDefinition> constraints)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (constraints is ImmutableArray<ValueConstraintDefinition>)
		{
			return (ImmutableArray<ValueConstraintDefinition>)(object)constraints;
		}
		return ImmutableArray.ToImmutableArray<ValueConstraintDefinition>(constraints);
	}
}
