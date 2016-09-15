
namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents numbers as a single character in stringmode
	/// Not possible for all numbers
	/// </summary>
	public class CharAlgorithm : RepAlgorithm
	{
		public CharAlgorithm(byte aid)
			: base(aid)
		{
			// NOP
		}

		protected override string Get(long value)
		{
			if (value <= -' ' && value >= -'~' && value != -'"')
			{
				return null;
			}
			else if (value >= ' ' && value <= '~' && value != '"')
			{
				return "\"" + (char)(value) + "\"";
			}
			else
			{
				return null;
			}
		}
	}
}
