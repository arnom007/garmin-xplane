using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Domain.JsonParsing;
using LanguageExt;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class TrainerWindowScreenCoordinatesStorage
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		public JToken newModuleData;

		internal void _003CClearStoredScreenCoords_003Eb__0(IPropertyChangeSet<SimInstallData> copy)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "d");
			copy.Mutate(Expression.Lambda<Func<SimInstallData, IReadOnlyJsonDataSource>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), (IReadOnlyJsonDataSource)new ReadOnlyJsonDataSource(newModuleData));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		[StructLayout(3)]
		private struct _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public TrainerWindowScreenCoordinates coords;

			public _003C_003Ec__DisplayClass9_0 _003C_003E4__this;

			private _003C_003Ec__DisplayClass9_1 _003C_003E8__1;

			public CancellationToken cancel;

			private JObject _003CwindowRectangleModuledata_003E5__2;

			private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_0189: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass9_0 _003C_003Ec__DisplayClass9_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaiter<SimInstallData> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01d7;
						}
						_003C_003E8__1 = new _003C_003Ec__DisplayClass9_1();
						_003CwindowRectangleModuledata_003E5__2 = new JObject();
						_003CwindowRectangleModuledata_003E5__2["left"] = coords.Left;
						_003CwindowRectangleModuledata_003E5__2["top"] = coords.Top;
						_003CwindowRectangleModuledata_003E5__2["width"] = coords.Width;
						_003CwindowRectangleModuledata_003E5__2["height"] = coords.Height;
						awaiter2 = _003C_003Ec__DisplayClass9_.sim.SimData.GetUpdatedValue().ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimInstallData>);
						num = (_003C_003E1__state = -1);
					}
					SimInstallData result = awaiter2.GetResult();
					JToken jToken = result.ModuleData.CopyAllData();
					jToken["window_rectangle"] = _003CwindowRectangleModuledata_003E5__2;
					_003C_003E8__1.updatedModuledata = new ReadOnlyJsonDataSource(jToken);
					SimInstallData updateToMake = result.With(delegate(IPropertyChangeSet<SimInstallData> copy)
					{
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Expected O, but got Unknown
						ParameterExpression val2 = Expression.Parameter(typeof(SimInstallData), "d");
						copy.Mutate(Expression.Lambda<Func<SimInstallData, IReadOnlyJsonDataSource>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), (IReadOnlyJsonDataSource)_003C_003E8__1.updatedModuledata);
					});
					((CancellationToken)(ref cancel)).ThrowIfCancellationRequested();
					ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass9_.sim.Update(updateToMake).ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_01d7;
					IL_01d7:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CwindowRectangleModuledata_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CwindowRectangleModuledata_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public ReactiveSimulator sim;

		[AsyncStateMachine(typeof(_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateScreenCoordsSaver_003Eb__0(TrainerWindowScreenCoordinates coords, CancellationToken cancel)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed = default(_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed);
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed.coords = coords;
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed.cancel = cancel;
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed>(ref _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_1
	{
		public ReadOnlyJsonDataSource updatedModuledata;

		internal void _003CCreateScreenCoordsSaver_003Eb__1(IPropertyChangeSet<SimInstallData> copy)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "d");
			copy.Mutate(Expression.Lambda<Func<SimInstallData, IReadOnlyJsonDataSource>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), (IReadOnlyJsonDataSource)updatedModuledata);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CClearStoredScreenCoords_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ReactiveSimulator sim;

		private _003C_003Ec__DisplayClass10_0 _003C_003E8__1;

		public CancellationToken cancel;

		private ConfiguredTaskAwaiter<SimInstallData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<SimInstallData> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_015a;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass10_0();
					awaiter2 = sim.SimData.GetUpdatedValue().ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimInstallData>, _003CClearStoredScreenCoords_003Ed__10>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimInstallData>);
					num = (_003C_003E1__state = -1);
				}
				SimInstallData result = awaiter2.GetResult();
				_003C_003E8__1.newModuleData = result.ModuleData.CopyAllData();
				if (_003C_003E8__1.newModuleData is JObject jObject && jObject.GetValue("window_rectangle", (StringComparison)4) != null)
				{
					jObject.Remove("window_rectangle");
				}
				SimInstallData updateToMake = result.With(delegate(IPropertyChangeSet<SimInstallData> copy)
				{
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Expected O, but got Unknown
					ParameterExpression val2 = Expression.Parameter(typeof(SimInstallData), "d");
					copy.Mutate(Expression.Lambda<Func<SimInstallData, IReadOnlyJsonDataSource>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), (IReadOnlyJsonDataSource)new ReadOnlyJsonDataSource(_003C_003E8__1.newModuleData));
				});
				((CancellationToken)(ref cancel)).ThrowIfCancellationRequested();
				ConfiguredTaskAwaitable val = sim.Update(updateToMake).ConfigureAwait(false);
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CClearStoredScreenCoords_003Ed__10>(ref awaiter, ref this);
					return;
				}
				goto IL_015a;
				IL_015a:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private const string LEFT_PROP_NAME = "left";

	private const string TOP_PROP_NAME = "top";

	private const string WIDTH_PROP_NAME = "width";

	private const string HEIGHT_PROP_NAME = "height";

	private const string ROOT_PROP_NAME = "window_rectangle";

	[field: CompilerGenerated]
	public static JsonParser<TrainerWindowScreenCoordinates> CoordinateObjectParser
	{
		[CompilerGenerated]
		get;
	} = JsonParsers.JsonObjectParser.Bind<JObject, TrainerWindowScreenCoordinates>((Func<JObject, Either<JsonError, TrainerWindowScreenCoordinates>>)delegate(JObject jo)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Validation<JsonError, int> validation = JsonParsers.IntParser.AtRequiredJsonPath(JsonPath.ParsePropertyOrThrow("left"))(jo).ToValidation();
		Validation<JsonError, int> validation2 = JsonParsers.IntParser.AtRequiredJsonPath(JsonPath.ParsePropertyOrThrow("top"))(jo).ToValidation();
		Validation<JsonError, int> validation3 = JsonParsers.IntParser.AtRequiredJsonPath(JsonPath.ParsePropertyOrThrow("width"))(jo).ToValidation();
		Validation<JsonError, int> validation4 = JsonParsers.IntParser.AtRequiredJsonPath(JsonPath.ParsePropertyOrThrow("height"))(jo).ToValidation();
		return new ValueTuple<Validation<JsonError, int>, Validation<JsonError, int>, Validation<JsonError, int>, Validation<JsonError, int>>(validation, validation2, validation3, validation4).Apply<JsonError, int, int, int, int, TrainerWindowScreenCoordinates>((int x, int y, int w, int h) => new TrainerWindowScreenCoordinates(x, y, w, h)).AggregateErrors();
	});


	public static TrainerWindowScreenCoordinates? ParseWindowRect(this SimInstallData simData)
	{
		JToken json = simData.ModuleData.CopyData("window_rectangle") ?? JValue.CreateNull();
		return CoordinateObjectParser(json).ToOption().IfNoneUnsafe((TrainerWindowScreenCoordinates?)null);
	}

	public static StoreSimulatorScreenCoords CreateScreenCoordsSaver(ReactiveSimulator sim)
	{
		_003C_003Ec__DisplayClass9_0 _003C_003Ec__DisplayClass9_ = new _003C_003Ec__DisplayClass9_0();
		_003C_003Ec__DisplayClass9_.sim = sim;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_0._003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed))] (TrainerWindowScreenCoordinates coords, CancellationToken cancel) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass9_0._003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass9_0._003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed);
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass9_;
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed.coords = coords;
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed.cancel = cancel;
			_003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass9_0._003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed>(ref _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateScreenCoordsSaver_003Eb__0_003Ed._003C_003Et__builder)).Task;
		};
	}

	[AsyncStateMachine(typeof(_003CClearStoredScreenCoords_003Ed__10))]
	public static System.Threading.Tasks.Task ClearStoredScreenCoords(ReactiveSimulator sim, CancellationToken cancel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003CClearStoredScreenCoords_003Ed__10 _003CClearStoredScreenCoords_003Ed__ = default(_003CClearStoredScreenCoords_003Ed__10);
		_003CClearStoredScreenCoords_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CClearStoredScreenCoords_003Ed__.sim = sim;
		_003CClearStoredScreenCoords_003Ed__.cancel = cancel;
		_003CClearStoredScreenCoords_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CClearStoredScreenCoords_003Ed__._003C_003Et__builder)).Start<_003CClearStoredScreenCoords_003Ed__10>(ref _003CClearStoredScreenCoords_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CClearStoredScreenCoords_003Ed__._003C_003Et__builder)).Task;
	}
}
