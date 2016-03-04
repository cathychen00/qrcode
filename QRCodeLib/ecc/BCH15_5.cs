namespace ThoughtWorks.QRCode.Codec.Ecc
{
    /// <summary>
    /// BCH码，用于版本信息纠错
    /// </summary>
	public class BCH15_5
	{
        internal int[][] gf16;
        internal bool[] recieveData;
        internal int numCorrectedError;

		virtual public int NumCorrectedError
		{
			get
			{
				return numCorrectedError;
			}		
		}
		
		public BCH15_5(bool[] source)
		{
			gf16 = CreateGf16();
			recieveData = source;
		}
		
        /// <summary>
        /// 纠正错误
        /// </summary>
		public virtual bool[] Correct()
		{
			int[] s = calcSyndrome(recieveData);
			
			int[] errorPos = detectErrorBitPosition(s);
			bool[] output = correctErrorBit(recieveData, errorPos);
			return output;
		}
		
		internal virtual int[][] CreateGf16()
		{
			gf16 = new int[16][];
			for (int i = 0; i < 16; i++)
			{
				gf16[i] = new int[4];
			}
			int[] seed = {1, 1, 0, 0};
			for (int i = 0; i < 4; i++)
				gf16[i][i] = 1;
			for (int i = 0; i < 4; i++)
				gf16[4][i] = seed[i];
			for (int i = 5; i < 16; i++)
			{
				for (int j = 1; j < 4; j++)
				{
					gf16[i][j] = gf16[i - 1][j - 1];
				}
				if (gf16[i - 1][3] == 1)
				{
					for (int j = 0; j < 4; j++)
						gf16[i][j] = (gf16[i][j] + seed[j]) % 2;
				}
			}

			return gf16;
		}
		
		internal virtual int SearchElement(int[] x)
		{
			int k;
			for (k = 0; k < 15; k++)
			{
				if (x[0] == gf16[k][0] && x[1] == gf16[k][1] && x[2] == gf16[k][2] && x[3] == gf16[k][3])
					break;
			}
			return k;
		}
				
		internal virtual int AddGf(int arg1, int arg2)
		{
			int[] p = new int[4];
			for (int m = 0; m < 4; m++)
			{
				int w1 = (arg1 < 0 || arg1 >= 15)?0:gf16[arg1][m];
				int w2 = (arg2 < 0 || arg2 >= 15)?0:gf16[arg2][m];
				p[m] = (w1 + w2) % 2;
			}
			return SearchElement(p);
		}
	
		internal virtual int[] calcSyndrome(bool[] y)
		{
			int[] s = new int[5];
			int[] p = new int[4];
			int k;
			for (k = 0; k < 15; k++)
			{
				if (y[k] == true)
					for (int m = 0; m < 4; m++)
						p[m] = (p[m] + gf16[k][m]) % 2;
			}
			k = SearchElement(p);
			s[0] = (k >= 15)?- 1:k;
	
			p = new int[4];
			for (k = 0; k < 15; k++)
			{
				if (y[k])
					for (int m = 0; m < 4; m++)
						p[m] = (p[m] + gf16[(k * 3) % 15][m]) % 2;
			}
			
			k = SearchElement(p);
			
			s[2] = (k >= 15)?- 1:k;		
			p = new int[4];
			for (k = 0; k < 15; k++)
			{
				if (y[k])
					for (int m = 0; m < 4; m++)
						p[m] = (p[m] + gf16[(k * 5) % 15][m]) % 2;
			}
			k = SearchElement(p);
			s[4] = (k >= 15)?- 1:k;		
			return s;
		}
		
		
		internal virtual int[] calcErrorPositionVariable(int[] s)
		{
			int[] e = new int[4];
			// calc σ1
			e[0] = s[0];
			//Console.out.println("σ1 = " + String.valueOf(e[0]));
			
			// calc σ2
			int t = (s[0] + s[1]) % 15;
			int mother = AddGf(s[2], t);
			mother = (mother >= 15)?- 1:mother;
			
			t = (s[2] + s[1]) % 15;
			int child = AddGf(s[4], t);
			child = (child >= 15)?- 1:child;
			e[1] = (child < 0 && mother < 0)?- 1:(child - mother + 15) % 15;
		
			// calc σ3
			t = (s[1] + e[0]) % 15;
			int t1 = AddGf(s[2], t);
			t = (s[0] + e[1]) % 15;
			e[2] = AddGf(t1, t);
				
			return e;
		}
		
		internal virtual int[] detectErrorBitPosition(int[] s)
		{			
			int[] e = calcErrorPositionVariable(s);
			int[] errorPos = new int[4];
			if (e[0] == - 1)
			{
				//Console.out.println("No errors.");               
				return errorPos;
			}
			else if (e[1] == - 1)
			{
				errorPos[0] = 1;
				errorPos[1] = e[0];
				return errorPos;
			}
			int x3, x2, x1;
			int t, t1, t2, anError;
			//error detection
			for (int i = 0; i < 15; i++)
			{
				x3 = (i * 3) % 15;
				x2 = (i * 2) % 15;
				x1 = i;
				
				//p = new int[4];
				
				t = (e[0] + x2) % 15;
				t1 = AddGf(x3, t);
				
				t = (e[1] + x1) % 15;
				t2 = AddGf(t, e[2]);
				
				anError = AddGf(t1, t2);
				
				if (anError >= 15)
				{
					errorPos[0]++;
					errorPos[errorPos[0]] = i;
				}
			}
			
			return errorPos;
		}
		
		internal virtual bool[] correctErrorBit(bool[] y, int[] errorPos)
		{
			for (int i = 1; i <= errorPos[0]; i++)
				y[errorPos[i]] = !y[errorPos[i]];
			
			numCorrectedError = errorPos[0];
			return y;
		}
	}
}