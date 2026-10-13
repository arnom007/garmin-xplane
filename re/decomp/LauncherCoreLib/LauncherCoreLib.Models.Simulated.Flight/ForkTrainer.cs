using System.Threading.Tasks;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public delegate System.Threading.Tasks.Task<IProcessSession> ForkTrainer(TrainerForkParameters forkParameters);
