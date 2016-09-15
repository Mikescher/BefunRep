
using BefunRep.FileHandling;
namespace BefunRep.OutputHandling
{
	public class DummyOutputFormatter : OutputFormatter
	{
		public override void Output(RepresentationSafe safe, string filepath, long maxOutputSize, long omin, long omax)
		{
			// Do nothing
		}

		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			// Do nothing
		}

		public override string Convert(RepresentationSafe safe, long min, long max)
		{
			return string.Empty; // Do nothing
		}
	}
}
