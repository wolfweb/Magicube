using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FileExtensionsAttribute : DataTypeAttribute {
        public string Extensions { get; private set; }

        public FileExtensionsAttribute(string allowedExtensions = "png,jpg,jpeg,gif")
            : base("fileextension") {
            Extensions = string.IsNullOrWhiteSpace(allowedExtensions) ? "png,jpg,jpeg,gif" : allowedExtensions.Replace("|", ",").Replace(" ", "");
        }

        public override string FormatErrorMessage(string name) {
            if (ErrorMessage == null && ErrorMessageResourceName == null) {
                ErrorMessage = "文件 {0} 只接受以下有效文件类型: {1}";
            }

            return string.Format(ErrorMessageString, name, Extensions);
        }

        public override bool IsValid(object value) {
            if (value == null) {
                return true;
            }

            string valueAsString;
            if (value != null && value is IFormFile) {
                valueAsString = (value as IFormFile).FileName;
            } else {
                valueAsString = value as string;
            }

            if (valueAsString != null) {
                return ValidateExtension(valueAsString);
            }

            return false;
        }

        private bool ValidateExtension(string fileName) {
            try {
                return Extensions.Split(',').Contains(Path.GetExtension(fileName).Replace(".", "").ToLowerInvariant());
            } catch (ArgumentException) {
                return false;
            }
        }
    }
}
