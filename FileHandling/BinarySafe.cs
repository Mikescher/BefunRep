using BefunRep.Log;
using System;
using System.IO;
using System.Text;

namespace BefunRep.FileHandling
{
	/// <summary>
	/// Binary Format
	/// 
	/// - erste 32 Byte Header (HEADER_SIZE)
	///	  -> 4 Byte codeLength
	///	  -> 8 Byte startValue
	///	  -> 8 Byte endValue
	/// 
	/// - Dann immer wieder Blöcke der Größe $codelength 
	///   -> In diesen stehen die Representations in ASCII
	///   -> jeweils der eihe nach von startVaue bis endValue
	/// 
	/// </summary>
	public class BinarySafe : RepresentationSafe
	{
		private const int INITIAL_CODE_LEN = 32; // Wird von Base9 erst bei über 43.046.720 gesprengt    (enthält das eine byte für den algo)
		private const int HEADER_SIZE = 32;

		private readonly long initialValueStart;
		private readonly long initialValueEnd;

		private readonly string filepath;
		private FileStream fstream;

		private int codeLength;

		private long valueStart;
		private long valueEnd;

		public bool Changed { get; private set; }

		public BinarySafe(string path, long min, long max)
		{
			filepath = path;

			initialValueStart = min;
			initialValueEnd = max;
		}

		public override string GetRep(long key)
		{
			if (key < valueStart || key >= valueEnd)
				return null;

			fstream.Seek(HEADER_SIZE + codeLength * (key - valueStart), SeekOrigin.Begin);

			byte[] read = new byte[codeLength];
			fstream.Read(read, 0, codeLength);

			if (read[0] == 0)
			{
				return null;
			}

			StringBuilder b = new StringBuilder();
			for (int i = 0; i < codeLength; i++)
			{
				if (read[i] == 0)
					break;

				b.Append((char)read[i]);
			}

			var result = b.ToString();

			return result;
		}

		public override byte? GetAlgorithm(long key)
		{
			if (key < valueStart || key >= valueEnd)
				return null;

			fstream.Seek(HEADER_SIZE + codeLength * (key - valueStart), SeekOrigin.Begin);

			byte[] read = new byte[codeLength];
			fstream.Read(read, 0, codeLength);

			if (read[0] == 0)
			{
				return null;
			}

			return read[codeLength - 1];
		}

		public override BefungeRepresentation GetCombined(long key)
		{
			if (key < valueStart || key >= valueEnd)
				return null;

			fstream.Seek(HEADER_SIZE + codeLength * (key - valueStart), SeekOrigin.Begin);

			byte[] read = new byte[codeLength];
			fstream.Read(read, 0, codeLength);

			if (read[0] == 0)
			{
				return null;
			}

			StringBuilder b = new StringBuilder();
			for (int i = 0; i < codeLength; i++)
			{
				if (read[i] == 0)
					break;

				b.Append((char)read[i]);
			}

			return new BefungeRepresentation(read[codeLength - 1], b.ToString());
		}

		public override void Put(long key, string representation, byte algorithm)
		{
			if (key >= valueEnd)
				UpdateEndSize(key);

			if (key < valueStart)
				UpdateStartSize(key);

			if ((representation.Length + 1) > codeLength)
				UpdateCodeLength(representation.Length + 1);

			fstream.Seek(HEADER_SIZE + codeLength * (key - valueStart), SeekOrigin.Begin);

			byte[] write = new byte[codeLength];

			for (int i = 0; i < codeLength; i++)
			{
				if (i < representation.Length)
					write[i] = (byte)representation[i];
				else
					write[i] = 0;
			}
			write[codeLength - 1] = algorithm;

			fstream.Write(write, 0, codeLength);

			Changed = true;
		}

		public override void Start()
		{
			if (File.Exists(filepath))
			{
				fstream = new FileStream(filepath, FileMode.Open);

				fstream.Seek(0, SeekOrigin.Begin);
				byte[] arr = new byte[HEADER_SIZE];
				fstream.Read(arr, 0, HEADER_SIZE);
				codeLength = BitConverter.ToInt32(arr, 0);
				valueStart = BitConverter.ToInt64(arr, 4);
				valueEnd = BitConverter.ToInt64(arr, 12);

				if (initialValueEnd > valueEnd)
					UpdateEndSize(initialValueEnd, 0);
				if (initialValueStart < valueStart)
					UpdateStartSize(initialValueStart, 0);
			}
			else
			{
				fstream = new FileStream(filepath, FileMode.CreateNew);

				codeLength = INITIAL_CODE_LEN;
				valueStart = initialValueStart;
				valueEnd = initialValueEnd;

				fstream.SetLength(HEADER_SIZE + (valueEnd - valueStart) * codeLength);

				WriteHeader();
			}

			Changed = false;
		}

		public override void Stop()
		{
			fstream.Close();
		}

		private void UpdateEndSize(long key, int buffer = 32) // 32 values buffer
		{
			long newValueEnd = key + buffer;

			ConsoleLogger.WriteTimedLine("Update Safe Size Right (from {0} to {1})", valueEnd, newValueEnd);

			valueEnd = newValueEnd;
			fstream.SetLength(HEADER_SIZE + (valueEnd - valueStart) * codeLength);

			WriteHeader();

			Changed = true;
		}

		private void UpdateStartSize(long key, int buffer = 1240) // 10 kB Buffer
		{
			long newValueStart = key - buffer;

			ConsoleLogger.WriteTimedLine("Update Safe Size Left (from {0} to {1})", valueStart, newValueStart);

			fstream.SetLength(HEADER_SIZE + (valueEnd - newValueStart) * codeLength);
			MoveRight(valueEnd - valueStart, valueStart - newValueStart);
			valueStart = newValueStart;

			WriteHeader();

			Changed = true;
		}

		private void UpdateCodeLength(int len)
		{
			int newCodeLength = Math.Max(codeLength + (len - codeLength) * 2, codeLength * 2);

			ConsoleLogger.WriteTimedLine("Update Safe Code Length (from {0} to {1})", codeLength, newCodeLength);

			fstream.SetLength(HEADER_SIZE + (valueEnd - valueStart) * newCodeLength);

			byte[] empty = new byte[codeLength];
			empty.Fill<byte>(0);

			byte[] arr = new byte[codeLength];

			for (long i = (valueEnd - valueStart) - 1; i >= valueStart; i--)
			{
				fstream.Seek(HEADER_SIZE + i * codeLength, SeekOrigin.Begin);
				fstream.Read(arr, 0, codeLength);

				fstream.Seek(HEADER_SIZE + i * codeLength, SeekOrigin.Begin);
				fstream.Write(empty, 0, codeLength);

				fstream.Seek(HEADER_SIZE + i * newCodeLength, SeekOrigin.Begin);
				fstream.Write(arr, 0, codeLength);
			}

			codeLength = newCodeLength;

			WriteHeader();

			Changed = true;
		}

		private void WriteHeader()
		{
			byte[] arr = new byte[HEADER_SIZE];
			arr.Fill<byte>(0);

			Array.Copy(BitConverter.GetBytes(codeLength), 0, arr, 0, 4);
			Array.Copy(BitConverter.GetBytes(valueStart), 0, arr, 4, 8);
			Array.Copy(BitConverter.GetBytes(valueEnd), 0, arr, 12, 8);

			fstream.Seek(0, SeekOrigin.Begin);
			fstream.Write(arr, 0, 4 + 8 + 8);

			Changed = true;
		}

		private void MoveRight(long count, long offset)
		{
			byte[] empty = new byte[codeLength];
			empty.Fill<byte>(0);

			byte[] arr = new byte[codeLength];

			for (long i = count - 1; i >= 0; i--)
			{
				fstream.Seek(HEADER_SIZE + i * codeLength, SeekOrigin.Begin);
				fstream.Read(arr, 0, codeLength);

				fstream.Seek(HEADER_SIZE + i * codeLength, SeekOrigin.Begin);
				fstream.Write(empty, 0, codeLength);

				fstream.Seek(HEADER_SIZE + (i + offset) * codeLength, SeekOrigin.Begin);
				fstream.Write(arr, 0, codeLength);
			}

			Changed = true;
		}

		public override void LightLoad()
		{
			if (File.Exists(filepath))
			{
				using (fstream = new FileStream(filepath, FileMode.Open))
				{
					LightLoad(fstream, out codeLength, out valueStart, out valueEnd);
				}
			}
		}

		public static void LightLoad(Stream stream, out int length, out long start, out long end)
		{
			if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

			byte[] arr = new byte[HEADER_SIZE];
			stream.Read(arr, 0, HEADER_SIZE);

			length = BitConverter.ToInt32(arr, 0);
			start = BitConverter.ToInt64(arr, 4);
			end = BitConverter.ToInt64(arr, 12);
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
