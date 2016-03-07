namespace QRCodeLib.reader.pattern
{
	/// <summary> 
    /// Alignment Pattern的位置
	/// </summary>
	/// <remarks>见文档附录E Position of Alignment Patterns</remarks>
	public class LogicalSeed
	{
		/// <summary> The positions</summary>
		private static readonly int[][] Seed;
		
		/// <summary> 获取某版本的Alignment Pattern坐标</summary>
		public static int[] GetSeed(int version)
		{
			return (Seed[version - 1]);
		}
		
		/// <summary> 
        /// 初始化所有版本对应的Alignment Pattern坐标
		/// </summary>
		static LogicalSeed()
		{
			{
				Seed = new int[40][];
				Seed[0] = new[]{6, 14};
				Seed[1] = new[]{6, 18};
				Seed[2] = new[]{6, 22};
				Seed[3] = new[]{6, 26};
				Seed[4] = new[]{6, 30};
				Seed[5] = new[]{6, 34};
				Seed[6] = new[]{6, 22, 38};
				Seed[7] = new[]{6, 24, 42};
				Seed[8] = new[]{6, 26, 46};
				Seed[9] = new[]{6, 28, 50};
				Seed[10] = new[]{6, 30, 54};
				Seed[11] = new[]{6, 32, 58};
				Seed[12] = new[]{6, 34, 62};
				Seed[13] = new[]{6, 26, 46, 66};
				Seed[14] = new[]{6, 26, 48, 70};
				Seed[15] = new[]{6, 26, 50, 74};
				Seed[16] = new[]{6, 30, 54, 78};
				Seed[17] = new[]{6, 30, 56, 82};
				Seed[18] = new[]{6, 30, 58, 86};
				Seed[19] = new[]{6, 34, 62, 90};
				Seed[20] = new[]{6, 28, 50, 72, 94};
				Seed[21] = new[]{6, 26, 50, 74, 98};
				Seed[22] = new[]{6, 30, 54, 78, 102};
				Seed[23] = new[]{6, 28, 54, 80, 106};
				Seed[24] = new[]{6, 32, 58, 84, 110};
				Seed[25] = new[]{6, 30, 58, 86, 114};
				Seed[26] = new[]{6, 34, 62, 90, 118};
				Seed[27] = new[]{6, 26, 50, 74, 98, 122};
				Seed[28] = new[]{6, 30, 54, 78, 102, 126};
				Seed[29] = new[]{6, 26, 52, 78, 104, 130};
				Seed[30] = new[]{6, 30, 56, 82, 108, 134};
				Seed[31] = new[]{6, 34, 60, 86, 112, 138};
				Seed[32] = new[]{6, 30, 58, 86, 114, 142};
				Seed[33] = new[]{6, 34, 62, 90, 118, 146};
				Seed[34] = new[]{6, 30, 54, 78, 102, 126, 150};
				Seed[35] = new[]{6, 24, 50, 76, 102, 128, 154};
				Seed[36] = new[]{6, 28, 54, 80, 106, 132, 158};
				Seed[37] = new[]{6, 32, 58, 84, 110, 136, 162};
				Seed[38] = new[]{6, 26, 54, 82, 110, 138, 166};
				Seed[39] = new[]{6, 30, 58, 86, 114, 142, 170};
			}
		}
	}
}