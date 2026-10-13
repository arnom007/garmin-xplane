using LauncherCoreLib.Models.CrashReporting;
using LauncherCoreLib.Models.Winfs;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class DefaultWorkflowProvider : ISessionWorkflowProvider
{
	private readonly StackedWorkflows _workflowStack = new StackedWorkflows();

	public ISessionWorkflow Workflow => _workflowStack;

	public DefaultWorkflowProvider(SetupTrainerDataWorkflow setupSimulators, CrashReportingSessionWorkflow crashReporting, SerializeSessionJsonWorkflow serializeSessionJSONWorkflow, EnhancedSessionWorkflow enhancedWorkflow, WinfsSessionWorkflow winfs, LogRecentSessionJsonWorkflow logSessionJsonWorkflow, WinfsCompressionSessionWorkflow compressionWorkflow, WinfsNonAsciiErrorSessionWorkflow nonAsciiSessionWorkflow)
	{
		_workflowStack.AddWorkflow(setupSimulators);
		_workflowStack.AddWorkflow(crashReporting);
		_workflowStack.AddWorkflow(compressionWorkflow);
		_workflowStack.AddWorkflow(winfs);
		_workflowStack.AddWorkflow(enhancedWorkflow);
		_workflowStack.AddWorkflow(nonAsciiSessionWorkflow);
		_workflowStack.AddWorkflow(serializeSessionJSONWorkflow);
		_workflowStack.AddWorkflow(logSessionJsonWorkflow);
	}
}
