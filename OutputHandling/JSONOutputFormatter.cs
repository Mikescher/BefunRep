using BefunRep.FileHandling;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace BefunRep.OutputHandling
{
	public class JSONOutputFormatter : OutputFormatter
	{
		public JSONOutputFormatter()
			: base()
		{
			//
		}

		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			var data = CustomExtensions
				.LongRange(min, max)
				.Where(p => safe.get(p) != null)
				.Where(p => safe.getAlgorithm(p) != null)
				.Select(p => new
				{
					value = p,
					representation = safe.get(p),
					algorithmID = safe.getAlgorithm(p),
					algorithm = RepCalculator.algorithmNames[safe.getAlgorithm(p).Value]
				});

			File.WriteAllText(filepath, JsonConvert.SerializeObject(data, Formatting.Indented));
		}
	}
}
