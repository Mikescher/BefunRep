using BefunRep.Algorithms;
using BefunRep.FileHandling;
using BefunRep.Log;
using BefunRep.Test;
using System;
using System.Linq;
using System.Threading;

namespace BefunRep
{
	public class RepCalculator
	{
		public const byte ALGO_ID_BASE9          = 0;
		public const byte ALGO_ID_FACTORIZATION  = 1;
		public const byte ALGO_ID_CHAR           = 2;
		public const byte ALGO_ID_STRINGIFY      = 3;
		public const byte ALGO_ID_BASEN          = 4;
		public const byte ALGO_ID_POWER          = 5;
		public const byte ALGO_ID_DIGITADD       = 6;
		public const byte ALGO_ID_SIMPLENEGATIVE = 7;
		public const byte ALGO_ID_DIGITMULT      = 8;
		public const byte ALGO_ID_CHARMULT       = 9;

		public static readonly RepAlgorithm[] algorithms =
		{
			new Base9Algorithm(0),				// [0]
			new FactorizationAlgorithm(1),		// [1]
			new CharAlgorithm(2),				// [2]
			new StringifyAlgorithm(3),			// [3]
			new BaseNAlgorithm(4),				// [4]
			new PowerAlgorithm(5),				// [5]
			new DigitAddAlgorithm(6),			// [6]
			new SimpleNegativeAlgorithm(7),		// [7]
			new DigitMultAlgorithm(8),			// [8]
			new CharMultAlgorithm(9),			// [9]
		};

		public static string[] algorithmNames = algorithms.Select(p => p.GetType().Name.Replace("Algorithm", "")).ToArray();
		public static long[] algorithmTime = new long[algorithms.Length];

		private readonly long lowerB;
		private readonly long upperB;
		private readonly bool quiet;

		private readonly RepresentationSafe safe;
		private readonly ResultTester tester;

		public RepCalculator(long low, long high, bool test, RepresentationSafe rsafe, bool isQuiet)
		{
			this.lowerB = low;
			this.upperB = high;
			this.safe = rsafe;
			this.quiet = isQuiet;

			if (test)
				tester = new ExecuteResultTester();
			else
				tester = new DummyResultTester();

			foreach (var algo in algorithms)
				algo.Representations = rsafe;

			algorithmTime = Enumerable.Repeat(0L, algorithmTime.Length).ToArray();
		}

		public int calculate(int algonum)
		{
			int found = 0;

			if (algonum < 0)
				return calculate();

			safe.Start();

			found += calculateSingleAlgorithm(algonum);

			safe.Stop();

			return found;
		}

		public int calculate()
		{
			int found = 0;

			safe.Start();

			for (int algonum = 0; algonum < algorithms.Length; algonum++)
			{
				found += calculateSingleAlgorithm(algonum);
			}

			safe.Stop();

			return found;
		}

		private int calculateSingleAlgorithm(int algonum)
		{
			int algofound = 0;

			long time = Environment.TickCount;

			RepAlgorithm algo = algorithms[algonum];

			for (long v = lowerB; v < upperB; v++)
			{
				algofound += calculateSingleNumber(algo, v) ? 1 : 0;
			}
			time = Environment.TickCount - time;
			algorithmTime[algonum] += time;

			ConsoleLogger.WriteTimedLine("Algorithm {0} Finished (+{1})", algorithmNames[algonum], algofound);

			return algofound;
		}

		private bool calculateSingleNumber(RepAlgorithm algo, long v)
		{
			bool found = false;

			string outerror;
			string before = safe.GetRep(v);
			string beforeAlgo = before == null ? null : algorithmNames[safe.GetAlgorithm(v).Value];
			if (before == null)
				before = "";
			if (beforeAlgo == null)
				beforeAlgo = "";
			string result = algo.Calculate(v);

			if (result != null)
			{
				found = true;

				if (!quiet)
				{
					if (before != "")
						ConsoleLogger.WriteTimedLine("{0,16} Found: {1,11}  ->  {2,-24} (before {3,-16} {4,-24} = +{5})",
								algo.GetType().Name.Replace("Algorithm", ""),
								v,
								result,
								beforeAlgo,
								before,
								before.Length - result.Length);
					else
						ConsoleLogger.WriteTimedLine("{0,16} Found: {1,11}  ->  {2,-24}",
								algo.GetType().Name.Replace("Algorithm", ""),
								v,
								result);
				}

				if (!tester.test(result, v, out outerror))
				{
					Console.Error.WriteLine("[{0:HH:mm:ss}] TEST RESULT ERROR", DateTime.Now);
					Console.Error.WriteLine("[{0:HH:mm:ss}] #################", DateTime.Now);
					Console.Error.WriteLine("[{0:HH:mm:ss}] TEST Program = {1}", DateTime.Now, result);
					Console.Error.WriteLine("[{0:HH:mm:ss}] TEST Algorithm = {1}", DateTime.Now, algo.GetType().Name);
					Console.Error.WriteLine("[{0:HH:mm:ss}] TEST Expected Result = {1}", DateTime.Now, v);
					Console.Error.WriteLine("[{0:HH:mm:ss}] TEST Problem = \"{1}\"", DateTime.Now, outerror);

					Thread.Sleep(5 * 1000);
				}
			}

			return found;
		}
	}
}
