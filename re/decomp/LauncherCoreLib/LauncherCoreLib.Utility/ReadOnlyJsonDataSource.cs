using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Utility;

public class ReadOnlyJsonDataSource : IReadOnlyJsonDataSource
{
	private JToken _objectRoot = JValue.CreateUndefined();

	public JToken ObjectRoot
	{
		get
		{
			return _objectRoot;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			_objectRoot = value ?? throw new ArgumentNullException("value");
		}
	}

	public ReadOnlyJsonDataSource()
	{
		ObjectRoot = new JObject();
	}

	public ReadOnlyJsonDataSource(object? root)
	{
		if (root is JToken objectRoot)
		{
			ObjectRoot = objectRoot;
		}
		else if (root == null)
		{
			ObjectRoot = JValue.CreateNull();
		}
		else
		{
			ObjectRoot = JObject.FromObject(root, SerializationUtil.JSON_SERIALIZER.Value);
		}
	}

	public static ReadOnlyJsonDataSource<TExpectedType> Create<TExpectedType>(TExpectedType root)
	{
		return new ReadOnlyJsonDataSource<TExpectedType>(root);
	}

	public JToken CopyData(string jsonPath)
	{
		if (jsonPath == null)
		{
			return ObjectRoot.DeepClone();
		}
		JToken jToken = ObjectRoot.SelectToken(jsonPath);
		if (jToken == null)
		{
			return JValue.CreateUndefined();
		}
		return jToken.DeepClone();
	}

	public void Overwrite(object newData)
	{
		SetRootObject(newData);
	}

	public bool IsPathValid(string jsonPath)
	{
		return !ObjectRoot.SelectToken(jsonPath).IsUndefined();
	}

	public bool IsTrackingSameSource(IReadOnlyJsonDataSource otherSource)
	{
		if (otherSource is ReadOnlyJsonDataSource readOnlyJsonDataSource)
		{
			if (!EqualityComparer<JToken>.Default.Equals(ObjectRoot, readOnlyJsonDataSource.ObjectRoot))
			{
				return JToken.DeepEquals(ObjectRoot, readOnlyJsonDataSource.ObjectRoot);
			}
			return true;
		}
		return false;
	}

	protected void SetRootObject(object root)
	{
		ObjectRoot = JToken.FromObject(root, SerializationUtil.JSON_SERIALIZER.Value);
	}
}
public class ReadOnlyJsonDataSource<TExpectedType> : ReadOnlyJsonDataSource, IReadOnlyJsonDataSource<TExpectedType>, IReadOnlyJsonDataSource
{
	public ReadOnlyJsonDataSource(TExpectedType root)
	{
		SetRootObject(root);
	}

	public ReadOnlyJsonDataSource()
	{
	}

	public void Overwrite(TExpectedType newData)
	{
		SetRootObject(newData);
	}
}
