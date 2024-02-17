using System;
using System.IO;

namespace Magicube.IO {
    public static class DirectoryExtension {
        public static long GetAvailableFreeSpaceOnDisk(this string dir) {
            try {
                var drive = new DriveInfo(dir);
                if (drive.IsReady) {
                    return drive.AvailableFreeSpace;
                }
                return 0L;
            }
            catch (ArgumentException) {
                return 0L;
            }
        }

        public static long GetAvailableFreeSpaceOnDisk(this DirectoryInfo dirInfo) {
            return GetAvailableFreeSpaceOnDisk(dirInfo.FullName);
        }
    }
}
