using JetBrains.Annotations;
using System;
using System.IO;
using System.Text;

namespace BefunRep.Log
{
	public static class ConsoleLogger
	{
		private static readonly StringBuilder _builder = new StringBuilder();

		private static string _savepath = null;

		public static void SetPath(string p)
		{
			if (p == null)
			{
				_savepath = null;
				return;
			}

			p = p.Trim();

			if (p.EndsWith("/") || p.EndsWith("\\"))
			{
				_savepath = Path.Combine(p, string.Format("log_{0:yyyy-MM-dd_HH-mm-ss}.txt", DateTime.Now));

				int ctr = 0;
				while (File.Exists(_savepath))
				{
					ctr++;

					_savepath = Path.Combine(p, string.Format("log_{0:yyyy-MM-dd_HH-mm-ss} ({1}).txt", DateTime.Now, ctr));
				}
			}
			else
			{
				string fldr = Path.GetDirectoryName(p);
				if (fldr != null) Directory.CreateDirectory(fldr);

				_savepath = p;
			}

			WriteLine();
			WriteTimedLine("Logging to " + _savepath);
			WriteLine();
		}

		public static void WriteTimedLine(string text)
		{
			WriteLine(string.Format("[{0:HH:mm:ss}] ", DateTime.Now) + text);
		}

		[StringFormatMethod("format")]
		public static void WriteTimedLine(string format, object arg0)
		{
			WriteLine(string.Format("[{0:HH:mm:ss}] ", DateTime.Now) + string.Format(format, arg0));
		}

		[StringFormatMethod("format")]
		public static void WriteTimedLine(string format, object arg0, object arg1)
		{
			WriteLine(string.Format("[{0:HH:mm:ss}] ", DateTime.Now) + string.Format(format, arg0, arg1));
		}

		[StringFormatMethod("format")]
		public static void WriteTimedLine(string format, object arg0, object arg1, object arg2)
		{
			WriteLine(string.Format("[{0:HH:mm:ss}] ", DateTime.Now) + string.Format(format, arg0, arg1, arg2));
		}

		[StringFormatMethod("format")]
		public static void WriteTimedLine(string format, params object[] args)
		{
			WriteLine(string.Format("[{0:HH:mm:ss}] ", DateTime.Now) + string.Format(format, args));
		}

		[StringFormatMethod("format")]
		public static void WriteLineFormatted(string format, object arg0)
		{
			WriteLine(string.Format(format, arg0));
		}

		[StringFormatMethod("format")]
		public static void WriteLineFormatted(string format, object arg0, object arg1)
		{
			WriteLine(string.Format(format, arg0, arg1));
		}

		[StringFormatMethod("format")]
		public static void WriteLineFormatted(string format, object arg0, object arg1, object arg2)
		{
			WriteLine(string.Format(format, arg0, arg1, arg2));
		}

		[StringFormatMethod("format")]
		public static void WriteLineFormatted(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}

		[StringFormatMethod("format")]
		public static void WriteFormatted(string format, object arg0)
		{
			Write(string.Format(format, arg0));
		}

		[StringFormatMethod("format")]
		public static void WriteFormatted(string format, object arg0, object arg1)
		{
			Write(string.Format(format, arg0, arg1));
		}

		[StringFormatMethod("format")]
		public static void WriteFormatted(string format, object arg0, object arg1, object arg2)
		{
			Write(string.Format(format, arg0, arg1, arg2));
		}

		[StringFormatMethod("format")]
		public static void WriteFormatted(string format, params object[] args)
		{
			Write(string.Format(format, args));
		}

		public static void WriteLine()
		{
			WriteLine(string.Empty);
		}

		public static void WriteLine(string s)
		{
			Console.Out.WriteLine(s);

			if (_savepath != null)
				_builder.AppendLine(s);
		}

		public static void Write(string s)
		{
			Console.Out.Write(s);

			if (_savepath != null)
				_builder.Append(s);
		}

		public static void Save()
		{
			if (_savepath != null)
			{
				var dir = Path.GetDirectoryName(_savepath);
				if (dir != null) Directory.CreateDirectory(dir);

				WriteLine("Saving log ...");
				File.WriteAllText(_savepath, _builder.ToString());

			}
		}
	}
}
