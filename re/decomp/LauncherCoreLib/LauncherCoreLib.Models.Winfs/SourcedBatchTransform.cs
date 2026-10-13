using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Winfs;

public sealed class SourcedBatchTransform : IEquatable<SourcedBatchTransform>
{
	[field: CompilerGenerated]
	public BatchTransformSource Source
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public BatchTransformation Transform
	{
		[CompilerGenerated]
		get;
	}

	public SourcedBatchTransform(BatchTransformation transform, BatchTransformSource source)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Transform = transform ?? throw new ArgumentNullException("transform");
		Source = source;
	}

	public bool Equals(SourcedBatchTransform? other)
	{
		if (this != other)
		{
			if (other != null && Source == other.Source)
			{
				return EqualityComparer<BatchTransformation>.Default.Equals(Transform, other.Transform);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SourcedBatchTransform other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<BatchTransformSource, BatchTransformation>(Source, Transform);
	}
}
