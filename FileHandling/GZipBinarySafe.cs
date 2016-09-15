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
		private readonly long initialValueStart;
		private readonly long initialValueEnd;

		private readonly string filepath;

		private string tempFilePath;
		private BinarySafe tempBinSafe;
		private FileStream fstream;

		private long valueStart;
		private long valueEnd;

		public GZipBinarySafe(string path, long min, long max)
		{
			filepath = path;

			initialValueStart = min;
			initialValueEnd = max;
		}

		public override string GetRep(long key)
		{
			return tempBinSafe.GetRep(key);
		}

		public override byte? GetAlgorithm(long key)
		{
			return tempBinSafe.GetAlgorithm(key);
		}
		
		public override BefungeRepresentation GetCombined(long key)
		{
			return tempBinSafe.GetCombined(key);
		}

		public override void Put(long key, string representation, byte algorithm)
		{
			tempBinSafe.Put(key, representation, algorithm);
		}

		public override void Start()
		{
			tempFilePath = Path.Combine(Path.GetTempPath(), "repsafe-" + Guid.NewGuid().ToString("B") + ".bin");

			if (File.Exists(filepath))
			{
				ConsoleLogger.WriteLine("Opening gzipped safe");
				GZipHelper.DecompressFile(filepath, tempFilePath);

				fstream = new FileStream(filepath, FileMode.Open);

				tempBinSafe = new BinarySafe(tempFilePath, initialValueStart, initialValueEnd);
				tempBinSafe.Start();
			}
			else
			{
				fstream = new FileStream(filepath, FileMode.CreateNew);

				tempBinSafe = new BinarySafe(tempFilePath, initialValueStart, initialValueEnd);
				tempBinSafe.Start();
			}

			valueStart = tempBinSafe.GetLowestValue();
			valueEnd = tempBinSafe.GetHighestValue();
		}

		public override void Stop()
		{
			valueStart = tempBinSafe.GetLowestValue();
			valueEnd = tempBinSafe.GetHighestValue();

			fstream.Close();
			tempBinSafe.Stop();

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

		public override long GetLowestValue()
		{
			return valueStart;
		}

		public override long GetHighestValue()
		{
			return valueEnd;
		}

	}
}
