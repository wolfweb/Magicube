using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicube.Identity {
    public class UserToken : Entity<long> {
        public string  Name          { get; set; }
        public string  Value         { get; set; }
        public long    UserId        { get; set; }
        public string? LoginProvider { get; set; }
    }
}
