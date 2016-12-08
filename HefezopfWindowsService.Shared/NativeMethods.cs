namespace HefezopfWindowsService.Shared {
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    public class NativeMethods {
        public const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        public const int SERVICE_NO_CHANGE = -1; //this value is found in winsvc.h
                                                          // System.ServiceProcess.NativeMethods
        public const int SERVICE_QUERY_CONFIG = 1;
        public const int SERVICE_CHANGE_CONFIG = 2;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, int access);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr databaseHandle, string serviceName, int access);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CloseServiceHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean ChangeServiceConfig(IntPtr hService, int nServiceType, int nStartType, int nErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, [In] char[] lpDependencies, string lpServiceStartName, string lpPassword, string lpDisplayName);

        public static bool SetServicePathName(string serviceName,string pathName) {
            IntPtr scManager = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            IntPtr service = IntPtr.Zero;
            if (scManager == IntPtr.Zero) {
                throw new System.ComponentModel.Win32Exception();
            }
            try {
                service = OpenService(scManager, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
                if (service == IntPtr.Zero) {
                    throw new System.ComponentModel.Win32Exception();
                }
                return ChangeServiceConfig(service, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, pathName, null, IntPtr.Zero, null, null, null, null);
            } finally {
                if (service != IntPtr.Zero) {
                    CloseServiceHandle(service);
                }
                CloseServiceHandle(scManager);
            }
        }
    }
}
