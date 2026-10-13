using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class InlineEventDataDescriptor : GenericTrackedEventDataDescriptor
{
	[field: CompilerGenerated]
	protected override TrackedEventPrototype EventPrototype
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	protected override PropertiesBuilder BuildProperties
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	private ShouldCensorUserDataInValueForProperty? ShouldCensorUserData
	{
		[CompilerGenerated]
		get;
	}

	private InlineEventDataDescriptor(TrackedEventPrototype eventPrototype, PropertiesBuilder buildProperties, ShouldCensorUserDataInValueForProperty? shouldCensorUserData)
	{
		BuildProperties = buildProperties;
		EventPrototype = eventPrototype;
		ShouldCensorUserData = shouldCensorUserData;
	}

	public static InlineEventDataDescriptor Create(TrackedEventPrototype prototype, PropertiesBuilder properties, ShouldCensorUserDataInValueForProperty shouldCensorUserData)
	{
		return new InlineEventDataDescriptor(prototype, properties, shouldCensorUserData);
	}

	public override TrackedEventContainer Instance()
	{
		if (ShouldCensorUserData != null)
		{
			return EventPrototype.ToInstance(BuildProperties, ShouldCensorUserData);
		}
		return EventPrototype.ToInstance(BuildProperties);
	}
}
