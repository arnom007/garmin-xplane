using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Winfs;

public static class AggregateSourcedTransformFilter
{
	public static IObservable<SourcedBatchTransform> Filter(IObservable<SourcedBatchTransform> source, params ISourcedBatchFilter[] filters)
	{
		ISourcedBatchFilter[] filters2 = filters;
		return source.WhereAsync<SourcedBatchTransform>([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CFilter_003Eb__0_003Ed))] async (SourcedBatchTransform st) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			List<System.Threading.Tasks.Task<bool>> obj = Enumerable.ToList<System.Threading.Tasks.Task<bool>>(Enumerable.Select<ISourcedBatchFilter, System.Threading.Tasks.Task<bool>>((System.Collections.Generic.IEnumerable<ISourcedBatchFilter>)filters2, (Func<ISourcedBatchFilter, System.Threading.Tasks.Task<bool>>)((ISourcedBatchFilter f) => f.Keep(st))));
			List<System.Threading.Tasks.Task<bool>> throwAwayTasks = Enumerable.ToList<System.Threading.Tasks.Task<bool>>(Enumerable.Select<ISourcedBatchFilter, System.Threading.Tasks.Task<bool>>((System.Collections.Generic.IEnumerable<ISourcedBatchFilter>)filters2, (Func<ISourcedBatchFilter, System.Threading.Tasks.Task<bool>>)((ISourcedBatchFilter f) => f.ThrowAway(st))));
			bool[] keeps = await System.Threading.Tasks.Task.WhenAll<bool>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<bool>>)obj).DetachContext();
			bool[] stream = await System.Threading.Tasks.Task.WhenAll<bool>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<bool>>)throwAwayTasks).DetachContext();
			return Enumerable.Any<bool>(keeps.WhereEqual(operand: true)) && !Enumerable.Any<bool>(stream.WhereEqual(operand: true));
		});
	}
}
