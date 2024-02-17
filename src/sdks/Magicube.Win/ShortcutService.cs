using System;
using System.Runtime.Versioning;

namespace Magicube.Win {
    [SupportedOSPlatform("windows")]
    public class ShortcutService {
        public ShortcutDescription ReadShortcut(string lnkFilePath) {
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            var shortcut = shell.CreateShortcut(lnkFilePath);
            return new ShortcutDescription() {
                TargetPath       = shortcut.TargetPath,
                Arguments        = shortcut.Arguments,
                Description      = shortcut.Description,
                FullName         = shortcut.FullName,
                Hotkey           = shortcut.Hotkey,
                IconLocation     = shortcut.IconLocation,
                WindowStyle      = shortcut.WindowStyle,
                WorkingDirectory = shortcut.WorkingDirectory,
            };
        }

		public void CreateShortcut(string lnkFilePath, ShortcutDescription desc) {
			var shellType = Type.GetTypeFromProgID("WScript.Shell");
			dynamic shell = Activator.CreateInstance(shellType);
			var shortcut  = shell.CreateShortcut(lnkFilePath);

			shortcut.Hotkey           = desc.Hotkey;
			shortcut.FullName         = desc.FullName;
			shortcut.Arguments        = desc.Arguments;
			shortcut.TargetPath       = desc.TargetPath;
			shortcut.Description      = desc.Description;
			shortcut.WindowStyle      = desc.WindowStyle;
			shortcut.IconLocation     = desc.IconLocation;
			shortcut.WorkingDirectory = desc.WorkingDirectory;

			shortcut.Save();
		}

		public void CreateShortcut(string lnkFilePath, string targetPath, string workDir, string args = "") {
			var shellType = Type.GetTypeFromProgID("WScript.Shell");
			dynamic shell = Activator.CreateInstance(shellType);
			var shortcut  = shell.CreateShortcut(lnkFilePath);
			shortcut.Arguments        = args;
			shortcut.TargetPath       = targetPath;
			shortcut.WorkingDirectory = workDir;
			shortcut.Save();
		}
    }

    public class ShortcutDescription {
		public string Hotkey           { get; set; }
		public string FullName         { get; set; }
		public string Arguments        { get; set; }
		public string TargetPath       { get; set; }
		public string Description      { get; set; }
		public int    WindowStyle      { get; set; }
		public string IconLocation     { get; set; }
		public string WorkingDirectory { get; set; }
	}
}
