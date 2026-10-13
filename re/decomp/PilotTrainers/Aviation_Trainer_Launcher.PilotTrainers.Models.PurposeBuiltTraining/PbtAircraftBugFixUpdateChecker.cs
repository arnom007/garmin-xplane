using System.Threading;
using Domain.PurposeBuiltTraining;
using LanguageExt;
using LauncherCoreLib.Models.Simulated.Airport;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public delegate OptionAsync<PbtAircraftError> PbtAircraftBugFixUpdateChecker(PbtAircraftDescriptor descriptor, ReactiveAircraft localAircraft, CancellationToken token);
