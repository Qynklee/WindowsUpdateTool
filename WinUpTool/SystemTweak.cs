using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Security.Principal;
using System.Runtime.Remoting.Services;
using System.Runtime.InteropServices;

namespace WinUpTool
{
    /// <summary>
    /// this class is core: check, set and manage Windows Update.
    /// </summary>
    public class SystemTweak
    {

        public string CheckStatusServiceUpdate()
        {
            ServiceController sc = new ServiceController("wuauserv");

            switch (sc.StartType)
            {
                case ServiceStartMode.Automatic:
                    return "Automatic";
                case ServiceStartMode.Manual:
                    return "Manual";
                case ServiceStartMode.Disabled:
                    return "Disable";
                case ServiceStartMode.Boot:
                    return "Boot";
                case ServiceStartMode.System:
                    return "System";
                default:
                    return "Cant Check :(";
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


        public bool BlockUpdate()
        {
            try
            {
                RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var key = root.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", true);
                key.SetValue(@"PauseFeatureUpdatesStartTime", @"2015-01-01T12:00:00Z", RegistryValueKind.String);
                key.SetValue(@"PauseQualityUpdatesStartTime", @"2015-01-01T12:00:00Z", RegistryValueKind.String);
                key.SetValue(@"PauseUpdatesExpiryTime", @"2050-01-01T12:00:00Z", RegistryValueKind.String);
                key.SetValue(@"PauseFeatureUpdatesEndTime", @"2050-01-01T12:00:00Z", RegistryValueKind.String);
                key.SetValue(@"PauseQualityUpdatesEndTime", @"2050-01-01T12:00:00Z", RegistryValueKind.String);

                var key2 = root.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UpdatePolicy\Settings", true);
                key2.SetValue("PausedQualityStatus", 1, RegistryValueKind.DWord);
                key2.SetValue("PausedFeatureStatus", 1, RegistryValueKind.DWord);
                key2.SetValue("PausedFeatureDate", 132087839360000000, RegistryValueKind.QWord);
                key2.SetValue("PausedQualityDate", 132087839360000000, RegistryValueKind.QWord);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public bool EnableUpdate()
        {
            try
            {
                RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                var key = root.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", true);
                key.DeleteValue(@"PauseFeatureUpdatesStartTime");
                key.DeleteValue(@"PauseQualityUpdatesStartTime");
                key.DeleteValue(@"PauseUpdatesExpiryTime");
                key.DeleteValue(@"PauseFeatureUpdatesEndTime");
                key.DeleteValue(@"PauseQualityUpdatesEndTime");

                var key2 = root.OpenSubKey(@"SOFTWARE\Microsoft\WindowsUpdate\UpdatePolicy\Settings", true);
                key2.SetValue("PausedQualityStatus", 0, RegistryValueKind.DWord);
                key2.SetValue("PausedFeatureStatus", 0, RegistryValueKind.DWord);
                key2.DeleteValue("PausedFeatureDate");
                key2.DeleteValue("PausedQualityDate");

                ChangeServiceStartType("wuauserv", ServiceStartupType.Manual);
            }
            catch
            {
                return false;
            }
            return true;
        }




        public void ChangeStartupTypeServiceUpdate()
        {
            string STTUpdate = CheckStatusServiceUpdate();
            if (STTUpdate != "System" && STTUpdate != "Boot" && STTUpdate != "Disable")
            {
                ChangeServiceStartType("wuauserv", ServiceStartupType.Disabled);
            }
            else
            {
                ChangeServiceStartType("wuauserv", ServiceStartupType.Manual);
            }

        }


        #region Service
        public void ChangeServiceStartType(string serviceName, ServiceStartupType startType)
        {
            //Obtain a handle to the service control manager database
            IntPtr scmHandle = OpenSCManager(null, null, SC_MANAGER_CONNECT);
            if (scmHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to obtain a handle to the service control manager database.");
            }

            //Obtain a handle to the specified windows service
            IntPtr serviceHandle = OpenService(scmHandle, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
            if (serviceHandle == IntPtr.Zero)
            {
                throw new Exception($"Failed to obtain a handle to service '{serviceName}'.");
            }

            //Change the start mode
            bool changeServiceSuccess = ChangeServiceConfig(serviceHandle, SERVICE_NO_CHANGE, (uint)startType, SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, null, null, null);
            if (!changeServiceSuccess)
            {
                string msg = $"Failed to update service configuration for service '{serviceName}'. ChangeServiceConfig returned error {Marshal.GetLastWin32Error()}.";
                throw new Exception(msg);
            }

            //Clean up
            if (scmHandle != IntPtr.Zero)
                CloseServiceHandle(scmHandle);
            if (serviceHandle != IntPtr.Zero)
                CloseServiceHandle(serviceHandle);
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ChangeServiceConfig(
            IntPtr hService,
            uint nServiceType,
            uint nStartType,
            uint nErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[] lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);

        private const uint SC_MANAGER_CONNECT = 0x0001;
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;

        public enum ServiceStartupType : uint
        {
            /// <summary>
            /// A device driver started by the system loader. This value is valid only for driver services.
            /// </summary>
            BootStart = 0,

            /// <summary>
            /// A device driver started by the IoInitSystem function. This value is valid only for driver services.
            /// </summary>
            SystemStart = 1,

            /// <summary>
            /// A service started automatically by the service control manager during system startup.
            /// </summary>
            Automatic = 2,

            /// <summary>
            /// A service started by the service control manager when a process calls the StartService function.
            /// </summary>
            Manual = 3,

            /// <summary>
            /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
            /// </summary>
            Disabled = 4
        }
        #endregion
    }
}
