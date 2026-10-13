using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Data_format_channels : INotifyPropertyChanged
{
	private Dictionary<string, string> _a429 = new Dictionary<string, string>();

	private Dictionary<string, string> _rS232 = new Dictionary<string, string>();

	private Dictionary<string, string> _gENERIC = new Dictionary<string, string>();

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("A429", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, string> A429
	{
		get
		{
			return _a429;
		}
		set
		{
			if (_a429 != value)
			{
				_a429 = value;
				RaisePropertyChanged("A429");
			}
		}
	}

	[JsonProperty("RS232", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, string> RS232
	{
		get
		{
			return _rS232;
		}
		set
		{
			if (_rS232 != value)
			{
				_rS232 = value;
				RaisePropertyChanged("RS232");
			}
		}
	}

	[JsonProperty("GENERIC", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, string> GENERIC
	{
		get
		{
			return _gENERIC;
		}
		set
		{
			if (_gENERIC != value)
			{
				_gENERIC = value;
				RaisePropertyChanged("GENERIC");
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

	public static Data_format_channels FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Data_format_channels>(data);
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
