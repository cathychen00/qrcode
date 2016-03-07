using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShuokeQrCodeService.Dto;
using ShuokeQrCodeService.Utils;

namespace ShuokeQrCodeTests
{
    [TestClass]
    public class MqUtilsTests
    {
        [TestMethod]
        public void TestSend()
        {
            BlogArticleDto article=new BlogArticleDto()
            {
                ArticleId = 1000,
                Content = "测试文章内容"
            };
            MqUtils.SendMessage(article, "*", "qrcodetest");
        }

        [TestMethod]
        public void TestGet()
        {
            BlogArticleDto article =new BlogArticleDto();
            MqUtils.GetRabbitMq<BlogArticleDto>("*", "qrcodetest", (t) =>
            {
                article = t;
            });

            Assert.AreEqual(1000,article.ArticleId);
            Assert.AreEqual("测试文章内容",article.Content);
        }

        //[TestMethod]
        //public void TestSendArticle()
        //{
        //    BlogArticleDto article = new BlogArticleDto()
        //    {
        //        ArticleId = 200,
        //        Content = "<p style=\"text - indent: 2em; \">测试</p><p style=\"text - align: center; \"><img src=\"http://www3.autoimg.cn/newsdfs/g22/M08/B5/BF/620x0_1_autohomecar__wKgFVlbdLP2AIRFwAADuiA8ia7w244.jpg\" style=\"width:430px;height:430px\" /></p><p style=\"text-indent: 2em;\"><br /></p>"
        //    };
        //    MqUtils.SendMessage(article, "*", "qrcodetest");
        //}
    }
}
