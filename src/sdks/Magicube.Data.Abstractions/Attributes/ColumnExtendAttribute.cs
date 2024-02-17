using Magicube.Data.Abstractions.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Magicube.Data.Abstractions.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnExtendAttribute : ColumnAttribute{
        public int  Size     { get; set; }
        public bool Nullable { get; set; } = true;
    }
}
