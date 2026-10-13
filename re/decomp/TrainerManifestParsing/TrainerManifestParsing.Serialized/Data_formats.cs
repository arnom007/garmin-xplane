using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Data_formats : INotifyPropertyChanged
{
	private string _dataRouterClientVersion;

	private Instances _instances;

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("DataRouterClientVersion", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[RegularExpression("^(\\d+\\.){0,3}(\\d+)$")]
	public string DataRouterClientVersion
	{
		get
		{
			return _dataRouterClientVersion;
		}
		set
		{
			if (_dataRouterClientVersion != value)
			{
				_dataRouterClientVersion = value;
				RaisePropertyChanged("DataRouterClientVersion");
			}
		}
	}

	[JsonProperty("Instances", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public Instances Instances
	{
		get
		{
			return _instances;
		}
		set
		{
			if (_instances != value)
			{
				_instances = value;
				RaisePropertyChanged("Instances");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			PropertyChangedEventHandler val = this.m_PropertyChanged;
			PropertyChangedEventHandler val2;
			do
			{
				val2 = val;
				PropertyChangedEventHandler val3 = (PropertyChangedEventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.m_PropertyChanged, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			PropertyChangedEventHandler val = this.m_PropertyChanged;
			PropertyChangedEventHandler val2;
			do
			{
				val2 = val;
				PropertyChangedEventHandler val3 = (PropertyChangedEventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.m_PropertyChanged, val3, val2);
			}
			while (val != val2);
		}
	}

	public static Data_formats FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Data_formats>(data);
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}

	protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged != null)
		{
			propertyChanged.Invoke((object)this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
