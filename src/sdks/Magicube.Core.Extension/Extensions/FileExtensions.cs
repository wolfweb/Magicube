namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class FileExtensions {
        public static string ToFriendly(this int value) {
            if (value < 0x3E8)
                return "{0} Byte".Format(value);
            else if (value >= 1000 && value < 0xF4240)
                return "{0:F2} Kb".Format(((double)value) / 0x400);
            else if (value >= 1000 && value < 0x3B9ACA00)
                return "{0:F2} M".Format(((double)value) / 0x100000);
            else
                return "{0:F2} G".Format(((double)value) / 0x40000000);
        }

        public static string ToFriendly(this long value) {
            if (value < 0x3E8) 
                return "{0} Byte".Format(value);
             else if (value >= 0x3E8 && value < 0xF4240) 
                return "{0:F2} Kb".Format(((double)value) / 0x400);
             else if (value >= 0x3E8 && value < 0x3B9ACA00) 
                return "{0:F2} M".Format(((double)value) / 0x100000);
             else if (value >= 0x3B9ACA00 && value < 0xE8D4A51000) 
                return "{0:F2} G".Format(((double)value) / 0x40000000);
             else 
                return "{0:F2} T".Format(((double)value) / 0x10000000000);            
        }
    }
}
