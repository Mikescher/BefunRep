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
		public static readonly RepAlgorithm[] Algorithms =
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

		public static readonly string[] AlgorithmNames = Algorithms.Select(p => p.GetType().Name.Replace("Algorithm", "")).ToArray();
		public static long[] AlgorithmTimes = new long[Algorithms.Length];

		private readonly long lowerB;
		private readonly long upperB;
		private readonly bool quiet;

		private readonly RepresentationSafe safe;
		private readonly ResultTester tester;

		public RepCalculator(long low, long high, bool test, RepresentationSafe rsafe, bool isQuiet)
		{
			lowerB = low;
			upperB = high;
			safe = rsafe;
			quiet = isQuiet;

			if (test)
				tester = new ExecuteResultTester();
			else
				tester = new DummyResultTester();

			foreach (var algo in Algorithms)
				algo.Representations = rsafe;

			AlgorithmTimes = Enumerable.Repeat(0L, AlgorithmTimes.Length).ToArray();
		}

		public int Calculate(int algonum)
		{
			int found = 0;

			if (algonum < 0)
				return Calculate();

			safe.Start();

			found += CalculateSingleAlgorithm(algonum);

			safe.Stop();

			return found;
		}

		public int Calculate()
		{
			int found = 0;

			safe.Start();

			for (int algonum = 0; algonum < Algorithms.Length; algonum++)
			{
				found += CalculateSingleAlgorithm(algonum);
			}

			safe.Stop();

			return found;
		}

		private int CalculateSingleAlgorithm(int algonum)
		{
			int algofound = 0;

			long time = Environment.TickCount;

			RepAlgorithm algo = Algorithms[algonum];

			for (long v = lowerB; v < upperB; v++)
			{
				algofound += CalculateSingleNumber(algo, v) ? 1 : 0;
			}
			time = Environment.TickCount - time;
			AlgorithmTimes[algonum] += time;

			ConsoleLogger.WriteTimedLine("Algorithm {0} Finished (+{1})", AlgorithmNames[algonum], algofound);

			return algofound;
		}

		private bool CalculateSingleNumber(RepAlgorithm algo, long v)
		{
			bool found = false;

			string before = safe.GetRep(v);
			string beforeAlgo = before == null ? null : AlgorithmNames[safe.GetAlgorithm(v) ?? 0];
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

				string outerror;
				if (!tester.Test(result, v, out outerror))
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
