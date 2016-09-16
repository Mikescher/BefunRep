using BefunRep.FileHandling;
using BefunRep.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BefunRep.OutputHandling
{
	public abstract class OutputFormatter
	{
		public virtual void Output(RepresentationSafe safe, string filepath, long maxOutputSize, long maxFileCount, long omin, long omax)
		{
			long min = Math.Max(safe.GetLowestValue(), omin);
			long max = Math.Min(safe.GetHighestValue(), omax);

			maxFileCount = Math.Min(maxFileCount, 16);

			if (maxOutputSize < 0 || (max - min) <= maxOutputSize)
			{
				OutputSimple(safe, filepath, min, max);
			}
			else
			{
				if (Math.Ceiling((max - min) * 1d / maxOutputSize) <= maxFileCount)
				{
					OutputFlat(safe, filepath, min, max, maxOutputSize);
				}
				else
				{
					OutputStructured(safe, filepath, min, max, maxOutputSize, maxFileCount);
				}
			}
		}

		private void OutputSimple(RepresentationSafe safe, string filepath, long min, long max)
		{
			ConsoleLogger.WriteTimedLine("Outputting to {0}", Path.GetFileName(filepath));

			Directory.CreateDirectory(Path.GetDirectoryName(filepath) ?? "");
			Output(safe, filepath, min, max);
		}

		private void OutputFlat(RepresentationSafe safe, string filepath, long min, long max, long maxOutputSize)
		{
			string fpStart = filepath.Substring(0, filepath.Length - Path.GetExtension(filepath).Length);
			string fpEnd = Path.GetExtension(filepath);

			int digitLen = Math.Max(4, max.ToString().Length);
			var fileformat = "{0}_{1:" + new string('0', digitLen) + "}{2}";

			for (int i = 1; min < max; i++)
			{
				string segmentedFilepath = string.Format(fileformat, fpStart, i, fpEnd);
				ConsoleLogger.WriteTimedLine("Outputting to {0}", Path.GetFileName(segmentedFilepath));

				Directory.CreateDirectory(Path.GetDirectoryName(segmentedFilepath) ?? "");
				Output(safe, segmentedFilepath, min, Math.Min(min + maxOutputSize, max));
				min += maxOutputSize;
			}
		}

		struct OutputFileStruct
		{
			public int Index;
			public long Min;
			public long Max;
			public List<OutputFileStruct> Children;
		}

		private void OutputStructured(RepresentationSafe safe, string filepath, long min, long max, long maxOutputSize, long maxFileCount)
		{
			List<OutputFileStruct> files = new List<OutputFileStruct>();

			for (int i = 1; min < max; i++)
			{
				files.Add(new OutputFileStruct
				{
					Index = i,
					Min = min,
					Max = Math.Min(min + maxOutputSize, max),
					Children = new List<OutputFileStruct>(),
				});

				min += maxOutputSize;
			}

			while (files.Count > maxFileCount)
			{
				List<OutputFileStruct> filesCombined = new List<OutputFileStruct>();
				for (int i = 0; i < files.Count;)
				{
					int iStart = i;
					int iCount = Math.Min(files.Count - i, 10);
					filesCombined.Add(new OutputFileStruct
					{
						Index = -1,
						Min = files[iStart].Min,
						Max = files[iStart + iCount - 1].Max,
						Children = files.Skip(iStart).Take(iCount).ToList()
					});

					i += iCount;
				}
				files = filesCombined;
			}

			string filename = Path.GetFileName(filepath);
			string fpStart = filepath.Substring(0, filename.Length - Path.GetExtension(filename).Length);
			string fpEnd = Path.GetExtension(filename);
			int digitLen = Math.Max(4, max.ToString().Length);
			var fileformat = fpStart + "_{0:" + new string('0', digitLen) + "}" + fpEnd;

			foreach (var f in files)
			{
				OutputStructured(safe, Path.GetDirectoryName(filepath), fileformat, f);
			}
		}

		private void OutputStructured(RepresentationSafe safe, string filepath, string filenameformat, OutputFileStruct ofs)
		{
			if (ofs.Index == -1)
			{
				foreach (var f in ofs.Children)
				{
					OutputStructured(safe, Path.Combine(filepath, ofs.Min.ToString()), filenameformat, f);
				}
			}
			else
			{
				var filename = string.Format(filenameformat, ofs.Min);

				ConsoleLogger.WriteTimedLine("Outputting to {0}", Path.GetFileName(filename));

				Directory.CreateDirectory(filepath);
				Output(safe, Path.Combine(filepath, filename), ofs.Min, ofs.Max);
			}
		}

		public abstract void Output(RepresentationSafe safe, string filepath, long min, long max);

		public abstract string Convert(RepresentationSafe safe, long min, long max);
	}
}
