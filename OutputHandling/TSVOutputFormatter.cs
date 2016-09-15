using BefunRep.FileHandling;
using System;
using System.IO;
using System.Text;

namespace BefunRep.OutputHandling
{
	public class TSVOutputFormatter : OutputFormatter
	{
		public TSVOutputFormatter()
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
					string rep = safe.GetRep(v);
					byte? algo = safe.GetAlgorithm(v);

					if (rep == null || algo == null)
						continue;

					writer.WriteLine("{0:X}\t{1}", v, rep);
				}
			}
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			StringBuilder writer = new StringBuilder();

			for (long v = min; v < max; v++)
			{
				string rep = safe.GetRep(v);
				byte? algo = safe.GetAlgorithm(v);

				if (rep == null || algo == null)
					continue;

				writer.AppendLine(String.Format("{0:X}\t{1}", v, rep));
			}

			return writer.ToString();
		}
	}
}
