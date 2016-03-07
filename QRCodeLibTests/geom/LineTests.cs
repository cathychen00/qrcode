using Microsoft.VisualStudio.TestTools.UnitTesting;
using QRCodeLib.geom;

namespace QRCodeLibTests.geom
{
    [TestClass()]
    public class LineTests
    {
        #region IsNeighbor
        [TestMethod()]
        public void IsNeighbor_1_1_5_5_2_2_6_6_True()
        {
            Line line1=new Line(1,1,5,5);
            Line line2=new Line(2,2,6,6);

            Assert.IsTrue(Line.IsNeighbor(line1,line2));
        }

        [TestMethod()]
        public void IsNeighbor_1_1_5_5_3_2_6_6_False()
        {
            Line line1 = new Line(1, 1, 5, 5);
            Line line2 = new Line(3, 2, 6, 6);

            Assert.IsFalse(Line.IsNeighbor(line1, line2));
        }

        [TestMethod()]
        public void IsNeighbor_1_1_5_5_2_3_6_6_False()
        {
            Line line1 = new Line(1, 1, 5, 5);
            Line line2 = new Line(2, 3, 6, 6);

            Assert.IsFalse(Line.IsNeighbor(line1, line2));
        }

        [TestMethod()]
        public void IsNeighbor_1_1_5_5_2_2_3_6_False()
        {
            Line line1 = new Line(1, 1, 5, 5);
            Line line2 = new Line(2, 2, 3, 6);

            Assert.IsFalse(Line.IsNeighbor(line1, line2));
        }

        [TestMethod()]
        public void IsNeighbor_1_1_5_5_2_2_4_7_False()
        {
            Line line1 = new Line(1, 1, 5, 5);
            Line line2 = new Line(2, 2, 4, 7);

            Assert.IsFalse(Line.IsNeighbor(line1, line2));
        }

        #endregion

        #region IsCross
        [TestMethod()]
        public void IsCross_CrossLine_True()
        {
            Line line1=new Line(10,10,20,10);
            Line line2=new Line(15,5,15,20);
            
            Assert.IsTrue(Line.IsCross(line1,line2));
        }

        [TestMethod()]
        public void IsCross_VerticalOnLeft_False()
        {
            Line line1 = new Line(10, 10, 20, 10);
            Line line2 = new Line(5, 5, 5, 20);

            Assert.IsFalse(Line.IsCross(line1, line2));
        }

        [TestMethod()]
        public void IsCross_VerticalOnRight_False()
        {
            Line line1 = new Line(10, 10, 20, 10);
            Line line2 = new Line(25, 5, 25, 20);

            Assert.IsFalse(Line.IsCross(line1, line2));
        }
        #endregion

        [TestMethod()]
        public void GetLongest_LineItems_ReturnLongestOne()
        {
            Line line1 = new Line(1, 1, 4, 4);
            Line line2 = new Line(2, 4, 5, 3);
            Line line3 = new Line(10, 10, 20, 20);
            Line line4 = new Line(5, 5, 15, 15);
            Line line5 = new Line(3, 3, 21, 21);

            Line[] lines = {line1, line2, line3, line4, line5};

            Line expected = line5;
            Line actual = Line.GetLongest(lines);

            Assert.AreSame(expected,actual);
        }
    }
}
