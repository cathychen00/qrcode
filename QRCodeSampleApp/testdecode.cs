using System;
using System.Windows.Forms;
using QRCodeLib;

namespace QRCodeSample
{
    public partial class testdecode : Form
    {
        public testdecode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";

            string url = txtUrl.Text;
            string result = string.Empty;
            try
            {
                result = new QRCodeDecoder().Decode(url);
            }
            catch (Exception ex)
            {
                result ="不是二维码"+ ex.Message;
            }
            txtResult.Text = result;
        }
    }
}
