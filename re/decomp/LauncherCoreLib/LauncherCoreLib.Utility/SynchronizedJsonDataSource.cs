using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;

namespace LauncherCoreLib.Utility;

public class SynchronizedJsonDataSource : ISynchronizedJsonDataSource
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetValue_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<JToken> _003C_003Et__builder;

		public SynchronizedJsonDataSource _003C_003E4__this;

		public string jsonPath;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SynchronizedJsonDataSource synchronizedJsonDataSource = _003C_003E4__this;
			JToken result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					awaiter = synchronizedJsonDataSource._rwMutex.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetValue_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				try
				{
					result2 = CopyPathFromToken(jsonPath, synchronizedJsonDataSource._root);
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
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
	private struct _003CStoreValue_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public string jsonPath;

		public JToken value;

		public SynchronizedJsonDataSource _003C_003E4__this;

		private JToken _003Ccopy_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SynchronizedJsonDataSource synchronizedJsonDataSource = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					ThrowIfPathIsNotSupported(jsonPath);
					_003Ccopy_003E5__2 = CopyToken(value);
					awaiter = synchronizedJsonDataSource._rwMutex.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CStoreValue_003Ed__10>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				try
				{
					synchronizedJsonDataSource.InsertOrReplaceTokenAtPathWithLock(_003Ccopy_003E5__2, jsonPath);
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				synchronizedJsonDataSource.RaiseValueUpdate(jsonPath, _003Ccopy_003E5__2);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Ccopy_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Ccopy_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly AsyncReaderWriterLock _rwMutex = new AsyncReaderWriterLock();

	private readonly ISubject<JsonDataUpdate> _valueUpdates = new Subject<JsonDataUpdate>();

	private JToken? _root;

	public IObservable<JsonDataUpdate> WhenValuesAreUpdated => ((IObservable<JsonDataUpdate>)_valueUpdates).AsObservable<JsonDataUpdate>();

	public SynchronizedJsonDataSource()
	{
		_root = null;
	}

	public SynchronizedJsonDataSource(JToken manageToken)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!(manageToken is JObject))
		{
			throw new NotSupportedException(((MemberInfo)((object)manageToken).GetType()).Name + " are not currently supported as a root container.");
		}
		_root = CopyToken(manageToken);
	}

	public SynchronizedJsonDataSource(IReadOnlyJsonDataSource readonlySource)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (readonlySource == null)
		{
			throw new ArgumentNullException("readonlySource");
		}
		_root = readonlySource.CopyData(string.Empty);
	}

	[AsyncStateMachine(typeof(_003CGetValue_003Ed__8))]
	public async System.Threading.Tasks.Task<JToken?> GetValue(string jsonPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		using (await _rwMutex.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			return CopyPathFromToken(jsonPath, _root);
		}
	}

	public JToken? GetValueSync(string jsonPath)
	{
		using (_rwMutex.ReaderLock())
		{
			return CopyPathFromToken(jsonPath, _root);
		}
	}

	[AsyncStateMachine(typeof(_003CStoreValue_003Ed__10))]
	public System.Threading.Tasks.Task StoreValue(string jsonPath, JToken? value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CStoreValue_003Ed__10 _003CStoreValue_003Ed__ = default(_003CStoreValue_003Ed__10);
		_003CStoreValue_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CStoreValue_003Ed__._003C_003E4__this = this;
		_003CStoreValue_003Ed__.jsonPath = jsonPath;
		_003CStoreValue_003Ed__.value = value;
		_003CStoreValue_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CStoreValue_003Ed__._003C_003Et__builder)).Start<_003CStoreValue_003Ed__10>(ref _003CStoreValue_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CStoreValue_003Ed__._003C_003Et__builder)).Task;
	}

	public void StoreValueSync(string jsonPath, JToken? value)
	{
		ThrowIfPathIsNotSupported(jsonPath);
		JToken copy = CopyToken(value);
		using (_rwMutex.WriterLock())
		{
			InsertOrReplaceTokenAtPathWithLock(copy, jsonPath);
		}
		RaiseValueUpdate(jsonPath, copy);
	}

	private static void ThrowIfPathIsNotSupported(string jsonPath)
	{
		JsonExtensions.ThrowIfPathIsNotSupported(jsonPath);
	}

	private static JToken? CopyToken(JToken? token)
	{
		return token?.DeepClone();
	}

	private static JToken? CopyPathFromToken(string path, JToken? root)
	{
		return CopyToken(root?.SelectToken(path ?? string.Empty));
	}

	private void InsertOrReplaceTokenAtPathWithLock(JToken? copy, string jsonPath)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		jsonPath = JsonExtensions.ThrowIfPathIsNotSupported(jsonPath);
		if (jsonPath.Length > 0)
		{
			_root = _root.CreateObjectsForPath(jsonPath);
			(_root.SelectToken(jsonPath) ?? throw new KeyNotFoundException("The JSON path (" + jsonPath + ") was not created.")).Replace(copy);
		}
		else
		{
			_root = copy;
		}
	}

	private void RaiseValueUpdate(string moduleDataJsonPath, JToken? copy)
	{
		((IObserver<JsonDataUpdate>)_valueUpdates).OnNext(new JsonDataUpdate(moduleDataJsonPath, new ReadOnlyJsonDataSource(copy)));
	}
}
