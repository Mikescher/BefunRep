﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BefunRep.FileHandling
{
	public class JSONSafe : RepresentationSafe
	{
		private readonly string filepath;

		private SortedDictionary<long, Tuple<byte, string>> representations;

		public JSONSafe(string path)
		{
			filepath = path;

			Load();
		}

		private void Load()
		{
			if (!File.Exists(filepath))
			{
				File.CreateText(filepath).Close();
				representations = new SortedDictionary<long, Tuple<byte, string>>();
				return;
			}

			string file = File.ReadAllText(filepath);

			representations = JsonConvert.DeserializeObject<SortedDictionary<long, Tuple<byte, string>>>(file) ?? new SortedDictionary<long, Tuple<byte, string>>();
		}

		private void Save()
		{
			string txt = JsonConvert.SerializeObject(representations, Formatting.Indented);

			File.WriteAllText(filepath, txt);
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

			Save();
		}

		public override void Start()
		{
			//
		}

		public override void Stop()
		{
			Save();
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
