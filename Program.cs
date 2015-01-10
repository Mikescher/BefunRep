using BefunRep.FileHandling;
using BefunRep.Log;
using BefunRep.OutputHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BefunRep
{
	class Program
	{
		public const string VERSION = "1.0";
		public const string TITLE = "BefunRep";

		private readonly DateTime startTime = DateTime.Now;
		private List<int> founds = new List<int>();

		private long lowerBoundary;
		private long upperBoundary;
		private bool testResults;
		private bool doReset;
		private int statsLevel;
		private int algorithm;
		private string safepath;
		private string outpath;
		private bool quiet;
		private int iterations;
		private string logpath;

		private bool boundaryDiscovery = false;

		private RepresentationSafe safe;
		private OutputFormatter formatter;

		static void Main(string[] args)
		{
			new Program(args);
		}

		public Program(string[] args)
		{
			try
			{
				Console.SetBufferSize(256, 8192);
				Console.WindowHeight = Math.Max(Console.WindowHeight, 40);
				Console.WindowWidth = Math.Max(Console.WindowWidth, 140);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Can't configure Console:");
				Console.Error.WriteLine(e.ToString());
			}

			printHeader();

			//##############

			CommandLineArguments cmda = loadCMDA(args);

			if (cmda.isEmpty() || cmda.IsSet("help"))
			{
				printHelp();
				return;
			}


			interpreteCMDA(cmda);

			RepCalculator r = new RepCalculator(lowerBoundary, upperBoundary, testResults, safe, quiet);
			outputCMDA();

			//##############

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Calculations Started.", DateTime.Now);
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();

			for (int i = 0; i < iterations || iterations < 0; i++) // iterations neg => run until no changes
			{
				int foundcount = r.calculate(algorithm);
				founds.Add(foundcount);

				ConsoleLogger.WriteLine();
				ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Iteration {1} Finished (+{2})", DateTime.Now, i, foundcount);
				ConsoleLogger.WriteLine();

				if (foundcount == 0)
					break;
			}

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Caclulations Finished.", DateTime.Now);
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Outputting Started.", DateTime.Now);

			safe.start();
			formatter.Output(safe);
			safe.stop();

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Outputting Finished.", DateTime.Now);

			//##############

			ConsoleLogger.WriteLine();

			printStats(safe);

			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();

			ConsoleLogger.save(); // ############# ENDE #############
			printAnyKeyMessage();
		}

		[ConditionalAttribute("DEBUG")]
		private static void printAnyKeyMessage()
		{
			ConsoleLogger.WriteLine("Press any Key to exit.");

			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();

			Console.ReadLine();
		}

		private static void printHeader()
		{
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine(@"                      ,...                                  // mikescher.de    ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@"7MM'''Yp,           .d' ''                      `7MM'''Mq.                     ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@" MM    Yb           dM`                           MM   `MM.                    ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@" MM    dP  .gP'Ya  mMMmm`7MM  `7MM  `7MMpMMMb.    MM   ,M9  .gP'Ya `7MMpdMAo.  ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@" MM'''bg. ,M'   Yb  MM    MM    MM    MM    MM    MMmmdM9  ,M'   Yb  MM   `Wb  ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@" MM    `Y 8M''''''  MM    MM    MM    MM    MM    MM  YM.  8M''''''  MM    M8  ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@" MM    ,9 YM.    ,  MM    MM    MM    MM    MM    MM   `Mb.YM.    ,  MM   ,AP  ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@"JMMmmmd9   `Mbmmd'.JMML.  `Mbod'YML..JMML  JMML..JMML. .JMM.`Mbmmd'  MMbmmd'   ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@"                                                                     MM        ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@"                                                                   .JMML.      ".Replace("'", "\""));
			ConsoleLogger.WriteLine(@"VERSION: " + VERSION);
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine("###############################################################################");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
		}

		private void printHelp()
		{
			ConsoleLogger.WriteLine("Possible Commandline Arguments:");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine("################################");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine("-lower=[int]");
			ConsoleLogger.WriteLine("-upper=[int]");
			ConsoleLogger.WriteLine("-notest");
			ConsoleLogger.WriteLine("-quiet");
			ConsoleLogger.WriteLine("-reset");
			ConsoleLogger.WriteLine("-algorithm=[0 - " + (RepCalculator.algorithms.Length - 1) + "]");
			ConsoleLogger.WriteLine("-safe=[filename].[csv|json|bin|dat]");
			ConsoleLogger.WriteLine("-out=[filename].[csv|json|xml]");
			ConsoleLogger.WriteLine("-iterations=[-1 | 0-n ]");
			ConsoleLogger.WriteLine("-stats=[0-3]");
			ConsoleLogger.WriteLine("-log={directory of file}");
			ConsoleLogger.WriteLine("-help");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine("################################");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
		}

		private void outputCMDA()
		{
			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Limits     := [{1}, {2}]{3}", DateTime.Now,
				lowerBoundary,
				upperBoundary,
				boundaryDiscovery ? "      (via auto discovery)" : "");

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Iterations := {1}", DateTime.Now, iterations < 0 ? "INF" : (iterations == 0 ? "NONE" : (iterations.ToString())));

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Testing    := {1}", DateTime.Now, testResults.ToString().ToLower());

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Reset      := {1}", DateTime.Now, doReset.ToString().ToLower());

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Quiet      := {1}", DateTime.Now, quiet.ToString().ToLower());

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Statistics := {1}", DateTime.Now, new string[] { "none", "simple", "verbose", "all" }[statsLevel]);

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Algorithm  := {1}", DateTime.Now, algorithm == -1 ? "all" : RepCalculator.algorithmNames[algorithm]);

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Safetype   := {1}", DateTime.Now, safe.GetType().Name);

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Safepath   := {1}", DateTime.Now, safepath);

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Outputtype := {1}", DateTime.Now, formatter.GetType().Name);

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Outputpath := {1}", DateTime.Now, outpath);

			ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Logpath := {1}", DateTime.Now, logpath ?? "<NULL>");

			ConsoleLogger.WriteLine();
		}

		private void interpreteCMDA(CommandLineArguments cmda)
		{
			if (doReset)
				File.Delete(safepath); // reset;

			if (safepath.ToLower().EndsWith(".csv"))
				safe = new CSVSafe(safepath);
			else if (safepath.ToLower().EndsWith(".json"))
				safe = new JSONSafe(safepath);
			else if (safepath.ToLower().EndsWith(".bin") || safepath.ToLower().EndsWith(".dat"))
				safe = new BinarySafe(safepath, lowerBoundary, upperBoundary);
			else
				safe = new CSVSafe(safepath);

			// Init values
			safe.start();
			safe.stop();

			if (!(cmda.IsSet("lower") || cmda.IsSet("upper")))
			{
				lowerBoundary = safe.getLowestValue();
				upperBoundary = safe.getHighestValue();

				boundaryDiscovery = true;
			}

			if (outpath != null)
			{
				if (outpath.ToLower().EndsWith(".csv"))
					formatter = new CSVOutputFormatter(outpath);
				else if (outpath.ToLower().EndsWith(".json"))
					formatter = new JSONOutputFormatter(outpath);
				else if (outpath.ToLower().EndsWith(".xml"))
					formatter = new XMLOutputFormatter(outpath);
				else
					formatter = new CSVOutputFormatter(outpath);
			}
			else
			{
				formatter = new DummyOutputFormatter();
			}

			ConsoleLogger.setPath(logpath);
		}

		private CommandLineArguments loadCMDA(string[] args)
		{
			CommandLineArguments cmda = new CommandLineArguments(args);

			lowerBoundary = cmda.GetLongDefault("lower", 0);
			upperBoundary = cmda.GetLongDefault("upper", 0);
			testResults = !cmda.IsSet("notest");
			doReset = cmda.IsSet("reset");
			quiet = cmda.IsSet("q") || cmda.IsSet("quiet");
			statsLevel = cmda.GetIntDefaultRange("stats", 1, 0, 4);
			algorithm = cmda.GetIntDefaultRange("algorithm", -1, -1, RepCalculator.algorithms.Length);
			safepath = cmda.GetStringDefault("safe", "out.csv");
			outpath = cmda.GetStringDefault("out", null);
			iterations = cmda.GetIntDefault("iterations", 1);
			logpath = cmda.GetStringDefault("log", null);
			return cmda;
		}

		private void printStats(RepresentationSafe safe)
		{
			safe.start();

			SafeInfo info = safe.getInformations();

			if (statsLevel >= 1) //############################################
			{

				ConsoleLogger.WriteLine("  Statistics  ");
				ConsoleLogger.WriteLine("##############");
				ConsoleLogger.WriteLine();

				for (int i = 0; i < founds.Count; i++)
				{
					ConsoleLogger.WriteLineFormatted("{0,-8} Updates found in iteration {1}", founds[i], i);
				}

				ConsoleLogger.WriteLine();

				ConsoleLogger.WriteLineFormatted("{0}/{1} Representations found", info.nonNullCount, info.count);
				ConsoleLogger.WriteLineFormatted("{0} Algorithms registered", RepCalculator.algorithms.Length);
				ConsoleLogger.WriteLineFormatted("Run Duration = {0:hh} hours {0:mm} minutes {0:ss} seconds {0:ff} milliseconds", startTime - DateTime.Now);

				ConsoleLogger.WriteLine();

				for (int i = 0; i < RepCalculator.algorithmTime.Length; i++)
				{
					ConsoleLogger.WriteLineFormatted("Time per algorithm {0, 24}: {1,6} ms",
						RepCalculator.algorithmNames[i],
						RepCalculator.algorithmTime[i]);
				}

				ConsoleLogger.WriteLine();

				if (statsLevel >= 2) //########################################
				{
					for (int i = 0; i < RepCalculator.algorithms.Length; i++)
					{
						ConsoleLogger.WriteLineFormatted("{0,6} Representations with algorithm {1,16} ({2:0.##}%)",
							info.nonNullPerAlgorithm[i],
							RepCalculator.algorithmNames[i],
							info.nonNullPerAlgorithm[i] * 100d / info.count);
					}

					ConsoleLogger.WriteLine();

					if (statsLevel >= 3) //####################################
					{
						ConsoleLogger.WriteLineFormatted("Average representation width = {0:0.###}", info.avgLen);

						for (int i = 0; i < RepCalculator.algorithms.Length; i++)
						{
							ConsoleLogger.WriteLineFormatted("Average representation width with algorithm {0,16}  = {1:0.###}",
								RepCalculator.algorithmNames[i],
								info.avgLenPerAlgorithm[i]);
						}

						ConsoleLogger.WriteLine();

						ConsoleLogger.WriteLineFormatted("Representation length (min|max) = [{0,3},{1,3}]", info.minLen, info.maxLen);

						for (int i = 0; i < RepCalculator.algorithms.Length; i++)
						{
							ConsoleLogger.WriteLineFormatted("Representation length (min|max) for {0,16} = [{1,3},{2,3}]",
								RepCalculator.algorithmNames[i],
								info.minLenPerAlgorithm[i],
								info.maxLenPerAlgorithm[i]);
						}

						ConsoleLogger.WriteLine();
					}
				}
			}

			safe.stop();
		}
	}
}
