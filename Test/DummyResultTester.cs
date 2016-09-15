
namespace BefunRep.Test
{
	public class DummyResultTester : ResultTester
	{
		public override bool Test(string code, long result, out string error)
		{
			error = "DUMMY";
			return true;
		}
	}
}
