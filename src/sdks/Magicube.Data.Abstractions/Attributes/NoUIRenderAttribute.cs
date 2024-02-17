using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Data.Abstractions.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class NoUIRenderAttribute : Attribute {
        
    }
}
