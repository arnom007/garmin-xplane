using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public delegate IObservable<TrainerWindowScreenCoordinates> WatchTrainerWindowRectangle(SimProcessSession session);
