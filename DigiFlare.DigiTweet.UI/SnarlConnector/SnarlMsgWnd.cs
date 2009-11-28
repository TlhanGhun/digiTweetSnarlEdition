using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Snarl;

namespace NativeWindowApplication
{

    // Summary description for SnarlMsgWnd.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    
    
    public class SnarlMsgWnd : NativeWindow
    {
        CreateParams cp = new CreateParams();

        public string pathToIcon = "";

        int SNARL_GLOBAL_MESSAGE;

        public SnarlMsgWnd()
        {
            // Create the actual window
            this.CreateHandle(cp);
            this.SNARL_GLOBAL_MESSAGE = Snarl.SnarlConnector.GetGlobalMsg();
        }

        protected override void WndProc(ref Message m)
        {

        if (m.Msg == this.SNARL_GLOBAL_MESSAGE)
        {
            if ((int)m.WParam == Snarl.SnarlConnector.SNARL_LAUNCHED)
            {
                // Snarl has been (re)started during Shaim already running
                // so let's register (again)
                Snarl.SnarlConnector.GetSnarlWindow(true);

            SnarlConnector.RegisterConfig(this.Handle, "digiTweet", Snarl.WindowsMessage.WM_USER + 55,pathToIcon);

            SnarlConnector.RegisterAlert("digiTweet", "New tweet");
            SnarlConnector.RegisterAlert("digiTweet", "New direct message");
            SnarlConnector.RegisterAlert("digiTweet", "Search results");

            }
        }
            base.WndProc(ref m);

        }

    }

}
