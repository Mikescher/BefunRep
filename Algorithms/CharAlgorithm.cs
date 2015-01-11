
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

		public override string get(long value)
		{
			if (value <= -(int)' ' && value >= -(int)'~' && value != -(int)'"')
			{
				return null;
			}
			else if (value >= (int)' ' && value <= (int)'~' && value != (int)'"')
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
