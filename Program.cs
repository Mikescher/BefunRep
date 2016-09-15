using BefunRep.FileHandling;
using BefunRep.Log;
using BefunRep.OutputHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BefunRep
{
	class Program
	{
		public const string VERSION = "1.1";
		public const string TITLE = "BefunRep";

		private readonly DateTime startTime = DateTime.Now;
		private readonly List<int> founds = new List<int>();

		private long lowerBoundary;
		private long upperBoundary;
		private bool testResults;
		private bool doReset;
		private int statsLevel;
		private int algorithm;
		private string safepath;
		private string outpath;
		private long maxoutputsize;
		private long? outputminimum;
		private long? outputmaximum;
		private bool quiet;
		private int iterations;
		private string logpath;

		private bool boundaryDiscovery = false;

		private RepresentationSafe safe;
		private OutputFormatter formatter;

		static void Main(string[] args)
		{
			new Program().Run(args);
		}

		private void Run(string[] args)
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

			PrintHeader();

			//##############

			CommandLineArguments cmda = LoadCMDA(args);
			InterpreteCMDA(cmda);

			if (cmda.isEmpty() || cmda.IsSet("help"))
			{
				PrintHelp();
				PrintAnyKeyMessage();
				return;
			}

			RepCalculator r = new RepCalculator(lowerBoundary, upperBoundary, testResults, safe, quiet);
			OutputCMDA();

			//##############

			ConsoleLogger.WriteTimedLine("Calculations Started.");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();

			for (int i = 0; i < iterations || iterations < 0; i++) // iterations neg => run until no changes
			{
				int foundcount = r.calculate(algorithm);
				founds.Add(foundcount);

				ConsoleLogger.WriteLine();
				ConsoleLogger.WriteTimedLine("Iteration {0} Finished (+{1})", i, foundcount);
				ConsoleLogger.WriteLine();

				if (foundcount == 0)
					break;
			}

			ConsoleLogger.WriteTimedLine("Caclulations Finished.");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteTimedLine("Outputting Started.");

			if (outpath != null || statsLevel > 0)
			{
				safe.Start();
				{
					if (outpath != null)
					{
						formatter.Output(safe, outpath, maxoutputsize, outputminimum ?? long.MinValue, outputmaximum ?? long.MaxValue);

						ConsoleLogger.WriteTimedLine("Outputting Finished.");
					}

					if (statsLevel > 0)
					{
						ConsoleLogger.WriteLine();

						PrintStats(safe);
					}
				}
				safe.Stop();
			}

			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();

			ConsoleLogger.Save(); // ############# ENDE #############

			if (logpath == null)
				PrintAnyKeyMessage();
		}

		private static void PrintAnyKeyMessage()
		{
			ConsoleLogger.WriteLine("Press any Key to exit.");

			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();

			Console.ReadLine();
		}

		private static void PrintHeader()
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

		private void PrintHelp()
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
			ConsoleLogger.WriteLine("-safe=[filename].[csv|json|bin|bin.gz]");
			ConsoleLogger.WriteLine("-out=[filename].[csv|tsv|json|xml]");
			ConsoleLogger.WriteLine("-iterations=[-1 | 0-n ]");
			ConsoleLogger.WriteLine("-stats=[0-3]");
			ConsoleLogger.WriteLine("-log={directory of file}");
			ConsoleLogger.WriteLine("-maxoutput=[-1 | 1-n]");
			ConsoleLogger.WriteLine("-outmin=[int]");
			ConsoleLogger.WriteLine("-outmax=[int]");
			ConsoleLogger.WriteLine("-help");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine("################################");
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
			ConsoleLogger.WriteLine();
		}

		private void OutputCMDA()
		{
			ConsoleLogger.WriteTimedLine("Limits        := [{0}, {1}]{2}",
				lowerBoundary,
				upperBoundary,
				boundaryDiscovery ? "      (via auto discovery)" : "");

			ConsoleLogger.WriteTimedLine("Iterations    := {0}", iterations < 0 ? "INF" : (iterations == 0 ? "NONE" : (iterations.ToString())));

			ConsoleLogger.WriteTimedLine("Testing       := {0}", testResults.ToString().ToLower());

			ConsoleLogger.WriteTimedLine("Reset         := {0}", doReset.ToString().ToLower());

			ConsoleLogger.WriteTimedLine("Quiet         := {0}", quiet.ToString().ToLower());

			ConsoleLogger.WriteTimedLine("Statistics    := {0}", new[] { "none", "simple", "verbose", "all" }[statsLevel]);

			ConsoleLogger.WriteTimedLine("Algorithm     := {0}", algorithm == -1 ? "all" : RepCalculator.algorithmNames[algorithm]);

			ConsoleLogger.WriteTimedLine("Safetype      := {0}", safe.GetType().Name);

			ConsoleLogger.WriteTimedLine("Safepath      := {0}", safepath);

			ConsoleLogger.WriteTimedLine("Outputtype    := {0}", formatter.GetType().Name);

			ConsoleLogger.WriteTimedLine("Outputrange   := {0}", ((outputminimum ?? outputmaximum) == null) ? ("[ALL]") : ("[" + outputminimum + " - " + outputmaximum + "]"));

			ConsoleLogger.WriteTimedLine("Outputpath    := {0}", outpath);

			ConsoleLogger.WriteTimedLine("MaxOutputSize := {0}", maxoutputsize);

			ConsoleLogger.WriteTimedLine("Logpath       := {0}", logpath ?? "<NULL>");

			ConsoleLogger.WriteLine();
		}

		private void InterpreteCMDA(CommandLineArguments cmda)
		{
			if (doReset)
				File.Delete(safepath); // reset;

			if (safepath.ToLower().EndsWith(".csv"))
				safe = new CSVSafe(safepath);
			else if (safepath.ToLower().EndsWith(".json"))
				safe = new JSONSafe(safepath);
			else if (safepath.ToLower().EndsWith(".bin.gz") || safepath.ToLower().EndsWith(".dat.gz"))
				safe = new GZipBinarySafe(safepath, lowerBoundary, upperBoundary);
			else if (safepath.ToLower().EndsWith(".bin") || safepath.ToLower().EndsWith(".dat"))
				safe = new BinarySafe(safepath, lowerBoundary, upperBoundary);
			else
				safe = new CSVSafe(safepath);

			// Init values
			safe.LightLoad();

			if (!(cmda.IsSet("lower") || cmda.IsSet("upper")))
			{
				lowerBoundary = safe.GetLowestValue();
				upperBoundary = safe.GetHighestValue();

				boundaryDiscovery = true;
			}

			if (outpath != null)
			{
				if (outpath.ToLower().EndsWith(".csv"))
					formatter = new CSVOutputFormatter();
				if (outpath.ToLower().EndsWith(".tsv"))
					formatter = new TSVOutputFormatter();
				else if (outpath.ToLower().EndsWith(".json"))
					formatter = new JSONOutputFormatter();
				else if (outpath.ToLower().EndsWith(".xml"))
					formatter = new XMLOutputFormatter();
				else
					formatter = new CSVOutputFormatter();
			}
			else
			{
				formatter = new DummyOutputFormatter();
			}

			ConsoleLogger.SetPath(logpath);
		}

		private CommandLineArguments LoadCMDA(string[] args)
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
			maxoutputsize = cmda.GetLongDefault("maxoutput", -1);
			outputminimum = cmda.GetLongDefaultNull("outmin");
			outputmaximum = cmda.GetLongDefaultNull("outmax");
			return cmda;
		}

		private void PrintStats(RepresentationSafe statsafe)
		{
			if (statsLevel >= 1) //############################################
			{
				SafeInfo info = statsafe.GetInformations();

				ConsoleLogger.WriteLine("  Statistics  ");
				ConsoleLogger.WriteLine("##############");
				ConsoleLogger.WriteLine();

				for (int i = 0; i < founds.Count; i++)
				{
					ConsoleLogger.WriteLineFormatted("{0,-8} Updates found in iteration {1}", founds[i], i);
				}

				ConsoleLogger.WriteLine();

				ConsoleLogger.WriteLineFormatted("{0}/{1} Representations found", info.NonNullCount, info.Count);
				ConsoleLogger.WriteLineFormatted("{0} Algorithms registered", RepCalculator.algorithms.Length);
				ConsoleLogger.WriteLineFormatted("Run Duration = {0:hh} hours {0:mm} minutes {0:ss} seconds {0:ff} milliseconds", startTime - DateTime.Now);

				ConsoleLogger.WriteLine();

				for (int i = 0; i < RepCalculator.algorithmTime.Length; i++)
				{
					ConsoleLogger.WriteLineFormatted("Time per algorithm {0, 24}: {1,6} ms (= {2,-20} )",
						RepCalculator.algorithmNames[i],
						RepCalculator.algorithmTime[i],
						FormatMilliseconds(RepCalculator.algorithmTime[i]));
				}

				ConsoleLogger.WriteLine();

				if (statsLevel >= 2) //########################################
				{
					for (int i = 0; i < RepCalculator.algorithms.Length; i++)
					{
						ConsoleLogger.WriteLineFormatted("{0,6} Representations with algorithm {1,16} ({2:0.##}%)",
							info.NonNullPerAlgorithm[i],
							RepCalculator.algorithmNames[i],
							info.NonNullPerAlgorithm[i] * 100d / info.Count);
					}

					ConsoleLogger.WriteLine();

					if (statsLevel >= 3) //####################################
					{
						ConsoleLogger.WriteLineFormatted("Average representation width = {0:0.###}", info.AvgLen);

						for (int i = 0; i < RepCalculator.algorithms.Length; i++)
						{
							ConsoleLogger.WriteLineFormatted("Average representation width with algorithm {0,16}  = {1:0.###}",
								RepCalculator.algorithmNames[i],
								info.AvgLenPerAlgorithm[i]);
						}

						ConsoleLogger.WriteLine();

						ConsoleLogger.WriteLineFormatted("Representation length (min|max) = [{0,3},{1,3}]", info.MinLen, info.MaxLen);

						for (int i = 0; i < RepCalculator.algorithms.Length; i++)
						{
							ConsoleLogger.WriteLineFormatted("Representation length (min|max) for {0,16} = [{1,3},{2,3}]",
								RepCalculator.algorithmNames[i],
								info.MinLenPerAlgorithm[i],
								info.MaxLenPerAlgorithm[i]);
						}

						ConsoleLogger.WriteLine();
					}
				}
			}
		}

		public static string FormatTimespan(TimeSpan ts)
		{
			var parts = string
							.Format("{0}d:{1}h:{2}m:{3}s:{4}ms", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds)
							.Split(':')
							.SkipWhile(s => Regex.Match(s, @"0\w").Success); // skip zero-valued components

			var join = string.Join(" ", parts);

			if (join == "")
				join = "0ms";

			return join;
		}

		public static string FormatMilliseconds(long ms)
		{
			return FormatTimespan(TimeSpan.FromMilliseconds(ms));
		}
	}
}
