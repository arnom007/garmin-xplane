using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Environment_settings : INotifyPropertyChanged
{
	private Airframe_type _airframe;

	private bool _auto_poweron;

	private Mode _mode;

	private bool _pbt_aircraft;

	private IDictionary<string, JToken> _additionalProperties = (IDictionary<string, JToken>)(object)new Dictionary<string, JToken>();

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("airframe", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	[JsonConverter(typeof(StringEnumConverter))]
	public Airframe_type Airframe
	{
		get
		{
			return _airframe;
		}
		set
		{
			if (_airframe != value)
			{
				_airframe = value;
				RaisePropertyChanged("Airframe");
			}
		}
	}

	[JsonProperty("auto_poweron", Required = Required.Always)]
	public bool Auto_poweron
	{
		get
		{
			return _auto_poweron;
		}
		set
		{
			if (_auto_poweron != value)
			{
				_auto_poweron = value;
				RaisePropertyChanged("Auto_poweron");
			}
		}
	}

	[JsonProperty("mode", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
	[JsonConverter(typeof(StringEnumConverter))]
	[DefaultValue(Mode.DEMO)]
	public Mode Mode
	{
		get
		{
			return _mode;
		}
		set
		{
			if (_mode != value)
			{
				_mode = value;
				RaisePropertyChanged("Mode");
			}
		}
	}

	[JsonProperty("pbt_aircraft", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
	[DefaultValue(false)]
	public bool Pbt_aircraft
	{
		get
		{
			return _pbt_aircraft;
		}
		set
		{
			if (_pbt_aircraft != value)
			{
				_pbt_aircraft = value;
				RaisePropertyChanged("Pbt_aircraft");
			}
		}
	}

	[JsonExtensionData]
	public IDictionary<string, JToken> AdditionalProperties
	{
		get
		{
			return _additionalProperties;
		}
		set
		{
			_additionalProperties = value;
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

	public bool ShouldSerializePbt_aircraft()
	{
		return Pbt_aircraft;
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}

	public static Environment_settings FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Environment_settings>(data);
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
