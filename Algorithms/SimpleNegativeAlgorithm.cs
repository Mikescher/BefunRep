
namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents negative numbers as [0-9] [positive] -
	/// Not possible for all numbers
	/// </summary>
	public class SimpleNegativeAlgorithm : RepAlgorithm
	{
		public SimpleNegativeAlgorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long value)
		{
			if (value >= 0)
				return null;

			string best = null;

			for (int i = 0; i <= 9; i++)
			{
				string rep = Representations.GetRep(-value + i);
				if (rep == null)
					continue;

				if (best == null || rep.Length + 2 < best.Length)
					best = Dig(i) + rep + "-";
			}

			return best;
		}

	}
}
