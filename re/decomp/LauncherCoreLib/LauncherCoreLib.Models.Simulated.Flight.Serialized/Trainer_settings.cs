using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Trainer_settings : INotifyPropertyChanged
{
	private int _instance_id;

	private string _model;

	private IDictionary<string, JToken> _additionalProperties = (IDictionary<string, JToken>)(object)new Dictionary<string, JToken>();

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("instance_id", Required = Required.Always)]
	public int Instance_id
	{
		get
		{
			return _instance_id;
		}
		set
		{
			if (_instance_id != value)
			{
				_instance_id = value;
				RaisePropertyChanged("Instance_id");
			}
		}
	}

	[JsonProperty("model", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	public string Model
	{
		get
		{
			return _model;
		}
		set
		{
			if (_model != value)
			{
				_model = value;
				RaisePropertyChanged("Model");
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

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}

	public static Trainer_settings FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Trainer_settings>(data);
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
