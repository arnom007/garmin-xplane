using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Slot : INotifyPropertyChanged
{
	private Avionics_category _category;

	private int _instance_id;

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("category", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[JsonConverter(typeof(StringEnumConverter))]
	public Avionics_category Category
	{
		get
		{
			return _category;
		}
		set
		{
			if (_category != value)
			{
				_category = value;
				RaisePropertyChanged("Category");
			}
		}
	}

	[JsonProperty("instance_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
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

	public static Slot FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Slot>(data);
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
