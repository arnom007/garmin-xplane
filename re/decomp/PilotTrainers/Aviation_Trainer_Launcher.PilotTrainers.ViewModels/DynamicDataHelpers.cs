using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Kernel;

namespace Aviation_Trainer_Launcher.PilotTrainers.ViewModels;

public static class DynamicDataHelpers
{
	public static IObservable<IChangeSet<T>> FilterOutAddReplacePairs<T>(this IObservable<IChangeSet<T>> changes, IEqualityComparer<T>? equality = null) where T : notnull
	{
		IEqualityComparer<T> equality2 = equality;
		if (equality2 == null)
		{
			equality2 = EqualityComparer<T>.Default;
		}
		return changes.Select(delegate(IChangeSet<T> cs)
		{
			if (cs.Count <= 1)
			{
				return cs;
			}
			int num = 0;
			ChangeSet<T> changeSet = new ChangeSet<T>(cs);
			while (num < changeSet.Count - 1)
			{
				Change<T> change = changeSet[num];
				Change<T> change2 = changeSet[num + 1];
				if (change.Reason == ListChangeReason.Remove && change2.Reason == ListChangeReason.Add && change.Item.CurrentIndex == change2.Item.CurrentIndex)
				{
					if (equality2.Equals(change2.Item.Current, change.Item.Current))
					{
						if (change2.Item.CurrentIndex == -1)
						{
							changeSet.RemoveAt(num);
							changeSet.RemoveAt(num);
							continue;
						}
						changeSet[num] = new Change<T>(ListChangeReason.Refresh, change2.Item.Current, change2.Item.CurrentIndex);
						changeSet.RemoveAt(num + 1);
					}
					else
					{
						int index = num;
						T current = change2.Item.Current;
						Optional<T> previous = change.Item.Current;
						changeSet[index] = new Change<T>(ListChangeReason.Replace, current, in previous, change2.Item.CurrentIndex);
						changeSet.RemoveAt(num + 1);
					}
				}
				num++;
			}
			return changeSet;
		});
	}
}
