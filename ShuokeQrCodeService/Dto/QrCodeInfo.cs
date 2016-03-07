using System.Collections.Generic;

namespace ShuokeQrCodeService.Dto
{
    public class QrCodeInfo
    {
        public int ArticleId { get; set; }   
        public string ImgUrl { get; set; }
        public string DecodeInfo { get; set; }
    }
}
