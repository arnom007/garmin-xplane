using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LauncherCoreLib.Models.Simulated.Panel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

[GeneratedCode("NJsonSchema", "9.10.42.0 (Newtonsoft.Json v9.0.0.0)")]
public class SimSessionData : INotifyPropertyChanged
{
	private int _session_number;

	private string _configuration_name;

	private Environment_settings _environment_settings;

	private Dictionary<string, Trainer> _trainers = new Dictionary<string, Trainer>();

	private Dictionary<string, Dictionary<string, string>> _install_slots = new Dictionary<string, Dictionary<string, string>>();

	[CompilerGenerated]
	private PropertyChangedEventHandler m_PropertyChanged;

	[JsonProperty("session_number", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
	[Range(1, 2147483647)]
	public int Session_number
	{
		get
		{
			return _session_number;
		}
		set
		{
			if (_session_number != value)
			{
				_session_number = value;
				RaisePropertyChanged("Session_number");
			}
		}
	}

	[JsonProperty("configuration_name", Required = Required.Always)]
	[Required]
	public string Configuration_name
	{
		get
		{
			return _configuration_name;
		}
		set
		{
			if (_configuration_name != value)
			{
				_configuration_name = value;
				RaisePropertyChanged("Configuration_name");
			}
		}
	}

	[JsonProperty("environment_settings", Required = Required.Always)]
	public Environment_settings Environment_settings
	{
		get
		{
			return _environment_settings;
		}
		set
		{
			if (_environment_settings != value)
			{
				_environment_settings = value;
				RaisePropertyChanged("Environment_settings");
			}
		}
	}

	[JsonProperty("trainers", Required = Required.Always)]
	[Required]
	public Dictionary<string, Trainer> Trainers
	{
		get
		{
			return _trainers;
		}
		set
		{
			if (_trainers != value)
			{
				_trainers = value;
				RaisePropertyChanged("Trainers");
			}
		}
	}

	[JsonProperty("install_slots", Required = Required.Always)]
	[Required]
	public Dictionary<string, Dictionary<string, string>> Install_slots
	{
		get
		{
			return _install_slots;
		}
		set
		{
			if (_install_slots != value)
			{
				_install_slots = value;
				RaisePropertyChanged("Install_slots");
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

	public static InstallationSlot ConvertSerialToActual(Slot slot)
	{
		return new InstallationSlot(slot.Instance_id, ConvertSerialToActual(slot.Category));
	}

	public static AvionicsType ConvertSerialToActual(Avionics_category serialEnum)
	{
		return PermanentCommonDataTypes.ConvertSerialToActual(serialEnum);
	}

	public static AirframeCategoryClassType ConvertSerialToActual(Airframe_type airframe)
	{
		return PermanentCommonDataTypes.ConvertSerialToActual(airframe);
	}

	public static Avionics_category ConvertActualToSerial(AvionicsType actualEnum)
	{
		return PermanentCommonDataTypes.ConvertActualToSerial(actualEnum);
	}

	public static Slot ConvertActualToSerial(InstallationSlot installSlot)
	{
		return new Slot
		{
			Category = ConvertActualToSerial(installSlot.Category),
			Instance_id = installSlot.InstanceId
		};
	}

	public static Airframe_type ConvertActualToSerial(AirframeCategoryClassType airframeCCT)
	{
		return PermanentCommonDataTypes.ConvertActualToSerial(airframeCCT);
	}

	public static System.Collections.Generic.IEnumerable<KeyValuePair<string, string>> AllTrainerSettings(Trainer_settings settings)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		return Enumerable.Concat<KeyValuePair<string, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)new KeyValuePair<string, string>[2]
		{
			new KeyValuePair<string, string>("Instance_id", settings.Instance_id.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)),
			new KeyValuePair<string, string>("Model", settings.Model)
		}, Enumerable.Select<KeyValuePair<string, JToken>, KeyValuePair<string, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, JToken>>)settings.AdditionalProperties, (Func<KeyValuePair<string, JToken>, KeyValuePair<string, string>>)((KeyValuePair<string, JToken> kv) => new KeyValuePair<string, string>(kv.Key, kv.Value.ToObject<string>() ?? string.Empty))));
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}

	public static SimSessionData FromJson(string data)
	{
		return JsonConvert.DeserializeObject<SimSessionData>(data);
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
