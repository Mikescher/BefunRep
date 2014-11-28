﻿using BefunRep.Exceptions;
using BefunRep.FileHandling;
using System.Collections.Generic;
using System.Text;

namespace BefunRep.Algorithms
{
	public class FactorizationAlgorithm : RepAlgorithm
	{

		public FactorizationAlgorithm(RepresentationSafe s)
			: base(s)
		{
			//
		}

		public override string get(int lit)
		{
			bool isneg;
			if (lit < 0)
			{
				StringBuilder p = new StringBuilder();
				p.Append('0');
				getFactors(p, -lit);
				p.Append('-');

				return p.ToString();
			}
			else if (lit == 0)
			{
				return "0";
			}
			else
			{
				return getFactors(new StringBuilder(), lit).ToString();
			}
		}

		private StringBuilder getFactors(StringBuilder p, long a)
		{
			List<int> result = new List<int>();

			if (a < 10)
			{
				p.Append(dig(a));
				return p;
			}

			for (byte i = 9; i > 1; i--)
			{
				if (a % i == 0)
				{
					getFactors(p, a / i);
					p.Append(dig(i));
					p.Append('*');
					return p;
				}
			}

			for (byte i = 1; i < 10; i++)
			{
				if ((a - i) % 9 == 0)
				{
					getFactors(p, a - i);
					p.Append(dig(i));
					p.Append('+');
					return p;
				}
			}

			throw new WTFException();
		}
	}
}