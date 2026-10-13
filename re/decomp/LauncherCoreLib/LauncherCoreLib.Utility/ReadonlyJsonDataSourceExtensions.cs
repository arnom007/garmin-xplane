using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public static class ReadonlyJsonDataSourceExtensions
{
	public static TResult CopyData<TResult>(this IReadOnlyJsonDataSource source, string jsonPath)
	{
		return source.CopyData<TResult>(jsonPath, Default<TResult>);
	}

	public static TResult CopyData<TResult>(this IReadOnlyJsonDataSource source, string jsonPath, Func<TResult> defaultValueFactory)
	{
		JToken jToken = source.CopyData(jsonPath);
		if (jToken == null)
		{
			return defaultValueFactory.Invoke();
		}
		return jToken.ToObject<TResult>();
	}

	public static JToken CopyAllData(this IReadOnlyJsonDataSource source)
	{
		return source.CopyData(string.Empty);
	}

	public static TExpectedType CopyAllData<TExpectedType>(this IReadOnlyJsonDataSource<TExpectedType> source)
	{
		return source.CopyData<TExpectedType>(string.Empty);
	}

	public static JObject CopyAllDataAsObject(this IReadOnlyJsonDataSource source)
	{
		JToken jToken = source.CopyData(string.Empty);
		if (jToken.IsNull())
		{
			return new JObject();
		}
		return (JObject)jToken;
	}

	public static IReadOnlyJsonDataSource CopyAndUpdate(this IReadOnlyJsonDataSource source, Func<JToken, JToken> tokenMutator)
	{
		JToken jToken = source.CopyData(string.Empty) ?? JValue.CreateUndefined();
		jToken = tokenMutator.Invoke(jToken);
		return new ReadOnlyJsonDataSource(jToken);
	}

	public static IReadOnlyJsonDataSource CopyAndUpdate(this IReadOnlyJsonDataSource source, string jsonPath, JToken updatedData)
	{
		string jsonPath2 = jsonPath;
		JToken updatedData2 = updatedData;
		return source.CopyAndUpdate(delegate(JToken fullCopy)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			fullCopy = fullCopy.CreateObjectsForPath(jsonPath2) ?? JValue.CreateUndefined();
			(fullCopy.SelectToken(jsonPath2) ?? throw new KeyNotFoundException("The JSON path (" + jsonPath2 + ") was not created.")).Replace(updatedData2);
			return fullCopy;
		});
	}

	public static ISynchronizedJsonDataSource ToSynchronizedSource(this IReadOnlyJsonDataSource source)
	{
		return new SynchronizedJsonDataSource(source);
	}

	private static TResult Default<TResult>()
	{
		return default(TResult);
	}
}
