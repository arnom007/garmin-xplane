using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class PersistedSessionCounter : IFlightSessionCounter
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetAndIncrementSessionCount_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<int> _003C_003Et__builder;

		private int _003CsessionCount_003E5__2;

		private IsolatedStorageFile _003Ciso_store_003E5__3;

		private ConfiguredTaskAwaiter<int> _003C_003Eu__1;

		private IsolatedStorageFileStream _003Ciso_file_003E5__4;

		private StreamWriter _003Csr_003E5__5;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Expected O, but got Unknown
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			int result2;
			try
			{
				if ((uint)num > 1u)
				{
					_003CsessionCount_003E5__2 = 0;
				}
				try
				{
					if ((uint)num > 1u)
					{
						_003Ciso_store_003E5__3 = OpenIsolatedStorage();
					}
					try
					{
						ConfiguredTaskAwaiter<int> awaiter;
						if (num != 0)
						{
							if (num == 1)
							{
								goto IL_00af;
							}
							awaiter = GetCountFromStorage(_003Ciso_store_003E5__3).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<int>, _003CGetAndIncrementSessionCount_003Ed__2>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<int>);
							num = (_003C_003E1__state = -1);
						}
						int result = awaiter.GetResult();
						_003CsessionCount_003E5__2 = result;
						_003CsessionCount_003E5__2++;
						goto IL_00af;
						IL_00af:
						try
						{
							if (num != 1)
							{
								_003Ciso_file_003E5__4 = _003Ciso_store_003E5__3.OpenFile("sess_count.txt", (FileMode)((!_003Ciso_store_003E5__3.FileExists("sess_count.txt")) ? 1 : 5));
							}
							try
							{
								if (num != 1)
								{
									_003Csr_003E5__5 = new StreamWriter((Stream)(object)_003Ciso_file_003E5__4);
								}
								try
								{
									ConfiguredTaskAwaiter awaiter2;
									if (num != 1)
									{
										ConfiguredTaskAwaitable val = ((TextWriter)_003Csr_003E5__5).WriteLineAsync(_003CsessionCount_003E5__2.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)).DetachContext();
										awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
										if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
										{
											num = (_003C_003E1__state = 1);
											_003C_003Eu__2 = awaiter2;
											_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CGetAndIncrementSessionCount_003Ed__2>(ref awaiter2, ref this);
											return;
										}
									}
									else
									{
										awaiter2 = _003C_003Eu__2;
										_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
										num = (_003C_003E1__state = -1);
									}
									((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
								}
								finally
								{
									if (num < 0 && _003Csr_003E5__5 != null)
									{
										((System.IDisposable)_003Csr_003E5__5).Dispose();
									}
								}
								_003Csr_003E5__5 = null;
							}
							finally
							{
								if (num < 0 && _003Ciso_file_003E5__4 != null)
								{
									((System.IDisposable)_003Ciso_file_003E5__4).Dispose();
								}
							}
							_003Ciso_file_003E5__4 = null;
						}
						catch (object obj) when (((Func<bool>)delegate
						{
							// Could not convert BlockContainer to single expression
							object obj2 = ((obj is IOException) ? obj : null);
							return obj2 != null && ((IOException)obj2).IsDiskFull();
						}).Invoke())
						{
							_003CsessionCount_003E5__2 = 2147483647;
							Logger.Log(LogLevel.Warning, $"Out of disk space. Cannot increment session counter. Using reserved value ({_003CsessionCount_003E5__2}).");
						}
					}
					finally
					{
						if (num < 0 && _003Ciso_store_003E5__3 != null)
						{
							((System.IDisposable)_003Ciso_store_003E5__3).Dispose();
						}
					}
					_003Ciso_store_003E5__3 = null;
				}
				catch (System.Exception e)
				{
					Logger.Log(e);
					throw;
				}
				result2 = _003CsessionCount_003E5__2;
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
	private struct _003CGetCountFromStorage_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<int> _003C_003Et__builder;

		public IsolatedStorageFile iso_store;

		private int _003CsessionCount_003E5__2;

		private IsolatedStorageFileStream _003Ciso_file_003E5__3;

		private StreamReader _003Csr_003E5__4;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			int result;
			try
			{
				if (num != 0)
				{
					_003CsessionCount_003E5__2 = 0;
					if (!iso_store.FileExists("sess_count.txt"))
					{
						goto IL_011d;
					}
				}
				try
				{
					if (num != 0)
					{
						_003Ciso_file_003E5__3 = iso_store.OpenFile("sess_count.txt", (FileMode)3);
					}
					try
					{
						if (num != 0)
						{
							_003Csr_003E5__4 = new StreamReader((Stream)(object)_003Ciso_file_003E5__3);
						}
						try
						{
							ConfiguredTaskAwaiter<string> awaiter;
							if (num != 0)
							{
								awaiter = ((TextReader)_003Csr_003E5__4).ReadLineAsync().DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CGetCountFromStorage_003Ed__4>(ref awaiter, ref this);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
								num = (_003C_003E1__state = -1);
							}
							if (!int.TryParse(awaiter.GetResult(), ref _003CsessionCount_003E5__2))
							{
								_003CsessionCount_003E5__2 = 0;
							}
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
						if (num < 0 && _003Ciso_file_003E5__3 != null)
						{
							((System.IDisposable)_003Ciso_file_003E5__3).Dispose();
						}
					}
					_003Ciso_file_003E5__3 = null;
				}
				catch (FileNotFoundException)
				{
				}
				goto IL_011d;
				IL_011d:
				result = _003CsessionCount_003E5__2;
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
	private struct _003CGetSessionCount_003Ed__1 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<int> _003C_003Et__builder;

		private IsolatedStorageFile _003Ciso_store_003E5__2;

		private ConfiguredTaskAwaiter<int> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			int result;
			try
			{
				int num2;
				if (num != 0)
				{
					num2 = 0;
					_003Ciso_store_003E5__2 = OpenIsolatedStorage();
				}
				try
				{
					ConfiguredTaskAwaiter<int> awaiter;
					if (num != 0)
					{
						awaiter = GetCountFromStorage(_003Ciso_store_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<int>, _003CGetSessionCount_003Ed__1>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<int>);
						num = (_003C_003E1__state = -1);
					}
					num2 = awaiter.GetResult();
				}
				finally
				{
					if (num < 0 && _003Ciso_store_003E5__2 != null)
					{
						((System.IDisposable)_003Ciso_store_003E5__2).Dispose();
					}
				}
				_003Ciso_store_003E5__2 = null;
				result = num2;
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

	private const string SESS_CNT_FILE = "sess_count.txt";

	[AsyncStateMachine(typeof(_003CGetSessionCount_003Ed__1))]
	public async System.Threading.Tasks.Task<int> GetSessionCount()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		IsolatedStorageFile iso_store = OpenIsolatedStorage();
		try
		{
			result = await GetCountFromStorage(iso_store).DetachContext();
		}
		finally
		{
			((System.IDisposable)iso_store)?.Dispose();
		}
		return result;
	}

	[AsyncStateMachine(typeof(_003CGetAndIncrementSessionCount_003Ed__2))]
	public async System.Threading.Tasks.Task<int> GetAndIncrementSessionCount()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		int sessionCount = 0;
		try
		{
			IsolatedStorageFile iso_store = OpenIsolatedStorage();
			try
			{
				sessionCount = await GetCountFromStorage(iso_store).DetachContext();
				sessionCount++;
				try
				{
					IsolatedStorageFileStream iso_file = iso_store.OpenFile("sess_count.txt", (FileMode)((!iso_store.FileExists("sess_count.txt")) ? 1 : 5));
					try
					{
						StreamWriter sr = new StreamWriter((Stream)(object)iso_file);
						try
						{
							await ((TextWriter)sr).WriteLineAsync(sessionCount.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)).DetachContext();
						}
						finally
						{
							((System.IDisposable)sr)?.Dispose();
						}
					}
					finally
					{
						((System.IDisposable)iso_file)?.Dispose();
					}
				}
				catch (object obj) when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					object obj2 = ((obj is IOException) ? obj : null);
					return obj2 != null && ((IOException)obj2).IsDiskFull();
				}).Invoke())
				{
					sessionCount = 2147483647;
					Logger.Log(LogLevel.Warning, $"Out of disk space. Cannot increment session counter. Using reserved value ({sessionCount}).");
				}
			}
			finally
			{
				((System.IDisposable)iso_store)?.Dispose();
			}
			return sessionCount;
		}
		catch (System.Exception e)
		{
			Logger.Log(e);
			throw;
		}
	}

	private static IsolatedStorageFile OpenIsolatedStorage()
	{
		return IsolatedStorageFile.GetStore((IsolatedStorageScope)7, (System.Type)null, (System.Type)null);
	}

	[AsyncStateMachine(typeof(_003CGetCountFromStorage_003Ed__4))]
	private static async System.Threading.Tasks.Task<int> GetCountFromStorage(IsolatedStorageFile iso_store)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		int sessionCount = 0;
		if (iso_store.FileExists("sess_count.txt"))
		{
			try
			{
				IsolatedStorageFileStream iso_file = iso_store.OpenFile("sess_count.txt", (FileMode)3);
				try
				{
					StreamReader sr = new StreamReader((Stream)(object)iso_file);
					try
					{
						if (!int.TryParse(await ((TextReader)sr).ReadLineAsync().DetachContext(), ref sessionCount))
						{
							sessionCount = 0;
						}
					}
					finally
					{
						((System.IDisposable)sr)?.Dispose();
					}
				}
				finally
				{
					((System.IDisposable)iso_file)?.Dispose();
				}
			}
			catch (FileNotFoundException)
			{
			}
		}
		return sessionCount;
	}
}
