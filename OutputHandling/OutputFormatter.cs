using BefunRep.FileHandling;
using BefunRep.Log;
using System;
using System.IO;

namespace BefunRep.OutputHandling
{
	public abstract class OutputFormatter
	{
		public OutputFormatter()
		{
			//
		}

		public virtual void Output(RepresentationSafe safe, string filepath, long maxOutputSize, long omin, long omax)
		{
			long min = Math.Max(safe.GetLowestValue(), omin);
			long max = Math.Min(safe.GetHighestValue(), omax);

			if (maxOutputSize < 0 || (max - min) <= maxOutputSize)
			{
				ConsoleLogger.WriteTimedLine("Outputting to {0}", Path.GetFileName(filepath));

				Output(safe, filepath, min, max);
			}
			else
			{
				string fpStart = filepath.Substring(0, filepath.Length - Path.GetExtension(filepath).Length);
				string fpEnd = Path.GetExtension(filepath);

				for (int i = 1; min < max; i++)
				{
					string segmentedFilepath = string.Format("{0}_{1:0000}{2}", fpStart, i, fpEnd);
					ConsoleLogger.WriteTimedLine("Outputting to {0}", Path.GetFileName(segmentedFilepath));

					Output(safe, segmentedFilepath, min, Math.Min(min + maxOutputSize, max));
					min += maxOutputSize;
				}
			}
		}

		public abstract void Output(RepresentationSafe safe, string filepath, long min, long max);

		public abstract string Convert(RepresentationSafe safe, long min, long max);
	}
}
