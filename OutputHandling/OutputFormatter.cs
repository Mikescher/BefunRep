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

		public void Output(RepresentationSafe safe, string filepath, long maxOutputSize)
		{
			long min = safe.getLowestValue();
			long max = safe.getHighestValue();

			if (maxOutputSize < 0 || (max - min) <= maxOutputSize)
			{
				ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Outputting to {1}", DateTime.Now, Path.GetFileName(filepath));

				Output(safe, filepath, min, max);
			}
			else
			{
				string fp_start = Path.GetDirectoryName(filepath) + Path.GetFileNameWithoutExtension(filepath);
				string fp_end = Path.GetExtension(filepath);

				for (int i = 1; min < max; i++)
				{
					string segmented_fp = fp_start + "_" + i + fp_end;
					ConsoleLogger.WriteLineFormatted("[{0:HH:mm:ss}] Outputting to {1}", DateTime.Now, Path.GetFileName(segmented_fp));

					Output(safe, segmented_fp, min, Math.Min(min + maxOutputSize, max));
					min += maxOutputSize;
				}
			}
		}

		public abstract void Output(RepresentationSafe safe, string filepath, long min, long max);
	}
}
