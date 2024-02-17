namespace Magicube.Core.Reflection {
    using System;

    internal static class Utility {
        public static void ThrowIfArgumentNull(object argument, string argumentName) {
            if (argument == null) {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ThrowIfArgumentNullOrEmpty(string argument, string argumentName) {
            Utility.ThrowIfArgumentNull(argument, argumentName);

            if (argument == string.Empty) {
                throw new ArgumentException("Argument is empty", argumentName);
            }
        }

        public static void ThrowIfArgumentNullEmptyOrWhitespace(string argument, string argumentName) {
            Utility.ThrowIfArgumentNullOrEmpty(argument, argumentName);

            if (argument.IndexOf(' ') != -1) {
                throw new ArgumentException("Argument contains whitespace", argumentName);
            }
        }
    }
}