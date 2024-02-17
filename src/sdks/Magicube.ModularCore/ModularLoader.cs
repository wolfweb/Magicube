using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Magicube.ModularCore {
    public class ModularLoader : IDisposable {
        private readonly AssemblyLoadContextBuilder _contextBuilder;
        private readonly ModularConfig _config;
        
        private FileSystemWatcher _fileWatcher;
        private ManagedLoadContext _context;        
        private volatile bool _disposed;

        private Debouncer _debouncer;

        public static ModularLoader CreateFromAssemblyFile(string assemblyFile, bool isUnloadable, Assembly[] sharedAssemblies) => CreateFromAssemblyFile(assemblyFile, isUnloadable, sharedAssemblies, _ => { });

        public static ModularLoader CreateFromAssemblyFile(string assemblyFile, bool isUnloadable, Assembly[] sharedAssemblies, Action<ModularConfig> configure) {
            return CreateFromAssemblyFile(assemblyFile,
                    sharedAssemblies,
                    config => {
                        config.IsUnloadable = isUnloadable;
                        configure(config);
                    });
        }

        public static ModularLoader CreateFromAssemblyFile(string assemblyFile, Assembly[] sharedAssemblies) => CreateFromAssemblyFile(assemblyFile, sharedAssemblies, _ => { });

        public static ModularLoader CreateFromAssemblyFile(string assemblyFile, Assembly[] sharedAssemblies, Action<ModularConfig> configure) {
            return CreateFromAssemblyFile(assemblyFile,
                    config => {
                        if (sharedAssemblies != null) {
                            foreach (var item in sharedAssemblies) {
                                config.SharedAssemblies.Add(item.GetName());
                            }
                        }
                        configure(config);
                    });
        }

        public static ModularLoader CreateFromAssemblyFile(string assemblyFile) => CreateFromAssemblyFile(assemblyFile, _ => { });

        public static ModularLoader CreateFromAssemblyFile(string assemblyFile, Action<ModularConfig> configure) {
            if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            var config = new ModularConfig(assemblyFile);
            configure(config);
            return new ModularLoader(config);
        }

        public ModularLoader(ModularConfig config) {
            _config         = config ?? throw new ArgumentNullException(nameof(config));
            _contextBuilder = CreateLoadContextBuilder(config);
            _context        = (ManagedLoadContext)_contextBuilder.Build();

            if (config.EnableHotReload) {
                StartFileWatcher();
            }
        }

        public bool IsUnloadable {
            get {
                return _context.IsCollectible;
            }
        }

        public void Reload() {
            EnsureNotDisposed();

            if (!IsUnloadable) {
                throw new InvalidOperationException("Reload cannot be used because IsUnloadable is false");
            }

            _context.Unload();
            _context = (ManagedLoadContext)_contextBuilder.Build();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void StartFileWatcher() {
            _debouncer = new Debouncer(_config.ReloadDelay);

            _fileWatcher                     = new FileSystemWatcher();
            _fileWatcher.Path                = Path.GetDirectoryName(_config.MainAssemblyPath);
            _fileWatcher.Changed             += OnFileChanged;
            _fileWatcher.Filter              = "*.dll";
            _fileWatcher.NotifyFilter        = NotifyFilters.LastWrite;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object source, FileSystemEventArgs e) {
            if (!_disposed) {
                _debouncer?.Execute(Reload);
            }
        }

        public Assembly LoadDefaultAssembly() {
            EnsureNotDisposed();
            return _context.LoadAssemblyFromFilePath(_config.MainAssemblyPath);
        }

        public Assembly LoadAssembly(AssemblyName assemblyName) {
            EnsureNotDisposed();
            return _context.LoadFromAssemblyName(assemblyName);
        }

        public Assembly LoadAssemblyFromPath(string assemblyPath) => _context.LoadAssemblyFromFilePath(assemblyPath);

        public Assembly LoadAssembly(string assemblyName) {
            EnsureNotDisposed();
            return LoadAssembly(new AssemblyName(assemblyName));
        }

        public AssemblyLoadContext.ContextualReflectionScope EnterContextualReflection() => _context.EnterContextualReflection();

        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;

            if (_fileWatcher != null) {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnFileChanged;
                _fileWatcher.Dispose();
            }

            _debouncer?.Dispose();

            if (_context.IsCollectible) {
                _context.Unload();
            }
        }

        private void EnsureNotDisposed() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(ModularLoader));
            }
        }

        private static AssemblyLoadContextBuilder CreateLoadContextBuilder(ModularConfig config) {
            var builder = new AssemblyLoadContextBuilder();

            builder.SetMainAssemblyPath(config.MainAssemblyPath);
            builder.SetDefaultContext(config.DefaultContext);

            foreach (var ext in config.PrivateAssemblies) {
                builder.PreferLoadContextAssembly(ext);
            }

            if (config.PreferSharedTypes) {
                builder.PreferDefaultLoadContext(true);
            }

            if (config.IsUnloadable || config.EnableHotReload) {
                builder.EnableUnloading();
            }

            if (config.EnableHotReload) {
                builder.PreloadAssembliesIntoMemory();
                builder.ShadowCopyNativeLibraries();
            }

            builder.IsLazyLoaded(config.IsLazyLoaded);

            foreach (var assemblyName in config.SharedAssemblies) {
                builder.PreferDefaultLoadContextAssembly(assemblyName);
            }

            return builder;
        }
    }
}
