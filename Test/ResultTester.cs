﻿namespace BefunRep.Test
{
	public abstract class ResultTester
	{
		public abstract bool Test(string code, long result, out string error);
	}
}
