using System;
using System.IO;
using System.Text;

namespace BefunRep.Log
{
	public class ConsoleLogger
	{
		private static StringBuilder builder = new StringBuilder();
		private static int writecount = 0;

		private static string savepath = null;

		public static void setPath(string p)
		{
			if (p == null)
			{
				savepath = null;
				return;
			}

			p = p.Trim();

			if (p.EndsWith("/") || p.EndsWith("\\"))
			{
				savepath = Path.Combine(p, string.Format("log_{0:yyyy-MM-dd_HH-mm-ss}.txt", DateTime.Now));

				int ctr = 0;
				while (File.Exists(savepath))
				{
					ctr++;

					savepath = Path.Combine(p, string.Format("log_{0:yyyy-MM-dd_HH-mm-ss} ({1}).txt", DateTime.Now, ctr));
				}
			}
			else
			{
				string fldr = Path.GetDirectoryName(p);
				Directory.CreateDirectory(fldr);

				savepath = p;
			}

			WriteLine();
			WriteLineFormatted("[{0:HH:mm:ss}] Logging to " + savepath, DateTime.Now);
			WriteLine();
		}

		public static void WriteLineFormatted(string format, object arg0)
		{
			WriteLine(string.Format(format, arg0));
		}

		public static void WriteLineFormatted(string format, object arg0, object arg1)
		{
			WriteLine(string.Format(format, arg0, arg1));
		}

		public static void WriteLineFormatted(string format, object arg0, object arg1, object arg2)
		{
			WriteLine(string.Format(format, arg0, arg1, arg2));
		}

		public static void WriteLineFormatted(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}

		public static void WriteFormatted(string s, object arg0)
		{
			Write(string.Format(s, arg0));
		}

		public static void WriteFormatted(string s, object arg0, object arg1)
		{
			Write(string.Format(s, arg0, arg1));
		}

		public static void WriteFormatted(string s, object arg0, object arg1, object arg2)
		{
			Write(string.Format(s, arg0, arg1, arg2));
		}

		public static void WriteFormatted(string s, params object[] args)
		{
			Write(string.Format(s, args));
		}

		public static void WriteLine()
		{
			WriteLine(string.Empty);
		}

		public static void WriteLine(string s)
		{
			Console.Out.WriteLine(s);
			builder.AppendLine(s);

			writecount++;
		}

		public static void Write(string s)
		{
			Console.Out.Write(s);
			builder.Append(s);

			writecount++;
		}

		public static void save()
		{
			if (savepath != null)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(savepath));

				WriteLine("Saving log ...");
				File.WriteAllText(savepath, builder.ToString());

			}
		}
	}
}
