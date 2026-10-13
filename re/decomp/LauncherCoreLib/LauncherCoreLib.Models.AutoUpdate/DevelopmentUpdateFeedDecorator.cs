using System;
using System.Collections.Generic;
using System.Linq;
using LOLA.Extensions;
using LauncherCoreLib.Models.DeployedEnv;

namespace LauncherCoreLib.Models.AutoUpdate;

public class DevelopmentUpdateFeedDecorator : IUpdateUrlProvider
{
	private readonly IUpdateUrlProvider _urlProvider;

	private readonly bool _isDevLaunch;

	public Uri? UpdateFeedUrl
	{
		get
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Uri val = _urlProvider.UpdateFeedUrl;
			if (val != (Uri)null && _isDevLaunch)
			{
				UriBuilder val2 = new UriBuilder(val);
				int num = val2.Path.LastIndexOf('/');
				val2.Path = val2.Path.Insert(num, "/dev");
				val = val2.Uri;
			}
			return val;
		}
	}

	public DevelopmentUpdateFeedDecorator(IUpdateUrlProvider updateUrlProvider, ICommandLineProvider commandLineProvider)
	{
		_urlProvider = updateUrlProvider;
		_isDevLaunch = Enumerable.Any<string>((System.Collections.Generic.IEnumerable<string>)Exceptions.ThrowIfNull(commandLineProvider, "commandLineProvider").CommandLineArgs, (Func<string, bool>)((string c) => string.Equals(c, "--dev", (StringComparison)5)));
	}
}
