using System.Drawing;

namespace QRCodeLib.data
{
    public class QRCodeBitmapImage : IQRCodeImage
    {
        readonly Bitmap _image;

        /// <summary>
        /// Constructor
        /// </summary>
        public QRCodeBitmapImage(Bitmap image)
        {
            this._image = image;
        }

        virtual public int Width
        {
            get
            {
                return _image.Width;
            }

        }
        virtual public int Height
        {
            get
            {
                return _image.Height;
            }

        }
     

        public virtual int getPixel(int x, int y)
        {
            return _image.GetPixel(x, y).ToArgb();
        }
    }
}
