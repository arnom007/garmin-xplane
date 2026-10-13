using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageErrorProvider : IPackageErrorProvider
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CGetAllCurrentErrors_003Eb__4_1_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TrainerPackageLoadException> _003C_003Et__builder;

		public PackageErrorProvider _003C_003E4__this;

		public PotentialPackageDirectory dir;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageErrorProvider packageErrorProvider = _003C_003E4__this;
			TrainerPackageLoadException result;
			try
			{
				if (num == 0 || num != 1)
				{
					bool flag;
					try
					{
						ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
						if (num != 0)
						{
							awaiter = packageErrorProvider._repo.GetPackageByLocationOrNull(dir.PackageDirectory.FullPath).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CGetAllCurrentErrors_003Eb__4_1_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
							num = (_003C_003E1__state = -1);
						}
						flag = awaiter.GetResult() != null;
					}
					catch (KeyNotFoundException)
					{
						flag = false;
					}
					if (flag)
					{
						goto IL_0120;
					}
				}
				try
				{
					ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
					if (num != 1)
					{
						awaiter = packageErrorProvider._parser.ParsePackage(dir).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CGetAllCurrentErrors_003Eb__4_1_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
				}
				catch (TrainerPackageLoadException ex)
				{
					result = ex;
					goto end_IL_000e;
				}
				goto IL_0120;
				IL_0120:
				result = null;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ITrainerPackageRepoEvolved _repo;

	private readonly ITrainerPackageDirectoryProvider _directories;

	private readonly ITrainerPackageParser _parser;

	public PackageErrorProvider(ITrainerPackageRepoEvolved repo, ITrainerPackageDirectoryProvider directories, ITrainerPackageParser parser)
	{
		_repo = repo;
		_directories = directories;
		_parser = parser;
	}

	public IObservable<TrainerPackageLoadException> GetAllCurrentErrors()
	{
		return _directories.SearchDirectories().Where<PotentialPackageDirectory>((Func<PotentialPackageDirectory, bool>)((PotentialPackageDirectory dir) => PotentialPackageDirectory.IsValid(dir))).SelectMany<PotentialPackageDirectory, TrainerPackageLoadException>((Func<PotentialPackageDirectory, System.Threading.Tasks.Task<TrainerPackageLoadException>>)([AsyncStateMachine(typeof(_003C_003CGetAllCurrentErrors_003Eb__4_1_003Ed))] [CompilerGenerated] async (PotentialPackageDirectory dir) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			bool flag;
			try
			{
				flag = await _repo.GetPackageByLocationOrNull(dir.PackageDirectory.FullPath).DetachContext() != null;
			}
			catch (KeyNotFoundException)
			{
				flag = false;
			}
			if (!flag)
			{
				try
				{
					await _parser.ParsePackage(dir).DetachContext();
				}
				catch (TrainerPackageLoadException result)
				{
					return result;
				}
			}
			return (TrainerPackageLoadException)null;
		}))
			.WhereClassNotNull<TrainerPackageLoadException>();
	}
}
