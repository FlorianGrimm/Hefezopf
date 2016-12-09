namespace Hefezopf.WindowsService.Shared {
    using System;
    using System.Collections.Generic;
    using System.Configuration.Install;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading;

    public class HZBootingService {
        /// <summary>
        /// for the installer only
        /// </summary>
        internal static string ServiceNameForInstaller;
        internal static string ServiceUsernameForInstaller;
        internal static string ServicePasswordForInstaller;

        private readonly Func<Assembly> _GetAssembly;
        private readonly Func<string, ServiceBase> _CreateService;
        private readonly string _EventLogName;
        private string _ServiceName;
        private string _Location;
        private string _ServiceUsername;
        private string _ServicePassword;
        private Action<string> OutWriteLine;
        private Action<string> ErrorWriteLine;
        private bool _ConsoleIsAttached;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getAssembly">func that returns the assembly.</param>
        /// <param name="createService">func that create a new service.</param>
        /// <param name="serviceName">the servicename.</param>
        /// <param name="eventLogName">the event log source name</param>
        public HZBootingService(Func<Assembly> getAssembly, Func<string, ServiceBase> createService, string serviceName, string eventLogName) {
            this._GetAssembly = getAssembly;
            this._CreateService = createService;
            this._ServiceName = serviceName;
            this.OutWriteLine = DefaultOutWriteLine;
            this.ErrorWriteLine = DefaultErrorWriteLine;
            if (!string.IsNullOrEmpty(eventLogName)) {
                try {
                    if (!System.Diagnostics.EventLog.SourceExists(eventLogName)) {
                        System.Diagnostics.EventLog.CreateEventSource(eventLogName, "Application");
                        System.Threading.Thread.Sleep(250);
                        System.Diagnostics.EventLog.WriteEntry(eventLogName, eventLogName, System.Diagnostics.EventLogEntryType.Information);
                    }
                    this._EventLogName = eventLogName;
                } catch { }
            }
        }

        /// <summary>
        /// Parse the given args and execute the given commands.
        /// </summary>
        /// <param name="args">The args from the commandline.</param>
        /// <returns>0 for succes</returns>
        public int Main(string[] args) {
            int result = 0;
            bool bVerbose = false;
            bool bSilent = false;
            var stepsParsed = new List<KeyValuePair<string, List<string>>>();
            var stepsToRun = new List<KeyValuePair<string, List<string>>>();
            var cmdArgs = new List<string>();
            var programArgs = cmdArgs;

            if (args.Length > 0) {
                // Parse args            
                foreach (var argument in args) {
                    string name = null;
                    // only / not - ; - is reserved for the service specific ones.
                    if (argument.StartsWith("/", StringComparison.OrdinalIgnoreCase)) {
                        name = argument.TrimStart(new char[] { '/' });
                        if (string.Equals("verbose", name, StringComparison.InvariantCultureIgnoreCase)) {
                            bVerbose = true;
                        }
                        if (string.Equals("silent", name, StringComparison.InvariantCultureIgnoreCase)) {
                            bSilent = true;
                        }
                        cmdArgs = new List<string>();
                        stepsParsed.Add(new KeyValuePair<string, List<string>>(name, cmdArgs));
                    } else {
                        cmdArgs.Add(argument);
                    }
                }

                // global args               
                foreach (var step in stepsParsed) {
                    if (string.Equals("name", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        string value = step.Value.FirstOrDefault();
                        if (!string.IsNullOrEmpty(value)) {
                            this._ServiceName = value;
                        }
                        continue;
                    }
                    if (string.Equals("location", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        string value = step.Value.FirstOrDefault();
                        if (!string.IsNullOrEmpty(value)) {
                            this._Location = value;
                        }
                        continue;
                    }
                    if (string.Equals("username", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        string value = step.Value.FirstOrDefault();
                        if (!string.IsNullOrEmpty(value)) {
                            this._ServiceUsername = value;
                        }
                        continue;
                    }
                    if (string.Equals("password", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        string value = step.Value.FirstOrDefault();
                        if (!string.IsNullOrEmpty(value)) {
                            this._ServicePassword = value;
                        }
                        continue;
                    }
                    // add command
                    stepsToRun.Add(step);

                    // side effects console
                    if (string.Equals("install", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        bVerbose = true;
                        continue;
                    }
                    if (string.Equals("uninstall", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        bVerbose = true;
                        continue;
                    }
                    if (string.Equals("debug", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                        bSilent = false;
                        bVerbose = true;
                        continue;
                    }
                }

                // console
                if (bSilent) {
                } else if (bVerbose) {
                    this.attachConsole();
                }
            }
            if (stepsToRun.Any()) {
                //
                // evaluate args
                try {
                    foreach (var step in stepsToRun) {
                        if (string.Equals("help", step.Key, StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals("?", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.ShowHelp(new KeyValuePair<string, List<string>>(null, null));
                            continue;
                        }
                        if (string.Equals("install", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Install(step);
                            continue;
                        }
                        if (string.Equals("uninstall", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Uninstall(step);
                            continue;
                        }
                        if (string.Equals("start", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Start(step);
                            continue;
                        }
                        if (string.Equals("stop", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Stop(step);
                            continue;
                        }
                        if (string.Equals("update", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Update(step);
                            continue;
                        }
                        if (string.Equals("run", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Run(step.Value);
                            continue;
                        }
                        if (string.Equals("debug", step.Key, StringComparison.InvariantCultureIgnoreCase)) {
                            this.Debug(step.Value);
                            continue;
                        }
                        {
                            this.ShowHelp(step);
                        }
                    }
                } catch (Exception exception) {
                    this.DefaultErrorWriteLine(exception.ToString());
                    result = 1;
                }
            } else {
                this.Run(programArgs);
            }
            return result;
        }


        public void ShowHelp(KeyValuePair<string, List<string>> step) {
            this.attachConsole();
            if (step.Key != null) {
                OutWriteLine("Unkown command");
                OutWriteLine(step.Key);
            }
            OutWriteLine("Commands");
            OutWriteLine("/install   installs the servive with the name /name at the current location or /location with the /úsername and /password or LocaalSystem.");
            OutWriteLine("/update    updates the servive with the name /name at the current location.");
            OutWriteLine("/uninstall uninstalls the service with the name /name.");
            OutWriteLine("/start     starts the service with the name /name.");
            OutWriteLine("/stop      stops the service with the name /name.");
            OutWriteLine("/help /?   show help");
            OutWriteLine("");
            OutWriteLine("Parameter");
            OutWriteLine("/name <ServiceName> set the servicename. for all. ");
            OutWriteLine("/location <ServiceName> set the location. for /install");
            OutWriteLine("/username <windowsusername> set the service account. for /install");
            OutWriteLine("/password <password> set the service password. for /install");
            OutWriteLine("");
            OutWriteLine("/silent no console output.");
            OutWriteLine("/verbose enable console output.");
            OutWriteLine("");
        }
        public void Install(KeyValuePair<string, List<string>> step) {
            ServiceNameForInstaller = this._ServiceName;
            ServiceUsernameForInstaller = this._ServiceUsername;
            ServicePasswordForInstaller = this._ServicePassword;
            string serviceLocation = GetServiceLocation(this._Location);
            var assembly = this._GetAssembly();

            if (string.IsNullOrEmpty(serviceLocation)) {
                serviceLocation = assembly.Location;
            } else if (string.Equals(serviceLocation, assembly.Location, StringComparison.InvariantCultureIgnoreCase)) {
                serviceLocation = assembly.Location;
            } else {
                CopyServiceFile(serviceLocation);
            }
            var logPath = serviceLocation + ".InstallLog";
            var transactedInstaller = new TransactedInstaller();
            var ai = new AssemblyInstaller(assembly, new string[] { });
            transactedInstaller.Installers.Add(ai);
            transactedInstaller.Context = new InstallContext(logPath, step.Value.ToArray());
            //
            var savedState = new System.Collections.Hashtable();
            transactedInstaller.Install(savedState);
            //            SetServiceCommandLine(serviceLocation);
            var serviceArgs = $"\"{serviceLocation}\" /name \"{this._ServiceName}\"";
            NativeMethods.SetServicePathName(this._ServiceName, serviceArgs);
            ServiceNameForInstaller = null;
            ServiceUsernameForInstaller = null;
            ServicePasswordForInstaller = null;
        }

        public void Uninstall(KeyValuePair<string, List<string>> step) {
            ServiceNameForInstaller = this._ServiceName;
            var assembly = this._GetAssembly();
            var logPath = assembly.Location + ".InstallLog";
            var transactedInstaller = new TransactedInstaller();
            var ai = new AssemblyInstaller(assembly, new string[] { });
            transactedInstaller.Installers.Add(ai);
            transactedInstaller.Context = new InstallContext(logPath, step.Value.ToArray());
            System.Collections.IDictionary savedState = null;
            transactedInstaller.Uninstall(savedState);
        }

        public void Start(KeyValuePair<string, List<string>> step) {
            var sc = new ServiceController(this._ServiceName);
            if (sc.Status == ServiceControllerStatus.Stopped) {
                if (step.Value.Any()) {
                    sc.Start(step.Value.ToArray());
                } else {
                    sc.Start();
                }
            }
            if (sc.Status == ServiceControllerStatus.StartPending) {
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            }
        }

        public void Stop(KeyValuePair<string, List<string>> step) {
            var sc = new ServiceController(this._ServiceName);
            if (sc.Status == ServiceControllerStatus.Running) {
                sc.Stop();
            }
            if (sc.Status == ServiceControllerStatus.StopPending) {
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
        }

        public void Update(KeyValuePair<string, List<string>> step) {
            var sc = new ServiceController(this._ServiceName);
            if (sc.Status == ServiceControllerStatus.Running) {
                sc.Stop();
            }
            if (sc.Status == ServiceControllerStatus.StopPending) {
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
            //
            string serviceLocation = GetServiceLocation(this._Location);
            CopyServiceFile(serviceLocation);
            //
            if (step.Value.Any()) {
                sc.Start(step.Value.ToArray());
            } else {
                sc.Start();
            }
            if (sc.Status == ServiceControllerStatus.StartPending) {
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            }
        }

        public void Run(IEnumerable<string> args) {
            var serviceBase = this._CreateService(this._ServiceName);
            if (args != null && args.Any()) {
                var serviceBaseWithArguments = serviceBase as IHZServiceBaseWithArguments;
                if (serviceBaseWithArguments != null) {
                    serviceBaseWithArguments.SetArguments(args);
                }
            }
            ServiceBase.Run(new ServiceBase[] { serviceBase });
        }
        public void Debug(IEnumerable<string> args) {
            var serviceBase = this._CreateService(this._ServiceName);
            if (args == null) { args = new string[0]; }
            var serviceBaseWithArguments = (IHZServiceBaseWithArguments)serviceBase;
            serviceBaseWithArguments.SetArguments(args);
            using (var stop = serviceBaseWithArguments.Debug()) {
                this.OutWriteLine("Press CTRL-C to exit.");
                using (var autoResetEventStop = new AutoResetEvent(false)) {
                    System.Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { autoResetEventStop.Set(); e.Cancel = true; };
                    autoResetEventStop.WaitOne();
                }
                this.OutWriteLine("Going down...");
            }
            return;
        }

        private void DefaultOutWriteLine(string msg) {
            System.Diagnostics.Trace.WriteLine(msg);
        }

        private void DefaultErrorWriteLine(string msg) {
            System.Diagnostics.Trace.WriteLine(msg);
            try {
                System.Diagnostics.EventLog.WriteEntry(this._EventLogName, msg, System.Diagnostics.EventLogEntryType.Error);
            } catch { }
        }

        private void ConsoleOutWriteLine(string msg) {
            try {
                System.Diagnostics.Trace.WriteLine(msg);
                System.Console.Out.WriteLine(msg);
            } catch { }
        }

        private void ConsoleErrorWriteLine(string msg) {
            try {
                System.Diagnostics.Trace.WriteLine(msg);
                System.Console.Error.WriteLine(msg);
                System.Diagnostics.EventLog.WriteEntry(this._EventLogName, msg, System.Diagnostics.EventLogEntryType.Error);
            } catch { }
        }

        private string GetServiceLocation(string serviceLocation) {
            if (string.IsNullOrEmpty(serviceLocation)) {
                try {
                    using (var wmiService = new System.Management.ManagementObject($"Win32_Service.Name='{this._ServiceName}'")) {
                        wmiService.Get();
                        serviceLocation = wmiService["PathName"] as string;
                    }
                    System.GC.Collect();
                } catch (System.Management.ManagementException) {
                }
            }
            return serviceLocation;
        }

        private bool CopyServiceFile(string serviceLocation) {
            if (string.IsNullOrEmpty(serviceLocation)) { return false; }
            if (serviceLocation.StartsWith("\"", StringComparison.Ordinal)) {
                serviceLocation = serviceLocation.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
            if (serviceLocation.EndsWith(".exe", StringComparison.Ordinal)) {
                //update it
                var sourceLocationExe = this._GetAssembly().Location;
                if (!string.Equals(sourceLocationExe, serviceLocation, StringComparison.InvariantCultureIgnoreCase)) {
                    var sourceLocationConfig = sourceLocationExe + ".config";
                    var serviceLocationConfig = serviceLocation + ".config";
                    byte[] contentExe = System.IO.File.ReadAllBytes(sourceLocationExe);
                    byte[] contentConfig = null;
                    if (System.IO.File.Exists(sourceLocationConfig)) {
                        contentConfig = System.IO.File.ReadAllBytes(sourceLocationConfig);
                    } else {
                        sourceLocationConfig = null;
                    }
                    for (int watchDog = 10; watchDog >= 0; watchDog--) {
                        try {
                            if (contentExe != null) {
                                System.IO.File.WriteAllBytes(serviceLocation, contentExe);
                                contentExe = null;
                            }
                            if (contentConfig != null) {
                                System.IO.File.WriteAllBytes(serviceLocationConfig, contentConfig);
                                contentConfig = null;
                            }
                            return true;
                        } catch (Exception exception) {
                            this.ErrorWriteLine(exception.ToString());
                            System.GC.Collect();
                            System.Threading.Thread.Sleep(250);
                        }
                    }
                }
            }
            return false;
        }

        private void attachConsole() {
            if (!_ConsoleIsAttached) {
                _ConsoleIsAttached = HZWindowConsoleUtility.AttachConsole();
                if (!_ConsoleIsAttached) {
                    _ConsoleIsAttached = HZWindowConsoleUtility.CreateConsole();
                }
                if (_ConsoleIsAttached) {
                    HZWindowConsoleUtility.AdjustConsoleCodePage();
                    this.OutWriteLine = this.ConsoleOutWriteLine;

                    this.ErrorWriteLine = this.ConsoleErrorWriteLine;
                }
            }
        }
    }
}
