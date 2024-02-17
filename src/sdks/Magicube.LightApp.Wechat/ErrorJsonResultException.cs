using Magicube.LightApp.Abstractions;
using System;

namespace Magicube.LightApp.Wechat {
    public class ErrorJsonResultException : LightAppException {        
        public string Url { get; set; }

        public ErrorJsonResultException(string message, Exception inner, string url = null)
            : base(message, inner) {
            Url = url;
        }
    }
}
