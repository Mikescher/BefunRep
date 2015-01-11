using System;
using System.Collections.Generic;

namespace BefunRep.Test
{
	public class CPTester
	{
		private static Random Rand = new Random();

		public int w;

		public char[] raster;
		public int PC = 0;
		public bool Stringmode = false;

		public Stack<long> Stack;

		public bool finished = false;

		public CPTester(string s)
		{
			w = s.Length;

			raster = s.ToCharArray();

			Stack = new Stack<long>();
		}

		public void run()
		{
			while (!finished)
			{
				runSingle();
			}
		}

		private void runSingle()
		{
			executCmd(raster[PC]);

			move();
		}

		public void Push(long i)
		{
			Stack.Push(i);
		}

		public long Pop()
		{
			if (Stack.Count == 0)
				throw new BFRunException("Popped an empty stack");

			return Stack.Pop();
		}

		public long Peek()
		{
			if (Stack.Count == 0)
				throw new BFRunException("Popped an empty stack");

			return Stack.Peek();
		}

		public bool Pop_b()
		{
			return Pop() != 0;
		}

		public void Push(bool b)
		{
			Push(b ? 1 : 0);
		}

		private void executCmd(long cmd)
		{
			if (Stringmode)
			{
				if (cmd == '"')
				{
					Stringmode = false;
					return;
				}
				else
				{
					if (cmd < ' ' || cmd > '~')
						throw new BFRunException("Inknown SM charcater: '" + (char)cmd + "' (" + cmd + ")");

					Push(cmd);
					return;
				}
			}

			long t1;
			long t2;

			switch (cmd)
			{
				case ' ':
					// NOP
					break;
				case '+':
					Push(Pop() + Pop());
					break;
				case '-':
					t1 = Pop();
					Push(Pop() - t1);
					break;
				case '*':
					t1 = Pop();
					Push(Pop() * t1);
					break;
				case '/':
					t1 = Pop();
					Push(Pop() / t1);
					break;
				case '%':
					t1 = Pop();
					Push(Pop() % t1);
					break;
				case '!':
					Push(!Pop_b());
					break;
				case '`':
					t1 = Pop();
					Push(Pop() > t1);
					break;
				case '>':
				case '<':
				case '^':
				case 'v':
				case '?':
				case '_':
				case '|':
				case '.':
				case ',':
				case '&':
				case '~':
				case 'p':
				case 'g':
				case '@':
					throw new BFRunException("Illegal command: " + cmd);
				case '"':
					Stringmode = true;
					break;
				case ':':
					Push(Peek());
					break;
				case '\\':
					t1 = Pop();
					t2 = Pop();
					Push(t1);
					Push(t2);
					break;
				case '$':
					Pop();
					break;
				case '#':
					move();
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					Push(cmd - '0');
					break;
				default:
					throw new BFRunException("Unknown Command:" + cmd + "(" + (char)cmd + ")");
			}
		}

		private void move()
		{
			PC++;

			if (PC >= w)
				finished = true;
		}
	}
}
