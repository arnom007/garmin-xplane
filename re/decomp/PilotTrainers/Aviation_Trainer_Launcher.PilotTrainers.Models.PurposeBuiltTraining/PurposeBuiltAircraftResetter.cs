using System.Threading.Tasks;
using Domain.PurposeBuiltTraining;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public delegate Task<PbtLocalRepresentation> PurposeBuiltAircraftResetter(PbtAircraftDescriptor aircraftDescriptor);
