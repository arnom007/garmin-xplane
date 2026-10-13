using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GTI.Net;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public static class GtiAppCenterExtensions
{
	private sealed class GtiErrorTrackedEventDescriptor : GenericTrackedEventDataDescriptor
	{
		private readonly GtiProcessResult _errorResult;

		private readonly AppCenterPropertyName _stderrPropName = AppCenterPropertyName.ParseAndThrowOnErrors("STDERR_Excerpt");

		protected override PropertiesBuilder BuildProperties => [CompilerGenerated] () =>
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			System.Collections.Generic.IReadOnlyList<string> readOnlyList2;
			if (((System.Collections.Generic.IReadOnlyCollection<string>)_errorResult.StandardErrorMessages).Count <= 0)
			{
				System.Collections.Generic.IReadOnlyList<string> readOnlyList = new string[1] { "<NONE>" };
				readOnlyList2 = readOnlyList;
			}
			else
			{
				readOnlyList2 = _errorResult.StandardErrorMessages;
			}
			System.Collections.Generic.IReadOnlyList<string> list = readOnlyList2;
			return new KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>[3]
			{
				KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("Command"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(GetCommand(_errorResult))),
				KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("Exit_Code"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(FormatExitCode(_errorResult))),
				KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(_stderrPropName, AppCenterPropertyValue.ParseTruncateAndHandleErrors(Enumerable.LastOrDefault<string>(Enumerable.Select<ImmutableList<string>, string>(Enumerable.TakeWhile<ImmutableList<string>>(Enumerable.Skip<ImmutableList<string>>(Enumerable.Skip<string>(((System.Collections.Generic.IEnumerable<string>)list).Scan(string.Empty, (string acc, string line) => (acc.Length <= 0) ? line : (acc + "\n" + line)), 1).Scan<ImmutableList<string>, string>(ImmutableList<string>.Empty, delegate(ImmutableList<string> acc, string next)
				{
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					switch (acc.Count)
					{
					case 0:
					case 1:
						return acc.Add(next);
					case 2:
						return acc.RemoveAt(0).Add(next);
					default:
						throw new InvalidOperationException("Internal error");
					}
				}), 1), (Func<ImmutableList<string>, bool>)((ImmutableList<string> l) => l[0].Length <= 125)), (Func<ImmutableList<string>, string>)((ImmutableList<string> l) => Enumerable.Last<string>((System.Collections.Generic.IEnumerable<string>)l))), string.Empty)))
			};
		};

		protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("GTI_Result"), EventTargetApps.Launcher);

		private GtiErrorTrackedEventDescriptor(GtiProcessResult errorResult)
		{
			_errorResult = errorResult;
		}

		public static void LogToAppCenter(GtiProcessResult result)
		{
			TrackedEvent.Log(new GtiErrorTrackedEventDescriptor(result));
		}

		public override TrackedEventContainer Instance()
		{
			return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.CensorProperties(_stderrPropName));
		}

		private static string GetCommand(GtiProcessResult errorResult)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			return ((object)Enumerable.FirstOrDefault<ReadOnlyMemory<char>>(Enumerable.Take<ReadOnlyMemory<char>>(LauncherTextHelpers.SplitCommandLine(MemoryExtensions.AsMemory(errorResult.RawResult.StartUp?.CommandLineArgs ?? string.Empty)), 2), (Func<ReadOnlyMemory<char>, bool>)((ReadOnlyMemory<char> p) => LooksLikeACommandAndNotAPath(p)), MemoryExtensions.AsMemory("<NONE>"))).ToString();
		}

		private static bool LooksLikeACommandAndNotAPath(ReadOnlyMemory<char> p)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Enumerable.All<char>(MemoryMarshal.ToEnumerable<char>(MemoryExtensions.Trim(p)), (Func<char, bool>)((char c) => char.IsLetter(c)));
		}

		private static string FormatExitCode(GtiProcessResult errorResult)
		{
			string text = $"0x{errorResult.ExitCode:X8}";
			string text2 = "Unknown";
			if (errorResult.WasCanceled)
			{
				text2 = "Canceled";
			}
			else if (errorResult.IsDiskSpaceIssue)
			{
				text2 = "Out of Disk";
			}
			else if (errorResult.IsInternetIssue)
			{
				text2 = "Internet";
			}
			else if (errorResult.Succeeded)
			{
				text2 = "Success";
			}
			else if (errorResult.ExitCode == 2)
			{
				text2 = "Invalid Usage";
			}
			else if (errorResult.ExitCode == 5)
			{
				text2 = "Invalid URL";
			}
			else if (errorResult.ExitCode == 6)
			{
				text2 = "Invalid File format";
			}
			else if (errorResult.ExitCode == 10)
			{
				text2 = "Tampering";
			}
			return text2 + " (" + text + ")";
		}
	}

	private static readonly Regex LIST_COMMAND_PATTERN = new Regex("\\blist\\b");

	public static void LogInterestingResultToAppCenter(this GtiProcessResult result)
	{
		GtiProcessResult result2 = result;
		if (!IsNotInteresting())
		{
			GtiErrorTrackedEventDescriptor.LogToAppCenter(result2);
		}
		[CompilerGenerated]
		bool IsNotInteresting()
		{
			if (!LIST_COMMAND_PATTERN.IsMatch(result2.RawResult.StartUp?.CommandLineArgs ?? string.Empty) || (!result2.Succeeded && !result2.WasCanceled))
			{
				return result2.RawResult.StartUp?.CommandLineArgs.Contains("--help", (StringComparison)4) ?? false;
			}
			return true;
		}
	}
}
