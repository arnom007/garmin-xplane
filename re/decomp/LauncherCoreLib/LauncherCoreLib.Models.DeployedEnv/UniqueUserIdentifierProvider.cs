using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using LOLA.Logging;
using LOLA.StringHelpers;

namespace LauncherCoreLib.Models.DeployedEnv;

public static class UniqueUserIdentifierProvider
{
	private sealed class ErrorIdProvider : IUserIdProvider
	{
		[field: CompilerGenerated]
		public System.Exception IdError
		{
			[CompilerGenerated]
			get;
		}

		string IUserIdProvider.UniqueUserId
		{
			get
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				throw new NotSupportedException("Cannot provide a unique identifier", IdError);
			}
		}

		public ErrorIdProvider(System.Exception error)
		{
			IdError = error;
		}
	}

	private static readonly Lazy<IUserIdProvider> LAZY_USER_ID = new Lazy<IUserIdProvider>((Func<IUserIdProvider>)LookupSidAndHash);

	public static IUserIdProvider Provider => LAZY_USER_ID.Value;

	public static string UniqueUserId => Provider.UniqueUserId;

	private static IUserIdProvider LookupSidAndHash()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0072: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			string text = ((object)current.User)?.ToString() ?? throw new NotSupportedException("Missing user SID");
			string text2 = TextHelpers.HashWithSha512(text, 12);
			Logger.Log(LogLevel.Debug, "Mapping current user({0}) SID: {1} -> {2}", ((ClaimsIdentity)current).Name, text, text2);
			return new ConstantIdProvider(text2);
		}
		catch (SecurityException val)
		{
			Logger.LogFormattedException((System.Exception)val, "Could not get the current user.");
			return new ErrorIdProvider((System.Exception)val);
		}
	}
}
