using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

namespace TrainerManifestParsing.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Data_format_directions : INotifyPropertyChanged
{
	private Data_format_channels _iN = new Data_format_channels();

	private Data_format_channels _oUT = new Data_format_channels();

	private int _hSDB;

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("IN", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public Data_format_channels IN
	{
		get
		{
			return _iN;
		}
		set
		{
			if (_iN != value)
			{
				_iN = value;
				RaisePropertyChanged("IN");
			}
		}
	}

	[JsonProperty("OUT", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	public Data_format_channels OUT
	{
		get
		{
			return _oUT;
		}
		set
		{
			if (_oUT != value)
			{
				_oUT = value;
				RaisePropertyChanged("OUT");
			}
		}
	}

	[JsonProperty("HSDB", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[Range(0, 2147483647)]
	public int HSDB
	{
		get
		{
			return _hSDB;
		}
		set
		{
			if (_hSDB != value)
			{
				_hSDB = value;
				RaisePropertyChanged("HSDB");
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

	public static Data_format_directions FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Data_format_directions>(data);
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
