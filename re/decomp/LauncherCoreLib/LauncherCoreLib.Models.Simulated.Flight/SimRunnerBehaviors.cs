using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class SimRunnerBehaviors
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<PowerStage, Option<Squawk>> _003C_003E9__1_0;

		public static Func<ISimSessionEvolved, IObservable<Option<Squawk>>> _003C_003E9__1_6;

		public static Func<ISimRunnerEvolved, IObservable<Option<Squawk>>> _003C_003E9__1_1;

		public static Func<Option<Squawk>, bool> _003C_003E9__1_2;

		public static Func<Option<Squawk>, Squawk> _003C_003E9__1_3;

		public static Func<string, string> _003C_003E9__1_8;

		public static MatchEvaluator _003C_003E9__1_9;

		internal Option<Squawk> _003CSimPowerSquawks_003Eb__1_0(PowerStage stage)
		{
			switch (stage)
			{
			case PowerStage.PoweringUp:
				return Prelude.Some(Squawk.CreateIndeterminateProgress("Avionics powering up."));
			case PowerStage.PoweredUp:
				return Squawk.Create("Avionics powered up.");
			case PowerStage.PoweringDown:
				return Squawk.CreateIndeterminateProgress("Avionics powering down.");
			case PowerStage.PoweredDown:
				return Prelude.None;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(stage);
				Option<Squawk> result = default(Option<Squawk>);
				return result;
			}
			}
		}

		internal IObservable<Option<Squawk>> _003CSimPowerSquawks_003Eb__1_1(ISimRunnerEvolved r)
		{
			return r.Sessions.GetUpdateToDateAndThenContinualValues().Select<ISimSessionEvolved, IObservable<Option<Squawk>>>((Func<ISimSessionEvolved, IObservable<Option<Squawk>>>)((ISimSessionEvolved s) => Observable.FromAsync<Option<Squawk>>((Func<System.Threading.Tasks.Task<Option<Squawk>>>)new _003C_003Ec__DisplayClass1_0
			{
				s = s
			}._003CSimPowerSquawks_003Eb__7))).Switch<Option<Squawk>>();
		}

		internal IObservable<Option<Squawk>> _003CSimPowerSquawks_003Eb__1_6(ISimSessionEvolved s)
		{
			return Observable.FromAsync<Option<Squawk>>((Func<System.Threading.Tasks.Task<Option<Squawk>>>)new _003C_003Ec__DisplayClass1_0
			{
				s = s
			}._003CSimPowerSquawks_003Eb__7);
		}

		internal bool _003CSimPowerSquawks_003Eb__1_2(Option<Squawk> o)
		{
			return o.IsSome;
		}

		internal Squawk _003CSimPowerSquawks_003Eb__1_3(Option<Squawk> o)
		{
			return o.ValueUnsafe();
		}

		internal string _003CSimPowerSquawks_003Eb__1_8(string line)
		{
			return _003CSimPowerSquawks_003Eg__RemoveStartingDash_007C1_5(line).Trim();
		}

		internal string _003CSimPowerSquawks_003Eb__1_9(Match m)
		{
			return ((Capture)m.Groups["indentation"]).Value + ((Capture)m.Groups["EverythingElse"]).Value;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_0
	{
		[StructLayout(3)]
		private struct _003C_003CSimPowerSquawks_003Eb__7_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Option<Squawk>> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass1_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass1_0 _003C_003Ec__DisplayClass1_ = _003C_003E4__this;
				Option<Squawk> result;
				try
				{
					if (num == 0 || _003C_003Ec__DisplayClass1_.s != null)
					{
						try
						{
							ConfiguredTaskAwaiter awaiter;
							if (num != 0)
							{
								ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass1_.s.Completion.DetachContext();
								awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
								if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CSimPowerSquawks_003Eb__7_003Ed>(ref awaiter, ref this);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
							}
							((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
							result = Prelude.Some(Squawk.Create("Avionics powered down."));
						}
						catch (System.Exception ex)
						{
							Squawk squawk = SquawkFlightSessionError(ex);
							string text = (_003C_003Ec__DisplayClass1_.s as IAircraftSimRunner)?.Aircraft.AircraftIdentifier.IdentString ?? "Unknown Aircraft";
							Logger.LogFormattedException(ex, "Flight Session for {0}", text);
							result = squawk;
						}
					}
					else
					{
						result = Prelude.None;
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetResult(result);
				[CompilerGenerated]
				static string RemoveStartingDash(string squawkMessage)
				{
					//IL_002c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0037: Expected O, but got Unknown
					if (((Group)PRETTY_EXCEPTION_TREE_REGEX.Match(squawkMessage)).Success)
					{
						Regex pRETTY_EXCEPTION_TREE_REGEX = PRETTY_EXCEPTION_TREE_REGEX;
						string text2 = squawkMessage;
						object obj = _003C_003Ec._003C_003E9__1_9;
						if (obj == null)
						{
							MatchEvaluator val2 = (Match m) => ((Capture)m.Groups["indentation"]).Value + ((Capture)m.Groups["EverythingElse"]).Value;
							_003C_003Ec._003C_003E9__1_9 = val2;
							obj = (object)val2;
						}
						squawkMessage = pRETTY_EXCEPTION_TREE_REGEX.Replace(text2, (MatchEvaluator)obj);
					}
					return squawkMessage;
				}
				[CompilerGenerated]
				static Squawk SquawkFlightSessionError(System.Exception flightError)
				{
					string text3 = flightError.FormatExceptionTree().Replace("\t", "    ", (StringComparison)4);
					string[] array = text3.Split('\n', (StringSplitOptions)0);
					if (array.Length == 2)
					{
						text3 = string.Join(" ", Enumerable.Select<string, string>((System.Collections.Generic.IEnumerable<string>)array, (Func<string, string>)((string line) => RemoveStartingDash(line).Trim())));
					}
					text3 = RemoveStartingDash(text3);
					return Squawk.CreateError(text3);
				}
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public ISimSessionEvolved s;

		[AsyncStateMachine(typeof(_003C_003CSimPowerSquawks_003Eb__7_003Ed))]
		internal async System.Threading.Tasks.Task<Option<Squawk>> _003CSimPowerSquawks_003Eb__7()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (s != null)
			{
				try
				{
					await s.Completion.DetachContext();
					return Prelude.Some(Squawk.Create("Avionics powered down."));
				}
				catch (System.Exception ex)
				{
					Squawk squawk = _003CSimPowerSquawks_003Eg__SquawkFlightSessionError_007C1_4(ex);
					string text = (s as IAircraftSimRunner)?.Aircraft.AircraftIdentifier.IdentString ?? "Unknown Aircraft";
					Logger.LogFormattedException(ex, "Flight Session for {0}", text);
					return squawk;
				}
			}
			return Prelude.None;
		}
	}

	private static readonly Regex PRETTY_EXCEPTION_TREE_REGEX = new Regex("^(?<indentation>\\s*)(?<ExceptionTreeDash>\\s-\\s)(?<EverythingElse>.+)");

	public static IObservable<Squawk> SimPowerSquawks(IObservable<PowerStage> stages, IObservable<ISimRunnerEvolved> runners)
	{
		return stages.Select<PowerStage, Option<Squawk>>((Func<PowerStage, Option<Squawk>>)delegate(PowerStage stage)
		{
			switch (stage)
			{
			case PowerStage.PoweringUp:
				return Prelude.Some(Squawk.CreateIndeterminateProgress("Avionics powering up."));
			case PowerStage.PoweredUp:
				return Squawk.Create("Avionics powered up.");
			case PowerStage.PoweringDown:
				return Squawk.CreateIndeterminateProgress("Avionics powering down.");
			case PowerStage.PoweredDown:
				return Prelude.None;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(stage);
				Option<Squawk> result = default(Option<Squawk>);
				return result;
			}
			}
		}).Merge<Option<Squawk>>(runners.Select<ISimRunnerEvolved, IObservable<Option<Squawk>>>((Func<ISimRunnerEvolved, IObservable<Option<Squawk>>>)((ISimRunnerEvolved r) => r.Sessions.GetUpdateToDateAndThenContinualValues().Select<ISimSessionEvolved, IObservable<Option<Squawk>>>((Func<ISimSessionEvolved, IObservable<Option<Squawk>>>)((ISimSessionEvolved s) => Observable.FromAsync<Option<Squawk>>((Func<System.Threading.Tasks.Task<Option<Squawk>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass1_0._003C_003CSimPowerSquawks_003Eb__7_003Ed))] async () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (s != null)
			{
				try
				{
					await s.Completion.DetachContext();
					return Prelude.Some(Squawk.Create("Avionics powered down."));
				}
				catch (System.Exception ex)
				{
					Squawk squawk = SquawkFlightSessionError(ex);
					string text3 = (s as IAircraftSimRunner)?.Aircraft.AircraftIdentifier.IdentString ?? "Unknown Aircraft";
					Logger.LogFormattedException(ex, "Flight Session for {0}", text3);
					return squawk;
				}
			}
			return Prelude.None;
		})))).Switch<Option<Squawk>>())).Switch<Option<Squawk>>()).Where<Option<Squawk>>((Func<Option<Squawk>, bool>)((Option<Squawk> o) => o.IsSome))
			.Select<Option<Squawk>, Squawk>((Func<Option<Squawk>, Squawk>)((Option<Squawk> o) => o.ValueUnsafe()));
		[CompilerGenerated]
		static string RemoveStartingDash(string squawkMessage)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			if (((Group)PRETTY_EXCEPTION_TREE_REGEX.Match(squawkMessage)).Success)
			{
				Regex pRETTY_EXCEPTION_TREE_REGEX = PRETTY_EXCEPTION_TREE_REGEX;
				string text = squawkMessage;
				object obj = _003C_003Ec._003C_003E9__1_9;
				if (obj == null)
				{
					MatchEvaluator val = (Match m) => ((Capture)m.Groups["indentation"]).Value + ((Capture)m.Groups["EverythingElse"]).Value;
					_003C_003Ec._003C_003E9__1_9 = val;
					obj = (object)val;
				}
				squawkMessage = pRETTY_EXCEPTION_TREE_REGEX.Replace(text, (MatchEvaluator)obj);
			}
			return squawkMessage;
		}
		[CompilerGenerated]
		static Squawk SquawkFlightSessionError(System.Exception flightError)
		{
			string text2 = flightError.FormatExceptionTree().Replace("\t", "    ", (StringComparison)4);
			string[] array = text2.Split('\n', (StringSplitOptions)0);
			if (array.Length == 2)
			{
				text2 = string.Join(" ", Enumerable.Select<string, string>((System.Collections.Generic.IEnumerable<string>)array, (Func<string, string>)((string line) => RemoveStartingDash(line).Trim())));
			}
			text2 = RemoveStartingDash(text2);
			return Squawk.CreateError(text2);
		}
	}

	[CompilerGenerated]
	internal static Squawk _003CSimPowerSquawks_003Eg__SquawkFlightSessionError_007C1_4(System.Exception flightError)
	{
		string text = flightError.FormatExceptionTree().Replace("\t", "    ", (StringComparison)4);
		string[] array = text.Split('\n', (StringSplitOptions)0);
		if (array.Length == 2)
		{
			text = string.Join(" ", Enumerable.Select<string, string>((System.Collections.Generic.IEnumerable<string>)array, (Func<string, string>)((string line) => _003CSimPowerSquawks_003Eg__RemoveStartingDash_007C1_5(line).Trim())));
		}
		text = _003CSimPowerSquawks_003Eg__RemoveStartingDash_007C1_5(text);
		return Squawk.CreateError(text);
	}

	[CompilerGenerated]
	internal static string _003CSimPowerSquawks_003Eg__RemoveStartingDash_007C1_5(string squawkMessage)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		if (((Group)PRETTY_EXCEPTION_TREE_REGEX.Match(squawkMessage)).Success)
		{
			Regex pRETTY_EXCEPTION_TREE_REGEX = PRETTY_EXCEPTION_TREE_REGEX;
			string text = squawkMessage;
			object obj = _003C_003Ec._003C_003E9__1_9;
			if (obj == null)
			{
				MatchEvaluator val = (Match m) => ((Capture)m.Groups["indentation"]).Value + ((Capture)m.Groups["EverythingElse"]).Value;
				_003C_003Ec._003C_003E9__1_9 = val;
				obj = (object)val;
			}
			squawkMessage = pRETTY_EXCEPTION_TREE_REGEX.Replace(text, (MatchEvaluator)obj);
		}
		return squawkMessage;
	}
}
