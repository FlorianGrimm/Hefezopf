using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hefezopf.AssemblyInResource.Shared {
    public class HZAssemblyInResource : System.IDisposable {
        private readonly Assembly _ResourceAssembly;
        private readonly string _ResourceNamespace;
        private readonly string _ResourceNamespaceSubPath;
        private bool _IsDisposed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">A type that live in the resource - namespace. - if null this.GetType() will be used.</param>
        /// <param name="subPath">The SubPath (for Debug or release).</param>
        public HZAssemblyInResource(Type type, string subPath) {
            if (type == null) {
                type = this.GetType();
            }
            this._ResourceNamespace = type.Namespace;
            this._ResourceAssembly = type.Assembly;
            if (string.IsNullOrEmpty(subPath)) {
                this._ResourceNamespaceSubPath = this._ResourceNamespace;
            } else {
                this._ResourceNamespaceSubPath = this._ResourceNamespace + "." + subPath;
            }
        }
        public void ExtractTo(string targetRelativePath) {
            if (string.IsNullOrEmpty(targetRelativePath)) { targetRelativePath = ""; }
            var targetPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                , targetRelativePath);
            if (System.IO.Directory.Exists(targetPath)) {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            var manifestResourceNames = this._ResourceAssembly.GetManifestResourceNames();
            foreach (var manifestResourceName in manifestResourceNames) {
                // doing down?
                if (this._IsDisposed) { return; }

                // which folder => namespace => resourcename
                string resourceNamespace = null;
                if (manifestResourceName.StartsWith(this._ResourceNamespaceSubPath)) {
                    resourceNamespace = this._ResourceNamespaceSubPath;
                } else if (manifestResourceName.StartsWith(this._ResourceNamespace)) {
                    resourceNamespace = this._ResourceNamespace;
                } else {
                    continue;
                }
                var relativeName = manifestResourceName.Substring(resourceNamespace.Length + 1);
                var targetFilePathName = System.IO.Path.Combine(targetPath, relativeName);
                var targetFilePathNameExtension = System.IO.Path.GetExtension(targetFilePathName);
                var targetVersionInfo =
                    (   (string.Equals(".exe", targetFilePathNameExtension, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(".dll", targetFilePathNameExtension, StringComparison.OrdinalIgnoreCase)
                        )
                        && System.IO.File.Exists(targetFilePathName)
                    ) ? System.Diagnostics.FileVersionInfo.GetVersionInfo(targetFilePathName)
                    : null;
                var currentTargetFileVersion = targetVersionInfo?.FileVersion;
                if (currentTargetFileVersion == null) {
                    // copy needed
                } else if (currentTargetFileVersion == Hefezopf.HZVersion.AssemblyFileVersion) {
#if DEBUG
                    // overwite if date

                    var sourceModDate = System.IO.File.GetLastWriteTimeUtc(this.GetType().Assembly.Location ?? System.Reflection.Assembly.GetEntryAssembly().Location);
                    var targetModDate = System.IO.File.GetLastWriteTimeUtc(targetFilePathName);
                    if (sourceModDate < targetModDate) {
                        // skip
                        continue;
                    }
#else
                    // skip
                    continue;
#endif
                }
                if (!System.IO.Directory.Exists(targetPath)) {
                    System.IO.Directory.CreateDirectory(targetPath);
                }
                if (System.IO.Directory.Exists(targetPath)) {
                    using (var sourceStream = this._ResourceAssembly.GetManifestResourceStream(manifestResourceName)) {
                        using (var targetStream = System.IO.File.Create(targetFilePathName)) {
                            sourceStream.CopyTo(targetStream);
                        }
                    }
                }
            }
        }
        public void Dispose() {
            this._IsDisposed = true;
        }
    }
}
