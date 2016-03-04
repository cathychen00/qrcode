using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Reader;
using ThoughtWorks.QRCode.Codec.Util;
using ThoughtWorks.QRCode.ExceptionHandler;
using ThoughtWorks.QRCode.Geom;

namespace ThoughtWorks.QRCode.reader.pattern
{
	
	public class FinderPattern
	{
        public const int UL = 0;
        public const int UR = 1;
        public const int DL = 2;

        #region ����

        internal int version;
		virtual public int Version
		{
			get
			{
				return version;
			}
			
		}
		virtual public int SqrtNumModules
		{
			get
			{
				return 17 + 4 * version;
			}
			
		}


        internal int[] moduleSize;
        public virtual int GetModuleSize()
        {
            return moduleSize[UL];
        }

        internal int[] width;

	    public virtual int[] GetWidth()
	    {
	        return width;
	    }
        #endregion

        #region ���캯��
        internal static IDebugCanvas canvas;
        static FinderPattern()
        {
            canvas = QRCodeDecoder.Canvas;
        }

        internal FinderPattern(Point[] center, int version, int[] sincos, int[] width, int[] moduleSize)
        {
            this.Center = center;
            this.version = version;
            this.sincos = sincos;
            this.width = width;
            this.moduleSize = moduleSize;
        }
        #endregion

        public static FinderPattern FindFinderPattern(bool[][] image)
		{
            //�ҵ�3�����ĵ�����
			Line[] lineAcross = FindLineAcross(image);
			Line[] lineCross = FindLineCross(lineAcross);
			Point[] center = null;
			try
			{
				center = GetCenter(lineCross);
			}
			catch (FinderPatternNotFoundException e)
			{
				throw e;
			}

            //��ά����ת�Ƕ�
			int[] sincos = GetAngle(center);
			center = SortCenterPoints(center, sincos);

            //����FinderPattern���
			int[] width = GetWidth(image, center, sincos);

			//��ʱ�汾��
			int version = CalcRoughVersion(center, width);

            // moduleSize for version recognition
            int[] moduleSize = { (width[UL] << QRCodeImageReader.DECIMAL_POINT) / 7, (width[UR] << QRCodeImageReader.DECIMAL_POINT) / 7, (width[DL] << QRCodeImageReader.DECIMAL_POINT) / 7 };

            //version6���ϣ�����汾��
			if (version > 6)
			{
				try
				{
					version = CalcExactVersion(center, sincos, moduleSize, image);
				}
				catch (VersionInformationException e)
				{
					//use rough version data
					// throw e;
				}
			}
			return new FinderPattern(center, version, sincos, width, moduleSize);
		}

        #region FindLineAcross

        internal static Line[] FindLineAcross(bool[][] image)
		{
			const int READ_HORIZONTAL = 0;
			const int READ_VERTICAL = 1;
			
			int imageWidth = image.Length;
			int imageHeight = image[0].Length;
			
			//int currentX = 0, currentY = 0;
			Point current = new Point();
			System.Collections.ArrayList lineAcross = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			
			//buffer contains recent length of modules which has same brightness
			int[] lengthBuffer = new int[5];
			int bufferPointer = 0;
			
			int direction = READ_HORIZONTAL; //start to read horizontally
			bool lastElement = QRCodeImageReader.POINT_LIGHT;
			
			while (true)
			{
				//check points in image
				bool currentElement = image[current.X][current.Y];
				if (currentElement == lastElement)
				{
					//target point has same brightness with last point
					lengthBuffer[bufferPointer]++;
				}
				else
				{
					//target point has different brightness with last point
					if (currentElement == QRCodeImageReader.POINT_LIGHT)
					{
						if (CheckPattern(lengthBuffer, bufferPointer))
						{
							//detected pattern
							int x1, y1, x2, y2;
							if (direction == READ_HORIZONTAL)
							{
								//obtain X coordinates of both side of the detected horizontal pattern
								x1 = current.X;
								for (int j = 0; j < 5; j++)
								{
									x1 -= lengthBuffer[j];
								}
								x2 = current.X - 1; //right side is last X coordinate
								y1 = y2 = current.Y;
							}
							else
							{
								x1 = x2 = current.X;
								//obtain Y coordinates of both side of the detected vertical pattern
								// upper side is sum of length of buffer
								y1 = current.Y;
								for (int j = 0; j < 5; j++)
								{
									y1 -= lengthBuffer[j];
								}
								y2 = current.Y - 1; // bottom side is last Y coordinate
							}
							lineAcross.Add(new Line(x1, y1, x2, y2));
						}
					}
					bufferPointer = (bufferPointer + 1) % 5;
					lengthBuffer[bufferPointer] = 1;
					lastElement = !lastElement;
				}
				
				// determine if read next, change read direction or terminate this loop
				if (direction == READ_HORIZONTAL)
				{
					if (current.X < imageWidth - 1)
					{
						current.Translate(1, 0);
					}
					else if (current.Y < imageHeight - 1)
					{
						current.set_Renamed(0, current.Y + 1);
						lengthBuffer = new int[5];
					}
					else
					{
						current.set_Renamed(0, 0); //reset target point
						lengthBuffer = new int[5];
						direction = READ_VERTICAL; //start to read vertically
					}
				}
				else
				{
					//reading vertically
					if (current.Y < imageHeight - 1)
						current.Translate(0, 1);
					else if (current.X < imageWidth - 1)
					{
						current.set_Renamed(current.X + 1, 0);
						lengthBuffer = new int[5];
					}
					else
					{
						break;
					}
				}
			}
			
			Line[] foundLines = new Line[lineAcross.Count];
			
			for (int i = 0; i < foundLines.Length; i++)
				foundLines[i] = (Line) lineAcross[i];
			
			canvas.drawLines(foundLines, ThoughtWorks.QRCode.Codec.Util.Color_Fields.LIGHTGREEN);
			return foundLines;
		}

		/// <summary>
		/// ������1��1��3��1��1
		/// </summary>
		internal static bool CheckPattern(int[] buffer, int pointer)
		{
			int[] modelRatio = {1, 1, 3, 1, 1};
			
			int baselength = 0;
			for (int i = 0; i < 5; i++)
			{
				baselength += buffer[i];
			}
			// pseudo fixed point calculation. I think it needs smarter code
			baselength <<= QRCodeImageReader.DECIMAL_POINT;
			baselength /= 7;
			int i2;
			for (i2 = 0; i2 < 5; i2++)
			{
				int leastlength = baselength * modelRatio[i2] - baselength / 2;
				int mostlength = baselength * modelRatio[i2] + baselength / 2;
				
				//TODO rough finder pattern detection
				
				int targetlength = buffer[(pointer + i2 + 1) % 5] << QRCodeImageReader.DECIMAL_POINT;
				if (targetlength < leastlength || targetlength > mostlength)
				{
					return false;
				}
			}
			return true;
		}
        #endregion

        #region FindLineCross
        /// <summary>
        /// ��ȡ����FinderPattern������
		/// </summary>
		internal static Line[] FindLineCross(Line[] lineAcross)
		{
			System.Collections.ArrayList crossLines = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			System.Collections.ArrayList lineNeighbor = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			System.Collections.ArrayList lineCandidate = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			Line compareLine;
			for (int i = 0; i < lineAcross.Length; i++)
				lineCandidate.Add(lineAcross[i]);
			
			for (int i = 0; i < lineCandidate.Count - 1; i++)
			{
				lineNeighbor.Clear();
				lineNeighbor.Add(lineCandidate[i]);
				for (int j = i + 1; j < lineCandidate.Count; j++)
				{
					if (Line.IsNeighbor((Line) lineNeighbor[lineNeighbor.Count - 1], (Line) lineCandidate[j]))
					{
						lineNeighbor.Add(lineCandidate[j]);
						compareLine = (Line) lineNeighbor[lineNeighbor.Count - 1];
						if (lineNeighbor.Count * 5 > compareLine.Length && j == lineCandidate.Count - 1)
						{
							crossLines.Add(lineNeighbor[lineNeighbor.Count / 2]);
							for (int k = 0; k < lineNeighbor.Count; k++)
								lineCandidate.Remove(lineNeighbor[k]);
						}
					}
					//terminate comparison if there are no possibility for found neighbour lines
					else if (CantNeighbor((Line) lineNeighbor[lineNeighbor.Count - 1], (Line) lineCandidate[j]) || (j == lineCandidate.Count - 1))
					{
						compareLine = (Line) lineNeighbor[lineNeighbor.Count - 1];
						/*
						* determine lines across Finder Patterns when number of neighbour lines are 
						* bigger than 1/6 length of theirselves
						*/
						if (lineNeighbor.Count * 6 > compareLine.Length)
						{
							crossLines.Add(lineNeighbor[lineNeighbor.Count / 2]);
							for (int k = 0; k < lineNeighbor.Count; k++)
							{
								lineCandidate.Remove(lineNeighbor[k]);
							}
						}
						break;
					}
				}
			}
			
			Line[] foundLines = new Line[crossLines.Count];
			for (int i = 0; i < foundLines.Length; i++)
			{
				foundLines[i] = (Line) crossLines[i];
			}
			return foundLines;
		}
		
		internal static bool CantNeighbor(Line line1, Line line2)
		{
			if (Line.IsCross(line1, line2))
				return true;
			
			if (line1.Horizontal)
			{
				if (System.Math.Abs(line1.getP1().Y - line2.getP1().Y) > 1)
					return true;
				else
					return false;
			}
			else
			{
				if (System.Math.Abs(line1.getP1().X - line2.getP1().X) > 1)
					return true;
				else
					return false;
			}
		}
        #endregion

        #region GetCenter

        internal Point[] Center;
        /// <summary>
        /// ��ȡ����FinderPattern��������
        /// </summary>
        /// <returns></returns>
        public virtual Point[] GetCenter()
        {
            return Center;
        }

        /// <summary>
        /// ��ȡĳ��λ�õ�Finder Patttern��������
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual Point GetCenter(int position)
        {
            if (position >= UL && position <= DL)
                return Center[position];
            else
                return null;
        }

        /// <summary>
        /// ��ȡ3��FinderPattern���ĵ�����
        /// </summary>
        /// <param name="crossLines"></param>
        /// <returns></returns>
        internal static Point[] GetCenter(Line[] crossLines)
        {
            System.Collections.ArrayList centers = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
            for (int i = 0; i < crossLines.Length - 1; i++)
            {
                Line compareLine = crossLines[i];
                for (int j = i + 1; j < crossLines.Length; j++)
                {
                    Line comparedLine = crossLines[j];
                    if (Line.IsCross(compareLine, comparedLine))
                    {
                        int x = 0;
                        int y = 0;
                        if (compareLine.Horizontal)
                        {
                            x = compareLine.Center.X;
                            y = comparedLine.Center.Y;
                        }
                        else
                        {
                            x = comparedLine.Center.X;
                            y = compareLine.Center.Y;
                        }
                        centers.Add(new Point(x, y));
                    }
                }
            }

            Point[] foundPoints = new Point[centers.Count];

            for (int i = 0; i < foundPoints.Length; i++)
            {
                foundPoints[i] = (Point)centers[i];
                //Console.out.println(foundPoints[i]);
            }
            //Console.out.println(foundPoints.length);

            if (foundPoints.Length == 3)
            {
                canvas.drawPolygon(foundPoints, ThoughtWorks.QRCode.Codec.Util.Color_Fields.RED);
                return foundPoints;
            }
            else
                throw new FinderPatternNotFoundException("Invalid number of Finder Pattern detected");
        }
        #endregion

        #region GetAngle
        internal int[] sincos;
        public virtual int[] GetAngle()
        {
            return sincos;
        }

        /// <summary>
		/// ��ȡ��ά����ת�Ƕ�
		/// </summary>
		/// <param name="centers">Finder Pattern3�����ĵ�����</param>
		internal static int[] GetAngle(Point[] centers)
		{
			
			Line[] additionalLine = new Line[3];
			
			for (int i = 0; i < additionalLine.Length; i++)
			{
				additionalLine[i] = new Line(centers[i], centers[(i + 1) % additionalLine.Length]);
			}

			//��ȡ�������㹹�������ε���ߣ�������UL���ĵ㣩
			Line remoteLine = Line.GetLongest(additionalLine);//���
			Point originPoint = new Point();//UL���ĵ�
			for (int i = 0; i < centers.Length; i++)
			{
				if (!remoteLine.getP1().equals(centers[i]) && !remoteLine.getP2().equals(centers[i]))
				{
					originPoint = centers[i];
					break;
				}
			}
			canvas.println("originPoint is: " + originPoint);
			Point remotePoint = new Point();
			
			//with origin that the center of Left-Up Finder Pattern, determine other two patterns center.
			//then calculate symbols angle
			if (originPoint.Y <= remoteLine.getP1().Y & originPoint.Y <= remoteLine.getP2().Y)
				if (remoteLine.getP1().X < remoteLine.getP2().X)
					remotePoint = remoteLine.getP2();
				else
					remotePoint = remoteLine.getP1();
			else if (originPoint.X >= remoteLine.getP1().X & originPoint.X >= remoteLine.getP2().X)
				if (remoteLine.getP1().Y < remoteLine.getP2().Y)
					remotePoint = remoteLine.getP2();
				else
					remotePoint = remoteLine.getP1();
			else if (originPoint.Y >= remoteLine.getP1().Y & originPoint.Y >= remoteLine.getP2().Y)
				if (remoteLine.getP1().X < remoteLine.getP2().X)
					remotePoint = remoteLine.getP1();
				else
					remotePoint = remoteLine.getP2();
			//1st or 4th quadrant
			else if (remoteLine.getP1().Y < remoteLine.getP2().Y)
				remotePoint = remoteLine.getP1();
			else
				remotePoint = remoteLine.getP2();
			
			int r = new Line(originPoint, remotePoint).Length;
			//canvas.println(Integer.toString(((remotePoint.getX() - originPoint.getX()) << QRCodeImageReader.DECIMAL_POINT)));
			int[] angle = new int[2];
			angle[0] = ((remotePoint.Y - originPoint.Y) << QRCodeImageReader.DECIMAL_POINT) / r; //Sin
			angle[1] = ((remotePoint.X - originPoint.X) << (QRCodeImageReader.DECIMAL_POINT)) / r; //Cos
			
			return angle;
		}
        #endregion       

        #region Sort
        /// <summary>
        /// ���������ĵ�����ΪLU: points[0];RU: points[1];LD: points[2].
        /// </summary>
        /// <param name="centers">�������ĵ�����</param>
        /// <param name="angle">�Ƕ�sin cos</param>
		internal static Point[] SortCenterPoints(Point[] centers, int[] angle)
		{
			
			Point[] sortedCenters = new Point[3];
			
            //��ȡ����
			int quadant = GetURQuadant(angle);

			switch (quadant)
			{
				
				case 1: 
					sortedCenters[1] = GetPointAtSide(centers, Point.RIGHT, Point.BOTTOM);
					sortedCenters[2] = GetPointAtSide(centers, Point.BOTTOM, Point.LEFT);
					break;
				
				case 2: 
					sortedCenters[1] = GetPointAtSide(centers, Point.BOTTOM, Point.LEFT);
					sortedCenters[2] = GetPointAtSide(centers, Point.TOP, Point.LEFT);
					break;
				
				case 3: 
					sortedCenters[1] = GetPointAtSide(centers, Point.LEFT, Point.TOP);
					sortedCenters[2] = GetPointAtSide(centers, Point.RIGHT, Point.TOP);
					break;
				
				case 4: 
					sortedCenters[1] = GetPointAtSide(centers, Point.TOP, Point.RIGHT);
					sortedCenters[2] = GetPointAtSide(centers, Point.BOTTOM, Point.RIGHT);
					break;
				}
			
			//last of centers is Left-Up patterns one
			for (int i = 0; i < centers.Length; i++)
			{
				if (!centers[i].equals(sortedCenters[1]) && !centers[i].equals(sortedCenters[2]))
				{
					sortedCenters[0] = centers[i];
				}
			}
			
			return sortedCenters;
		}
		
        /// <summary>
        /// ����sin��cosֵ��ȡ����
        /// </summary>
        /// <param name="angle">�Ƕ�angle[sin][cos]</param>
        /// <returns>����</returns>
		internal static int GetURQuadant(int[] angle)
		{
			int sin = angle[0];
			int cos = angle[1];
			if (sin >= 0 && cos > 0)
				return 1;
            if (sin > 0 && cos <= 0)
                return 2;
            if (sin <= 0 && cos < 0)
                return 3;
            if (sin < 0 && cos >= 0)
                return 4;

            return 0;
		}

        internal static Point GetPointAtSide(Point[] points, int side1, int side2)
        {
            Point sidePoint = new Point();
            int x = ((side1 == Point.RIGHT || side2 == Point.RIGHT) ? 0 : System.Int32.MaxValue);
            int y = ((side1 == Point.BOTTOM || side2 == Point.BOTTOM) ? 0 : System.Int32.MaxValue);
            sidePoint = new Point(x, y);

            for (int i = 0; i < points.Length; i++)
            {
                switch (side1)
                {

                    case Point.RIGHT:
                        if (sidePoint.X < points[i].X)
                        {
                            sidePoint = points[i];
                        }
                        else if (sidePoint.X == points[i].X)
                        {
                            if (side2 == Point.BOTTOM)
                            {
                                if (sidePoint.Y < points[i].Y)
                                {
                                    sidePoint = points[i];
                                }
                            }
                            else
                            {
                                if (sidePoint.Y > points[i].Y)
                                {
                                    sidePoint = points[i];
                                }
                            }
                        }
                        break;

                    case Point.BOTTOM:
                        if (sidePoint.Y < points[i].Y)
                        {
                            sidePoint = points[i];
                        }
                        else if (sidePoint.Y == points[i].Y)
                        {
                            if (side2 == Point.RIGHT)
                            {
                                if (sidePoint.X < points[i].X)
                                {
                                    sidePoint = points[i];
                                }
                            }
                            else
                            {
                                if (sidePoint.X > points[i].X)
                                {
                                    sidePoint = points[i];
                                }
                            }
                        }
                        break;

                    case Point.LEFT:
                        if (sidePoint.X > points[i].X)
                        {
                            sidePoint = points[i];
                        }
                        else if (sidePoint.X == points[i].X)
                        {
                            if (side2 == Point.BOTTOM)
                            {
                                if (sidePoint.Y < points[i].Y)
                                {
                                    sidePoint = points[i];
                                }
                            }
                            else
                            {
                                if (sidePoint.Y > points[i].Y)
                                {
                                    sidePoint = points[i];
                                }
                            }
                        }
                        break;

                    case Point.TOP:
                        if (sidePoint.Y > points[i].Y)
                        {
                            sidePoint = points[i];
                        }
                        else if (sidePoint.Y == points[i].Y)
                        {
                            if (side2 == Point.RIGHT)
                            {
                                if (sidePoint.X < points[i].X)
                                {
                                    sidePoint = points[i];
                                }
                            }
                            else
                            {
                                if (sidePoint.X > points[i].X)
                                {
                                    sidePoint = points[i];
                                }
                            }
                        }
                        break;
                }
            }
            return sidePoint;
        }
        #endregion

        #region GetWidth
        /// <summary>
        /// ��ȡ����FinderPattern�Ŀ��
        /// </summary>
        /// <param name="image">ͼƬ����</param>
        /// <param name="centers">����FinderPattern�����ĵ�����</param>
        /// <param name="sincos">��ά����ת�Ƕ�</param>
        /// <returns>Wul,Wur,Wdl</returns>
		internal static int[] GetWidth(bool[][] image, Point[] centers, int[] sincos)
		{
			
			int[] width = new int[3];
			
			for (int i = 0; i < 3; i++)
			{
			    Point centerPoint = centers[i];
				
				int lx, rx;
                int y = centerPoint.Y;

                bool flag = false;
                for (lx = centerPoint.X; lx > 0; lx--)
				{
					if (image[lx][y] == QRCodeImageReader.POINT_DARK && image[lx - 1][y] == QRCodeImageReader.POINT_LIGHT)
					{
						if (flag == false)
							flag = true;
						else
							break;
					}
				}

				flag = false;
                for (rx = centerPoint.X; rx < image.Length; rx++)
				{
					if (image[rx][y] == QRCodeImageReader.POINT_DARK && image[rx + 1][y] == QRCodeImageReader.POINT_LIGHT)
					{
						if (flag == false)
							flag = true;
						else
							break;
					}
				}
				width[i] = (rx - lx + 1);
			}
			return width;
		}
        #endregion

        #region Version
        /// <summary>
        /// ���Լ����ά��汾��
        /// </summary>
        /// <param name="center">����FinderPattern���������</param>
        /// <param name="width"></param>
        /// <returns>�汾��</returns>
		internal static int CalcRoughVersion(Point[] center, int[] width)
		{
			int dp = QRCodeImageReader.DECIMAL_POINT;
			int lengthAdditionalLine = (new Line(center[UL], center[UR]).Length) << dp;//D
			int avarageWidth = ((width[UL] + width[UR]) << dp) / 14;//X = (WUL + WUR) / 14
            int roughVersion = ((lengthAdditionalLine / avarageWidth) - 10) / 4;//V = [(D/X) - 10] / 4
			if (((lengthAdditionalLine / avarageWidth) - 10) % 4 >= 2)
			{
				roughVersion++;
			}
			
			return roughVersion;
		}
		
        /// <summary>
        /// ��ȡVersionInformation(V>=7)
        /// </summary>
        /// <param name="centers">FinderPattern��������</param>
        /// <param name="angle">��ά����ת�Ƕ�</param>
        /// <param name="moduleSize"></param>
        /// <param name="image">FinderPattern���</param>
        /// <returns>��ά��汾��</returns>
		internal static int CalcExactVersion(Point[] centers, int[] angle, int[] moduleSize, bool[][] image)
		{
			bool[] versionInformation = new bool[18];
			Point[] points = new Point[18];
			Point target;

            //�ȳ��Դ�UR����ȡVersionInformation
			Axis axis = new Axis(angle, moduleSize[UR]); //UR
			axis.Origin = centers[UR];
			
			for (int y = 0; y < 6; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					target = axis.translate(x - 7, y - 3);
					versionInformation[x + y * 3] = image[target.X][target.Y];
					points[x + y * 3] = target;
				}
			}
			canvas.drawPoints(points, ThoughtWorks.QRCode.Codec.Util.Color_Fields.RED);
			
			int exactVersion = 0;
			try
			{
				exactVersion = CheckVersionInfo(versionInformation);
			}
			catch (InvalidVersionInfoException e)
			{
                //ʧ�ܺ��ٳ��Դ�DL�Ϸ���ȡVersionInformation
				canvas.println("Version info error. now retry with other place one.");
				axis.Origin = centers[DL];
				axis.ModulePitch = moduleSize[DL]; //DL
				
				for (int x = 0; x < 6; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						target = axis.translate(x - 3, y - 7);
						versionInformation[y + x * 3] = image[target.X][target.Y];
						points[x + y * 3] = target;
					}
				}
				canvas.drawPoints(points, ThoughtWorks.QRCode.Codec.Util.Color_Fields.RED);
				
				try
				{
					exactVersion = CheckVersionInfo(versionInformation);
				}
				catch (VersionInformationException e2)
				{
					throw e2;
				}
			}
			return exactVersion;
		}

        internal static readonly int[] VersionInfoBit = { 0x07C94, 0x085BC, 0x09A99, 0x0A4D3, 0x0BBF6, 0x0C762, 0x0D847, 0x0E60D, 0x0F928, 0x10B78, 0x1145D, 0x12A17, 0x13532, 0x149A6, 0x15683, 0x168C9, 0x177EC, 0x18EC4, 0x191E1, 0x1AFAB, 0x1B08E, 0x1CC1A, 0x1D33F, 0x1ED75, 0x1F250, 0x209D5, 0x216F0, 0x228BA, 0x2379F, 0x24B0B, 0x2542E, 0x26A64, 0x27541, 0x28C69 }; 
        /// <summary>
        /// �汾��Ϣ����,�ο���¼D
        /// </summary>
        /// <param name="target">�汾��Ϣ</param>
        /// <returns></returns>
		internal static int CheckVersionInfo(bool[] target)
		{
			int errorCount = 0, versionBase;
			for (versionBase = 0; versionBase < VersionInfoBit.Length; versionBase++)
			{
				errorCount = 0;
				for (int j = 0; j < 18; j++)
				{
					if (target[j] ^ (VersionInfoBit[versionBase] >> j) % 2 == 1)
						errorCount++;
				}
				if (errorCount <= 3)
					break;
			}
			if (errorCount <= 3)
				return 7 + versionBase;
			else
				throw new InvalidVersionInfoException("Too many errors in version information");
        }
        #endregion
    }
}