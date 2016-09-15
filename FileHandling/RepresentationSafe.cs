
using System;
using System.Linq;
namespace BefunRep.FileHandling
{
	public struct SafeInfo
	{
		public long LowestValue;
		public long HighestValue;

		public long Count;

		public long NonNullCount;
		public long[] NonNullPerAlgorithm;

		public long TotalLen;
		public long[] TotalLenPerAlgorithm;

		public double AvgLen;
		public double[] AvgLenPerAlgorithm;

		public int MinLen;
		public int[] MinLenPerAlgorithm;

		public int MaxLen;
		public int[] MaxLenPerAlgorithm;
	}

	public abstract class RepresentationSafe
	{
		public abstract string GetRep(long key);
		public abstract byte? GetAlgorithm(long key);
		public abstract Tuple<byte,string> GetCombined(long key);
		public abstract void Put(long key, string representation, byte algorithm);

		public abstract void Start();
		public abstract void Stop();
		public abstract void LightLoad();

		public abstract long GetLowestValue();
		public abstract long GetHighestValue();

		public SafeInfo GetInformations()
		{
			long low = GetLowestValue();
			long high = GetHighestValue();

			long nonNullCount = 0;
			long[] nonNullPerAlgorithm = Enumerable.Repeat(0L, RepCalculator.algorithms.Length).ToArray();

			long totalLen = 0;
			long[] totalLenPerAlgorithm = Enumerable.Repeat(0L, RepCalculator.algorithms.Length).ToArray();

			int minLen = int.MaxValue;
			int[] minLenPerAlgorithm = Enumerable.Repeat(int.MaxValue, RepCalculator.algorithms.Length).ToArray();

			int maxLen = int.MinValue;
			int[] maxLenPerAlgorithm = Enumerable.Repeat(int.MinValue, RepCalculator.algorithms.Length).ToArray();

			for (long i = low; i < high; i++)
			{
				string rep = GetRep(i);

				if (rep == null)
					continue;

				byte? algoOptional = GetAlgorithm(i);

				if (algoOptional == null)
					continue;

				byte algo = algoOptional.Value;

				nonNullCount++;
				nonNullPerAlgorithm[algo]++;

				totalLen += rep.Length;
				totalLenPerAlgorithm[algo] += rep.Length;

				minLen = Math.Min(minLen, rep.Length);
				minLenPerAlgorithm[algo] = Math.Min(minLenPerAlgorithm[algo], rep.Length);

				maxLen = Math.Max(maxLen, rep.Length);
				maxLenPerAlgorithm[algo] = Math.Max(maxLenPerAlgorithm[algo], rep.Length);
			}


			if (minLen == int.MaxValue || maxLen == int.MinValue)
			{
				minLen = 0;
				maxLen = 0;
			}

			double avgLen;
			double[] avgLenPerAlgorithm = new double[RepCalculator.algorithms.Length];

			for (int i = 0; i < totalLenPerAlgorithm.Length; i++)
			{
				if (nonNullPerAlgorithm[i] == 0)
					avgLenPerAlgorithm[i] = 0;
				else
					avgLenPerAlgorithm[i] = totalLenPerAlgorithm[i] * 1.0 / nonNullPerAlgorithm[i];

				if (minLenPerAlgorithm[i] == int.MaxValue || maxLenPerAlgorithm[i] == int.MinValue)
				{
					minLenPerAlgorithm[i] = 0;
					maxLenPerAlgorithm[i] = 0;
				}
			}



			if (nonNullCount == 0)
				avgLen = 0;
			else
				avgLen = totalLen * 1.0 / nonNullCount;

			return new SafeInfo
			{
				LowestValue = low,
				HighestValue = high,
				Count = high - low,
				NonNullCount = nonNullCount,
				NonNullPerAlgorithm = nonNullPerAlgorithm,
				TotalLen = totalLen,
				TotalLenPerAlgorithm = totalLenPerAlgorithm,
				MinLen = minLen,
				MinLenPerAlgorithm = minLenPerAlgorithm,
				MaxLen = maxLen,
				MaxLenPerAlgorithm = maxLenPerAlgorithm,
				AvgLen = avgLen,
				AvgLenPerAlgorithm = avgLenPerAlgorithm,
			};
		}

		public long GetNonNullRepresentations()
		{
			long low = GetLowestValue();
			long high = GetHighestValue();

			long count = 0;

			for (long i = low; i < high; i++)
			{
				if (GetRep(i) != null)
					count++;
			}

			return count;
		}

		public long CountRepresentationsPerAlgorithm(int p)
		{
			long low = GetLowestValue();
			long high = GetHighestValue();

			long count = 0;

			for (long i = low; i < high; i++)
			{
				if (GetRep(i) != null && GetAlgorithm(i) == p)
					count++;
			}

			return count;
		}

		public double GetAverageRepresentationWidth()
		{
			long low = GetLowestValue();
			long high = GetHighestValue();

			long count = 0;
			long len = 0;

			for (long i = low; i < high; i++)
			{
				string rep = GetRep(i);
				if (GetRep(i) != null)
				{
					count++;
					len += rep.Length;
				}
			}

			if (count == 0)
				return 0;

			return len / count;
		}

		public double GetAverageRepresentationWidthPerAlgorithm(int p)
		{
			long low = GetLowestValue();
			long high = GetHighestValue();

			long count = 0;
			long len = 0;

			for (long i = low; i < high; i++)
			{
				string rep = GetRep(i);
				if (GetRep(i) != null && GetAlgorithm(i) == p)
				{
					count++;
					len += rep.Length;
				}
			}

			if (count == 0)
				return 0;

			return len / count;
		}
	}
}
