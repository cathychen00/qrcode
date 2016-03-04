using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.QRCode.Geom;

namespace QRCodeLibTests.geom
{
    [TestClass()]
    public class PointTests
    {
        [TestMethod()]
        public void DistanceOf_ReturnDistanceOfTwoPoints()
        {
            Point p1=new Point(1,1);
            Point pother=new Point(4,4);

            int expected = 4;
            int actual = p1.DistanceOf(pother);

            Assert.AreEqual(expected,actual);
        }
    }
}
