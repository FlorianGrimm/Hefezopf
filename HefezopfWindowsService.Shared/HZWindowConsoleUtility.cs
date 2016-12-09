namespace Hefezopf.WindowsService.Shared {
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>Utility for Console for a WindowApplication executeable.</summary>
    public class HZWindowConsoleUtility {
        /// <summary>
        /// redirect console output to parent process;
        /// must be before any calls to Console.WriteLine()
        /// </summary>
        /// <returns>success</returns>
        public static bool AttachConsole() => Native.AttachConsole();

        /// <summary>Creates a console window for a window application.</summary>
        /// <returns>success</returns>
        public static bool CreateConsole() => Native.CreateConsole();

        /// <summary>
        /// Adjust the codepage if needed
        /// </summary>
        public static void AdjustConsoleCodePage() {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture.GetConsoleFallbackUICulture();
            if (Console.OutputEncoding.CodePage != 65001
                && Console.OutputEncoding.CodePage != Thread.CurrentThread.CurrentUICulture.TextInfo.OEMCodePage
                && Console.OutputEncoding.CodePage != Thread.CurrentThread.CurrentUICulture.TextInfo.ANSICodePage) {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
        }

        private static class Native {
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool AllocConsole();

            [DllImport("kernel32.dll")]
            private static extern IntPtr GetConsoleWindow();
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool AttachConsole(int dwProcessId);
            internal static bool AttachConsole() {
                // redirect console output to parent process;
                // must be before any calls to Console.WriteLine()
                const int ATTACH_PARENT_PROCESS = -1;
                return AttachConsole(ATTACH_PARENT_PROCESS);
            }
            internal static bool CreateConsole() {
                if (GetConsoleWindow() == IntPtr.Zero) {
                    return AllocConsole();
                } else { return true; }
            }
        }
    }
}
