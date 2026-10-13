using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.CrashReporting;

public abstract class CrashReportStreamArgument : CrashReportArgument
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CLoadIntoMemory_003Ed__14 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<byte[]> _003C_003Et__builder;

		public CrashReportStreamArgument _003C_003E4__this;

		private Stream _003Cstream_003E5__2;

		private byte[] _003Cbytes_003E5__3;

		private ConfiguredValueTaskAwaiter _003C_003Eu__1;

		private MemoryStream _003Cmemory_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CrashReportStreamArgument crashReportStreamArgument = _003C_003E4__this;
			byte[] result;
			try
			{
				ConfiguredValueTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredValueTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00bf;
				}
				ConfiguredValueTaskAwaitable val2;
				if ((uint)(num - 1) > 1u)
				{
					_003Cstream_003E5__2 = crashReportStreamArgument.OpenStream();
					Stream obj = _003Cstream_003E5__2;
					MemoryStream val = (MemoryStream)(object)((obj is MemoryStream) ? obj : null);
					if (val != null)
					{
						_003Cbytes_003E5__3 = val.ToArray();
						if (crashReportStreamArgument.DisposeAfterSent(_003Cstream_003E5__2))
						{
							val2 = _003Cstream_003E5__2.DisposeAsync().ConfigureAwait(false);
							awaiter = ((ConfiguredValueTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredValueTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaiter, _003CLoadIntoMemory_003Ed__14>(ref awaiter, ref this);
								return;
							}
							goto IL_00bf;
						}
						goto IL_00c6;
					}
					_003Cmemory_003E5__4 = new MemoryStream();
				}
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01d3;
						}
						ConfiguredTaskAwaitable val3 = _003Cstream_003E5__2.CopyToAsync((Stream)(object)_003Cmemory_003E5__4).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val3)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CLoadIntoMemory_003Ed__14>(ref awaiter2, ref this);
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
					if (crashReportStreamArgument.DisposeAfterSent(_003Cstream_003E5__2))
					{
						val2 = _003Cstream_003E5__2.DisposeAsync().ConfigureAwait(false);
						awaiter = ((ConfiguredValueTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredValueTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaiter, _003CLoadIntoMemory_003Ed__14>(ref awaiter, ref this);
							return;
						}
						goto IL_01d3;
					}
					goto IL_01da;
					IL_01d3:
					((ConfiguredValueTaskAwaiter)(ref awaiter)).GetResult();
					goto IL_01da;
					IL_01da:
					result = _003Cmemory_003E5__4.ToArray();
				}
				finally
				{
					if (num < 0 && _003Cmemory_003E5__4 != null)
					{
						((System.IDisposable)_003Cmemory_003E5__4).Dispose();
					}
				}
				goto end_IL_000e;
				IL_00bf:
				((ConfiguredValueTaskAwaiter)(ref awaiter)).GetResult();
				goto IL_00c6;
				IL_00c6:
				result = _003Cbytes_003E5__3;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cstream_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cstream_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	public const string PLAIN_TEXT = "text/plain";

	public const string ZIP_ARCHIVE = "application/zip";

	[field: CompilerGenerated]
	public string ContentType
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public override bool IsValid
	{
		get
		{
			if (!string.IsNullOrEmpty(FileNameHint))
			{
				return !string.IsNullOrEmpty(ContentType);
			}
			return false;
		}
	}

	[field: CompilerGenerated]
	public virtual string FileNameHint
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	protected CrashReportStreamArgument()
	{
		ContentType = "text/plain";
	}

	public override System.IDisposable AddArgumentToForm(MultipartFormDataContent form)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(FileNameHint))
		{
			throw new InvalidOperationException("Cannot send a stream without a FileNameHint");
		}
		Stream val = OpenStream();
		StreamContent val2 = new StreamContent(val);
		((HttpContent)val2).Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
		((HttpContent)val2).Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "\"" + base.VariableName + "\"",
			FileName = "\"" + FileNameHint + "\""
		};
		((MultipartContent)form).Add((HttpContent)(object)val2);
		if (!DisposeAfterSent(val))
		{
			return CrashReportArgument.NOOP_DISPOSE;
		}
		return (System.IDisposable)val;
	}

	[AsyncStateMachine(typeof(_003CLoadIntoMemory_003Ed__14))]
	public async System.Threading.Tasks.Task<byte[]> LoadIntoMemory()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Stream stream = OpenStream();
		MemoryStream val = (MemoryStream)(object)((stream is MemoryStream) ? stream : null);
		if (val != null)
		{
			byte[] bytes = val.ToArray();
			if (DisposeAfterSent(stream))
			{
				await stream.DisposeAsync().ConfigureAwait(false);
			}
			return bytes;
		}
		MemoryStream memory = new MemoryStream();
		try
		{
			await stream.CopyToAsync((Stream)(object)memory).DetachContext();
			if (DisposeAfterSent(stream))
			{
				await stream.DisposeAsync().ConfigureAwait(false);
			}
			return memory.ToArray();
		}
		finally
		{
			((System.IDisposable)memory)?.Dispose();
		}
	}

	protected abstract Stream OpenStream();

	protected virtual bool DisposeAfterSent(Stream stream)
	{
		return true;
	}
}
