﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BefunRep.FileHandling
{
	public class CSVSafe : RepresentationSafe
	{
		private readonly string filepath;

		private SortedDictionary<long, Tuple<byte, string>> representations;

		public CSVSafe(string path)
		{
			filepath = path;

			Load();
		}

		private void Load()
		{
			if (!File.Exists(filepath))
				File.CreateText(filepath).Close();

			string file = File.ReadAllText(filepath);

			var elements = file
							.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
							.Where(p => p.Contains(' '))
							.Select(p => new[] { p.Substring(0, p.IndexOf(' ')), p.Substring(p.IndexOf(' ') + 1) })
							.Select(p => p.Select(q => q.Trim()))
							.Select(p => p.ToList())
							.Where(p => p.Count == 2)
							.Where(p => Regex.IsMatch(p[0], @"[0-9]+\.[012]?[0-9]?[0-9]"))
							.Where(p => p[1] != "");

			representations = new SortedDictionary<long, Tuple<byte, string>>();
			foreach (var item in elements)
			{
				string rep = item[1];
				string[] ident = item[0].Split('.');
				int key = int.Parse(ident[0]);
				byte algo = byte.Parse(ident[1]);

				representations.Add(key, Tuple.Create(algo, rep));
			}
		}

		private void Save()
		{
			string txt = String.Join(Environment.NewLine, representations.Select(p => String.Format("{0, -14} {1}", p.Key + "." + p.Value.Item1, p.Value.Item2)));

			File.WriteAllText(filepath, txt);
		}

		public override string GetRep(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key].Item2;
			return null;
		}

		public override byte? GetAlgorithm(long key)
		{
			if (representations.ContainsKey(key))
				return representations[key].Item1;
			return null;
		}

		public override BefungeRepresentation GetCombined(long key)
		{
			if (representations.ContainsKey(key))
				return new BefungeRepresentation(representations[key].Item1, representations[key].Item2);
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
