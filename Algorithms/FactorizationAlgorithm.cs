﻿using BefunRep.Exceptions;
using System.Text;

namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents numbers as multiple multiplications
	/// If this is not possible it tries to get as near as possible and then use an "add"
	/// </summary>
	public class FactorizationAlgorithm : RepAlgorithm
	{
		public FactorizationAlgorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long lit)
		{
			if (lit < 0)
			{
				return null;
			}
			else if (lit == 0)
			{
				return "0";
			}
			else
			{
				return GetFactors(new StringBuilder(), lit).ToString();
			}
		}

		private StringBuilder GetFactors(StringBuilder p, long a)
		{
			if (a < 10)
			{
				p.Append(Dig(a));
				return p;
			}

			for (byte i = 9; i > 1; i--)
			{
				if (a % i == 0)
				{
					GetFactors(p, a / i);
					p.Append(Dig(i));
					p.Append('*');
					return p;
				}
			}

			for (byte i = 1; i < 10; i++)
			{
				if ((a - i) % 9 == 0)
				{
					GetFactors(p, a - i);
					p.Append(Dig(i));
					p.Append('+');
					return p;
				}
			}

			throw new WTFException();
		}
	}
}
