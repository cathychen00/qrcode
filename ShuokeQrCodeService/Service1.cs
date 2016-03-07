using System.ServiceProcess;
using ShuokeQrCodeService.Jobs;

namespace ShuokeQrCodeService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ReadFromMq.Start();
        }

        protected override void OnStop()
        {
        }

        public void Start()
        {
            OnStart(null);
        }
    }
}
