using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net;

namespace ITA.Common.Host
{
    [RunInstaller(false)]
    public class EtwProviderInstaller : Installer
    {
        private const string WEVT_UTIL_FILE_NAME = "wevtutil.exe";
        private const string DLL_EXT = ".dll";
        private const string MAN_EXT = ".man";
        private const int WaitTimeOutMsec = 5000;

        private static ILog _logger = Log4NetItaHelper.GetLogger(typeof(EtwProviderInstaller).Name);

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container _components;
        /// <summary>
        /// ETW providers folder
        /// </summary>
        private readonly string _etwProvidersFolder;

        public EtwProviderInstaller(string etwProvidersFolder)
        {
            _etwProvidersFolder = etwProvidersFolder;

            // This call is required by the Designer.
            InitializeComponent();
        }

        #region Installer implementation

        public override void Commit(IDictionary savedState)
        {
            try
            {
                var utilExists = CheckWevtUtilExists();
                if (!utilExists)
                {
                    return;
                }
                foreach (var item in GetProviderItems(_etwProvidersFolder))
                {
                    UnregisterProvider(item);
                    RegisterProvider(item);
                }
            }
            catch (Exception ex)
            {
                Context.LogMessage(string.Format("Failed to register ETW providers: '{0}'", ex.Message));
                throw;
            }

            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            var utilExists = CheckWevtUtilExists();
            if (!utilExists)
            {
                return;
            }

            foreach (var item in GetProviderItems(_etwProvidersFolder))
            {
                UnregisterProvider(item);
            }
            base.Uninstall(savedState);
        }

        #endregion

        private bool CheckWevtUtilExists()
        {
            var processPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), WEVT_UTIL_FILE_NAME);
            var fileExists = File.Exists(processPath);

            _logger.DebugFormat("File '{0}' {1}found.", processPath, !fileExists ? "not " : string.Empty);

            return fileExists;
        }

        private void RegisterProvider(ProviderItem item)
        {
            _logger.DebugFormat("Register: {0}", item);

            var arguments = string.Format(" im \"{0}\" /rf:\"{1}\" /mf:\"{1}\"", item.Manifest, item.Resource);
            Run(arguments);
        }

        private void UnregisterProvider(ProviderItem item)
        {
            _logger.DebugFormat("Unregister: {0}", item);

            Run(string.Format(" um \"{0}\"", item.Manifest));
        }

        private static void Run(string arguments)
        {
            using (var proc = new Process())
            {
                proc.EnableRaisingEvents = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.FileName = WEVT_UTIL_FILE_NAME;
                proc.StartInfo.Arguments = arguments;
                proc.OutputDataReceived += (s, o) =>
                {
                    _logger.Debug(o.Data);
                };
                proc.ErrorDataReceived += (s, o) =>
                {
                    _logger.Debug(o.Data);
                };
                proc.Start();
                proc.WaitForExit(WaitTimeOutMsec);
            }
        }

        private List<ProviderItem> GetProviderItems(string manifestFolder)
        {
            var files = Directory.EnumerateFiles(manifestFolder).ToArray();
            var keys = files.Select(Path.GetFileNameWithoutExtension).Distinct();
            return keys.Select(k => new ProviderItem
            {
                Key = k,
                Manifest = files.First(f => f.Contains(k) && f.EndsWith(MAN_EXT)),
                Resource = files.First(f => f.Contains(k) && f.EndsWith(DLL_EXT))
            }).ToList();
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        private class ProviderItem
        {
            public string Key { get; set; }
            public string Manifest { get; set; }
            public string Resource { get; set; }

            public override string ToString()
            {
                return string.Format("Key='{0}' Manifest='{1}' Resource='{2}'", Key, Manifest, Resource);
            }
        }
    }
}