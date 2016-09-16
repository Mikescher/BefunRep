using BefunRep.FileHandling;

namespace BefunRep.Algorithms
{
	public abstract class RepAlgorithm
	{
		public RepresentationSafe Representations = null;
		public readonly byte AlgorithmID;

		protected RepAlgorithm(byte id)
		{
			AlgorithmID = id;
		}

		public string Calculate(long value)
		{
			string result = Get(value);

			if (string.IsNullOrEmpty(result))
			{
				return null;
			}

			var old = Representations.GetCombined(value);

			if (old == null)
			{
				// new value

				Representations.Put(value, result, AlgorithmID);
				return result;
			}
			else if (result == old.Representation)
			{
				// already in safe

				return null;
			}
			else if (!result.Contains("\"") && old.Length == result.Length && old.Representation.Contains("\""))
			{
				// non-stringmode is preffered

				Representations.Put(value, result, AlgorithmID);
				return result;
			}
			else if (!result.Contains(" ") && old.Length == result.Length && old.Representation.Contains(" "))
			{
				// non-space is preffered

				Representations.Put(value, result, AlgorithmID);
				return result;
			}
			else if (old.Length <= result.Length)
			{
				// safe value is better

				return null;
			}
			else
			{
				// improvement

				Representations.Put(value, result, AlgorithmID);
				return result;
			}

		}

		protected char Dig(long v)
		{
			return (char)(v + '0');
		}

		protected char ChrSign(long i)
		{
			if (i < 0)
				return '-';
			else if (i > 0)
				return '+';
			else
				return '0';
		}

		protected abstract string Get(long value);
	}
}
