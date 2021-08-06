using System.Security.Permissions;
using System.Windows.Forms;
using log4net;

namespace ITA.Common.UI
{
    public class SPIForm : Form
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(SPIForm).Name);

        protected readonly SingleProgramInstance spi;


        public SPIForm()
        {
        }

        public SPIForm(SingleProgramInstance spi) :
            this()
        {
            logger.DebugFormat("SPIForm::SPIForm ({0}) >>>", spi.Message);
            this.spi = spi;
            logger.Debug("SPIForm::SPIForm () <<<");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (spi != null)
            {
                if (m.Msg == spi.Message)
                {
                    logger.DebugFormat("SPIForm::WndProc (): Got message from 2nd instance: {0}", spi.Message);
                    ActionOn2ndInstanceLaunch();
                }
            }
            base.WndProc(ref m);
        }

        protected virtual void ActionOn2ndInstanceLaunch()
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
        }
    }
}