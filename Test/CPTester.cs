using System.Collections.Generic;

namespace BefunRep.Test
{
	public class CPTester
	{
		public readonly Stack<long> Stack;

		private readonly int width;
		private readonly char[] raster;
		private int pc = 0;
		private bool stringmode = false;
		
		private bool finished = false;

		public CPTester(string s)
		{
			width = s.Length;

			raster = s.ToCharArray();

			Stack = new Stack<long>();
		}

		public void Run()
		{
			while (!finished)
			{
				RunSingle();
			}
		}

		private void RunSingle()
		{
			ExecutCmd(raster[pc]);

			Move();
		}

		private void Push(long i)
		{
			Stack.Push(i);
		}

		private long Pop()
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

		private bool Pop_b()
		{
			return Pop() != 0;
		}

		private void Push(bool b)
		{
			Push(b ? 1 : 0);
		}

		private void ExecutCmd(long cmd)
		{
			if (stringmode)
			{
				if (cmd == '"')
				{
					stringmode = false;
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
					stringmode = true;
					break;
				case ':':
					Push(Peek());
					break;
				case '\\':
					t1 = Pop();
					var t2 = Pop();
					Push(t1);
					Push(t2);
					break;
				case '$':
					Pop();
					break;
				case '#':
					Move();
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

		private void Move()
		{
			pc++;

			if (pc >= width)
				finished = true;
		}
	}
}
