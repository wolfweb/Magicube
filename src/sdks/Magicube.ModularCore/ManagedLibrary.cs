using System;
using System.IO;
using System.Reflection;

namespace Magicube.ModularCore {
    public class ManagedLibrary {
        private ManagedLibrary(AssemblyName name, string additionalProbingPath, string appLocalPath) {
            Name                  = name ?? throw new ArgumentNullException(nameof(name));
            AppLocalPath          = appLocalPath ?? throw new ArgumentNullException(nameof(appLocalPath));
            AdditionalProbingPath = additionalProbingPath ?? throw new ArgumentNullException(nameof(additionalProbingPath));
        }

        public AssemblyName Name                  { get; private set; }

        public string       AppLocalPath          { get; private set; }
                            
        public string       AdditionalProbingPath { get; private set; }

        public static ManagedLibrary CreateFromPackage(string packageId, string packageVersion, string assetPath) {
            var appLocalPath = assetPath.StartsWith("lib/") ? Path.GetFileName(assetPath) : assetPath;

            return new ManagedLibrary(
                new AssemblyName(Path.GetFileNameWithoutExtension(assetPath)),
                Path.Combine(packageId.ToLowerInvariant(), packageVersion, assetPath),
                appLocalPath
            );
        }
    }
}
