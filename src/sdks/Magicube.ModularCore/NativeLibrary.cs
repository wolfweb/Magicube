using System;
using System.IO;

namespace Magicube.ModularCore {
    public class NativeLibrary {
        private NativeLibrary(string name, string appLocalPath, string additionalProbingPath) {
            Name                  = name ?? throw new ArgumentNullException(nameof(name));
            AppLocalPath          = appLocalPath ?? throw new ArgumentNullException(nameof(appLocalPath));
            AdditionalProbingPath = additionalProbingPath ?? throw new ArgumentNullException(nameof(additionalProbingPath));
        }

        public string Name                  { get; private set; }

        public string AppLocalPath          { get; private set; }

        public string AdditionalProbingPath { get; private set; }

        public static NativeLibrary CreateFromPackage(string packageId, string packageVersion, string assetPath) {
            return new NativeLibrary(
                Path.GetFileNameWithoutExtension(assetPath),
                assetPath,
                Path.Combine(packageId.ToLowerInvariant(), packageVersion, assetPath)
            );
        }
    }
}
