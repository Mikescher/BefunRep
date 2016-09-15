using System;
namespace BefunRep.Test
{
	public class ExecuteResultTester : ResultTester
	{
		public override bool Test(string code, long result, out string error)
		{
			CPTester t = new CPTester(code);

			try
			{
				t.Run();
			}
			catch (Exception e)
			{
				error = "Exception thrown = " + e;
				return false;
			}

			if (t.Stack.Count != 1)
			{
				error = "StackCount = " + t.Stack.Count + ". Stack = [" + string.Join(", ", t.Stack) + "]";
				return false;
			}

			if (t.Peek() != result)
			{
				error = "Result = " + t.Stack.Peek();
				return false;
			}

			error = "No error";
			return true;
		}
	}
}
