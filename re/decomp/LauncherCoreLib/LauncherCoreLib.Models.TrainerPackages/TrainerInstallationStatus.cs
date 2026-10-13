using System;
using System.Runtime.CompilerServices;
using GTI.Net;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public sealed class TrainerInstallationStatus
{
	[field: CompilerGenerated]
	public ITrainerPackage RequestingPackage
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ITrainerPackage? ResultingPackage
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerPackageOperationState OnDemandState
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public double Progress
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Exception? Error
	{
		[CompilerGenerated]
		get;
	}

	private TrainerInstallationStatus(ITrainerPackage request, ITrainerPackage? result, TrainerPackageOperationState state, System.Exception? error, double progress)
	{
		RequestingPackage = request;
		ResultingPackage = result;
		OnDemandState = state;
		Error = error;
		Progress = progress;
	}

	public static TrainerInstallationStatus CreateReady(ITrainerPackage initial, ITrainerPackage result)
	{
		return new TrainerInstallationStatus(initial, result, TrainerPackageOperationState.Success, null, 1.0);
	}

	public static TrainerInstallationStatus CreateAcceptableResultMissing(ITrainerPackage initial)
	{
		return new TrainerInstallationStatus(initial, null, TrainerPackageOperationState.Success, null, 1.0);
	}

	public static TrainerInstallationStatus CreateError(ITrainerPackage package, System.Exception error)
	{
		return new TrainerInstallationStatus(package, null, TrainerPackageOperationState.Error, error, 1.0);
	}

	public static TrainerInstallationStatus CreateCanceled(ITrainerPackage package)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		return new TrainerInstallationStatus(package, null, TrainerPackageOperationState.Canceled, (System.Exception?)new OperationCanceledException("The operation was canceled."), 1.0);
	}

	public static TrainerInstallationStatus CreateProgress(ITrainerPackage package, double progress)
	{
		return new TrainerInstallationStatus(package, null, TrainerPackageOperationState.InProgress, null, progress);
	}

	public static TrainerInstallationStatus CreateProgress(ITrainerPackage package, GtiProgress progress)
	{
		return CreateProgress(package, progress.OverallProgress);
	}
}
