using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.DAL;

public abstract class TemporaryUnitHarness<T> : TemporaryOperationHarness<T>
{
	private readonly TemporaryUnitOfWork _tempUnitOfWork;

	private bool _disposed;

	protected IUnitOfWork ActualUnit
	{
		[CompilerGenerated]
		get;
	}

	protected TemporaryUnitOfWork TemporaryUnit => _tempUnitOfWork;

	protected TemporaryUnitHarness(TemporaryUnitOfWork unitOFWork)
	{
		ActualUnit = unitOFWork.WrappedUnit;
		_tempUnitOfWork = unitOFWork;
	}

	protected override System.Threading.Tasks.Task DoCancel()
	{
		return System.Threading.Tasks.Task.CompletedTask;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (!_disposed)
		{
			_disposed = true;
			if (disposing)
			{
				_tempUnitOfWork.Dispose();
			}
		}
	}
}
