using System;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class PackageInstallRequestExtensions
{
	public static TType? GetAttachment<TType>(this IPackageInstallRequest packageInstallRequest, object key, Func<TType>? defaultFactory = null) where TType : class
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (defaultFactory == null && typeof(TType).IsValueType)
		{
			throw new ArgumentNullException("defaultFactory");
		}
		object attachment = packageInstallRequest.GetAttachment(key);
		if (attachment == null)
		{
			if (defaultFactory == null)
			{
				return null;
			}
			return defaultFactory.Invoke();
		}
		return (TType)attachment;
	}
}
