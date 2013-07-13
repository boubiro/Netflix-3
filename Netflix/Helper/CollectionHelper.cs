using System;
using System.Collections.Generic;

namespace Netflix
{
	public static class CollectionHelper
	{
		public static IEnumerable<IEnumerable<T>> Split<T> (this IEnumerable<T> collection, int chunk = 1000)
		{
			int count = 0;
			T[] group = null; // use arrays as buffer
			foreach (T item in collection) 
			{
				if (group == null) 
				{
					group = new T[chunk];
				}

				group [count++] = item;
				if (count == chunk) 
				{
					yield return group;
					group = null;
					count = 0;
				}
			}

			if (count > 0) 
			{
				Array.Resize (ref group, count);
				yield return group;
			}
		}
	}
}

