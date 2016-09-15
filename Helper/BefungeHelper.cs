using System;

namespace BefunRep.Helper
{
	public static class BefungeHelper
	{
		public static string ConvertToBase(long decimalNumber, int radix)
		{
			const int bitsInLong = 64;
			const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			if (radix < 2 || radix > digits.Length)
				throw new ArgumentException("The radix must be >= 2 and <= " + digits.Length);

			if (decimalNumber == 0)
				return "0";

			int index = bitsInLong - 1;
			long currentNumber = Math.Abs(decimalNumber);
			char[] charArray = new char[bitsInLong];

			while (currentNumber != 0)
			{
				int remainder = (int)(currentNumber % radix);
				charArray[index--] = digits[remainder];
				currentNumber = currentNumber / radix;
			}

			string result = new string(charArray, index + 1, bitsInLong - index - 1);
			if (decimalNumber < 0)
			{
				result = "-" + result;
			}

			return result;
		}
	}
}
