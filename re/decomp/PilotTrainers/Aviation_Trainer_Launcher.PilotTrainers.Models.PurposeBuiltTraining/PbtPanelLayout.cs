using System;
using System.Drawing;
using System.Threading;
using Domain.AircraftDescriptor;
using Domain.PanelLayout;
using Domain.PurposeBuiltTraining;
using LanguageExt;
using LauncherCoreLib.Models.Simulated.Flight;
using Microsoft.FSharp.Core;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public static class PbtPanelLayout
{
	public static GetDesiredTrainerWindow CreateSimWindowLocator(PbtAircraftApi pbtApi, PbtLocalAircraftRecognizer pbtRecognizer, GetScreenRectangle screenRectangle)
	{
		PbtLocalAircraftRecognizer pbtRecognizer2 = pbtRecognizer;
		PbtAircraftApi pbtApi2 = pbtApi;
		GetScreenRectangle screenRectangle2 = screenRectangle;
		return async delegate(ISessionEnvironment e, TrainerSessionData tsd, CancellationToken c)
		{
			TrainerSessionData tsd2 = tsd;
			TrainerWindowScreenCoordinates trainerWindowScreenCoordinates = tsd2.InstallData.ParseWindowRect();
			if ((object)trainerWindowScreenCoordinates != null)
			{
				return trainerWindowScreenCoordinates;
			}
			FlightSessionRequestInfo flightSessionRequestInfo = e.Inputs.CreationRequestOrThrow();
			return await pbtRecognizer2(flightSessionRequestInfo.AircraftSnapShot).Bind(delegate(PbtDataStoredInLocalAircraft pbt)
			{
				TrainingItemKey key = TrainingItemKey.Parse(pbt.DescriptorKey);
				return pbtApi2.Store.State.ListState.AircraftState.Lookup.Find(key).Filter((PbtAircraftState s) => s.AircraftDescriptor.Aircraft.Layouts.TargetedDeviceLayouts.Count > 0).Bind(delegate(PbtAircraftState s)
				{
					AircraftPanelLayouts layouts = s.AircraftDescriptor.Aircraft.Layouts;
					LayoutVec2<int> screen = screenRectangle2().Apply((Rectangle r) => LayoutVec2.createFrom(r.Width, r.Height));
					TargetDeviceLayout<AircraftDescriptorLayoutMeta> bestMatchingLayout = TargetDeviceLayout.chooseBestLayoutForScreen(screen, layouts.TargetedDeviceLayouts);
					return bestMatchingLayout.Layout.Entities.Values.Find((LayoutEntityData<AircraftDescriptorLayoutMeta> e) => e.Extra.Slot.Slot.ToLegacySlot() == tsd2.InstallSlot).Map(delegate(LayoutEntityData<AircraftDescriptorLayoutMeta> simEntity)
					{
						Rectangle rectangle = LayoutBoxModule.toDrawingRectangle(PanelLayout.scaleIndividualToScreen(screen, bestMatchingLayout.Layout, simEntity.Id).BoundingBox);
						return new TrainerWindowScreenCoordinates(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
					});
				});
			}).IfNoneUnsafeAsync((TrainerWindowScreenCoordinates?)null).ConfigureAwait(continueOnCapturedContext: false);
		};
	}

	private static FSharpFunc<T, TR> FsFunc<T, TR>(Func<T, TR> f)
	{
		Func<T, TR> f2 = f;
		return FSharpFunc<T, TR>.FromConverter((T i) => f2(i));
	}
}
