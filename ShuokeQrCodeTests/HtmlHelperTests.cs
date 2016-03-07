using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuokeQrCodeService.Utils;

namespace ShuokeQrCodeTests
{
    [TestClass]
    public class HtmlHelperTests
    {
        [TestMethod]
        public void GetImageNodesOfContent_NoImage()
        {
            string content = "<p>测试</p>";
            var actual = HtmlHelper.GetImageNodesOfContent(content);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetImageNodesOfContent_HasImage()
        {
            string content = "<p style=\"text - indent: 2em; \">测试</p><p style=\"text - align: center; \"><img src=\"http://www3.autoimg.cn/newsdfs/g22/M08/B5/BF/620x0_1_autohomecar__wKgFVlbdLP2AIRFwAADuiA8ia7w244.jpg\" style=\"width:430px;height:430px\" /></p><p style=\"text-indent: 2em;\"><br /></p>";
            var imageUrls = HtmlHelper.GetImageNodesOfContent(content);
            Assert.AreEqual(1, imageUrls.Count);
            Assert.AreEqual("http://www3.autoimg.cn/newsdfs/g22/M08/B5/BF/620x0_1_autohomecar__wKgFVlbdLP2AIRFwAADuiA8ia7w244.jpg", imageUrls[0]);
        }
    }
}
