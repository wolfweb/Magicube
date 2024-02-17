using System.Collections.Generic;

namespace Magicube.Net.Email {
    public class SmtpResult {
        public static SmtpResult Success                        { get; } = new SmtpResult { Succeeded = true };
        public static SmtpResult Failed(params string[] errors) => new SmtpResult { Succeeded = false, Errors = errors };

        public bool                Succeeded                    { get; protected set; }
        public IEnumerable<string> Errors                       { get; protected set; }
    }
}
