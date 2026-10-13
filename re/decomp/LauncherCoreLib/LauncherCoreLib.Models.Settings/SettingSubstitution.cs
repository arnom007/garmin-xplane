using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Settings;

public static class SettingSubstitution
{
	public const string SUB_ID_PATTERN_NAME = "SUB_ID";

	private static readonly Regex SUBSTITUTION_PATTERN = new Regex("\\$\\((?<SUB_ID>[^\\)]*)\\)");

	public static bool ContainsSubstitutions(string inputString)
	{
		return SUBSTITUTION_PATTERN.IsMatch(inputString);
	}

	public static System.Collections.Generic.IEnumerable<TrainerIdentifier> GetSubstitutionIds(string inputString)
	{
		return Enumerable.Select<string, TrainerIdentifier>(Enumerable.Where<string>(GetValidAndInvalidSubstitutionIds(inputString), (Func<string, bool>)TrainerIdentifier.IsValidIdentifier), (Func<string, TrainerIdentifier>)TrainerIdentifier.Create);
	}

	public static System.Collections.Generic.IEnumerable<string> GetInvalidSubstitutions(string inputString)
	{
		return Enumerable.Select<string, string>(Enumerable.Where<string>(GetValidAndInvalidSubstitutionIds(inputString), (Func<string, bool>)((string id) => !TrainerIdentifier.IsValidIdentifier(id))), (Func<string, string>)WrapAnything);
	}

	public static string ReplaceSubstitution(string inputString, TrainerIdentifier subid, string replacement)
	{
		return inputString.Replace(Wrap(subid), replacement, (StringComparison)4);
	}

	public static string Wrap(TrainerIdentifier subId)
	{
		return WrapAnything(subId.IdentStringChecked);
	}

	public static string WrapAnything(string subId)
	{
		return "$(" + subId + ")";
	}

	private static System.Collections.Generic.IEnumerable<string> GetValidAndInvalidSubstitutionIds(string inputString)
	{
		return Enumerable.Distinct<string>(Enumerable.Select<Group, string>(Enumerable.Where<Group>(Enumerable.Select<Match, Group>(Enumerable.Cast<Match>((System.Collections.IEnumerable)SUBSTITUTION_PATTERN.Matches(inputString)), (Func<Match, Group>)((Match m) => m.Groups["SUB_ID"])), (Func<Group, bool>)((Group g) => g.Success)), (Func<Group, string>)((Group g) => ((Capture)g).Value)));
	}
}
