using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;

namespace LauncherCoreLib.Models.CrashReporting;

public class AnonymousUserIdProvider : ICrashReportInstallationIdProvider
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetInstallationId_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public AnonymousUserIdProvider _003C_003E4__this;

		private TaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AnonymousUserIdProvider anonymousUserIdProvider = _003C_003E4__this;
			string result;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					awaiter = anonymousUserIdProvider._lazyInstallGuid.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, _003CGetInstallationId_003Ed__5>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CInitNewInstallGuid_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public IsolatedStorageFile isoStore;

		private Guid _003CinstallGuid_003E5__2;

		private IsolatedStorageFileStream _003CisoFile_003E5__3;

		private StreamWriter _003Csr_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			string result;
			try
			{
				if (num != 0)
				{
					_003CinstallGuid_003E5__2 = Guid.NewGuid();
					_003CisoFile_003E5__3 = isoStore.OpenFile("installguid.txt", (FileMode)((!isoStore.FileExists("installguid.txt")) ? 1 : 5));
				}
				try
				{
					if (num != 0)
					{
						_003Csr_003E5__4 = new StreamWriter((Stream)(object)_003CisoFile_003E5__3);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = ((TextWriter)_003Csr_003E5__4).WriteLineAsync(((object)(Guid)(ref _003CinstallGuid_003E5__2)).ToString()).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CInitNewInstallGuid_003Ed__9>(ref awaiter, ref this);
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
					}
					finally
					{
						if (num < 0 && _003Csr_003E5__4 != null)
						{
							((System.IDisposable)_003Csr_003E5__4).Dispose();
						}
					}
					_003Csr_003E5__4 = null;
				}
				finally
				{
					if (num < 0 && _003CisoFile_003E5__3 != null)
					{
						((System.IDisposable)_003CisoFile_003E5__3).Dispose();
					}
				}
				_003CisoFile_003E5__3 = null;
				result = ((object)(Guid)(ref _003CinstallGuid_003E5__2)).ToString();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CLoadInstallationIdOnce_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		private IsolatedStorageFile _003CisoStore_003E5__2;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00bb: Expected O, but got Unknown
			//IL_00d2: Expected O, but got Unknown
			//IL_00e9: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			string result;
			try
			{
				string text;
				try
				{
					if (num != 0)
					{
						_003CisoStore_003E5__2 = IsolatedStorageFile.GetStore((IsolatedStorageScope)7, (System.Type)null, (System.Type)null);
					}
					try
					{
						ConfiguredTaskAwaiter<string> awaiter;
						if (num != 0)
						{
							awaiter = ReadOrInitInstallGuid(_003CisoStore_003E5__2).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CLoadInstallationIdOnce_003Ed__7>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
							num = (_003C_003E1__state = -1);
						}
						text = awaiter.GetResult();
					}
					finally
					{
						if (num < 0 && _003CisoStore_003E5__2 != null)
						{
							((System.IDisposable)_003CisoStore_003E5__2).Dispose();
						}
					}
					_003CisoStore_003E5__2 = null;
				}
				catch (SecurityException val)
				{
					Logger.LogFormattedException((System.Exception)val, "System security policy appears to not allow apps to store app data.");
					text = "ffff-0000-0000-ffff-000000000001";
				}
				catch (IsolatedStorageException val2)
				{
					Logger.LogFormattedException((System.Exception)val2, "Isolated storage cannot be initialzed or the permissions for the application or calling assembly cannot be determined.");
					text = "ffff-0000-0000-ffff-000000000003";
				}
				catch (IOException val3)
				{
					Logger.LogFormattedException((System.Exception)val3, "OpenFile() failed for some reason. Could be locked by an anti-virus or drive may be out of disk space.");
					text = "ffff-0000-0000-ffff-000000000002";
				}
				catch (System.Exception ex)
				{
					Logger.Log(ex);
					throw new IdProviderException("Unexpected error encountered while persisting or retrieving anonymous reporting ID", ex);
				}
				result = text;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CReadFirstLineFromIsoStore_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public IsolatedStorageFile isoStore;

		public string file;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			string result;
			try
			{
				ConfiguredTaskAwaiter<string> awaiter;
				if (num != 0)
				{
					awaiter = isoStore.OpenFile(file, (FileMode)3).UseAsync<IsolatedStorageFileStream, string>((Func<IsolatedStorageFileStream, System.Threading.Tasks.Task<string>>)([AsyncStateMachine(typeof(_003C_003Ec._003C_003CReadFirstLineFromIsoStore_003Eb__8_0_003Ed))] async (IsolatedStorageFileStream isoFile) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						StreamReader sr = new StreamReader((Stream)(object)isoFile, (Encoding)null, true, -1, true);
						try
						{
							return await ((TextReader)sr).ReadLineAsync().Then<string, string>((Func<string, string>)((string l) => l ?? string.Empty)).DetachContext();
						}
						finally
						{
							((System.IDisposable)sr)?.Dispose();
						}
					})).ConfigureAwait(false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CReadFirstLineFromIsoStore_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CReadOrInitInstallGuid_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public IsolatedStorageFile isoStore;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			string result2;
			try
			{
				ConfiguredTaskAwaiter<string> awaiter;
				string text;
				string result;
				Guid val = default(Guid);
				switch (num)
				{
				default:
					if (isoStore.FileExists("installguid.txt"))
					{
						awaiter = ReadFirstLineFromIsoStore(isoStore, "installguid.txt").DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CReadOrInitInstallGuid_003Ed__10>(ref awaiter, ref this);
							return;
						}
						goto IL_0098;
					}
					awaiter = InitNewInstallGuid(isoStore).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CReadOrInitInstallGuid_003Ed__10>(ref awaiter, ref this);
						return;
					}
					goto IL_0182;
				case 0:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
					goto IL_0098;
				case 1:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
					goto IL_0116;
				case 2:
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
						num = (_003C_003E1__state = -1);
						goto IL_0182;
					}
					IL_0116:
					text = awaiter.GetResult();
					break;
					IL_0182:
					text = awaiter.GetResult();
					break;
					IL_0098:
					result = awaiter.GetResult();
					if (Guid.TryParse(result, ref val))
					{
						text = result;
						break;
					}
					awaiter = InitNewInstallGuid(isoStore).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CReadOrInitInstallGuid_003Ed__10>(ref awaiter, ref this);
						return;
					}
					goto IL_0116;
				}
				result2 = text;
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

	private const string INSTALL_ID_FILE = "installguid.txt";

	private const string ERR_GUID_ISOSTORE_SECURITY = "ffff-0000-0000-ffff-000000000001";

	private const string ERR_GUID_ISOSTORE_IO = "ffff-0000-0000-ffff-000000000002";

	private const string ERR_GUID_ISOSTORE_INIT = "ffff-0000-0000-ffff-000000000003";

	private readonly AsyncLazy<string> _lazyInstallGuid = new AsyncLazy<string>([AsyncStateMachine(typeof(_003C_003Ec._003C_003C_002Dctor_003Eb__11_0_003Ed))] async () => await LoadInstallationIdOnce());

	[AsyncStateMachine(typeof(_003CGetInstallationId_003Ed__5))]
	public async System.Threading.Tasks.Task<string> GetInstallationId()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await _lazyInstallGuid;
	}

	public System.Threading.Tasks.Task<string?> GetUserId()
	{
		return System.Threading.Tasks.Task.FromResult<string>((string)null);
	}

	[AsyncStateMachine(typeof(_003CLoadInstallationIdOnce_003Ed__7))]
	private static async System.Threading.Tasks.Task<string> LoadInstallationIdOnce()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string result;
		try
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore((IsolatedStorageScope)7, (System.Type)null, (System.Type)null);
			try
			{
				result = await ReadOrInitInstallGuid(isoStore).DetachContext();
			}
			finally
			{
				((System.IDisposable)isoStore)?.Dispose();
			}
		}
		catch (SecurityException val)
		{
			Logger.LogFormattedException((System.Exception)val, "System security policy appears to not allow apps to store app data.");
			result = "ffff-0000-0000-ffff-000000000001";
		}
		catch (IsolatedStorageException val2)
		{
			Logger.LogFormattedException((System.Exception)val2, "Isolated storage cannot be initialzed or the permissions for the application or calling assembly cannot be determined.");
			result = "ffff-0000-0000-ffff-000000000003";
		}
		catch (IOException val3)
		{
			Logger.LogFormattedException((System.Exception)val3, "OpenFile() failed for some reason. Could be locked by an anti-virus or drive may be out of disk space.");
			result = "ffff-0000-0000-ffff-000000000002";
		}
		catch (System.Exception ex)
		{
			Logger.Log(ex);
			throw new IdProviderException("Unexpected error encountered while persisting or retrieving anonymous reporting ID", ex);
		}
		return result;
	}

	[AsyncStateMachine(typeof(_003CReadFirstLineFromIsoStore_003Ed__8))]
	private static async System.Threading.Tasks.Task<string> ReadFirstLineFromIsoStore(IsolatedStorageFile isoStore, string file)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await isoStore.OpenFile(file, (FileMode)3).UseAsync<IsolatedStorageFileStream, string>((Func<IsolatedStorageFileStream, System.Threading.Tasks.Task<string>>)([AsyncStateMachine(typeof(_003C_003Ec._003C_003CReadFirstLineFromIsoStore_003Eb__8_0_003Ed))] async (IsolatedStorageFileStream isoFile) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			StreamReader sr = new StreamReader((Stream)(object)isoFile, (Encoding)null, true, -1, true);
			try
			{
				return await ((TextReader)sr).ReadLineAsync().Then<string, string>((Func<string, string>)((string l) => l ?? string.Empty)).DetachContext();
			}
			finally
			{
				((System.IDisposable)sr)?.Dispose();
			}
		})).ConfigureAwait(false);
	}

	[AsyncStateMachine(typeof(_003CInitNewInstallGuid_003Ed__9))]
	private static async System.Threading.Tasks.Task<string> InitNewInstallGuid(IsolatedStorageFile isoStore)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Guid installGuid = Guid.NewGuid();
		IsolatedStorageFileStream isoFile = isoStore.OpenFile("installguid.txt", (FileMode)((!isoStore.FileExists("installguid.txt")) ? 1 : 5));
		try
		{
			StreamWriter sr = new StreamWriter((Stream)(object)isoFile);
			try
			{
				await ((TextWriter)sr).WriteLineAsync(((object)(Guid)(ref installGuid)).ToString()).DetachContext();
			}
			finally
			{
				((System.IDisposable)sr)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)isoFile)?.Dispose();
		}
		return ((object)(Guid)(ref installGuid)).ToString();
	}

	[AsyncStateMachine(typeof(_003CReadOrInitInstallGuid_003Ed__10))]
	private static async System.Threading.Tasks.Task<string> ReadOrInitInstallGuid(IsolatedStorageFile isoStore)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string result;
		if (isoStore.FileExists("installguid.txt"))
		{
			string text = await ReadFirstLineFromIsoStore(isoStore, "installguid.txt").DetachContext();
			Guid val = default(Guid);
			result = ((!Guid.TryParse(text, ref val)) ? (await InitNewInstallGuid(isoStore).DetachContext()) : text);
		}
		else
		{
			result = await InitNewInstallGuid(isoStore).DetachContext();
		}
		return result;
	}
}
