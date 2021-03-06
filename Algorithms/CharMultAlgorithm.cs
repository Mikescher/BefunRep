﻿
using System;
using System.Collections.Generic;
using System.Text;
namespace BefunRep.Algorithms
{
	/// <summary>
	/// Tries to represent numbers as stringmode multiplications
	/// Not possible for all numbers
	/// </summary>
	public class CharMultAlgorithm : RepAlgorithm
	{
		private readonly Dictionary<long, string> cache = new Dictionary<long, string>();
		private readonly Stack<char> stack = new Stack<char>();

		public CharMultAlgorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long value)
		{
			if (value >= 0)
			{
				return GetPositive(value);
			}
			else
			{
				return null;
			}
		}

		private string GetPositive(long value)
		{
			string current = GetMultiplicands(value);
			if (current != null)
				current = "" + '"' + current + '"' + "*".Repeat(current.Length - 1);


			for (int i = -9; i <= +9; i++)
			{
				if (i == 0)
					continue;

				string result = GetMultiplicands(value + i);
				if (result == null)
					continue;

				if (current == null || (result.Length * (result.Length - 1) + 4) < current.Length)
					current = "" + '"' + result + '"' + "*".Repeat(result.Length - 1) + Dig(Math.Abs(i)) + ChrSign(-i);
			}

			for (int i = ' '; i <= '~'; i++)
			{
				if (i == '"')
					continue;

				string result = GetMultiplicands(value + i);
				if (result == null)
					continue;

				if (current == null || (result.Length * (result.Length - 1) + 5) < current.Length)
					current = "" + '"' + ((char)i) + result + '"' + "*".Repeat(result.Length - 1) + "\\" + "-";
			}

			for (int i = ' '; i <= '~'; i++)
			{
				if (i == '"')
					continue;

				string result = GetMultiplicands(value - i);
				if (result == null)
					continue;

				if (current == null || (result.Length * (result.Length - 1) + 4) < current.Length)
					current = "" + '"' + ((char)i) + result + '"' + "*".Repeat(result.Length - 1) + '+';
			}

			return current;
		}

		private string GetMultiplicands(long value)
		{
			if (value == 0)
				return null;

			string tgresult;
			if (cache.TryGetValue(value, out tgresult))
				return tgresult;

			long initv = value;

			stack.Clear();
			stack.Push((char)1);

			for (; ; )
			{
				int start = stack.Pop();
				value *= start;

				for (char chr = (char)(Math.Max(start, ' ' - 1) + 1); chr <= '~'; chr++)
				{
					if (chr == '"')
						continue;
					if (value % chr != 0)
						continue;

					stack.Push(chr);
					stack.Push((char)1);
					value /= chr;
					break;
				}

				if (stack.Count == 0)
				{
					cache.Add(initv, null);
					return null;
				}

				if (value == 1)
				{
					StringBuilder builder = new StringBuilder();

					while (stack.Count > 0)
						if (stack.Peek() != 1)
							builder.Append(stack.Pop());
						else
							stack.Pop();

					string result = builder.ToString();
					cache.Add(initv, result);
					return result;
				}
			}
		}
	}
}
