using BefunRep.FileHandling;
using System;
using System.IO;
using System.Text;

namespace BefunRep.OutputHandling
{
	public class CSVOutputFormatter : OutputFormatter
	{
		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			using (StreamWriter writer = new StreamWriter(filepath))
			{
				for (long v = min; v < max; v++)
				{
					var rep = safe.GetCombined(v);

					if (rep == null)
						continue;

					writer.WriteLine("{0, -10} {1}", rep.Algorithm, rep.Representation);
				}
			}
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			StringBuilder writer = new StringBuilder();

			for (long v = min; v < max; v++)
			{
				var rep = safe.GetCombined(v);

				if (rep == null)
					continue;

				writer.AppendLine(String.Format("{0, -10} {1}", rep.Algorithm, rep.Representation));
			}

			return writer.ToString();
		}
	}
}
