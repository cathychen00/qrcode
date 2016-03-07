using System;
using System.Drawing;
using System.Net;

namespace QRCodeLib.reader
{
    public class UrlReader
    {
        /// <summary>
        /// 获取网络图片
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static Image GetImage(string url, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                var reader = response.GetResponseStream();
                if (null == reader)
                {
                    errorMessage = "获取网络图片失败";
                    return null;
                }

                return Image.FromStream(reader);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return null;
        }
    }
}
