using System.IO;
using System.IO.Compression;

namespace BefunRep.Helper
{
	static class GZipHelper
	{
		public static void DecompressFile(string inputFile, string outputFile)
		{
			byte[] buffer = new byte[1024 * 1024];

			using (FileStream streamIn = File.OpenRead(inputFile))
			{
				using (FileStream streamOut = File.Create(outputFile))
				{
					using (GZipStream streamInGzip = new GZipStream(streamIn, CompressionMode.Decompress))
					{
						int numRead = streamInGzip.Read(buffer, 0, buffer.Length);

						while (numRead != 0)
						{
							streamOut.Write(buffer, 0, numRead);
							streamOut.Flush();
							numRead = streamInGzip.Read(buffer, 0, buffer.Length);
						}
					}
				}
			}
		}

		public static void CompressFile(string inputFile, string outputFile)
		{
			var clevel = CompressionLevel.Optimal;

			byte[] buffer = new byte[1024 * 1024];

			using (FileStream streamIn = File.OpenRead(inputFile))
			{
				if (streamIn.Length > 33554432) // 32MB
				{
					clevel = CompressionLevel.Fastest;
				}

				using (FileStream streamOut = File.Create(outputFile))
				{
					using (GZipStream streamOutGzip = new GZipStream(streamOut, clevel))
					{
						int numRead = streamIn.Read(buffer, 0, buffer.Length);

						while (numRead != 0)
						{
							streamOutGzip.Write(buffer, 0, numRead);
							streamOutGzip.Flush();
							numRead = streamIn.Read(buffer, 0, buffer.Length);
						}
					}
				}
			}
		}
	}
}
