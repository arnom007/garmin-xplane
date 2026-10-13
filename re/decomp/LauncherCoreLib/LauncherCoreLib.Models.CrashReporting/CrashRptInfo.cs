using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.CrashReporting;

[DefaultMember("Item")]
public class CrashRptInfo
{
	public enum CR_ExceptionType
	{
		CR_SEH_EXCEPTION,
		CR_CPP_TERMINATE_CALL,
		CR_CPP_UNEXPECTED_CALL,
		CR_CPP_PURE_CALL,
		CR_CPP_NEW_OPERATOR_ERROR,
		CR_CPP_SECURITY_ERROR,
		CR_CPP_INVALID_PARAMETER,
		CR_CPP_SIGABRT,
		CR_CPP_SIGFPE,
		CR_CPP_SIGILL,
		CR_CPP_SIGINT,
		CR_CPP_SIGSEGV,
		CR_CPP_SIGTERM
	}

	private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

	public Guid? CrashGUID
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Guid val = default(Guid);
			if (!Guid.TryParse(GetMember("CrashGUID"), ref val))
			{
				return null;
			}
			return val;
		}
		set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			object obj;
			if (!value.HasValue)
			{
				obj = null;
			}
			else
			{
				Guid valueOrDefault = value.GetValueOrDefault();
				obj = ((object)(Guid)(ref valueOrDefault)).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			SetMember("CrashGUID", (string)obj);
		}
	}

	[field: CompilerGenerated]
	public string CrashRptVersion
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	public string AppName
	{
		get
		{
			return GetMember("AppName");
		}
		set
		{
			SetMember("AppName", value);
		}
	}

	public string AppVersionString
	{
		get
		{
			return GetMember("AppVersion");
		}
		set
		{
			SetMember("AppVersion", value);
		}
	}

	public bool IsVersionStandard
	{
		get
		{
			Version appVersion;
			return TryParseVersion(out appVersion);
		}
	}

	public string ExecutablePath
	{
		get
		{
			return GetMember("ImageName");
		}
		set
		{
			SetMember("ImageName", value);
		}
	}

	public string OffendingDll => Path.GetFileName(ExceptionModuleString ?? string.Empty);

	public string UtcCrashOccurredString
	{
		get
		{
			return GetMember("SystemTimeUTC");
		}
		set
		{
			SetMember("SystemTimeUTC", value);
		}
	}

	public string ExceptionTypeString
	{
		get
		{
			return GetMember("ExceptionType");
		}
		set
		{
			SetMember("ExceptionType", value);
		}
	}

	public string ExceptionAddressString
	{
		get
		{
			return GetMember("ExceptionAddress");
		}
		set
		{
			SetMember("ExceptionAddress", value);
		}
	}

	public string ExceptionModuleBaseString
	{
		get
		{
			return GetMember("ExceptionModuleBase");
		}
		set
		{
			SetMember("ExceptionModuleBase", value);
		}
	}

	public string ExceptionModuleString
	{
		get
		{
			return GetMember("ExceptionModule");
		}
		set
		{
			SetMember("ExceptionModule", value);
		}
	}

	public string OperatingSystem
	{
		get
		{
			return GetMember("OperatingSystem");
		}
		set
		{
			SetMember("OperatingSystem", value);
		}
	}

	public ulong? ExceptionAddress => TryParseAddress(ExceptionAddressString);

	public ulong? ExceptionModuleBase => TryParseAddress(ExceptionModuleBaseString);

	public ulong? ExceptionRelativeVirtualAddress => OrZero(ExceptionAddress) - OrZero(ExceptionModuleBase);

	public CR_ExceptionType? ExceptionType
	{
		get
		{
			int num = default(int);
			if (string.IsNullOrEmpty(ExceptionTypeString) || !int.TryParse(ExceptionTypeString, ref num))
			{
				return null;
			}
			return (CR_ExceptionType)num;
		}
	}

	public string this[string key]
	{
		get
		{
			return _properties[key];
		}
		set
		{
			_properties[key] = value;
		}
	}

	public bool TryParseVersion([MaybeNullWhen(false)] out Version appVersion)
	{
		return Version.TryParse(AppVersionString, ref appVersion);
	}

	public string? CustomProps(string v)
	{
		string result = default(string);
		if (!_properties.TryGetValue("CustomProps." + v, ref result))
		{
			return null;
		}
		return result;
	}

	public bool TryParseUtcCrashTime(out System.DateTime occurred)
	{
		if (System.DateTime.TryParse(UtcCrashOccurredString, (IFormatProvider)(object)CultureInfo.InvariantCulture, (DateTimeStyles)64, ref occurred))
		{
			occurred = occurred.ToUniversalTime();
			return true;
		}
		return false;
	}

	public string GetMember(string memberName, string defaultString = "")
	{
		if (!_properties.ContainsKey(memberName))
		{
			return defaultString;
		}
		return _properties[memberName];
	}

	public void SetMember(string memberName, string value)
	{
		_properties[memberName] = value;
	}

	private static ulong? TryParseAddress(string address_string)
	{
		address_string = address_string.TrimStart("0xX".ToCharArray());
		ulong num = default(ulong);
		if (string.IsNullOrEmpty(address_string) || !ulong.TryParse(address_string, (NumberStyles)515, (IFormatProvider)(object)CultureInfo.InvariantCulture, ref num))
		{
			return null;
		}
		return num;
	}

	private static ulong OrZero(ulong? val)
	{
		return val.GetValueOrDefault();
	}
}
