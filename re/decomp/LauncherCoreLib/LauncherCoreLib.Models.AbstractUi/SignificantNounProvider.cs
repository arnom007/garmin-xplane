using System;
using System.Collections.Generic;

namespace LauncherCoreLib.Models.AbstractUi;

public class SignificantNounProvider : ISignificantNounProvider
{
	private readonly Dictionary<string, SignificantNoun> _nouns = new Dictionary<string, SignificantNoun>();

	public SignificantNoun? GetNounByKeyOrNull(string key)
	{
		SignificantNoun result = default(SignificantNoun);
		if (!_nouns.TryGetValue(key, ref result))
		{
			return null;
		}
		return result;
	}

	public SignificantNounProvider SetNoun(string key, SignificantNoun significantNoun)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_nouns[key] = significantNoun ?? throw new ArgumentNullException("significantNoun");
		return this;
	}
}
