using BefunRep.FileHandling;
using System;
using System.IO;
using System.Security;
using System.Text;

namespace BefunRep.OutputHandling
{
	public class XMLOutputFormatter : OutputFormatter
	{
		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			using (StreamWriter writer = new StreamWriter(filepath))
			{
				writer.WriteLine("<data>");

				for (long v = min; v < max; v++)
				{
					var rep = safe.GetCombined(v);

					if (rep == null)
						continue;

					writer.WriteLine("  <value v=\"{0}\" aID=\"{1}\" algorithm=\"{2}\">{3}</value>",
						v,
						rep.Algorithm,
						RepCalculator.AlgorithmNames[rep.Algorithm],
						SecurityElement.Escape(rep.Representation));
				}

				writer.WriteLine("</data>");
			}
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			StringBuilder writer = new StringBuilder();

			writer.AppendLine("<data>");

			for (long v = min; v < max; v++)
			{
				var rep = safe.GetCombined(v);

				if (rep == null)
					continue;

				writer.AppendLine(String.Format("  <value v=\"{0}\" aID=\"{1}\" algorithm=\"{2}\">{3}</value>",
						v,
						rep.Algorithm,
						RepCalculator.AlgorithmNames[rep.Algorithm],
						SecurityElement.Escape(rep.Representation)));
			}

			writer.AppendLine("</data>");

			return writer.ToString();
		}
	}
}
