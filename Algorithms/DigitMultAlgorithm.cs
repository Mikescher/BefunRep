
namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents numbers as 
	/// othernumber [*/] [0-9] 
	/// Not possible for all numbers
	/// </summary>
	public class DigitMultAlgorithm : RepAlgorithm
	{
		public DigitMultAlgorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long value)
		{
			string best = null;

			for (int i = 2; i <= 9; i++)
			{

				string other = Representations.GetRep(value * i);
				if (other == null)
					continue;

				if (best == null || other.Length + 2 < best.Length)
					best = other + Dig(i) + "/";
			}

			for (int i = 2; i <= 9; i++)
			{
				if (value % i != 0)
					continue;

				string other = Representations.GetRep(value / i);
				if (other == null)
					continue;

				if (best == null || other.Length + 2 < best.Length)
					best = other + Dig(i) + "*";
			}

			return best;
		}

	}
}
