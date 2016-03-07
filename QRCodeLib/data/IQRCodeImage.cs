namespace QRCodeLib.data
{
	public interface IQRCodeImage
	{
        int Width
        {
            get;

        }
        int Height
        {
            get;

        }
        int getPixel(int x, int y);
	}
}