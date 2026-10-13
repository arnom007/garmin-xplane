using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Winfs;

public sealed class WinfsTransformRequest : IEquatable<WinfsTransformRequest>
{
	[field: CompilerGenerated]
	public string InstalledPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string WinfsPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public WinfsTranformOperation Operation
	{
		[CompilerGenerated]
		get;
	}

	public WinfsTransformRequest(string installed, string winfs, WinfsTranformOperation operation)
	{
		InstalledPath = installed;
		WinfsPath = winfs;
		Operation = operation;
	}

	public WinfsTransformReport ToReport(WinfsOperationResult result)
	{
		return new WinfsTransformReport(this, result);
	}

	public WinfsTransformReport Skipped()
	{
		return ToReport(WinfsOperationResult.NotExecuted);
	}

	public WinfsTransformReport Succeeded()
	{
		return ToReport(WinfsOperationResult.Succeeded);
	}

	public WinfsTransformReport Failed(System.Exception error)
	{
		return new WinfsTransformReport(this, error);
	}

	public override string ToString()
	{
		string text = ((Operation == WinfsTranformOperation.None) ? "None" : ((Operation == WinfsTranformOperation.Copy) ? "{0}: {1} -> {2}" : ((Operation != WinfsTranformOperation.Link) ? "Unknown: {1}, {2}" : "{0}: {2} -> {1}")));
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, text, (object)Operation, (object)InstalledPath, (object)WinfsPath);
	}

	public bool Equals(WinfsTransformRequest? other)
	{
		if (this != other)
		{
			if (other != null && InstalledPath == other.InstalledPath && WinfsPath == other.WinfsPath)
			{
				return Operation == other.Operation;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is WinfsTransformRequest other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<string, string, WinfsTranformOperation>(InstalledPath, WinfsPath, Operation);
	}
}
