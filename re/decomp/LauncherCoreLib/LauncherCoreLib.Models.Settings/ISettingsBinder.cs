using System;

namespace LauncherCoreLib.Models.Settings;

public interface ISettingsBinder<out T> : System.IDisposable
{
	ISettingSymbolTable Table { get; }

	T Entity { get; }
}
