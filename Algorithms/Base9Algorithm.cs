using BefunRep.Helper;
using System.Text;

namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents the numbers as Base 9
	/// </summary>
	public class Base9Algorithm : RepAlgorithm
	{
		public Base9Algorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long value)
		{
			if (value < 0)
			{
				return "0" + GetPositive(-value) + "-";
			}
			else
			{
				return GetPositive(value);
			}
		}

		private string GetPositive(long value)
		{
			StringBuilder p = new StringBuilder();

			string rep = BefungeHelper.ConvertToBase(value, 9);

			for (int i = 0; i < rep.Length; i++)
			{
				p.Append(rep[rep.Length - i - 1]);

				if (i + 1 != rep.Length)
					p.Append('9');
			}

			int count = rep.Length - 1;

			for (int i = 0; i < count; i++)
			{
				p.Append('*');
				p.Append('+');
			}

			return p.ToString();
		}
	}
}
