using BefunRep.Helper;
using System.Linq;
using System.Text;

namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents the numbers in (optimized) Base representation 
	/// => Not always "normalized" base representation 
	/// 
	/// Includes Base_2 - Base_9
	/// </summary>
	public class BaseNAlgorithm : RepAlgorithm
	{
		public BaseNAlgorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long value)
		{
			return Enumerable.Range(2, 8).Select(p => Get(value, p)).Where(p => p != null).OrderBy(p => p.Length).FirstOrDefault();
		}

		private string Get(long value, int befbase)
		{
			if (value < 0)
				return null;
			else
				return GetPositive(value, befbase);
		}

		private string GetPositive(long value, int befbase)
		{
			StringBuilder pNum = new StringBuilder();

			StringBuilder pOp = new StringBuilder();

			string rep = BefungeHelper.ConvertToBase(value, befbase);

			bool skipMBase = false;
			for (int i = 0; i < rep.Length; i++)
			{
				int digit = rep[i] - '0';
				bool last = i == (rep.Length - 1);
				bool first = i == 0;

				if (first)
				{
					if (digit == 1 && !last)
					{
						pNum.Append(Dig(befbase)); // Don't calculate 1 * $befbase ... directly write $befbase
						skipMBase = true;
					}
					else
					{
						pNum.Append(Dig(digit));
					}
				}
				else
				{
					if (skipMBase)
					{
						skipMBase = false;
					}
					else
					{
						pNum.Append(Dig(befbase));
						pOp.Append("*");
					}

					if (digit != 0) // if digit == 0 dont calculate +0
					{
						pNum.Append(Dig(digit));
						pOp.Append("+");
					}
				}
			}

			return new string(pNum.ToString().ToCharArray().Reverse().ToArray()) + pOp;
		}
	}
}
