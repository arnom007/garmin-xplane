using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;

namespace LauncherCoreLib.Models.CrashReporting;

public class SystemInfo
{
	private delegate System.Threading.Tasks.Task<List<ManagementBaseObject>> WmiQuery(string queryString, CancellationToken token);

	[Flags]
	private enum DisplayDeviceState
	{
		AttachedToDesktop = 1,
		MultiDriver = 2,
		PrimaryDevice = 4,
		MirroringDriver = 8,
		VGACompatible = 0x10,
		Removable = 0x20,
		ModesPruned = 0x8000000,
		Remote = 0x4000000,
		Disconnect = 0x2000000
	}

	private static class WmiQueries
	{
		public static class Win32_ComputerSystem
		{
			public const string Manufacturer = "Manufacturer";

			public const string Model = "Model";

			public const string QUERY = "Select Manufacturer,Model from Win32_ComputerSystem";
		}

		public static class Win32_VideoController
		{
			public const string DriverVersion = "DriverVersion";

			public const string Name = "Name";

			public const string PNPDeviceID = "PNPDeviceID";

			public const string QUERY = "SELECT DriverVersion,Name,PNPDeviceID FROM Win32_VideoController";
		}

		public static class Win32_PnPSignedDriver
		{
			public const string DeviceID = "DeviceID";

			public const string Manufacturer = "Manufacturer";

			public const string QUERY = "SELECT DeviceID,Manufacturer FROM Win32_PnPSignedDriver";
		}
	}

	[StructLayout(0, CharSet = 4)]
	private struct DISPLAY_DEVICE
	{
		[MarshalAs(8)]
		public int cb;

		[MarshalAs(23, SizeConst = 32)]
		public string DeviceName;

		[MarshalAs(23, SizeConst = 128)]
		public string DeviceString;

		[MarshalAs(8)]
		public DisplayDeviceState StateFlags;

		[MarshalAs(23, SizeConst = 128)]
		public string DeviceID;

		[MarshalAs(23, SizeConst = 128)]
		public string DeviceKey;

		public string ToString()
		{
			return DeviceString;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed : System.Collections.Generic.IEnumerable<DISPLAY_DEVICE>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<DISPLAY_DEVICE>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private DISPLAY_DEVICE _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private uint flags;

		public uint _003C_003E3__flags;

		private DISPLAY_DEVICE _003Cd_003E5__2;

		private uint _003Cid_003E5__3;

		DISPLAY_DEVICE System.Collections.Generic.IEnumerator<DISPLAY_DEVICE>.Current
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
		public _003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed(int _003C_003E1__state)
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
				_003Cd_003E5__2 = default(DISPLAY_DEVICE);
				_003Cd_003E5__2.cb = Marshal.SizeOf<DISPLAY_DEVICE>(_003Cd_003E5__2);
				_003Cid_003E5__3 = 0u;
				break;
			case 1:
				_003C_003E1__state = -1;
				_003Cd_003E5__2.cb = Marshal.SizeOf<DISPLAY_DEVICE>(_003Cd_003E5__2);
				_003Cid_003E5__3++;
				break;
			}
			if (EnumDisplayDevices(null, _003Cid_003E5__3, ref _003Cd_003E5__2, flags))
			{
				_003C_003E2__current = _003Cd_003E5__2;
				_003C_003E1__state = 1;
				return true;
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
		System.Collections.Generic.IEnumerator<DISPLAY_DEVICE> System.Collections.Generic.IEnumerable<DISPLAY_DEVICE>.GetEnumerator()
		{
			_003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed _003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed = this;
			}
			else
			{
				_003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed = new _003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed(0);
			}
			_003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed.flags = _003C_003E3__flags;
			return _003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<DISPLAY_DEVICE>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_0
	{
		public string primaryDisplayName;

		internal bool _003CQuery_003Eb__0(DisplayDeviceInfo d)
		{
			return d.AdapterName == primaryDisplayName;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_0
	{
		public ILookup<string, ManagementObject> driversByPnpDeviceId;

		internal DisplayDeviceInfo _003CGetDisplayAdapters_003Eb__1(ManagementObject mo)
		{
			return new DisplayDeviceInfo(MoString(mo["Name"]), _003CGetDisplayAdapters_003Eg__Manufacturer_007C16_2(driversByPnpDeviceId, mo), MoString(mo["DriverVersion"]));
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetComputerInfo_003Ed__15 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ValueTuple<string, string>> _003C_003Et__builder;

		public CancellationToken cancel;

		private ConfiguredTaskAwaiter<List<ManagementBaseObject>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ValueTuple<string, string> result2;
			try
			{
				ConfiguredTaskAwaiter<List<ManagementBaseObject>> awaiter;
				if (num != 0)
				{
					awaiter = QueryWmiAsync.Apply<WmiQuery, WmiQuery>(LogQueryWithError).Apply<WmiQuery, WmiQuery>(ToNoResultsOnManagementError)("Select Manufacturer,Model from Win32_ComputerSystem", cancel).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<List<ManagementBaseObject>>, _003CGetComputerInfo_003Ed__15>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<List<ManagementBaseObject>>);
					num = (_003C_003E1__state = -1);
				}
				List<ManagementBaseObject> result = awaiter.GetResult();
				if (result.Count > 0)
				{
					ManagementObject managementObject = (ManagementObject)result[0];
					result2 = new ValueTuple<string, string>(MoString(managementObject["Manufacturer"]), MoString(managementObject["Model"]));
				}
				else
				{
					result2 = new ValueTuple<string, string>("Unknown Manufacturer", "Unknown Model");
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetDisplayAdapters_003Ed__16 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>> _003C_003Et__builder;

		public CancellationToken cancel;

		private _003C_003Ec__DisplayClass16_0 _003C_003E8__1;

		private System.Threading.Tasks.Task<List<ManagementBaseObject>> _003CdriverQueryTask_003E5__2;

		private List<ManagementBaseObject> _003Ccontrollers_003E5__3;

		private ConfiguredTaskAwaiter<List<ManagementBaseObject>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo> result2;
			try
			{
				ConfiguredTaskAwaiter<List<ManagementBaseObject>> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<List<ManagementBaseObject>>);
						num = (_003C_003E1__state = -1);
						goto IL_0167;
					}
					System.Threading.Tasks.Task<List<ManagementBaseObject>> t = QueryWmiAsync.Apply<WmiQuery, WmiQuery>(LogQueryWithError).Apply<WmiQuery, WmiQuery>(ToNoResultsOnManagementError)("SELECT DriverVersion,Name,PNPDeviceID FROM Win32_VideoController", cancel);
					_003CdriverQueryTask_003E5__2 = QueryWmiAsync.Apply<WmiQuery, WmiQuery>(LogQueryWithError).Apply<WmiQuery, WmiQuery>(ToNoResultsOnManagementError)("SELECT DeviceID,Manufacturer FROM Win32_PnPSignedDriver", cancel);
					awaiter = t.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<List<ManagementBaseObject>>, _003CGetDisplayAdapters_003Ed__16>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<List<ManagementBaseObject>>);
					num = (_003C_003E1__state = -1);
				}
				List<ManagementBaseObject> result = awaiter.GetResult();
				_003Ccontrollers_003E5__3 = result;
				if (_003Ccontrollers_003E5__3.Count > 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass16_0();
					awaiter = _003CdriverQueryTask_003E5__2.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<List<ManagementBaseObject>>, _003CGetDisplayAdapters_003Ed__16>(ref awaiter, ref this);
						return;
					}
					goto IL_0167;
				}
				Logger.Log(LogLevel.Warning, "No Display adapter results. Maybe the WMI query is not supported on this version of Windows?");
				result2 = System.Array.Empty<DisplayDeviceInfo>();
				goto end_IL_0007;
				IL_0167:
				List<ManagementBaseObject> result3 = awaiter.GetResult();
				_003C_003E8__1.driversByPnpDeviceId = Enumerable.ToLookup<ManagementObject, string>(Enumerable.OfType<ManagementObject>((System.Collections.IEnumerable)result3), (Func<ManagementObject, string>)((ManagementObject mo) => MoString(mo["DeviceID"])));
				result2 = (System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>)Enumerable.ToList<DisplayDeviceInfo>(Enumerable.Select<ManagementObject, DisplayDeviceInfo>(Enumerable.OfType<ManagementObject>((System.Collections.IEnumerable)_003Ccontrollers_003E5__3), (Func<ManagementObject, DisplayDeviceInfo>)((ManagementObject mo) => new DisplayDeviceInfo(MoString(mo["Name"]), _003CGetDisplayAdapters_003Eg__Manufacturer_007C16_2(_003C_003E8__1.driversByPnpDeviceId, mo), MoString(mo["DriverVersion"])))));
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CdriverQueryTask_003E5__2 = null;
				_003Ccontrollers_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CdriverQueryTask_003E5__2 = null;
			_003Ccontrollers_003E5__3 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CQuery_003Ed__13 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SystemInfo> _003C_003Et__builder;

		public CancellationToken cancel;

		private _003C_003Ec__DisplayClass13_0 _003C_003E8__1;

		private System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>> _003CdisplaysTask_003E5__2;

		private ValueTuple<string, string> _003CcomputerInfo_003E5__3;

		private ConfiguredTaskAwaiter<ValueTuple<string, string>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SystemInfo result3;
			try
			{
				ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>> awaiter;
				ConfiguredTaskAwaiter<ValueTuple<string, string>> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>>);
						num = (_003C_003E1__state = -1);
						goto IL_0111;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass13_0();
					System.Threading.Tasks.Task<ValueTuple<string, string>> computerInfo = GetComputerInfo(cancel);
					_003CdisplaysTask_003E5__2 = GetDisplayAdapters(cancel);
					_003C_003E8__1.primaryDisplayName = GetPrimaryDisplayDeviceName();
					awaiter2 = computerInfo.DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<string, string>>, _003CQuery_003Ed__13>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ValueTuple<string, string>>);
					num = (_003C_003E1__state = -1);
				}
				ValueTuple<string, string> result = awaiter2.GetResult();
				_003CcomputerInfo_003E5__3 = result;
				awaiter = _003CdisplaysTask_003E5__2.DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>>, _003CQuery_003Ed__13>(ref awaiter, ref this);
					return;
				}
				goto IL_0111;
				IL_0111:
				System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo> result2 = awaiter.GetResult();
				result2 = (System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>)Enumerable.ToList<DisplayDeviceInfo>((System.Collections.Generic.IEnumerable<DisplayDeviceInfo>)Enumerable.OrderByDescending<DisplayDeviceInfo, bool>((System.Collections.Generic.IEnumerable<DisplayDeviceInfo>)result2, (Func<DisplayDeviceInfo, bool>)((DisplayDeviceInfo d) => d.AdapterName == _003C_003E8__1.primaryDisplayName)));
				result3 = new SystemInfo(_003CcomputerInfo_003E5__3.Item1, _003CcomputerInfo_003E5__3.Item2, result2);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CdisplaysTask_003E5__2 = null;
				_003CcomputerInfo_003E5__3 = default(ValueTuple<string, string>);
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CdisplaysTask_003E5__2 = null;
			_003CcomputerInfo_003E5__3 = default(ValueTuple<string, string>);
			_003C_003Et__builder.SetResult(result3);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[field: CompilerGenerated]
	public string Manufacturer
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string Model
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo> Displays
	{
		[CompilerGenerated]
		get;
	}

	private static WmiQuery QueryWmiAsync => [AsyncStateMachine(typeof(_003C_003Ec._003C_003Cget_QueryWmiAsync_003Eb__11_0_003Ed))] async (string queryString, CancellationToken cancel) =>
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ManagementObjectSearcher searcher = new ManagementObjectSearcher(new SelectQuery(queryString));
		try
		{
			ManagementOperationObserver observer = new ManagementOperationObserver();
			List<ManagementBaseObject> records = new List<ManagementBaseObject>();
			TaskCompletionSource<object?> queryComplete = new TaskCompletionSource<object>();
			observer.ObjectReady += delegate(object s, ObjectReadyEventArgs e)
			{
				records.Add(e.NewObject);
			};
			observer.Completed += delegate(object s, CompletedEventArgs e)
			{
				if (e.Status == ManagementStatus.NoError)
				{
					queryComplete.SetResult((object)null);
				}
				else if (((CancellationToken)(ref cancel)).CanBeCanceled && ((CancellationToken)(ref cancel)).IsCancellationRequested)
				{
					queryComplete.SetCanceled();
				}
				else
				{
					queryComplete.SetException((System.Exception)(object)new ManagementException(((object)e.Status).ToString()));
				}
			};
			searcher.Get(observer);
			CancellationTokenRegistration val = ((CancellationToken)(ref cancel)).Register((Action)delegate
			{
				observer.Cancel();
			}, false);
			try
			{
				await queryComplete.Task.DetachContext();
				return records;
			}
			finally
			{
				((System.IDisposable)(CancellationTokenRegistration)(ref val)).Dispose();
			}
		}
		finally
		{
			((System.IDisposable)searcher)?.Dispose();
		}
	};

	private SystemInfo(string manufacturer, string model, System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo> displays)
	{
		Manufacturer = manufacturer;
		Model = model;
		Displays = displays;
	}

	[AsyncStateMachine(typeof(_003CQuery_003Ed__13))]
	public static async System.Threading.Tasks.Task<SystemInfo> Query(CancellationToken cancel = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		System.Threading.Tasks.Task<ValueTuple<string, string>> computerInfo2 = GetComputerInfo(cancel);
		System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>> displaysTask = GetDisplayAdapters(cancel);
		string primaryDisplayName = GetPrimaryDisplayDeviceName();
		ValueTuple<string, string> computerInfo = await computerInfo2.DetachContext();
		System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo> displays = (System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>)Enumerable.ToList<DisplayDeviceInfo>((System.Collections.Generic.IEnumerable<DisplayDeviceInfo>)Enumerable.OrderByDescending<DisplayDeviceInfo, bool>((System.Collections.Generic.IEnumerable<DisplayDeviceInfo>)(await displaysTask.DetachContext()), (Func<DisplayDeviceInfo, bool>)((DisplayDeviceInfo d) => d.AdapterName == primaryDisplayName)));
		return new SystemInfo(computerInfo.Item1, computerInfo.Item2, displays);
	}

	public static string MoString(object mo)
	{
		return mo.ToString() ?? string.Empty;
	}

	[AsyncStateMachine(typeof(_003CGetComputerInfo_003Ed__15))]
	private static async System.Threading.Tasks.Task<ValueTuple<string, string>> GetComputerInfo(CancellationToken cancel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		List<ManagementBaseObject> val = await QueryWmiAsync.Apply<WmiQuery, WmiQuery>(LogQueryWithError).Apply<WmiQuery, WmiQuery>(ToNoResultsOnManagementError)("Select Manufacturer,Model from Win32_ComputerSystem", cancel).DetachContext();
		if (val.Count > 0)
		{
			ManagementObject managementObject = (ManagementObject)val[0];
			return new ValueTuple<string, string>(MoString(managementObject["Manufacturer"]), MoString(managementObject["Model"]));
		}
		return new ValueTuple<string, string>("Unknown Manufacturer", "Unknown Model");
	}

	[AsyncStateMachine(typeof(_003CGetDisplayAdapters_003Ed__16))]
	private static async System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>> GetDisplayAdapters(CancellationToken cancel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		System.Threading.Tasks.Task<List<ManagementBaseObject>> t = QueryWmiAsync.Apply<WmiQuery, WmiQuery>(LogQueryWithError).Apply<WmiQuery, WmiQuery>(ToNoResultsOnManagementError)("SELECT DriverVersion,Name,PNPDeviceID FROM Win32_VideoController", cancel);
		System.Threading.Tasks.Task<List<ManagementBaseObject>> driverQueryTask = QueryWmiAsync.Apply<WmiQuery, WmiQuery>(LogQueryWithError).Apply<WmiQuery, WmiQuery>(ToNoResultsOnManagementError)("SELECT DeviceID,Manufacturer FROM Win32_PnPSignedDriver", cancel);
		List<ManagementBaseObject> controllers = await t.DetachContext();
		if (controllers.Count > 0)
		{
			ILookup<string, ManagementObject> driversByPnpDeviceId = Enumerable.ToLookup<ManagementObject, string>(Enumerable.OfType<ManagementObject>((System.Collections.IEnumerable)(await driverQueryTask.DetachContext())), (Func<ManagementObject, string>)((ManagementObject mo) => MoString(mo["DeviceID"])));
			return (System.Collections.Generic.IReadOnlyList<DisplayDeviceInfo>)Enumerable.ToList<DisplayDeviceInfo>(Enumerable.Select<ManagementObject, DisplayDeviceInfo>(Enumerable.OfType<ManagementObject>((System.Collections.IEnumerable)controllers), (Func<ManagementObject, DisplayDeviceInfo>)((ManagementObject mo) => new DisplayDeviceInfo(MoString(mo["Name"]), Manufacturer(driversByPnpDeviceId, mo), MoString(mo["DriverVersion"])))));
		}
		Logger.Log(LogLevel.Warning, "No Display adapter results. Maybe the WMI query is not supported on this version of Windows?");
		return System.Array.Empty<DisplayDeviceInfo>();
		[CompilerGenerated]
		static string Manufacturer(ILookup<string, ManagementObject> driverLookup, ManagementObject controller)
		{
			string text = MoString(controller["PNPDeviceID"]);
			return Enumerable.FirstOrDefault<ManagementObject>(driverLookup[text])?.GetPropertyValue("Manufacturer").ToString() ?? "Unknown Manufacturer";
		}
	}

	private static string? GetPrimaryDisplayDeviceName()
	{
		DISPLAY_DEVICE dISPLAY_DEVICE = Enumerable.FirstOrDefault<DISPLAY_DEVICE>((System.Collections.Generic.IEnumerable<DISPLAY_DEVICE>)Enumerable.ToList<DISPLAY_DEVICE>(EnumerateDeviceStructs(1u)), (Func<DISPLAY_DEVICE, bool>)((DISPLAY_DEVICE dd) => ((System.Enum)dd.StateFlags).HasFlag((System.Enum)DisplayDeviceState.PrimaryDevice)));
		if (dISPLAY_DEVICE.cb == 0)
		{
			Logger.Log(LogLevel.Warning, "No primary display found.");
			return null;
		}
		return dISPLAY_DEVICE.DeviceString;
		[IteratorStateMachine(typeof(_003C_003CGetPrimaryDisplayDeviceName_003Eg__EnumerateDeviceStructs_007C17_1_003Ed))]
		[CompilerGenerated]
		static System.Collections.Generic.IEnumerable<DISPLAY_DEVICE> EnumerateDeviceStructs(uint flags)
		{
			DISPLAY_DEVICE d = default(DISPLAY_DEVICE);
			d.cb = Marshal.SizeOf<DISPLAY_DEVICE>(d);
			for (uint id = 0u; EnumDisplayDevices(null, id, ref d, flags); id++)
			{
				yield return d;
				d.cb = Marshal.SizeOf<DISPLAY_DEVICE>(d);
			}
		}
	}

	private static WmiQuery LogQueryWithError(WmiQuery inner)
	{
		WmiQuery inner2 = inner;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass18_0._003C_003CLogQueryWithError_003Eb__0_003Ed))] async (string queryString, CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				return await inner2(queryString, t).ConfigureAwait(false);
			}
			catch (System.Exception ex)
			{
				Logger.LogFormattedException(ex, "WMI Query: '" + queryString + "'");
				AppCenterPropertyName appCenterPropertyName = AppCenterPropertyName.ParseAndThrowOnErrors("Exception_Type");
				TrackedEvent.Log(InlineEventDataDescriptor.Create(TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("WMI_ERROR"), LauncherAppTarget.Instance), EventProperties.FromEnumerable(new KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>[4]
				{
					KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("Query"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(queryString)),
					KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(appCenterPropertyName, AppCenterPropertyValue.ParseTruncateFromBeginningAndHandleErrors(ex.GetType().FullName ?? "Unknown")),
					KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("Exception_Message"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(ex.Message)),
					KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("Combined"), AppCenterPropertyValue.ParseTruncateAndHandleErrors($"'{queryString}'->{ex}"))
				}), EventUserDataCensor.CensorProperties(appCenterPropertyName).Invert()));
				throw;
			}
		};
	}

	private static WmiQuery ToNoResultsOnManagementError(WmiQuery inner)
	{
		WmiQuery inner2 = inner;
		return [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass19_0._003C_003CToNoResultsOnManagementError_003Eb__0_003Ed))] async (string query, CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				return await inner2(query, t).ConfigureAwait(false);
			}
			catch (ManagementException)
			{
				return new List<ManagementBaseObject>();
			}
		};
	}

	[DllImport("user32.dll", CharSet = 3)]
	[DefaultDllImportSearchPaths(/*Could not decode attribute arguments.*/)]
	private static extern bool EnumDisplayDevices(string? lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

	[CompilerGenerated]
	internal static string _003CGetDisplayAdapters_003Eg__Manufacturer_007C16_2(ILookup<string, ManagementObject> driverLookup, ManagementObject controller)
	{
		string text = MoString(controller["PNPDeviceID"]);
		return Enumerable.FirstOrDefault<ManagementObject>(driverLookup[text])?.GetPropertyValue("Manufacturer").ToString() ?? "Unknown Manufacturer";
	}
}
