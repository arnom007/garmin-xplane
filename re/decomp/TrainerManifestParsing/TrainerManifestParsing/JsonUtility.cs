using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Domain.UtilTypes;
using LanguageExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace TrainerManifestParsing;

public static class JsonUtility
{
	[field: CompilerGenerated]
	public static Lazy<JsonSerializer> DefaultSerializer
	{
		[CompilerGenerated]
		get;
	} = new Lazy<JsonSerializer>((Func<JsonSerializer>)CreateDefaultSerializer);


	[field: CompilerGenerated]
	public static JsonSerializer? GlobalSerializerOverride
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public static EitherAsync<JsonParseError, JToken> ParseJsonStream(this Stream jsonStream, CancellationToken t)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Stream jsonStream2 = jsonStream;
		return Prelude.unit.Apply<Unit, System.Threading.Tasks.Task<Either<JsonParseError, JToken>>>(Prelude.fun<Unit, System.Threading.Tasks.Task<Either<JsonParseError, JToken>>>((Func<Unit, System.Threading.Tasks.Task<Either<JsonParseError, JToken>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_0._003C_003CParseJsonStream_003Eb__0_003Ed))] async (Unit _) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			StreamReader sw = new StreamReader(jsonStream2);
			try
			{
				using JsonTextReader jw = new JsonTextReader((TextReader)(object)sw);
				try
				{
					return await JToken.LoadAsync(jw, t).ConfigureAwait(false);
				}
				catch (JsonException ex)
				{
					return Prelude.Left(new InvalidJsonFormatError(((System.Exception)ex).Message).AsParseError());
				}
				catch (IOException val)
				{
					return Prelude.Left(new IoExceptionWrapperError(val).AsParseError());
				}
				catch (OperationCanceledException)
				{
					return Prelude.Left(new ParsingCanceledError(t).AsParseError());
				}
			}
			finally
			{
				((System.IDisposable)sw)?.Dispose();
			}
		}))).ToAsync();
	}

	public static Either<JsonParseError, JToken> ParseInMemoryJsonStream(this MemoryStream jsonStream)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_003d: Expected O, but got Unknown
		StreamReader val = new StreamReader((Stream)(object)jsonStream);
		try
		{
			using JsonTextReader reader = new JsonTextReader((TextReader)(object)val);
			try
			{
				return JToken.Load(reader);
			}
			catch (JsonException ex)
			{
				return Prelude.Left(new InvalidJsonFormatError(((System.Exception)ex).Message).AsParseError());
			}
			catch (IOException val2)
			{
				return Prelude.Left(new IoExceptionWrapperError(val2).AsParseError());
			}
		}
		finally
		{
			((System.IDisposable)val)?.Dispose();
		}
	}

	public static Either<JsonParseError, JToken> ParseJsonString(this string jsonString)
	{
		try
		{
			return JToken.Parse(jsonString);
		}
		catch (JsonException ex)
		{
			return Prelude.Left(new InvalidJsonFormatError(((System.Exception)ex).Message).AsParseError());
		}
	}

	public static JsonProvider ToJsonProvider(this JToken json)
	{
		return new JsonProvider(json);
	}

	public static JsonObjectProvider ToJsonObjectProvider(this JObject json)
	{
		return new JsonObjectProvider(json);
	}

	public static Option<JToken> SelectJsonPath(this JToken token, string jsonPath)
	{
		JToken jToken = token.SelectToken(jsonPath);
		if (jToken != null)
		{
			return Prelude.Some(jToken);
		}
		return Prelude.None;
	}

	public static Either<JsonParseError, JToken> CopyRequiredJsonPath(this IJsonProvider json, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return json.CopyJsonPath(jsonPath2).ToEither<JsonParseError>(() => new MissingJsonPathError(jsonPath2).AsParseError());
	}

	public static Either<JsonParseError, JToken> SelectRequiredJsonPath(this JToken original, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return original.SelectJsonPath(jsonPath2).ToEither<JsonParseError>(() => new MissingJsonPathError(jsonPath2).AsParseError());
	}

	public static System.Collections.Generic.IEnumerable<Either<JsonParseError, JObject>> EnumerateRequiredJObjectLookupValuesAtPath(this IJsonProvider json, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return json.CopyRequiredJsonPath(jsonPath2).Bind<System.Collections.Generic.IEnumerable<Either<JsonParseError, JObject>>>((JToken t) => t.AsJsonObject().Map<System.Collections.Generic.IEnumerable<Either<JsonParseError, JObject>>>((JObject jo) => jo.EnumerateProperyValuesAsObjects()).AttachJsonPath(jsonPath2)).LeftToRightEnumerable();
	}

	public static Either<JsonParseError, Option<T>> GetWrapperAtPathOrNone<T>(this IJsonProvider json, string jsonPath, Func<JObject, T> factory)
	{
		Func<JObject, T> factory2 = factory;
		return Prelude.Right(json.CopyJsonPath(jsonPath)).Bind<JsonParseError>().Bind<Option<T>>((Option<JToken> maybeModelToken) => maybeModelToken.Map<Either<JsonParseError, T>>((JToken t) => t.AsJsonObject().Map<T>((JObject mo) => factory2.Invoke(mo))).Sequence());
	}

	public static System.Collections.Generic.IEnumerable<Either<JsonParseError, JObject>> EnumerateProperyValuesAsObjects(this JObject jobject)
	{
		return Enumerable.Select<JToken, Either<JsonParseError, JObject>>((System.Collections.Generic.IEnumerable<JToken>)jobject.PropertyValues(), (Func<JToken, Either<JsonParseError, JObject>>)((JToken t) => t.AsJsonObject()));
	}

	public static System.Collections.Generic.IEnumerable<ValueTuple<string, int, Either<JsonParseError, JObject>>> EnumerateProperyValuesAsObjectsWithInfo(this JObject jobject)
	{
		return Enumerable.Select<JProperty, ValueTuple<string, int, Either<JsonParseError, JObject>>>(jobject.Properties(), (Func<JProperty, int, ValueTuple<string, int, Either<JsonParseError, JObject>>>)((JProperty t, int i) => new ValueTuple<string, int, Either<JsonParseError, JObject>>(t.Name, i, t.Value.AsJsonObject())));
	}

	public static Either<JsonParseError, Option<T>> ConvertToTypeOrNull<T>(this JToken token, JsonSerializer? serializerOverride = null)
	{
		JToken token2 = token;
		JsonSerializer serializerOverride2 = serializerOverride;
		return Prelude.Try<Option<T>>(delegate
		{
			T val = token2.ToObject<T>(serializerOverride2 ?? GlobalSerializerOverride ?? DefaultSerializer.Value);
			return (val != null) ? Prelude.Some(val) : ((Option<T>)Prelude.None);
		}).ToEither<Option<T>, JsonParseError>((System.Exception e) => new JsonValueError(e.Message).AsParseError());
	}

	public static Either<JsonParseError, Option<T>> ConvertToTypeOrNull<T>(this Either<JsonParseError, JToken> token, JsonSerializer? serializerOverride = null)
	{
		JsonSerializer serializerOverride2 = serializerOverride;
		return token.Bind<Option<T>>((JToken t) => t.ConvertToTypeOrNull<T>(serializerOverride2));
	}

	public static Either<JsonParseError, T> NullIsInvalid<T>(this Either<JsonParseError, Option<T>> maybeConverted)
	{
		return maybeConverted.Bind<T>((Option<T> maybe) => maybe.ToEither<JsonParseError>(() => new JsonValueError("This is not allowed to be null.").AsParseError()));
	}

	public static Either<JsonParseError, T> ConvertToType<T>(this JToken token, JsonSerializer? serializerOverride = null)
	{
		return token.ConvertToTypeOrNull<T>(serializerOverride).NullIsInvalid();
	}

	public static Either<JsonParseError, Option<T>> ConvertToType<T>(this Option<JToken> token, JsonSerializer? serializerOverride = null)
	{
		JsonSerializer serializerOverride2 = serializerOverride;
		return token.Map<Either<JsonParseError, T>>((JToken t) => t.ConvertToTypeOrNull<T>(serializerOverride2).NullIsInvalid()).Sequence();
	}

	public static Either<JsonParseError, T> ConvertToType<T>(this Either<JsonParseError, JToken> token, JsonSerializer? serializerOverride = null)
	{
		JsonSerializer serializerOverride2 = serializerOverride;
		return token.Bind<Option<T>>((JToken t) => t.ConvertToTypeOrNull<T>(serializerOverride2)).NullIsInvalid();
	}

	public static Either<JsonParseError, Option<T>> ConvertToType<T>(this Either<JsonParseError, Option<JToken>> tokenOption, JsonSerializer? serializerOverride = null)
	{
		JsonSerializer serializerOverride2 = serializerOverride;
		return tokenOption.Bind<Option<T>>((Option<JToken> o) => o.ConvertToType<T>(serializerOverride2));
	}

	public static Either<JsonParseError, JObject> AsJsonObject(this Either<JsonParseError, JToken> jtoken)
	{
		return jtoken.Bind<JObject>((JToken jtok) => jtok.AsJsonObject());
	}

	public static EitherAsync<JsonParseError, JObject> AsJsonObject(this EitherAsync<JsonParseError, JToken> jtoken)
	{
		return jtoken.Bind<JObject>((JToken jtok) => jtok.AsJsonObject().ToAsync());
	}

	public static Either<JsonParseError, JObject> AsJsonObject(this JToken jtoken)
	{
		if (jtoken is JObject jObject)
		{
			return jObject;
		}
		return new JsonValueError($"Expected a JSON object, but got a {jtoken.Type} ({jtoken}).");
	}

	public static Either<JsonParseError, JArray> AsJsonArray(this Either<JsonParseError, JToken> jtoken)
	{
		return jtoken.Bind<JArray>((JToken jtok) => jtok.AsJsonArray());
	}

	public static Either<JsonParseError, JArray> AsJsonArray(this JToken jtoken)
	{
		if (jtoken is JArray jArray)
		{
			return jArray;
		}
		return new JsonValueError($"This is not a JSON array. It is a {jtoken.Type}.");
	}

	public static Either<TError, System.Collections.Generic.IEnumerable<T>> ParseArrayItems<TError, T>(this JArray jar, Func<JToken, Either<TError, T>> binder)
	{
		Func<JToken, Either<TError, T>> binder2 = binder;
		return Enumerable.Select<JToken, Either<TError, T>>((System.Collections.Generic.IEnumerable<JToken>)jar, (Func<JToken, Either<TError, T>>)((JToken t) => binder2.Invoke(t))).Sequence();
	}

	public static Either<JsonParseError, System.Collections.Generic.IEnumerable<T>> ParseArrayItems<T>(this Either<JsonParseError, JArray> either, Func<JToken, Either<JsonParseError, T>> binder)
	{
		Func<JToken, Either<JsonParseError, T>> binder2 = binder;
		return either.Bind<System.Collections.Generic.IEnumerable<T>>((JArray jar) => jar.ParseArrayItems<JsonParseError, T>(binder2));
	}

	public static Either<JsonParseError, System.Collections.Generic.IEnumerable<T>> ConvertEachToType<T>(this JArray jar, JsonSerializer? serializerOverride = null)
	{
		JsonSerializer serializerOverride2 = serializerOverride;
		return jar.ParseArrayItems<JsonParseError, T>((JToken i) => i.ConvertToType<T>(serializerOverride2));
	}

	public static Either<JsonParseError, System.Collections.Generic.IEnumerable<T>> ConvertEachToType<T>(this Either<JsonParseError, JArray> either, JsonSerializer? serializerOverride = null)
	{
		JsonSerializer serializerOverride2 = serializerOverride;
		return either.Bind<System.Collections.Generic.IEnumerable<T>>((JArray jar) => jar.ConvertEachToType<T>(serializerOverride2));
	}

	public static JsonParseError AttachJsonPath(this JsonParseError genericError, string jsonPath)
	{
		if (genericError is JsonPropertyError)
		{
			return genericError;
		}
		return new InvalidJsonPropertyFormatErrorWrapper(genericError, jsonPath);
	}

	public static Either<JsonParseError, T> AttachJsonPath<T, TJE>(this Either<TJE, T> either, string jsonPath) where TJE : JsonParseError
	{
		string jsonPath2 = jsonPath;
		return either.MapLeft<JsonParseError>((TJE e) => e.AttachJsonPath(jsonPath2).AsParseError());
	}

	public static Either<JsonParseError, T> MapLeftToJsonError<T, TJE>(this Either<TJE, T> either) where TJE : JsonParseError
	{
		return either.MapLeft<JsonParseError>((TJE je) => je.AsParseError());
	}

	public static Either<TE, string> ParseRequiredStringAtPath<TE>(this IJsonProvider json, string jsonPath, Func<Either<JsonParseError, string>, Either<TE, string>> mapToSpecializedError)
	{
		string jsonPath2 = jsonPath;
		return json.CopyRequiredJsonPath(jsonPath2).Bind<string>((JToken j) => j.ConvertToType<string>().AttachJsonPath(jsonPath2)).Apply<Either<JsonParseError, string>, Either<TE, string>>(mapToSpecializedError);
	}

	public static Either<JsonParseError, StringBasedId> ParseStringIdentifierAtPath(this IJsonProvider json, string jsonPath)
	{
		string jsonPath2 = jsonPath;
		return json.CopyRequiredJsonPath(jsonPath2).ConvertToType<string>().Bind<StringBasedId>((string s) => StringBasedId.Parse(s).ToEither().MapLeft<JsonParseError>((Seq<StringIdentifierError> e) => new InvalidJsonPropertyFormatError(jsonPath2, $"Invalid ID format: {e[0]}").AsParseError()))
			.AttachJsonPath(jsonPath2);
	}

	private static JsonSerializer CreateDefaultSerializer()
	{
		JsonSerializer jsonSerializer = new JsonSerializer();
		jsonSerializer.TypeNameHandling = TypeNameHandling.None;
		jsonSerializer.Formatting = Formatting.Indented;
		jsonSerializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
		jsonSerializer.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
		jsonSerializer.DateParseHandling = DateParseHandling.DateTimeOffset;
		((Collection<JsonConverter>)(object)jsonSerializer.Converters).Add((JsonConverter)new VersionConverter());
		((Collection<JsonConverter>)(object)jsonSerializer.Converters).Add((JsonConverter)new StringEnumConverter());
		return jsonSerializer;
	}
}
