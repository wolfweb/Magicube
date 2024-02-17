using Magicube.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Magicube.ModularCore {
    public class ManagedLoadContext : AssemblyLoadContext {
        private readonly IReadOnlyDictionary<string, ManagedLibrary> _managedAssemblies;
        private readonly IReadOnlyDictionary<string, NativeLibrary> _nativeLibraries;
        private readonly IReadOnlyCollection<string> _additionalProbingPaths;
        private readonly IReadOnlyCollection<string> _privateAssemblies;
        private readonly IReadOnlyCollection<string> _defaultAssemblies;
        private readonly AssemblyDependencyResolver _dependencyResolver;

        private readonly string _unmanagedDllShadowCopyDirectoryPath;
        private readonly bool _shadowCopyNativeLibraries;
        private readonly bool _preferDefaultLoadContext;
        private AssemblyLoadContext _defaultLoadContext;
        private readonly string _mainAssemblyPath;
        private readonly string[] _resourceRoots;
        private readonly string _basePath;
        private bool _loadInMemory;

        public ManagedLoadContext(
            string mainAssemblyPath,
            IReadOnlyDictionary<string, ManagedLibrary> managedAssemblies,
            IReadOnlyDictionary<string, NativeLibrary> nativeLibraries,
            IReadOnlyCollection<string> privateAssemblies,
            IReadOnlyCollection<string> defaultAssemblies,
            IReadOnlyCollection<string> additionalProbingPaths,
            IReadOnlyCollection<string> resourceProbingPaths,
            AssemblyLoadContext defaultLoadContext,
            bool preferDefaultLoadContext,
            bool isCollectible,
            bool loadInMemory,
            bool shadowCopyNativeLibraries
            ) : base(Path.GetFileNameWithoutExtension(mainAssemblyPath), isCollectible) {
            if (resourceProbingPaths == null) {
                throw new ArgumentNullException(nameof(resourceProbingPaths));
            }

            _dependencyResolver = new AssemblyDependencyResolver(mainAssemblyPath);

            _basePath                  = Path.GetDirectoryName(mainAssemblyPath) ?? throw new ArgumentException(nameof(mainAssemblyPath));
            _additionalProbingPaths    = additionalProbingPaths ?? throw new ArgumentNullException(nameof(additionalProbingPaths));
            _managedAssemblies         = managedAssemblies ?? throw new ArgumentNullException(nameof(managedAssemblies));
            _privateAssemblies         = privateAssemblies ?? throw new ArgumentNullException(nameof(privateAssemblies));
            _defaultAssemblies         = defaultAssemblies ?? throw new ArgumentNullException(nameof(defaultAssemblies));
            _mainAssemblyPath          = mainAssemblyPath ?? throw new ArgumentNullException(nameof(mainAssemblyPath));
            _nativeLibraries           = nativeLibraries ?? throw new ArgumentNullException(nameof(nativeLibraries));
            _shadowCopyNativeLibraries = shadowCopyNativeLibraries;
            _preferDefaultLoadContext  = preferDefaultLoadContext;
            _defaultLoadContext        = defaultLoadContext;
            _loadInMemory              = loadInMemory;

            _resourceRoots             = new[] { _basePath }.Concat(resourceProbingPaths).ToArray();

            _unmanagedDllShadowCopyDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (shadowCopyNativeLibraries) {
                Unloading += _ => OnUnloaded();
            }
        }

        protected override Assembly Load(AssemblyName assemblyName) {
            if (assemblyName.Name == null) {
                return null;
            }

            if ((_preferDefaultLoadContext || _defaultAssemblies.Contains(assemblyName.Name)) && !_privateAssemblies.Contains(assemblyName.Name)) {
                try {
                    var defaultAssembly = _defaultLoadContext.LoadFromAssemblyName(assemblyName);
                    if (defaultAssembly != null) {
                        return defaultAssembly;
                    }
                } catch { }
            }

            var resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (!resolvedPath.IsNullOrEmpty() && File.Exists(resolvedPath)) {
                return LoadAssemblyFromFilePath(resolvedPath);
            }

            if (!assemblyName.CultureName.IsNullOrEmpty() && !string.Equals("neutral", assemblyName.CultureName)) {
                foreach (var resourceRoot in _resourceRoots) {
                    var resourcePath = Path.Combine(resourceRoot, assemblyName.CultureName, assemblyName.Name + ".dll");
                    if (File.Exists(resourcePath)) {
                        return LoadAssemblyFromFilePath(resourcePath);
                    }
                }

                return null;
            }

            if (_managedAssemblies.TryGetValue(assemblyName.Name, out var library) && library != null) {
                if (SearchForLibrary(library, out var path) && path != null) {
                    return LoadAssemblyFromFilePath(path);
                }
            } else {
                var localFile = Path.Combine(_basePath, assemblyName.Name + ".dll");
                if (File.Exists(localFile)) {
                    return LoadAssemblyFromFilePath(localFile);
                }
            }

            return null;
        }

        public Assembly LoadAssemblyFromFilePath(string path) {
            if (!_loadInMemory) {
                return LoadFromAssemblyPath(path);
            }

            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return LoadFromStream(file);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
            var resolvedPath = _dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (!resolvedPath.IsNullOrEmpty() && File.Exists(resolvedPath)) {
                return LoadUnmanagedDllFromResolvedPath(resolvedPath, normalizePath: false);
            }

            foreach (var prefix in PlatformInformation.NativeLibraryPrefixes) {
                if (_nativeLibraries.TryGetValue(prefix + unmanagedDllName, out var library)) {
                    if (SearchForLibrary(library, prefix, out var path) && path != null) {
                        return LoadUnmanagedDllFromResolvedPath(path);
                    }
                } else {
                    foreach (var suffix in PlatformInformation.NativeLibraryExtensions) {
                        if (!unmanagedDllName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) {
                            continue;
                        }

                        var trimmedName = unmanagedDllName.Substring(0, unmanagedDllName.Length - suffix.Length);

                        if (_nativeLibraries.TryGetValue(prefix + trimmedName, out library)) {
                            if (SearchForLibrary(library, prefix, out var path) && path != null) {
                                return LoadUnmanagedDllFromResolvedPath(path);
                            }
                        } else {
                            var localFile = Path.Combine(_basePath, prefix + unmanagedDllName + suffix);
                            if (File.Exists(localFile)) {
                                return LoadUnmanagedDllFromResolvedPath(localFile);
                            }

                            var localFileWithoutSuffix = Path.Combine(_basePath, prefix + unmanagedDllName);
                            if (File.Exists(localFileWithoutSuffix)) {
                                return LoadUnmanagedDllFromResolvedPath(localFileWithoutSuffix);
                            }
                        }
                    }

                }
            }

            return base.LoadUnmanagedDll(unmanagedDllName);
        }

        private bool SearchForLibrary(ManagedLibrary library, out string path) {
            var localFile = Path.Combine(_basePath, library.AppLocalPath);
            if (File.Exists(localFile)) {
                path = localFile;
                return true;
            }

            foreach (var searchPath in _additionalProbingPaths) {
                var candidate = Path.Combine(searchPath, library.AdditionalProbingPath);
                if (File.Exists(candidate)) {
                    path = candidate;
                    return true;
                }
            }

            foreach (var ext in PlatformInformation.ManagedAssemblyExtensions) {
                var local = Path.Combine(_basePath, library.Name.Name + ext);
                if (File.Exists(local)) {
                    path = local;
                    return true;
                }
            }

            path = null;
            return false;
        }

        private bool SearchForLibrary(NativeLibrary library, string prefix, out string? path) {
            foreach (var ext in PlatformInformation.NativeLibraryExtensions) {
                var candidate = Path.Combine(_basePath, $"{prefix}{library.Name}{ext}");
                if (File.Exists(candidate)) {
                    path = candidate;
                    return true;
                }
            }

            var local = Path.Combine(_basePath, library.AppLocalPath);
            if (File.Exists(local)) {
                path = local;
                return true;
            }

            foreach (var searchPath in _additionalProbingPaths) {
                var candidate = Path.Combine(searchPath, library.AdditionalProbingPath);
                if (File.Exists(candidate)) {
                    path = candidate;
                    return true;
                }
            }

            path = null;
            return false;
        }

        private IntPtr LoadUnmanagedDllFromResolvedPath(string unmanagedDllPath, bool normalizePath = true) {
            if (normalizePath) {
                unmanagedDllPath = Path.GetFullPath(unmanagedDllPath);
            }

            return _shadowCopyNativeLibraries
                ? LoadUnmanagedDllFromShadowCopy(unmanagedDllPath)
                : LoadUnmanagedDllFromPath(unmanagedDllPath);
        }

        private IntPtr LoadUnmanagedDllFromShadowCopy(string unmanagedDllPath) {
            var shadowCopyDllPath = CreateShadowCopy(unmanagedDllPath);

            return LoadUnmanagedDllFromPath(shadowCopyDllPath);
        }

        private string CreateShadowCopy(string dllPath) {
            Directory.CreateDirectory(_unmanagedDllShadowCopyDirectoryPath);

            var dllFileName = Path.GetFileName(dllPath);
            var shadowCopyPath = Path.Combine(_unmanagedDllShadowCopyDirectoryPath, dllFileName);

            File.Copy(dllPath, shadowCopyPath);

            return shadowCopyPath;
        }

        private void OnUnloaded() {
            if (!_shadowCopyNativeLibraries || !Directory.Exists(_unmanagedDllShadowCopyDirectoryPath)) {
                return;
            }

            try {
                Directory.Delete(_unmanagedDllShadowCopyDirectoryPath, recursive: true);
            } catch (Exception) {
                
            }
        }
    }
}
