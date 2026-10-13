using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.CrashReporting;

internal static class CrashRptXmlParser
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CParseXmlFileAsync_003Ed__2 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<CrashRptInfo> _003C_003Et__builder;

		public string xmlPath;

		private FileStream _003Cfs_003E5__2;

		private MemoryStream _003Cmem_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CrashRptInfo result;
			try
			{
				if (num != 0)
				{
					_003Cfs_003E5__2 = File.OpenRead(xmlPath);
				}
				try
				{
					if (num != 0)
					{
						_003Cmem_003E5__3 = new MemoryStream();
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = ((Stream)_003Cfs_003E5__2).CopyToAsync((Stream)(object)_003Cmem_003E5__3).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CParseXmlFileAsync_003Ed__2>(ref awaiter, ref this);
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
						((Stream)_003Cmem_003E5__3).Seek(0L, (SeekOrigin)0);
						result = ReadFromStream((Stream)(object)_003Cmem_003E5__3);
					}
					finally
					{
						if (num < 0 && _003Cmem_003E5__3 != null)
						{
							((System.IDisposable)_003Cmem_003E5__3).Dispose();
						}
					}
				}
				finally
				{
					if (num < 0 && _003Cfs_003E5__2 != null)
					{
						((System.IDisposable)_003Cfs_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cfs_003E5__2 = null;
				_003Cmem_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cfs_003E5__2 = null;
			_003Cmem_003E5__3 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private const string ROOT_ELEMENT = "CrashRpt";

	public static CrashRptInfo ParseXmlFile(string xmlPath)
	{
		FileStream val = File.OpenRead(xmlPath);
		try
		{
			return ReadFromStream((Stream)(object)val);
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	[AsyncStateMachine(typeof(_003CParseXmlFileAsync_003Ed__2))]
	public static async System.Threading.Tasks.Task<CrashRptInfo> ParseXmlFileAsync(string xmlPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		FileStream fs = File.OpenRead(xmlPath);
		try
		{
			MemoryStream mem = new MemoryStream();
			try
			{
				await ((Stream)fs).CopyToAsync((Stream)(object)mem).DetachContext();
				((Stream)mem).Seek(0L, (SeekOrigin)0);
				return ReadFromStream((Stream)(object)mem);
			}
			finally
			{
				((System.IDisposable)mem)?.Dispose();
			}
		}
		finally
		{
			((System.IDisposable)fs)?.Dispose();
		}
	}

	private static CrashRptInfo ReadFromStream(Stream s)
	{
		XDocument obj = XDocument.Load(s);
		CrashRptInfo crashRptInfo = new CrashRptInfo();
		XElement val = ((XContainer)obj).Element(XName.op_Implicit("CrashRpt"));
		if (val != null)
		{
			XAttribute val2 = val.Attribute(XName.op_Implicit("version"));
			if (val2 != null)
			{
				crashRptInfo.CrashRptVersion = val2.Value;
			}
			System.Collections.Generic.IEnumerator<XElement> enumerator = ((XContainer)val).Elements().GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					XElement current = enumerator.Current;
					if (current.Name == XName.op_Implicit("CustomProps"))
					{
						System.Collections.Generic.IEnumerator<XElement> enumerator2 = ((XContainer)current).Elements().GetEnumerator();
						try
						{
							while (((System.Collections.IEnumerator)enumerator2).MoveNext())
							{
								XElement current2 = enumerator2.Current;
								XAttribute obj2 = current2.Attribute(XName.op_Implicit("name"));
								string text = ((obj2 != null) ? obj2.Value : null);
								XAttribute obj3 = current2.Attribute(XName.op_Implicit("value"));
								string text2 = ((obj3 != null) ? obj3.Value : null);
								if (text != null && text2 != null)
								{
									string key = $"{current.Name}.{text}";
									crashRptInfo[key] = text2;
								}
							}
						}
						finally
						{
							((System.IDisposable)enumerator2)?.Dispose();
						}
					}
					else
					{
						crashRptInfo[current.Name.LocalName] = current.Value;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
		return crashRptInfo;
	}
}
