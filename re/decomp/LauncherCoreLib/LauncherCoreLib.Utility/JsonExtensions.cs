using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public static class JsonExtensions
{
	private static readonly JValue UNDEFINED = JValue.CreateUndefined();

	private static readonly Regex SUPPORTED_JSON_PATH_REGEX = new Regex("^(\\w+\\.?)*\\w?$");

	public static JToken? CreateObjectsForPath(this JToken? root, string jsonPath)
	{
		return CreateObjectsForPathImpl(root, ThrowIfPathIsNotSupported(jsonPath));
	}

	public static string ThrowIfPathIsNotSupported(string jsonPath)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		jsonPath = jsonPath ?? string.Empty;
		if (ModulePathContainsUnsupportedElements(jsonPath))
		{
			throw new NotSupportedException("The JSON path contains syntax is not supported.");
		}
		return jsonPath;
	}

	public static bool IsNull(this JToken token)
	{
		if (token != null)
		{
			return token.Type == JTokenType.Null;
		}
		return true;
	}

	public static bool IsUndefined(this JToken? token)
	{
		if (token != null)
		{
			return JToken.EqualityComparer.Equals(token, UNDEFINED);
		}
		return true;
	}

	public static bool IsNullOrHasNoValue(this JToken token)
	{
		if (!token.IsNull())
		{
			return !token.HasValues;
		}
		return true;
	}

	private static bool ModulePathContainsUnsupportedElements(string jsonPath)
	{
		return !SUPPORTED_JSON_PATH_REGEX.IsMatch(jsonPath);
	}

	private static JToken? CreateObjectsForPathImpl(JToken? root, string jsonPath)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (root?.SelectToken(jsonPath) == null)
		{
			ThrowIfPathIsNotSupported(jsonPath);
			string[] array = jsonPath.Split('.', (StringSplitOptions)0);
			if (root.IsUndefined())
			{
				root = new JObject();
			}
			JToken jToken = root;
			StringBuilder val = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!(jToken is JObject))
				{
					string text2 = (string.IsNullOrEmpty(((object)val).ToString()) ? "<ROOT>" : ((object)val).ToString());
					throw new InvalidOperationException(string.Concat(new string[5] { "The JSON path: '", text2, "' is not an object and thus cannot have a property named '", text, "' added to it." }));
				}
				if (val.Length != 0)
				{
					val.Append('.');
				}
				val.Append(text);
				if (jToken[text] == null)
				{
					jToken[text] = new JObject();
				}
				jToken = (JObject)jToken[text];
			}
		}
		return root;
	}
}
