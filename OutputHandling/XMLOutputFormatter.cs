using BefunRep.FileHandling;
using System;
using System.IO;
using System.Security;
using System.Text;

namespace BefunRep.OutputHandling
{
	public class XMLOutputFormatter : OutputFormatter
	{
		public XMLOutputFormatter()
			: base()
		{
			//
		}

		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			using (StreamWriter writer = new StreamWriter(filepath))
			{
				writer.WriteLine("<data>");

				for (long v = min; v < max; v++)
				{
					string rep = safe.GetRep(v);
					byte? algo = safe.GetAlgorithm(v);

					if (rep == null || algo == null)
						continue;

					writer.WriteLine(String.Format("  <value v=\"{0}\" aID=\"{1}\" algorithm=\"{2}\">{3}</value>",
						v,
						algo,
						RepCalculator.algorithmNames[algo.Value],
						SecurityElement.Escape(rep)));
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
				string rep = safe.GetRep(v);
				byte? algo = safe.GetAlgorithm(v);

				if (rep == null || algo == null)
					continue;

				writer.AppendLine(String.Format("  <value v=\"{0}\" aID=\"{1}\" algorithm=\"{2}\">{3}</value>",
					v,
					algo,
					RepCalculator.algorithmNames[algo.Value],
					SecurityElement.Escape(rep)));
			}

			writer.AppendLine("</data>");

			return writer.ToString();
		}
	}
}
