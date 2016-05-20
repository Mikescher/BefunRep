using BefunRep.Helper;
using BefunRep.Log;
using System;
using System.IO;
using System.IO.Compression;

namespace BefunRep.FileHandling
{
	/// <summary>
	/// see BinarySafe - just gzipped
	/// </summary>
	public class GZipBinarySafe : RepresentationSafe
	{
		private readonly long INITIAL_VALUE_START;
		private readonly long INITIAL_VALUE_END;

		private readonly string filepath;

		private string tempFilePath;
		private BinarySafe tempBinSafe;
		private FileStream fstream;

		private long valueStart;
		private long valueEnd;

		public GZipBinarySafe(string path, long min, long max)
		{
			filepath = path;

			INITIAL_VALUE_START = min;
			INITIAL_VALUE_END = max;
		}

		public override string get(long key)
		{
			return tempBinSafe.get(key);
		}

		public override byte? getAlgorithm(long key)
		{
			return tempBinSafe.getAlgorithm(key);
		}

		public override void put(long key, string representation, byte algorithm)
		{
			tempBinSafe.put(key, representation, algorithm);
		}

		public override void start()
		{
			tempFilePath = Path.Combine(Path.GetTempPath(), "repsafe-" + Guid.NewGuid().ToString("B") + ".bin");

			if (File.Exists(filepath))
			{
				ConsoleLogger.WriteLine("Opening gzipped safe");
				GZipHelper.DecompressFile(filepath, tempFilePath);

				fstream = new FileStream(filepath, FileMode.Open);

				tempBinSafe = new BinarySafe(tempFilePath, INITIAL_VALUE_START, INITIAL_VALUE_END);
				tempBinSafe.start();
			}
			else
			{
				fstream = new FileStream(filepath, FileMode.CreateNew);

				tempBinSafe = new BinarySafe(tempFilePath, INITIAL_VALUE_START, INITIAL_VALUE_END);
				tempBinSafe.start();
			}

			valueStart = tempBinSafe.getLowestValue();
			valueEnd = tempBinSafe.getHighestValue();
		}

		public override void stop()
		{
			valueStart = tempBinSafe.getLowestValue();
			valueEnd = tempBinSafe.getHighestValue();

			fstream.Close();
			tempBinSafe.stop();

			ConsoleLogger.WriteLine("Closing gzipped safe");
			if (tempBinSafe.Changed) GZipHelper.CompressFile(tempFilePath, filepath);

			File.Delete(tempFilePath);
		}

		public override void LightLoad()
		{
			if (File.Exists(filepath))
			{
				using (FileStream streamIn = File.OpenRead(filepath))
				{
					using (GZipStream streamInGzip = new GZipStream(streamIn, CompressionMode.Decompress))
					{
						int tmp;
						BinarySafe.LightLoad(streamInGzip, out tmp, out valueStart, out valueEnd);
					}
				}
			}
		}

		public override long getLowestValue()
		{
			return valueStart;
		}

		public override long getHighestValue()
		{
			return valueEnd;
		}

	}
}
