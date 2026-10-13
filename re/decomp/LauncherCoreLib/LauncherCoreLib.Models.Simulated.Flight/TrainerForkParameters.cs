using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class TrainerForkParameters
{
	[field: CompilerGenerated]
	public ISessionEnvironment SessionEnvironment
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerSessionData TrainerSessionData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ExeWorkingDirectoryPath WorkingDirectory
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ExeFilePath ExePath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ExeCliArgs Arguments
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerWindowScreenCoordinates? StartingLocation
	{
		[CompilerGenerated]
		get;
	}

	public TrainerForkParameters(ISessionEnvironment sessionEnvironment, TrainerSessionData trainerSessionData, ExeWorkingDirectoryPath workingDirectory, ExeFilePath exePath, ExeCliArgs arguments, TrainerWindowScreenCoordinates? startingLocation)
	{
		WorkingDirectory = workingDirectory;
		ExePath = exePath;
		Arguments = arguments;
		SessionEnvironment = sessionEnvironment;
		TrainerSessionData = trainerSessionData;
		StartingLocation = startingLocation;
	}

	public TrainerForkParameters WrapExe(ExeFilePath newExePath, ExeCliArgs newArgs)
	{
		return new TrainerForkParameters(SessionEnvironment, TrainerSessionData, WorkingDirectory, newExePath, newArgs, StartingLocation);
	}
}
