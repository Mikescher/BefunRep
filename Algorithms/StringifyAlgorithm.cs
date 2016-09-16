using BefunRep.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BefunRep.Algorithms
{
	/// <summary>
	/// Tries to represent numbers as multiplication and addition of stringmode-representable characters
	/// Not possible for all numbers
	/// </summary>
	public class StringifyAlgorithm : RepAlgorithm
	{
		private enum StripOp { Add, Mult }

		private const char MIN_ASCII = ' '; // 32
		private const char MAX_ASCII = '~'; // 126

		public StringifyAlgorithm(byte aid)
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

			if (lit >= 0 && lit <= 9)
			{
				return "" + Dig(lit);
			}

			if (lit < MIN_ASCII && lit >= (MIN_ASCII - 9))
			{
				if (lit + 9 == '"')
				{
					string p = Get(lit + 8);

					if (p == null)
						return null;

					return p + "8-";
				}
				else
				{
					string p = Get(lit + 9);

					if (p == null)
						return null;

					return p + "9-";
				}

			}

			if (lit < (MIN_ASCII - 9))
			{
				return null;
			}

			List<char> str;
			List<StripOp> ops;

			if (CalculateStringOps(out str, out ops, lit))
			{
				StringBuilder p = new StringBuilder();

				p.Append('"');
				foreach (char c in str)
					p.Append(c);
				p.Append('"');

				foreach (StripOp op in ops)
				{
					switch (op)
					{
						case StripOp.Add:
							p.Append('+');
							break;
						case StripOp.Mult:
							p.Append('*');
							break;
						default:
							throw new WTFException();
					}
				}

				return p.ToString();
			}

			return null;
		}

		private static bool CalculateStringOps(out List<char> str, out List<StripOp> ops, long val)
		{
			if (val < MIN_ASCII)
			{
				ops = null;
				str = null;
				return false;
			}

			//##########################################################################

			if (val >= MIN_ASCII && val <= MAX_ASCII && val != '"')
			{
				ops = new List<StripOp>();
				str = new List<char>() { (char)val };
				return true;
			}

			//##########################################################################

			List<char> backupStr = null;
			List<StripOp> backupOps = null;

			for (char curr = MAX_ASCII; curr >= MIN_ASCII; curr--)
			{
				if (curr == '"')
					continue;

				if (val % curr == 0 && val / curr > MIN_ASCII)
				{
					List<char> oStr;
					List<StripOp> oOps;

					if (CalculateStringOps(out oStr, out oOps, val / curr))
					{
						str = oStr.ToList();
						ops = oOps.ToList();

						str.Insert(0, curr);
						ops.Add(StripOp.Mult);

						if (str.Contains(' '))
						{
							backupOps = ops;
							backupStr = str;
						}
						else
						{
							return true;
						}
					}
				}
			}

			//##########################################################################


			for (char curr = MAX_ASCII; curr >= MIN_ASCII; curr--)
			{
				if (curr == '"')
					continue;

				List<char> oStr;
				List<StripOp> oOps;

				if (CalculateStringOps(out oStr, out oOps, val - curr))
				{
					str = oStr.ToList();
					ops = oOps.ToList();

					str.Insert(0, curr);
					ops.Add(StripOp.Add);

					if (str.Contains(' '))
					{
						backupOps = ops;
						backupStr = str;
					}
					else
					{
						return true;
					}
				}
			}

			if (backupStr != null)
			{
				str = backupStr;
				ops = backupOps;
				return true;
			}

			str = null;
			ops = null;
			return false;
		}
	}
}
