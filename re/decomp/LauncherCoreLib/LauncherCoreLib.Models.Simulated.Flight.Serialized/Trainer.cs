using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class Trainer : INotifyPropertyChanged
{
	private string _id;

	private Slot _slot = new Slot();

	private string _family_name;

	private Trainer_settings _settings = new Trainer_settings();

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("id", Required = Required.Always)]
	[Required(AllowEmptyStrings = true)]
	public string Id
	{
		get
		{
			return _id;
		}
		set
		{
			if (_id != value)
			{
				_id = value;
				RaisePropertyChanged("Id");
			}
		}
	}

	[JsonProperty("slot", Required = Required.Always)]
	[Required]
	public Slot Slot
	{
		get
		{
			return _slot;
		}
		set
		{
			if (_slot != value)
			{
				_slot = value;
				RaisePropertyChanged("Slot");
			}
		}
	}

	[JsonProperty("family_name", Required = Required.Always)]
	[Required]
	[RegularExpression("^\\w+$")]
	public string Family_name
	{
		get
		{
			return _family_name;
		}
		set
		{
			if (_family_name != value)
			{
				_family_name = value;
				RaisePropertyChanged("Family_name");
			}
		}
	}

	[JsonProperty("settings", Required = Required.Always)]
	[Required]
	public Trainer_settings Settings
	{
		get
		{
			return _settings;
		}
		set
		{
			if (_settings != value)
			{
				_settings = value;
				RaisePropertyChanged("Settings");
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

	public static Trainer FromJson(string data)
	{
		return JsonConvert.DeserializeObject<Trainer>(data);
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
