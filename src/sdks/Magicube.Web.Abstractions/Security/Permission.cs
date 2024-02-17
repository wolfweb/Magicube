﻿using Magicube.Core;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Magicube.Web.Security {
    public class Permission {
        public const string ClaimType = "Permission";

        public Permission(string name) {
            if (name.IsNullOrEmpty()) {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public Permission(string name, string description) : this(name) {
            Description = description;
        }

        public string Name                       { get; set; }
        public string SubTitle                   { get; set; }
        public string Description                { get; set; }
        public string Category                   { get; set; }
        public IEnumerable<Permission> ImpliedBy { get; set; }

        public static implicit operator Claim(Permission p) {
            return new Claim(ClaimType, p.Name);
        }
    }
}
