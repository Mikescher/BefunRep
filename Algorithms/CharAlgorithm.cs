﻿using BefunRep.FileHandling;

namespace BefunRep.Algorithms
{
	/// <summary>
	/// Represents numbers as a single character in stringmode
	/// Not possible for all numbers
	/// </summary>
	public class CharAlgorithm : RepAlgorithm
	{
		public CharAlgorithm(RepresentationSafe s)
			: base(s)
		{
			//
		}

		public override string get(int value)
		{
			if (value <= -(int)' ' && value >= -(int)'~' && value != -(int)'"')
			{
				return "0\"" + (char)(-value) + "\"-";
			}
			else if (value >= (int)' ' && value <= (int)'~' && value != (int)'"')
			{
				return "\"" + (char)(value) + "\"";
			}
			else
			{
				return null;
			}
		}
	}
}
