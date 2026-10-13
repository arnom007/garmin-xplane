using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace LauncherCoreLib.Models.AutoUpdate;

public class HourlyCacheUpdateFeedProvider : IUpdateUrlProvider
{
	private readonly IUpdateUrlProvider _innerProvider;

	public Uri? UpdateFeedUrl
	{
		get
		{
			Uri updateFeedUrl = _innerProvider.UpdateFeedUrl;
			if (updateFeedUrl != null)
			{
				return AppendHourlyCacheParameter(updateFeedUrl);
			}
			return updateFeedUrl;
		}
	}

	public HourlyCacheUpdateFeedProvider(IUpdateUrlProvider innerProvider)
	{
		_innerProvider = innerProvider;
	}

	private static Uri AppendHourlyCacheParameter(Uri url)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		DateTimeOffset utcNow = DateTimeOffset.UtcNow;
		long num = ((DateTimeOffset)(ref utcNow)).ToUnixTimeSeconds() / 60 / 60;
		UriBuilder val = new UriBuilder(url);
		NameValueCollection val2 = HttpUtility.ParseQueryString(val.Query);
		val2["cb"] = num.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture);
		val.Query = ((object)val2).ToString();
		return val.Uri;
	}
}
