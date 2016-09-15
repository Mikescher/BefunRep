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
				.Where(p => safe.GetRep(p) != null)
				.Where(p => safe.GetAlgorithm(p) != null)
				.Select(p => new
				{
					value = p,
					representation = safe.GetRep(p),
					algorithmID = safe.GetAlgorithm(p),
					algorithm = RepCalculator.algorithmNames[safe.GetAlgorithm(p).Value]
				});

			File.WriteAllText(filepath, JsonConvert.SerializeObject(data, Formatting.Indented));
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			var data = CustomExtensions
				.LongRange(min, max)
				.Where(p => safe.GetRep(p) != null)
				.Where(p => safe.GetAlgorithm(p) != null)
				.Select(p => new
				{
					value = p,
					representation = safe.GetRep(p),
					algorithmID = safe.GetAlgorithm(p),
					algorithm = RepCalculator.algorithmNames[safe.GetAlgorithm(p).Value]
				});


			return JsonConvert.SerializeObject(data, Formatting.Indented);
		}
	}
}
