using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Security.Principal;

namespace WinUpTool
{
    /// <summary>
    /// this class is core: check, set and manage Windows Update.
    /// </summary>
    public class SystemTweak
    {
        public int CheckStatusServiceUpdate()
        {
            ServiceController sc = new ServiceController("wuauserv");

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                case ServiceControllerStatus.StartPending:
                    return 1;
                case ServiceControllerStatus.Stopped:
                    return 0;
                case ServiceControllerStatus.Paused:
                    return 0;
                case ServiceControllerStatus.StopPending:
                    return 0;
                default:
                    return -1; // changing
            }
        }

        /// <summary>
        /// 1 is Blocked, 0 is Not Block
        /// </summary>
        /// <returns></returns>
        public int NowIsBlocking()
        {
            string temp = "";
            try
            {
                RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var key = root.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UpdatePolicy\Settings", false);
                var thisValue = key.GetValue(@"PausedQualityStatus");
                temp = thisValue.ToString();
            }
            catch
            {
                
            }

            if (temp == "0")
            {
                return 0;
            }
            else if (temp == "1")
                return 1;
            else
                return -1;
        }


        public bool IsRunAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public string GetCurrentSID()
        {
            string currentSID = "";

            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            NTAccount f = new NTAccount(userName);
            SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
            currentSID = s.ToString();

            return currentSID;
        }
    }
}
