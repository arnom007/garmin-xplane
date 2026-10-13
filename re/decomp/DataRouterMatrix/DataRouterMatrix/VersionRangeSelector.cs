using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace DataRouterMatrix;

public static class VersionRangeSelector
{
	public static class CommonParsers
	{
		public static Version VersionParser(string versionstring)
		{
			return Version.Parse(versionstring);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CParseAsync_003Ed__5<T> : IAsyncStateMachine where T : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<VersionRangeSelector<T>> _003C_003Et__builder;

		public Stream stream;

		public Func<string, T> keyParser;

		public IEqualityComparer<T> comparer;

		private StreamReader _003CstreamReader_003E5__2;

		private JsonTextReader _003CjsonReader_003E5__3;

		private ConfiguredTaskAwaiter<JObject> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			VersionRangeSelector<T> result;
			try
			{
				if (num != 0)
				{
					_003CstreamReader_003E5__2 = new StreamReader(stream);
				}
				try
				{
					if (num != 0)
					{
						_003CjsonReader_003E5__3 = new JsonTextReader((TextReader)(object)_003CstreamReader_003E5__2);
					}
					try
					{
						ConfiguredTaskAwaiter<JObject> awaiter;
						if (num != 0)
						{
							awaiter = JObject.LoadAsync(_003CjsonReader_003E5__3).ConfigureAwait(false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<VersionRangeSelector<T>>, AsyncTaskMethodBuilder<VersionRangeSelector<VersionRangeSelector<T>>>>(ref _003C_003Et__builder).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<JObject>, _003CParseAsync_003Ed__5<T>>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<JObject>);
							num = (_003C_003E1__state = -1);
						}
						result = Parse<T>(awaiter.GetResult(), keyParser, comparer);
					}
					finally
					{
						if (num < 0 && _003CjsonReader_003E5__3 != null)
						{
							((System.IDisposable)_003CjsonReader_003E5__3).Dispose();
						}
					}
				}
				finally
				{
					if (num < 0 && _003CstreamReader_003E5__2 != null)
					{
						((System.IDisposable)_003CstreamReader_003E5__2).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<VersionRangeSelector<T>>, AsyncTaskMethodBuilder<VersionRangeSelector<VersionRangeSelector<T>>>>(ref _003C_003Et__builder).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<VersionRangeSelector<T>>, AsyncTaskMethodBuilder<VersionRangeSelector<VersionRangeSelector<T>>>>(ref _003C_003Et__builder).SetResult((VersionRangeSelector<VersionRangeSelector<T>>)(object)result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			System.Runtime.CompilerServices.Unsafe.As<AsyncTaskMethodBuilder<VersionRangeSelector<T>>, AsyncTaskMethodBuilder<VersionRangeSelector<VersionRangeSelector<T>>>>(ref _003C_003Et__builder).SetStateMachine(stateMachine);
		}
	}

	public static VersionRangeSelector<T> ParseJson<T>(string json, Func<string, T> keyParser, IEqualityComparer<T>? comparer = null) where T : notnull
	{
		return Parse<T>(JObject.Parse(json), keyParser, comparer);
	}

	public static VersionRangeSelector<Version> ParseJsonWithVersionKey(string json)
	{
		return ParseJson<Version>(json, CommonParsers.VersionParser);
	}

	public static VersionRangeSelector<Version> ParseWithVersionKey(JObject definitionObject)
	{
		return Parse<Version>(definitionObject, CommonParsers.VersionParser);
	}

	public static VersionRangeSelector<string> Parse(JObject definitionObject)
	{
		return Parse<string>(definitionObject, (string k) => k);
	}

	public static VersionRangeSelector<T> Parse<T>(JObject definitionObject, Func<string, T> keyParser, IEqualityComparer<T>? comparer = null) where T : notnull
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Func<string, T> keyParser2 = keyParser;
		if (definitionObject == null)
		{
			throw new ArgumentNullException("definitionObject");
		}
		if (keyParser2 == null)
		{
			throw new ArgumentNullException("keyParser");
		}
		return new VersionRangeSelector<T>(Enumerable.Select<JProperty, KeyValuePair<T, NuGet.Versioning.VersionRange>>(definitionObject.Properties(), (Func<JProperty, KeyValuePair<T, NuGet.Versioning.VersionRange>>)delegate(JProperty definitionKey)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			T val = keyParser2.Invoke(definitionKey.Name);
			if (definitionKey.Value.Type != JTokenType.String)
			{
				throw new FormatException("The '" + definitionKey.Name + "' property value is not a string.");
			}
			string text = ((object)definitionKey.Value).ToString();
			if (!NuGet.Versioning.VersionRange.TryParse(text, out var versionRange))
			{
				throw new FormatException("'" + text + "' is not valid version range.");
			}
			return new KeyValuePair<T, NuGet.Versioning.VersionRange>(val, versionRange);
		}), comparer);
	}

	[AsyncStateMachine(typeof(_003CParseAsync_003Ed__5<>))]
	public static async System.Threading.Tasks.Task<VersionRangeSelector<T>> ParseAsync<T>(this Stream stream, Func<string, T> keyParser, IEqualityComparer<T>? comparer = null) where T : notnull
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		StreamReader streamReader = new StreamReader(stream);
		try
		{
			using JsonTextReader jsonReader = new JsonTextReader((TextReader)(object)streamReader);
			return Parse<T>(await JObject.LoadAsync(jsonReader).ConfigureAwait(false), keyParser, comparer);
		}
		finally
		{
			((System.IDisposable)streamReader)?.Dispose();
		}
	}
}
public sealed class VersionRangeSelector<T> : IEquatable<VersionRangeSelector<T>> where T : notnull
{
	private sealed class EqualityAdapter : EqualityComparer<KeyValuePair<T, NuGet.Versioning.VersionRange>>
	{
		private readonly IEqualityComparer<T> _keyComparer;

		public EqualityAdapter(IEqualityComparer<T>? keyComparer)
		{
			((EqualityComparer<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>>)(object)this)._002Ector();
			_keyComparer = (IEqualityComparer<T>)(((object)keyComparer) ?? ((object)EqualityComparer<T>.Default));
		}

		public override bool Equals(KeyValuePair<T, NuGet.Versioning.VersionRange> x, KeyValuePair<T, NuGet.Versioning.VersionRange> y)
		{
			if (_keyComparer.Equals(x.Key, y.Key))
			{
				return EqualityComparer<NuGet.Versioning.VersionRange>.Default.Equals(x.Value, y.Value);
			}
			return false;
		}

		public override int GetHashCode(KeyValuePair<T, NuGet.Versioning.VersionRange> obj)
		{
			return _keyComparer.GetHashCode(obj.Key) ^ ((object)obj.Value).GetHashCode();
		}
	}

	private readonly ReadOnlyDictionary<T, NuGet.Versioning.VersionRange> _rangeByKey;

	private readonly IEqualityComparer<T> _keyEquality;

	private readonly IEqualityComparer<KeyValuePair<T, NuGet.Versioning.VersionRange>> _equalityComparer;

	private readonly int _hash;

	public System.Collections.Generic.IReadOnlyCollection<T> Keys => (System.Collections.Generic.IReadOnlyCollection<T>)_rangeByKey.Keys;

	public VersionRangeSelector(System.Collections.Generic.IEnumerable<KeyValuePair<T, string>> rangeByApilevel, IEqualityComparer<T>? comparer = null)
		: this(ConvertToNugetRange(rangeByApilevel), comparer)
	{
	}

	public VersionRangeSelector(System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>> rangeByApilevel, IEqualityComparer<T>? comparer = null)
	{
		_rangeByKey = new ReadOnlyDictionary<T, NuGet.Versioning.VersionRange>((IDictionary<T, NuGet.Versioning.VersionRange>)(object)rangeByApilevel.ToDictionary());
		_keyEquality = (IEqualityComparer<T>)(((object)comparer) ?? ((object)EqualityComparer<T>.Default));
		_equalityComparer = (IEqualityComparer<KeyValuePair<T, NuGet.Versioning.VersionRange>>)(object)new EqualityAdapter(_keyEquality);
		_hash = Enumerable.Aggregate<KeyValuePair<T, NuGet.Versioning.VersionRange>, int>((System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>>)_rangeByKey, 1, (Func<int, KeyValuePair<T, NuGet.Versioning.VersionRange>, int>)(object)(Func<int, KeyValuePair<int, NuGet.Versioning.VersionRange>, int>)((int acc, KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => acc ^ ((object)kv).GetHashCode()));
	}

	public bool ContainsKey(T key)
	{
		return _rangeByKey.ContainsKey(key);
	}

	public System.Collections.Generic.IEnumerable<T> SelectKeysThatSupportVersion(Version version)
	{
		NuGetVersion nugetVersion = new NuGetVersion(version);
		return Enumerable.Select<KeyValuePair<T, NuGet.Versioning.VersionRange>, T>(Enumerable.Where<KeyValuePair<T, NuGet.Versioning.VersionRange>>((System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>>)_rangeByKey, (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, bool>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, bool>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => kv.Value.Satisfies(nugetVersion))), (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, T>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, KeyValuePair<T, NuGet.Versioning.VersionRange>>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => kv.Key));
	}

	public bool DoesKeySupportVersion(T key, Version version)
	{
		NuGet.Versioning.VersionRange versionRange = default(NuGet.Versioning.VersionRange);
		if (_rangeByKey.TryGetValue(key, ref versionRange))
		{
			return versionRange.Satisfies(new NuGetVersion(version));
		}
		return false;
	}

	public Compatibility GetCompatibilityByKey(T key, Version version)
	{
		NuGet.Versioning.VersionRange versionRange = default(NuGet.Versioning.VersionRange);
		if (!_rangeByKey.TryGetValue(key, ref versionRange))
		{
			return Compatibility.Incompatible;
		}
		NuGetVersion nuGetVersion = new NuGetVersion(version);
		if (versionRange.Satisfies(nuGetVersion))
		{
			return Compatibility.Supported;
		}
		if (nuGetVersion < versionRange.MinVersion || (!versionRange.IsMinInclusive && nuGetVersion == versionRange.MinVersion))
		{
			return Compatibility.TooOld;
		}
		if (nuGetVersion > versionRange.MaxVersion || (!versionRange.IsMaxInclusive && nuGetVersion == versionRange.MaxVersion))
		{
			return Compatibility.TooNew;
		}
		return Compatibility.Incompatible;
	}

	public Compatibility GetCompatibilityOfVersion(Version version)
	{
		NuGetVersion nugetVersion = new NuGetVersion(version);
		if (Enumerable.Any<KeyValuePair<T, NuGet.Versioning.VersionRange>>((System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>>)_rangeByKey, (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, bool>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, bool>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => kv.Value.Satisfies(nugetVersion))))
		{
			return Compatibility.Supported;
		}
		System.Collections.Generic.IList<NuGet.Versioning.VersionRange> list = ((System.Collections.Generic.IEnumerable<NuGet.Versioning.VersionRange>)_rangeByKey.Values).MinBy<NuGet.Versioning.VersionRange, NuGetVersion>((NuGet.Versioning.VersionRange r) => r.MinVersion);
		NuGetVersion minVersion = list[0].MinVersion;
		if (nugetVersion < minVersion)
		{
			return Compatibility.TooOld;
		}
		if (nugetVersion == minVersion && Enumerable.Any<NuGet.Versioning.VersionRange>((System.Collections.Generic.IEnumerable<NuGet.Versioning.VersionRange>)list, (Func<NuGet.Versioning.VersionRange, bool>)((NuGet.Versioning.VersionRange r) => !r.IsMinInclusive)))
		{
			return Compatibility.TooOld;
		}
		System.Collections.Generic.IList<NuGet.Versioning.VersionRange> list2 = ((System.Collections.Generic.IEnumerable<NuGet.Versioning.VersionRange>)_rangeByKey.Values).MaxBy<NuGet.Versioning.VersionRange, NuGetVersion>((NuGet.Versioning.VersionRange r) => r.MaxVersion);
		NuGetVersion maxVersion = list2[0].MaxVersion;
		if (nugetVersion > maxVersion)
		{
			return Compatibility.TooNew;
		}
		if (nugetVersion == maxVersion && Enumerable.Any<NuGet.Versioning.VersionRange>((System.Collections.Generic.IEnumerable<NuGet.Versioning.VersionRange>)list2, (Func<NuGet.Versioning.VersionRange, bool>)((NuGet.Versioning.VersionRange r) => !r.IsMaxInclusive)))
		{
			return Compatibility.TooNew;
		}
		return Compatibility.Incompatible;
	}

	public System.Collections.Generic.IEnumerable<T> SelectKeysThatSupportVersions(System.Collections.Generic.IEnumerable<T> keys, System.Collections.Generic.IEnumerable<Version> versions)
	{
		HashSet<T> allKeys = new HashSet<T>(keys, _keyEquality);
		List<NuGetVersion> allVersions = Enumerable.ToList<NuGetVersion>(Enumerable.Select<Version, NuGetVersion>(versions, (Func<Version, NuGetVersion>)((Version v) => new NuGetVersion(v))));
		return Enumerable.Select<KeyValuePair<T, NuGet.Versioning.VersionRange>, T>(Enumerable.Where<KeyValuePair<T, NuGet.Versioning.VersionRange>>((System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>>)_rangeByKey, (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, bool>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, bool>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => allKeys.Contains(kv.Key) && Enumerable.All<NuGetVersion>((System.Collections.Generic.IEnumerable<NuGetVersion>)allVersions, (Func<NuGetVersion, bool>)((NuGetVersion v) => kv.Value.Satisfies(v))))), (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, T>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, KeyValuePair<T, NuGet.Versioning.VersionRange>>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => kv.Key));
	}

	public System.Collections.Generic.IEnumerable<Version> SelectVersionsSupportedByKeys(System.Collections.Generic.IEnumerable<Version> availableVersions, System.Collections.Generic.IEnumerable<T> keys)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (keys == null)
		{
			throw new ArgumentNullException("keys");
		}
		if (availableVersions == null)
		{
			throw new ArgumentNullException("availableVersions");
		}
		HashSet<T> allkeys = new HashSet<T>(keys);
		List<NuGet.Versioning.VersionRange> relevantRanges = Enumerable.ToList<NuGet.Versioning.VersionRange>(Enumerable.Select<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>(Enumerable.Where<KeyValuePair<T, NuGet.Versioning.VersionRange>>((System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>>)_rangeByKey, (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, bool>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, bool>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => allkeys.Contains(kv.Key))), (Func<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>)(object)(Func<KeyValuePair<KeyValuePair<T, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>, NuGet.Versioning.VersionRange>)((KeyValuePair<T, NuGet.Versioning.VersionRange> kv) => kv.Value)));
		return Enumerable.Where<Version>(availableVersions, (Func<Version, bool>)delegate(Version v)
		{
			NuGetVersion nugetVersion = new NuGetVersion(v);
			return Enumerable.All<NuGet.Versioning.VersionRange>((System.Collections.Generic.IEnumerable<NuGet.Versioning.VersionRange>)relevantRanges, (Func<NuGet.Versioning.VersionRange, bool>)((NuGet.Versioning.VersionRange r) => r.Satisfies(nugetVersion)));
		});
	}

	public System.Collections.Generic.IEnumerable<Version> SelectVersionsSupportedByAKey(System.Collections.Generic.IEnumerable<Version> availableVersions, System.Collections.Generic.IEnumerable<T> keys)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (keys == null)
		{
			throw new ArgumentNullException("keys");
		}
		if (availableVersions == null)
		{
			throw new ArgumentNullException("availableVersions");
		}
		return Enumerable.Where<Version>(availableVersions, (Func<Version, bool>)([CompilerGenerated] (Version av) => Enumerable.Any<T>(SelectKeysThatSupportVersion(av))));
	}

	public bool Equals(VersionRangeSelector<T>? other)
	{
		if (other != null)
		{
			return ((IDictionary<T, NuGet.Versioning.VersionRange>)(object)_rangeByKey).IsEquivalentTo<T, NuGet.Versioning.VersionRange>((IDictionary<T, NuGet.Versioning.VersionRange>)(object)other._rangeByKey, _equalityComparer);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is VersionRangeSelector<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _hash;
	}

	private static System.Collections.Generic.IEnumerable<KeyValuePair<T, NuGet.Versioning.VersionRange>> ConvertToNugetRange(System.Collections.Generic.IEnumerable<KeyValuePair<T, string>> rangeByApilevel)
	{
		return Enumerable.Select<KeyValuePair<T, string>, KeyValuePair<T, NuGet.Versioning.VersionRange>>(rangeByApilevel, (Func<KeyValuePair<T, string>, KeyValuePair<T, NuGet.Versioning.VersionRange>>)(object)(Func<KeyValuePair<KeyValuePair<T, string>, string>, KeyValuePair<KeyValuePair<T, string>, NuGet.Versioning.VersionRange>>)((KeyValuePair<T, string> kv) => new KeyValuePair<T, NuGet.Versioning.VersionRange>(kv.Key, NuGet.Versioning.VersionRange.Parse(kv.Value))));
	}
}
