using System;
using System.Collections.Generic;
using log4net;
using QRCodeLib;
using RuntimeLib.Utils;
using ShuokeQrCodeService.Dto;
using ShuokeQrCodeService.Utils;

namespace ShuokeQrCodeService
{
    public class FilterImage
    {
        public ILog logger= Log4netManager.GetLogger("toClient");
        public IList<QrCodeInfo> FilterContentQRcode(BlogArticleDto articleDto)
        {
            if (articleDto == null || articleDto.ArticleId == 0 || string.IsNullOrEmpty(articleDto.Content))
            {
                return null;
            }
            logger.Debug("[FilterContentQRcode]Content:" + articleDto.Content);
            IList<string> imgUrlAll = HtmlHelper.GetImageNodesOfContent(articleDto.Content);
            if (imgUrlAll == null || imgUrlAll.Count == 0)
            {
                logger.Debug("no imgs");
                return null;
            }

            IList<QrCodeInfo> result = GetQrCodeImages(imgUrlAll);
            if (result != null && result.Count > 0)
            {
                foreach (var qrCodeInfo in result)
                {
                    qrCodeInfo.ArticleId = articleDto.ArticleId;
                }
            }
            return result;
        }

        private IList<QrCodeInfo> GetQrCodeImages(IList<string> imgUrlAll)
        {
            if (imgUrlAll == null || imgUrlAll.Count == 0)
            {
                return null;
            }

            IList<QrCodeInfo> result=new List<QrCodeInfo>();
            var qrDecoder=new QRCodeDecoder();
            foreach (string imgUrl in imgUrlAll)
            {
                string decodeInfo = string.Empty;
                try
                {
                    decodeInfo = qrDecoder.Decode(imgUrl);
                    if (!string.IsNullOrEmpty(decodeInfo))
                    {
                        QrCodeInfo qrCodeInfo = new QrCodeInfo()
                        {
                            ImgUrl = imgUrl,
                            DecodeInfo = decodeInfo
                        };
                        result.Add(qrCodeInfo);
                        logger.Debug("[imgurl]"+imgUrl);
                        logger.Debug("[decodeinfo]"+decodeInfo);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            return result;
        }
    }
}
