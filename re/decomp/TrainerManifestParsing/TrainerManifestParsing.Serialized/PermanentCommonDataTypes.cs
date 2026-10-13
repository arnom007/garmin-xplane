using System;
using Domain.TrainerPackages;

namespace TrainerManifestParsing.Serialized;

public static class PermanentCommonDataTypes
{
	public static AvionicsType ConvertSerialToActual(Avionics_category serialEnum)
	{
		return serialEnum switch
		{
			Avionics_category.DISPLAY => AvionicsType.Display, 
			Avionics_category.NAVIGATOR => AvionicsType.Navigator, 
			_ => throw NotSupported(serialEnum), 
		};
	}

	public static AirframeCategoryClassType ConvertSerialToActual(Airframe_type serialEnum)
	{
		return serialEnum switch
		{
			Airframe_type.UNCONFIGURED => AirframeCategoryClassType.Unconfigured, 
			Airframe_type.SINGLE => AirframeCategoryClassType.SingleEngine, 
			Airframe_type.MULTI => AirframeCategoryClassType.MultiEngine, 
			Airframe_type.ROTOR => AirframeCategoryClassType.Rotorcraft, 
			Airframe_type.SINGLETURBO => AirframeCategoryClassType.SingleTurboProp, 
			Airframe_type.TURBO => AirframeCategoryClassType.MultiTurboProp, 
			Airframe_type.MULTITURBOFAN => AirframeCategoryClassType.MultiTurboFan, 
			_ => throw NotSupported(serialEnum), 
		};
	}

	public static Airframe_type ConvertActualToSerial(AirframeCategoryClassType airframe)
	{
		return airframe switch
		{
			AirframeCategoryClassType.Unconfigured => Airframe_type.UNCONFIGURED, 
			AirframeCategoryClassType.SingleEngine => Airframe_type.SINGLE, 
			AirframeCategoryClassType.MultiEngine => Airframe_type.MULTI, 
			AirframeCategoryClassType.Rotorcraft => Airframe_type.ROTOR, 
			AirframeCategoryClassType.SingleTurboProp => Airframe_type.SINGLETURBO, 
			AirframeCategoryClassType.MultiTurboProp => Airframe_type.TURBO, 
			AirframeCategoryClassType.MultiTurboFan => Airframe_type.MULTITURBOFAN, 
			_ => throw NotSupported(airframe), 
		};
	}

	public static Avionics_category ConvertActualToSerial(AvionicsType actualEnum)
	{
		return actualEnum switch
		{
			AvionicsType.Navigator => Avionics_category.NAVIGATOR, 
			AvionicsType.Display => Avionics_category.DISPLAY, 
			_ => throw NotSupported(actualEnum), 
		};
	}

	public static System.Exception NotSupported<TEnum>(TEnum enumVal)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		TEnum val = enumVal;
		return (System.Exception)new NotSupportedException(((object)val)?.ToString() + " is not supported.");
	}
}
