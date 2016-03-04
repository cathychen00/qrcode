using System;
using System.Text;

namespace ThoughtWorks.QRCode.Codec.Util
{
	public class QRCodeUtility
	{
        public static bool IsUniCode(String value)
        {
            byte[] ascii = AsciiStringToByteArray(value);
            byte[] unicode = UnicodeStringToByteArray(value);
            string value1 = FromASCIIByteArray(ascii);
            string value2 = FromUnicodeByteArray(unicode);
            if (value1 != value2)
                return true;
            return false;
        }

        public static bool IsUnicode(byte[] byteData)
        {
            string value1 = FromASCIIByteArray(byteData);
            string value2 = FromUnicodeByteArray(byteData);
            byte[] ascii = AsciiStringToByteArray(value1);
            byte[] unicode = UnicodeStringToByteArray(value2);
            if (ascii[0] != unicode[0])
                return true;
            return false;
        }

        public static String FromASCIIByteArray(byte[] characters)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            String constructedString = encoding.GetString(characters);
            return constructedString;
        }

        public static String FromUnicodeByteArray(byte[] characters)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            String constructedString = encoding.GetString(characters);
            return constructedString;
        }

        public static byte[] AsciiStringToByteArray(String str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static byte[] UnicodeStringToByteArray(String str)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetBytes(str);
        }
	}
}