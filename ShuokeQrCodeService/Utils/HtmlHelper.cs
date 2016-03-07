using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ShuokeQrCodeService.Utils
{
    public class HtmlHelper
    {
        /// <summary>
        /// 获取html结构中所有图片节点url集合
        /// </summary>
        /// <param name="content">正文html</param>
        /// <returns>图片节点url集合</returns>
        public static IList<String> GetImageNodesOfContent(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            var imgNodes = document.DocumentNode.SelectNodes("//img");
            if (imgNodes == null || imgNodes.Count == 0)
            {
                return null;
            }
            IList<string> imgUrls = new List<string>();
            foreach (var imgNode in imgNodes)
            {
                if (imgNode.Attributes["src"] != null)
                {
                    imgUrls.Add(imgNode.Attributes["src"].Value);
                }
            }
            return imgUrls;
        }
    }
}
