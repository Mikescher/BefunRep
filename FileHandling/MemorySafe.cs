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

		public override string GetRep(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key].Item2;
			else
				return null;
		}

		public override byte? GetAlgorithm(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key].Item1;
			else
				return null;
		}

		public override Tuple<byte, string> GetCombined(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key];
			else
				return null;
		}

		public override void Put(long key, string representation, byte algorithm)
		{
			representations[key] = Tuple.Create(algorithm, representation);

			safe();
		}

		public override void Start()
		{
			//
		}

		public override void Stop()
		{
			safe();
		}

		public override void LightLoad()
		{
			// NOP
		}

		public override long GetLowestValue()
		{
			return (representations.Count == 0) ? 0 : representations.Keys.Min();
		}

		public override long GetHighestValue()
		{
			return (representations.Count == 0) ? 0 : representations.Keys.Max();
		}

	}
}
