using System;
using System.Globalization;
using System.Reflection;
using LOLA.Logging;
using LOLA.StringHelpers;

namespace LauncherCoreLib.Models.AppLifeCycle;

public abstract class AppLifeCycleActionBase : IAppLifeCycleAction
{
	private readonly Lazy<string> _actionTypeCache;

	private string _actionName = string.Empty;

	public string ActionName
	{
		get
		{
			return _actionName;
		}
		protected set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			_actionName = value ?? throw new ArgumentNullException("value");
		}
	}

	protected AppLifeCycleActionBase()
	{
		ActionName = TextHelpers.SpaceOutCamelCase(((MemberInfo)base.GetType()).Name);
		_actionTypeCache = new Lazy<string>((Func<string>)DetermineActionType);
	}

	public void Log(LogLevel level, string format, params object[] formatArgs)
	{
		Logger.Log(level, "[{2} action][{0}] {1}", ActionName, string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs), _actionTypeCache.Value);
	}

	public void Log(System.Exception e, string format, params object[] formatArgs)
	{
		Logger.LogFormattedException(e, "[{2} action][{0}] {1}", ActionName, string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, format, formatArgs), _actionTypeCache.Value);
	}

	protected virtual string DetermineActionType()
	{
		if (string.IsNullOrEmpty(ActionName))
		{
			Logger.Log(LogLevel.Error, "Actions should specify a name for easier debugging and log interpretation. This class has not set its name: {0}", base.GetType().FullName ?? "Unknown Class");
		}
		if (this is IStartupAction)
		{
			return "Startup";
		}
		if (this is IShutdownAction)
		{
			return "Shutdown";
		}
		return ((MemberInfo)base.GetType()).Name;
	}
}
