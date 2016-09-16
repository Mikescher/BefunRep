using BefunRep.FileHandling;
using System;
using System.IO;
using System.Text;

namespace BefunRep.OutputHandling
{
	public class TSVOutputFormatter : OutputFormatter
	{
		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			using (StreamWriter writer = new StreamWriter(filepath))
			{
				writer.NewLine = "\n";

				for (long v = min; v < max; v++)
				{
					var rep = safe.GetCombined(v);

					if (rep == null)
						continue;

					writer.WriteLine("{0:X}\t{1}", v, rep.Representation);
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

				writer.AppendLine(String.Format("{0:X}\t{1}", v, rep.Representation));
			}

			return writer.ToString();
		}
	}
}
