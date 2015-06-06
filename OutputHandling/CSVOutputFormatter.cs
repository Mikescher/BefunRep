using BefunRep.FileHandling;
using System;
using System.IO;
using System.Text;

namespace BefunRep.OutputHandling
{
	public class CSVOutputFormatter : OutputFormatter
	{
		public CSVOutputFormatter()
			: base()
		{
			//
		}

		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			using (StreamWriter writer = new StreamWriter(filepath))
			{
				for (long v = min; v < max; v++)
				{
					string rep = safe.get(v);
					byte? algo = safe.getAlgorithm(v);

					if (rep == null || algo == null)
						continue;

					writer.WriteLine(String.Format("{0, -11}   {1,-30}   {2}",
						v,
						RepCalculator.algorithmNames[algo.Value] + "-Algorithm",
						rep));
				}
			}
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			StringBuilder writer = new StringBuilder();

			for (long v = min; v < max; v++)
			{
				string rep = safe.get(v);
				byte? algo = safe.getAlgorithm(v);

				if (rep == null || algo == null)
					continue;

				writer.AppendLine(String.Format("{0, -11}   {1,-30}   {2}",
					v,
					RepCalculator.algorithmNames[algo.Value] + "-Algorithm",
					rep));
			}

			return writer.ToString();
		}
	}
}
