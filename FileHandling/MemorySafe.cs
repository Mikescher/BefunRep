using System;
using System.Collections.Generic;
using System.Linq;

namespace BefunRep.FileHandling
{
	public class MemorySafe : RepresentationSafe
	{
		private SortedDictionary<long, Tuple<byte, string>> representations;

		public MemorySafe()
		{
			load();
		}

		private void load()
		{
			representations = new SortedDictionary<long, Tuple<byte, string>>();
		}

		private void safe()
		{
			// NOP
		}

		public override string get(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key].Item2;
			else
				return null;
		}

		public override byte? getAlgorithm(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key].Item1;
			else
				return null;
		}

		public override void put(long key, string representation, byte algorithm)
		{
			representations[key] = Tuple.Create(algorithm, representation);

			safe();
		}

		public override void start()
		{
			//
		}

		public override void stop()
		{
			safe();
		}

		public override long getLowestValue()
		{
			return (representations.Count == 0) ? 0 : representations.Keys.Min();
		}

		public override long getHighestValue()
		{
			return (representations.Count == 0) ? 0 : representations.Keys.Max();
		}

	}
}
