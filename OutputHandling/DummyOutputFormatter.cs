
using BefunRep.FileHandling;
namespace BefunRep.OutputHandling
{
	public class DummyOutputFormatter : OutputFormatter
	{
		public DummyOutputFormatter()
			: base()
		{
			//
		}
		public override void Output(RepresentationSafe safe, string filepath, long maxOutputSize, long omin, long omax)
		{
			// Do nothing
		}

		public override void Output(RepresentationSafe safe, string filepath, long min, long max)
		{
			// Do nothing
		}
	}
}
