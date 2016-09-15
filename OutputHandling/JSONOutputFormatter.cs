using BefunRep.FileHandling;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace BefunRep.OutputHandling
{
	public class JSONOutputFormatter : OutputFormatter
	{
		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			var data = CustomExtensions
				.LongRange(min, max)
				.Select(p => new {Number=p, Rep = safe.GetCombined(p)})
				.Where(p => p != null)
				.Select(p => new
				{
					value = p.Number,
					representation = p.Rep.Representation,
					algorithmID = p.Rep.Algorithm,
					algorithm = RepCalculator.AlgorithmNames[p.Rep.Algorithm]
				});

			File.WriteAllText(filepath, JsonConvert.SerializeObject(data, Formatting.Indented));
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			var data = CustomExtensions
				.LongRange(min, max)
				.Select(p => new { Number = p, Rep = safe.GetCombined(p) })
				.Where(p => p != null)
				.Select(p => new
				{
					value = p.Number,
					representation = p.Rep.Representation,
					algorithmID = p.Rep.Algorithm,
					algorithm = RepCalculator.AlgorithmNames[p.Rep.Algorithm]
				});

			return JsonConvert.SerializeObject(data, Formatting.Indented);
		}
	}
}
