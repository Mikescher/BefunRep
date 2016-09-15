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
			Load();
		}

		private void Load()
		{
			representations = new SortedDictionary<long, Tuple<byte, string>>();
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

		public override BefungeRepresentation GetCombined(long key)
		{
			if (representations.ContainsKey(key))
				return new BefungeRepresentation(representations[key].Item1, representations[key].Item2);
			else
				return null;
		}

		public override void Put(long key, string representation, byte algorithm)
		{
			representations[key] = Tuple.Create(algorithm, representation);
		}

		public override void Start()
		{
			//
		}

		public override void Stop()
		{
			//
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
