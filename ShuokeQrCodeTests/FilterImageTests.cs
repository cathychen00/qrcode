using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuokeQrCodeService;
using ShuokeQrCodeService.Dto;

namespace ShuokeQrCodeTests
{
    [TestClass]
    public class FilterImageTests
    {
        [TestMethod]
        public void FilterContentQRcode()
        {
            BlogArticleDto articleDto = new BlogArticleDto()
            {
                ArticleId = 100,
                Content = "<p style=\"text - indent: 2em; \">测试</p><p style=\"text - align: center; \"><img src=\"http://www3.autoimg.cn/newsdfs/g22/M08/B5/BF/620x0_1_autohomecar__wKgFVlbdLP2AIRFwAADuiA8ia7w244.jpg\" style=\"width:430px;height:430px\" /></p><p style=\"text-indent: 2em;\"><br /></p>"
            };
            var result = new FilterImage().FilterContentQRcode(articleDto);

            Assert.AreEqual(1,result.Count);
            Assert.AreEqual(100,result[0].ArticleId);
            Assert.AreEqual("http://www3.autoimg.cn/newsdfs/g22/M08/B5/BF/620x0_1_autohomecar__wKgFVlbdLP2AIRFwAADuiA8ia7w244.jpg",result[0].ImgUrl);
            Assert.AreEqual("http://weixin.qq.com/r/cnVSSmvEvjbBrS8p9yBg", result[0].DecodeInfo);
        }
    }
}
