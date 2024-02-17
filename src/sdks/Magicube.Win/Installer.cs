using Magicube.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace Magicube.Win {
	[SupportedOSPlatform("windows")]
    public class Installer {
		const string Win32UnInstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
		const string Win64UnInstallKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
		public static IEnumerable<InstalledApp> AllApp() {
			var list = new List<InstalledApp>();

			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                foreach (var it in GetInstalledPrograms(Registry.LocalMachine.OpenSubKey(Win32UnInstallKey))) {
                    list.Add(it);
				}

				foreach (var it in GetInstalledPrograms(Registry.CurrentUser.OpenSubKey(Win32UnInstallKey))) {
					if (list.All(m => m.DisplayName != it.DisplayName)) list.Add(it);
				}

				foreach (var it in GetInstalledPrograms(Registry.LocalMachine.OpenSubKey(Win64UnInstallKey))) {
					if (list.All(m => m.DisplayName != it.DisplayName)) list.Add(it);
				}
			}

			return list;
		}

		private static IEnumerable<InstalledApp> GetInstalledPrograms(RegistryKey registryKey) {
			foreach (var it in registryKey.GetSubKeyNames()) {
				var subkey = registryKey.OpenSubKey(it);
				var displayName = subkey.GetValue("DisplayName") as string;
				if (displayName.IsNullOrEmpty()) continue;

				yield return new InstalledApp {
					DisplayName     = displayName,
					InstallDate     = subkey.GetValue("InstallDate") as string,
					DisplayIcon     = subkey.GetValue("DisplayIcon") as string,
					Publisher       = subkey.GetValue("Publisher") as string,
					DisplayVersion  = subkey.GetValue("DisplayVersion") as string,
					InstallLocation = subkey.GetValue("InstallLocation") as string,
					UninstallString = subkey.GetValue("UninstallString") as string,
				};
			}
		}
	}

	public class InstalledApp {
		public string Publisher       { get; set; }
		public string DisplayName     { get; set; }
		public string DisplayIcon     { get; set; }
		public string InstallDate     { get; set; }
		public string DisplayVersion  { get; set; }
		public string InstallLocation { get; set; }
		public string UninstallString { get; set; }
	}
}
